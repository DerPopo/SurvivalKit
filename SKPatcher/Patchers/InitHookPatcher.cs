using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SKPatcher
{
	public class InitHookPatcher
	{
		ModuleDefinition module;
		ModuleDefinition skModule;
		Logger logger;
		public InitHookPatcher(ModuleDefinition module, ModuleDefinition skModule, ModuleDefinition unityModule, ModuleDefinition mscorlibModule, Logger logger)
		{
			this.module = module;
			this.skModule = skModule;
			this.logger = logger;

		}

		public void Patch()
		{
			HelperClass.SetLogger(logger);
			MethodDefinition onGameInit = HelperClass.findMember<MethodDefinition>(skModule, "SurvivalKit.SKMain", false,
				HelperClass.MemberNameComparer<MethodDefinition>("onGameInit"));
			MethodDefinition onGameEnable = HelperClass.findMember<MethodDefinition>(skModule, "SurvivalKit.SKMain", false,
				HelperClass.MemberNameComparer<MethodDefinition>("onGameEnable"));
			MethodDefinition onGameDisable = HelperClass.findMember<MethodDefinition>(skModule, "SurvivalKit.SKMain", false,
				HelperClass.MemberNameComparer<MethodDefinition>("onGameDisable"));
			if (onGameInit == null || onGameEnable == null || onGameDisable == null)
				return;

			MethodDefinition initMethod = HelperClass.findMember<MethodDefinition>(module, "ConnectionManager", false,
				HelperClass.MemberNameComparer<MethodDefinition>(".cctor"));
			MethodDefinition awakeMethod = HelperClass.findMember<MethodDefinition>(module, "GameManager", false,
				HelperClass.MemberNameComparer<MethodDefinition>("Awake"));
			MethodDefinition cleanupMethod = HelperClass.findMember<MethodDefinition>(module, "GameManager", false,
				HelperClass.MemberNameComparer<MethodDefinition>("Cleanup"));
			if (initMethod == null || awakeMethod == null || cleanupMethod == null)
				return;

			{
				ILProcessor proc = initMethod.Body.GetILProcessor();
				proc.InsertBefore(initMethod.Body.Instructions[0], proc.Create(OpCodes.Call, module.Import(onGameInit)));
			}
			{
				ILProcessor proc = awakeMethod.Body.GetILProcessor();
				Instruction lastInstr;
				proc.InsertBefore(awakeMethod.Body.Instructions[0], (lastInstr = proc.Create(OpCodes.Ldarg_0)));
				proc.InsertAfter(lastInstr, proc.Create(OpCodes.Call, module.Import(onGameEnable)));
			}
			{
				ILProcessor proc = cleanupMethod.Body.GetILProcessor();
				Instruction lastInstr;
				proc.InsertBefore(cleanupMethod.Body.Instructions[0], (lastInstr = proc.Create(OpCodes.Ldarg_0)));
				proc.InsertAfter(lastInstr, proc.Create(OpCodes.Call, module.Import(onGameDisable)));
			}
		}
	}
}

