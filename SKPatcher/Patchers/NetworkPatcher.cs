using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SKPatcher
{
	public class NetworkPatcher
	{
		ModuleDefinition module;
		ModuleDefinition mscorlibModule;
		ModuleDefinition skModule;
		Logger logger;
		public NetworkPatcher(ModuleDefinition module, ModuleDefinition skModule, ModuleDefinition unityModule, ModuleDefinition mscorlibModule, Logger logger)
		{
			this.module = module;
			this.skModule = skModule;
			this.mscorlibModule = mscorlibModule;
			this.logger = logger;
		}

		public void Patch()
		{
			HelperClass.SetLogger(null); //HelperClass.OnError would otherwise show errors if a NetPackage class doesn't override Process,Read or Write
			TypeDefinition[] netPackageTypes = HelperClass.findTypes(module,
				HelperClass.CombinedComparer<MethodDefinition>(
					HelperClass.MemberNameComparer<MethodDefinition>("GetPackageType"), 
					HelperClass.MethodReturnTypeComparer("PackageType"), 
					HelperClass.MethodNegAttributeComparer(MethodAttributes.Abstract))
			);
			if (netPackageTypes == null || netPackageTypes.Length == 0)
			{
				logger.Error("Unable to find any NetPackage classes!");
				return;
			}

			foreach (TypeDefinition curPackageType in netPackageTypes)
			{
				MethodDefinition processMethod = HelperClass.findMember<MethodDefinition>(module, curPackageType, false,
					HelperClass.MemberNameComparer<MethodDefinition>("Process"),
					HelperClass.MethodReturnTypeComparer("System.Void"),
					HelperClass.MethodParametersComparer("World", "INetConnectionCallbacks"));
				if (processMethod != null)
				{
					MethodBody body = processMethod.Body;
					if (body != null)
					{
						ILProcessor proc = body.GetILProcessor();
						List<Instruction> eventHook = HookHelper.Instance.prepareEventHook(processMethod, "ProcessPacket",
							new Instruction[][]{
								new Instruction[] {
									proc.Create(OpCodes.Ldc_I4_0),
									proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Boolean"))),
								},
								new Instruction[]{
									proc.Create(OpCodes.Ldarg_1),
								},
								new Instruction[]{
									proc.Create(OpCodes.Ldarg_2),
								},
							}
						);
						eventHook.Add(proc.Create(OpCodes.Ldc_I4_0));
						eventHook.Add(proc.Create(OpCodes.Ldelem_Ref));
						eventHook.Add(proc.Create(OpCodes.Unbox_Any, module.Import(mscorlibModule.GetType("System.Boolean"))));
						eventHook.Add(proc.Create(OpCodes.Brfalse, body.Instructions[0]));
						eventHook.Add(proc.Create(OpCodes.Ret));
						HookHelper.insertAt(body, 0, eventHook.ToArray());
					}
				}
				MethodDefinition readMethod = HelperClass.findMember<MethodDefinition>(module, curPackageType, false,
					HelperClass.MemberNameComparer<MethodDefinition>("Read"),
					HelperClass.MethodReturnTypeComparer("System.Void"),
					HelperClass.MethodParametersComparer("System.IO.BinaryReader"));
				if (readMethod != null)
				{
					MethodBody body = readMethod.Body;
					if (body != null)
					{
						ILProcessor proc = body.GetILProcessor();
						List<Instruction> eventHook = HookHelper.Instance.prepareEventHook(readMethod, "ReadPacketFromBuf", new Instruction[][]{});
						eventHook.Add(proc.Create(OpCodes.Pop));
						HookHelper.insertAt(body, body.Instructions.Count, eventHook.ToArray());
					}
				}
				MethodDefinition writeMethod = HelperClass.findMember<MethodDefinition>(module, curPackageType, false,
					HelperClass.MemberNameComparer<MethodDefinition>("Write"),
					HelperClass.MethodReturnTypeComparer("System.Void"),
					HelperClass.MethodParametersComparer("System.IO.BinaryWriter"));
				if (writeMethod != null)
				{
					MethodBody body = writeMethod.Body;
					if (body != null)
					{
						ILProcessor proc = body.GetILProcessor();
						List<Instruction> eventHook = HookHelper.Instance.prepareEventHook(writeMethod, "WritePacketToBuf",
							new Instruction[][]{
								new Instruction[] {
									proc.Create(OpCodes.Ldc_I4_0),
									proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Boolean"))),
								},
							}
						);
						eventHook.Add(proc.Create(OpCodes.Ldc_I4_0));
						eventHook.Add(proc.Create(OpCodes.Ldelem_Ref));
						eventHook.Add(proc.Create(OpCodes.Unbox_Any, module.Import(mscorlibModule.GetType("System.Boolean"))));
						eventHook.Add(proc.Create(OpCodes.Brfalse, body.Instructions[0]));
						eventHook.Add(proc.Create(OpCodes.Ret));
						HookHelper.insertAt(body, 0, eventHook.ToArray());
					}
				}
			}
		}
	}
}

