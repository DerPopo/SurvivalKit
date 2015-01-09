using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SKPatcher
{
	public class RPCPatcher
	{
		ModuleDefinition module;
		ModuleDefinition skModule;
		ModuleDefinition unityModule;
		ModuleDefinition mscorlibModule;
		Logger logger;
		public RPCPatcher(ModuleDefinition module, ModuleDefinition skModule, ModuleDefinition unityModule, ModuleDefinition mscorlibModule, Logger logger)
		{
			this.module = module;
			this.skModule = skModule;
			this.unityModule = unityModule;
			this.mscorlibModule = mscorlibModule;
			this.logger = logger;
		}

		public void Patch()
		{
			HelperClass.SetLogger(null);
			//foreach RPC method in ConnectionManager
			foreach (MethodDefinition mdef in HelperClass.findMembers<MethodDefinition>(module, "ConnectionManager", HelperClass.MemberCustomAttributeComparer<MethodDefinition>("UnityEngine.RPC")))
			{
				if (mdef.HasBody)
				{
					Mono.Collections.Generic.Collection<ParameterDefinition> _params = mdef.Parameters;
					if (_params.Count == 0 || !_params[_params.Count - 1].ParameterType.FullName.Equals("UnityEngine.NetworkMessageInfo"))
					{
						ParameterDefinition messageInfoPardef = new ParameterDefinition("_messageInfo", ParameterAttributes.None,
							mdef.DeclaringType.Module.Import(unityModule.GetType("UnityEngine.NetworkMessageInfo")));
						mdef.Parameters.Add(messageInfoPardef);
					}

					ILProcessor proc = mdef.Body.GetILProcessor();
					List<Instruction[]> argLoaders = new List<Instruction[]>();
					argLoaders.Add(new Instruction[] {
						proc.Create(OpCodes.Ldc_I4_0),
						proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Boolean"))),
					});
					argLoaders.Add(new Instruction[] {
						proc.Create(OpCodes.Ldstr, mdef.Name),
					});
					List<Instruction> instrTmp = new List<Instruction>();
					for (int i = 0; i < _params.Count; i++)
					{
						ParameterDefinition param = _params[i];
						instrTmp.Clear();
						instrTmp.Add(proc.Create(OpCodes.Ldarg, param));
						if (param.ParameterType.IsValueType)
						{
							instrTmp.Add(proc.Create(OpCodes.Box, module.Import(param.ParameterType)));
						}
						argLoaders.Add(instrTmp.ToArray());
					}

					List<Instruction> hook = HookHelper.Instance.prepareEventHook(mdef, "RPC", argLoaders.ToArray());
					for (int i = 0; i < _params.Count; i++)
					{
						ParameterDefinition param = _params[i];
						hook.Add(proc.Create(OpCodes.Dup));
						hook.Add(proc.Create(OpCodes.Ldc_I4, i + 3));
						hook.Add(proc.Create(OpCodes.Ldelem_Ref));
						if (param.ParameterType.IsValueType)
						{
							hook.Add(proc.Create(OpCodes.Unbox_Any, module.Import(param.ParameterType)));
						}
						else
						{
							hook.Add(proc.Create(OpCodes.Castclass, module.Import(param.ParameterType)));
						}
						hook.Add(proc.Create(OpCodes.Starg, param));
					}
					hook.Add(proc.Create(OpCodes.Ldc_I4_1));
					hook.Add(proc.Create(OpCodes.Ldelem_Ref));
					hook.Add(proc.Create(OpCodes.Unbox_Any, module.Import(mscorlibModule.GetType("System.Boolean"))));
					hook.Add(proc.Create(OpCodes.Brfalse, mdef.Body.Instructions[0]));
					hook.Add(proc.Create(OpCodes.Ret));
					HookHelper.insertAt(mdef.Body, 0, hook.ToArray());
				}
			}
		}
	}
}

