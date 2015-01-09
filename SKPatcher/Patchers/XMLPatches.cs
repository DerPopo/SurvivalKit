using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ExtensionMethods.Xml;

namespace SKPatcher
{
	public class XMLPatches
	{
		private static Dictionary<string, OpCode> opCodeList;
		static XMLPatches()
		{
			opCodeList = new Dictionary<string, OpCode>();
			foreach (System.Reflection.FieldInfo curOpcode in typeof(OpCodes).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
			{
				opCodeList.Add(curOpcode.Name.ToLower(), (OpCode)curOpcode.GetValue(null));
			}
		}

		ModuleDefinition module;
		ModuleDefinition skModule;
		ModuleDefinition unityModule;
		ModuleDefinition mscorlibModule;
		ModuleDefinition[] modules;
		Logger logger;
		public XMLPatches(ModuleDefinition module, ModuleDefinition skModule, ModuleDefinition unityModule, ModuleDefinition mscorlibModule, Logger logger)
		{
			this.module = module;
			this.skModule = skModule;
			this.unityModule = unityModule;
			this.mscorlibModule = mscorlibModule;
			modules = new ModuleDefinition[]{module, unityModule, mscorlibModule, skModule};
			this.logger = logger;
		}

		public void Patch()
		{
			System.Reflection.Assembly ownAssembly = System.Reflection.Assembly.GetCallingAssembly();
			Stream xmlStream = ownAssembly.GetManifestResourceStream("SKPatcher.xmlpatchers.xml");
			XmlDocument xml = new XmlDocument();
			xml.Load(xmlStream);
			XmlElement document = xml.DocumentElement;
			XmlNode[] hookNodes = document.ChildNodes.Items("hook");
			foreach (XmlNode hookNode in hookNodes)
			{
				if (hookNode.Attributes["type"] == null || hookNode.Attributes["name"] == null || hookNode.Attributes["insertAt"] == null)
				{
					logger.Error("Error in xmlpatchers.xml : Unable to find one of the necessary attributes!");
					break;
				}
				if (hookNode.Attributes["type"].Value.ToLower().Equals("event"))
				{
					string eventName = hookNode.Attributes["name"].Value;
					bool isCancellable = (hookNode.Attributes["cancellable"] != null) && (hookNode.Attributes["cancellable"].Value.ToLower().Equals("true"));
					int insertAt;
					if (!Int32.TryParse(hookNode.Attributes["insertAt"].Value, out insertAt))
					{
						logger.Error("Error in xmlpatchers.xml : Unable to parse the insertAt value!");
						continue;
					}
					XmlNode typeNode = hookNode.ChildNodes.Item("type");
					XmlNode methodNode = hookNode.ChildNodes.Item("method");
					XmlNode parametersNode = hookNode.ChildNodes.Item("parameters");
					if (typeNode == null || methodNode == null || parametersNode == null)
					{
						logger.Error("Error in xmlpatchers.xml : ChildNodes missing for event (name = \"" + eventName + "\")!");
						continue;
					}
					string typeName = typeNode.Attributes["name"].Value;
					XmlAttribute methodName = methodNode.Attributes["name"];
					XmlAttribute methodParameters = methodNode.Attributes["parameters"];
					XmlAttribute methodReturn = methodNode.Attributes["return"];
					TypeDefinition targetType = module.GetType(typeName);
					if (targetType == null)
					{
						logger.Error("Error in xmlpatchers.xml : Unable to find type \"" + typeName + "\"!");
						continue;
					}

					//I don't always use 'var' but when I do, it would be odd not to.
					var methodComparers = new List<HelperClass.GenericFuncContainer<MethodDefinition,bool>>(3);
					if (methodName != null)
						methodComparers.Add(HelperClass.MemberNameComparer<MethodDefinition>(methodName.Value));
					if (methodParameters != null)
						methodComparers.Add(HelperClass.MethodParametersComparer(methodParameters.Value.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries)));
					if (methodReturn != null)
						methodComparers.Add(HelperClass.MethodReturnTypeComparer(methodReturn.Value));
					methodComparers.Add(new HelperClass.GenericFuncContainer<MethodDefinition,bool>(method => Matches(method, methodNode)));
					MethodDefinition targetMethod = HelperClass.findMember<MethodDefinition>(module, targetType, false,
						methodComparers.ToArray());

					int parameterId = (isCancellable ? 0 : -1);
					if (targetMethod != null)
					{
						MethodBody body = targetMethod.Body;
						ILProcessor proc = body.GetILProcessor();
						List<Instruction[]> argLoaders = new List<Instruction[]>(parametersNode.ChildNodes.Count + (isCancellable ? 1 : 0));
						if (isCancellable)
							argLoaders.Add(new Instruction[]{proc.Create(OpCodes.Ldc_I4_0), proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Boolean")))});
						List<Instruction[]> argWriters = new List<Instruction[]>(parametersNode.ChildNodes.Count);
						if (isCancellable)
						{
							Instruction[] cancelInstr = new Instruction[8];
							cancelInstr[0] = proc.Create(OpCodes.Dup);
							cancelInstr[1] = proc.Create(OpCodes.Ldc_I4_0);
							cancelInstr[2] = proc.Create(OpCodes.Ldelem_Ref);
							cancelInstr[3] = proc.Create(OpCodes.Unbox_Any, module.Import(mscorlibModule.GetType("System.Boolean")));

							cancelInstr[5] = proc.Create(OpCodes.Pop);
							cancelInstr[6] = proc.Create(OpCodes.Ret);
							cancelInstr[7] = proc.Create(OpCodes.Nop);

							cancelInstr[4] = proc.Create(OpCodes.Brfalse, cancelInstr[7]);
							argWriters.Add(cancelInstr);
						}


						foreach (XmlNode parNode in parametersNode.ChildNodes.Items("parameter"))
						{
							parameterId++;
							XmlAttribute _typeAttr = parNode.Attributes["type"];
							string parType = (_typeAttr == null) ? "System.Object" : _typeAttr.Value;
							TypeDefinition parTypeDef = findType(parType);
							if (parTypeDef == null)
							{
								logger.Error("Error in xmlpatchers.xml : Unable to find type \"" + parType + "\"!");
								continue;
							}
							bool isCustom = parNode.Attributes.ValueOf("isCustom").ToLower().Equals("true");
							XmlNode loadNode = parNode.ChildNodes.Item("load");
							XmlNode writeNode = parNode.ChildNodes.Item("write");
							if (loadNode != null)
							{
								List<Instruction> loadInstr = new List<Instruction>(ParseInstructions(loadNode, proc));
								if (parTypeDef.IsValueType)
									loadInstr.Add(proc.Create(OpCodes.Box, module.Import(parTypeDef)));
								argLoaders.Add(loadInstr.ToArray());
							}
							if (writeNode != null)
							{
								List<Instruction> writeInstr = new List<Instruction> ();
								if (!isCustom)
								{
									writeInstr.Add(proc.Create(OpCodes.Dup));
									writeInstr.Add(proc.Create(OpCodes.Ldc_I4, parameterId));
									writeInstr.Add(proc.Create(OpCodes.Ldelem_Ref));
									if (parTypeDef.IsValueType)
										writeInstr.Add(proc.Create(OpCodes.Unbox_Any, module.Import(parTypeDef)));
									else
										writeInstr.Add(proc.Create(OpCodes.Castclass, module.Import(parTypeDef)));
								}
								writeInstr.AddRange(ParseInstructions(writeNode, proc));

								argWriters.Add(writeInstr.ToArray());
							}
						}
						List<Instruction> hook = HookHelper.Instance.prepareEventHook(targetMethod, eventName, argLoaders.ToArray());
						for (int i = 0; i < argWriters.Count; i++)
							hook.AddRange(argWriters[i]);
						hook.Add(proc.Create(OpCodes.Pop));
						HookHelper.insertAt(body, insertAt, hook.ToArray());
					}
				}
			}
		}

		private TypeDefinition findType(string name)
		{
			TypeDefinition ret = null;
			for (int i = 0; i < modules.Length; i++)
			{
				ret = modules[i].GetType(name);
				if (ret != null)
					break;
			}
			return ret;
		}

		private interface IInstructionGenerator
		{
			//returns null if a referenced Instruction is out of range
			Instruction Generate(ILProcessor proc, List<Instruction> instructions);
			OpCode GetOpCode();
			bool HasOperand();
		}
		private class NoOPInstructionGenerator : IInstructionGenerator
		{
			private OpCode opcode;
			public NoOPInstructionGenerator(OpCode opcode)
			{
				this.opcode = opcode;
			}
			public Instruction Generate(ILProcessor proc, List<Instruction> instructions)
			{
				return proc.Create(opcode);
			}
			public OpCode GetOpCode() { return opcode; }
			public bool HasOperand() { return false; }
		}
		private class InstructionGenerator : IInstructionGenerator
		{
			public delegate Instruction CreateDelegate(ILProcessor proc, List<Instruction> instructions, OpCode opcode, object operand);
			private OpCode opcode;
			private object operand;
			private CreateDelegate createFn;
			public InstructionGenerator(OpCode opcode, object operand, CreateDelegate createFn)
			{
				this.opcode = opcode;
				this.operand = operand;
				this.createFn = createFn;
			}

			public Instruction Generate(ILProcessor proc, List<Instruction> instructions)
			{
				return createFn(proc, instructions, opcode, operand);
			}
			public OpCode GetOpCode() { return opcode; }
			public bool HasOperand() { return operand != null; }
		}
		private IInstructionGenerator ParseOpcode(XmlNode opNode)
		{
			XmlNode opcodeNode = opNode.Attributes["name"];
			if (opcodeNode == null)
				throw new ArgumentException("xmlpatchers.xml : An opcode node doesn't define a name attribute!");
			string opcodeName = opcodeNode.Value.ToLower();
			if (!opCodeList.ContainsKey(opcodeName))
				throw new ArgumentException("xmlpatchers.xml : An invalid opcode was passed (" + opcodeNode.Value + ")!");
			string opType = opNode.Attributes.ValueOf("opType").ToLower();
			string op = opNode.Attributes.ValueOf("op");
			OpCode opcode = opCodeList[opcodeName];
			switch (opType)
			{
				case "sbyte":
					return new InstructionGenerator(opcode, SByte.Parse(op), (_p,_i,_opcode,_operand) => _p.Create(opcode,(SByte)_operand));
				case "byte":
					return new InstructionGenerator(opcode, Byte.Parse(op), (_p,_i,_opcode,_operand) => _p.Create(opcode,(Byte)_operand));
				case "int32":
				case "int":
					return new InstructionGenerator(opcode, Int32.Parse(op), (_p,_i,_opcode,_operand) => _p.Create(opcode,(Int32)_operand));
				case "int64":
				case "long":
					return new InstructionGenerator(opcode, Int64.Parse(op), (_p,_i,_opcode,_operand) => _p.Create(opcode,(Int64)_operand));
				case "single":
				case "float":
					return new InstructionGenerator(opcode, Single.Parse(op), (_p,_i,_opcode,_operand) => _p.Create(opcode,(Single)_operand));
				case "double":
					return new InstructionGenerator(opcode, Double.Parse(op), (_p,_i,_opcode,_operand) => _p.Create(opcode,(Double)_operand));
				case "string":
					return new InstructionGenerator(opcode, op, (_p,_i,_opcode,_operand) => _p.Create(opcode,(string)_operand));
				case "tref":
					{
						TypeDefinition typeDef = findType(op);
						if (typeDef == null)
							throw new ArgumentException("xmlpatchers.xml : Unable to find type \"" + op + "\"!");
						return new InstructionGenerator(opcode, module.Import(typeDef), (_p,_i,_opcode,_operand) => _p.Create(opcode,(TypeReference)_operand));
					}
				case "fref":
					{
						string[] opSplit = op.Split(new string[]{";"}, StringSplitOptions.RemoveEmptyEntries);
						if (opSplit.Length != 2)
							throw new ArgumentException("xmlpatchers.xml : A fref operand has an illegal count of semicolons!");

						TypeDefinition typeDef = findType(opSplit[0]);
						if (typeDef == null)
							throw new ArgumentException("xmlpatchers.xml : Unable to find type \"" + opSplit[0] + "\"!");

						HelperClass.SetLogger(null);
						FieldDefinition fdef = HelperClass.findMember<FieldDefinition>(module, typeDef, false,
							HelperClass.MemberNameComparer<FieldDefinition>(opSplit[1]));
						HelperClass.SetLogger(logger);
						if (fdef == null)
							throw new ArgumentException("xmlpatchers.xml : Unable to find field \"" + (opSplit[0] + "." + opSplit[1]) + "\"!");

						return new InstructionGenerator(opcode, module.Import(fdef), (_p,_i,_opcode,_operand) => _p.Create(opcode,(FieldReference)_operand));
					}
				case "mref":
					{
						string[] opSplit = op.Split(new string[]{";"}, StringSplitOptions.RemoveEmptyEntries);
						if (opSplit.Length != 2 && opSplit.Length != 3)
							throw new ArgumentException("xmlpatchers.xml : A fref operand has an illegal count of semicolons!");

						TypeDefinition typeDef = findType(opSplit[0]);
						if (typeDef == null)
							throw new ArgumentException("xmlpatchers.xml : Unable to find type \"" + opSplit[0] + "\"!");

						string[] _params = (opSplit.Length == 2) ? null : opSplit[2].Split(new string[]{";"}, StringSplitOptions.RemoveEmptyEntries);
						//I don't always use 'var' but when I do, it would be odd not to.
						var methodComparers = new List<HelperClass.GenericFuncContainer<MethodDefinition,bool>>(opSplit.Length-1);
						methodComparers.Add(HelperClass.MemberNameComparer<MethodDefinition>(opSplit[1]));
						if (_params != null)
							methodComparers.Add(HelperClass.MethodParametersComparer(_params));
						HelperClass.SetLogger(null);
						MethodDefinition mdef = HelperClass.findMember<MethodDefinition>(module, typeDef, false, methodComparers.ToArray());
						HelperClass.SetLogger(logger);
						if (mdef == null)
							throw new ArgumentException("xmlpatchers.xml : Unable to find method \"" + (opSplit[0] + "." + opSplit[1]) + "\"!");

						return new InstructionGenerator(opcode, module.Import(mdef), (_p,_i,_opcode,_operand) => _p.Create(opcode,(MethodReference)_operand));
					}
				case "instref":
					return new InstructionGenerator(opcode, Int32.Parse(op), (_p,_i,_opcode,_operand) => {
						int _targetIndex = (int)_operand;
						if (_targetIndex >= _i.Count)
							return null;
						return _p.Create(_opcode, _i[_targetIndex]);
					});
				case "none":
				case "":
					return new NoOPInstructionGenerator(opcode);
				default:
					throw new ArgumentException("xmlpatchers.xml : Unknown operand type \"" + opType + "\"!");
			}
		}

		private class QueuedGenerator
		{
			public IInstructionGenerator generator;
			public int index;
			public QueuedGenerator(IInstructionGenerator generator, int index)
			{
				this.generator = generator;
				this.index = index;
			}
		}

		private Instruction[] ParseInstructions(XmlNode opList, ILProcessor proc)
		{
			List<Instruction> ret = new List<Instruction>();
			List<QueuedGenerator> queuedGenerators = new List<QueuedGenerator>();
			foreach (XmlNode opNode in opList.ChildNodes)
			{
				if (opNode is XmlComment)
					continue;
				//if (opNode.Attributes["name"] == null)
				//	continue;
				IInstructionGenerator instGen = ParseOpcode(opNode);
				Instruction inst = instGen.Generate(proc, ret);
				if (inst == null)
				{
					inst = proc.Create(OpCodes.Nop);
					queuedGenerators.Add(new QueuedGenerator(instGen, ret.Count));
				}
				ret.Add(inst);
			}
			foreach (QueuedGenerator curGen in queuedGenerators)
			{
				Instruction newInstr = curGen.generator.Generate(proc, ret);
				if (newInstr == null)
					throw new ArgumentOutOfRangeException("xmlpatchers.xml : Instruction reference out of range!");
				Instruction prevInstr = ret[curGen.index];
				prevInstr.OpCode = newInstr.OpCode;
				prevInstr.Operand = newInstr.Operand;
			}
			return ret.ToArray();
		}
		private bool Matches(MethodDefinition method, XmlNode opList)
		{
			ILProcessor proc = method.Body.GetILProcessor();
			List<Instruction> instrs = new List<Instruction>(method.Body.Instructions);
			int i = 0;
			foreach (XmlNode opNode in opList.ChildNodes)
			{
				if (opNode is XmlComment)
					continue;
				int index;
				XmlAttribute indexAttribute = opNode.Attributes["index"];
				if (indexAttribute == null)
					index = i++;
				else
				{
					index = Int32.Parse(indexAttribute.Value);
					if (index < 0)
						index += instrs.Count;
					i = index + 1;
				}
				if (index >= instrs.Count || index < 0)
					return false;
				//if (opNode.Attributes["name"] == null)
				//	continue;
				IInstructionGenerator instGen = ParseOpcode(opNode);
				if (instGen.GetOpCode() != instrs[index].OpCode)
					return false;
				if (instGen.HasOperand() != (instrs[index].Operand != null))
					continue;
				Instruction inst = instGen.Generate(proc, instrs);
				if (inst == null)
					return false;

				object methodOp = instrs[index].Operand;
				object compareOp = inst.Operand;
				Type methodOpType = instrs[index].Operand.GetType();
				if (methodOpType != inst.Operand.GetType())
					return false;
				switch (methodOpType.Name)
				{
					case "Byte":
						if ((Byte)methodOp != (Byte)compareOp)
							return false;
						break;
					case "SByte":
						if ((SByte)methodOp != (SByte)compareOp)
							return false;
						break;
					case "Int32":
						if ((Int32)methodOp != (Int32)compareOp)
							return false;
						break;
					case "Int64":
						if ((Int64)methodOp != (Int64)compareOp)
							return false;
						break;
					case "Single":
						if ((Single)methodOp != (Single)compareOp)
							return false;
						break;
					case "Double":
						if ((Double)methodOp != (Double)compareOp)
							return false;
						break;
					case "String":
						if (!methodOp.Equals(compareOp))
							return false;
						break;
					case "Instruction":
						if (methodOp != compareOp)
							return false;
						break;
					case "TypeReference":
						if (!((TypeReference)methodOp).FullName.Equals(((TypeReference)compareOp).FullName))
							return false;
						break;
					case "FieldReference":
						if (!((FieldReference)methodOp).FullName.Equals(((FieldReference)compareOp).FullName))
							return false;
						break;
					case "MethodReference":
						if (!((MethodReference)methodOp).ToString().Equals(((MethodReference)compareOp).ToString())) //ToString includes the method 
							return false;
						break;
					default:
						logger.Warning("XmlPatches : Nobody told me how to compare two " + methodOpType.Name + "s!");
						break;
				}
			}
			return true;
		}
	}
}

namespace ExtensionMethods.Xml
{
	public static class NodeList
	{
		public static XmlNode Item(this XmlNodeList _this, string name)
		{
			XmlNode[] ret = _this.Items(name);
			if (ret.Length == 0)
				return null;
			return ret[0];
		}

		public static XmlNode[] Items(this XmlNodeList _this, string name)
		{
			name = name.ToLower();
			List<XmlNode> ret = new List<XmlNode>();
			for (int i = 0; i < _this.Count; i++)
			{
				XmlNode curNode = _this.Item(i);
				if (curNode is XmlComment)
					continue;
				if (curNode.Name.ToLower().Equals(name))
					ret.Add(curNode);
			}
			return ret.ToArray();
		}

		public static string ValueOf(this XmlAttributeCollection _this, string name)
		{
			name = name.ToLower();
			for (int i = 0; i < _this.Count; i++)
			{
				XmlNode curNode = _this.Item(i);
				if (!(curNode is XmlAttribute))
					continue;
				if (curNode.Name.ToLower().Equals(name))
					return curNode.Value;
			}
			return "";
		}
	}
}