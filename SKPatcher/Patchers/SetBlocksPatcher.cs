using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SKPatcher
{
	public class SetBlocksPatcher
	{
		ModuleDefinition module;
		ModuleDefinition skModule;
		ModuleDefinition unityModule;
		ModuleDefinition mscorlibModule;
		Logger logger;
		public SetBlocksPatcher(ModuleDefinition module, ModuleDefinition skModule, ModuleDefinition unityModule, ModuleDefinition mscorlibModule, Logger logger)
		{
			this.module = module;
			this.skModule = skModule;
			this.unityModule = unityModule;
			this.mscorlibModule = mscorlibModule;
			this.logger = logger;
		}

		public void Patch()
		{
			HelperClass.SetLogger(logger);
			MethodDefinition setBlocksMethod = HelperClass.findMember<MethodDefinition>(module, "GameManager", false,
				HelperClass.MemberNameComparer<MethodDefinition>("SetBlocksRPC"));
			PropertyDefinition worldProperty = HelperClass.findMember<PropertyDefinition>(module, "GameManager", false,
				HelperClass.MemberNameComparer<PropertyDefinition>("World"));
			if (setBlocksMethod != null && worldProperty != null)
			{
				MethodBody body = setBlocksMethod.Body;
				ILProcessor proc = body.GetILProcessor();
				List<Instruction> hook = HookHelper.Instance.prepareEventHook(setBlocksMethod, "SetBlocks", new Instruction[][] { 
					new Instruction[] { proc.Create(OpCodes.Ldc_I4_0), proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Boolean"))) },
					new Instruction[] { proc.Create(OpCodes.Ldarg_1) },
					new Instruction[] { proc.Create(OpCodes.Ldarg_0), proc.Create(OpCodes.Call, module.Import(worldProperty.GetMethod)) },
				});
				hook.Add(proc.Create(OpCodes.Dup));
				hook.Add(proc.Create(OpCodes.Ldc_I4_0));
				hook.Add(proc.Create(OpCodes.Ldelem_Ref));
				hook.Add(proc.Create(OpCodes.Unbox_Any, module.Import(mscorlibModule.GetType("System.Boolean"))));
				int jmp1_sindex = hook.Count; hook.Add(null); //Brfalse
				hook.Add(proc.Create(OpCodes.Pop));
				hook.Add(proc.Create(OpCodes.Ret));
				int jmp1_tindex = hook.Count;
				hook.Add(proc.Create(OpCodes.Ldc_I4_1));
				hook.Add(proc.Create(OpCodes.Ldelem_Ref));
				hook.Add(proc.Create(OpCodes.Starg, 1));

				hook.Insert(jmp1_sindex, proc.Create(OpCodes.Brfalse, hook[jmp1_tindex])); hook.RemoveAt(jmp1_sindex+1);
				HookHelper.insertAt(body, 0, hook.ToArray());
			}
		}
	}
}

