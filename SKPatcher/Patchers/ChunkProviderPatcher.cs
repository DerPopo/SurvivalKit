using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SKPatcher
{
	public class ChunkProviderPatcher
	{
		ModuleDefinition module;
		ModuleDefinition mscorlibModule;
		ModuleDefinition skModule;
		Logger logger;
		public ChunkProviderPatcher(ModuleDefinition module, ModuleDefinition skModule, ModuleDefinition unityModule, ModuleDefinition mscorlibModule, Logger logger)
		{
			this.module = module;
			this.skModule = skModule;
			this.mscorlibModule = mscorlibModule;
			this.logger = logger;
		}

		public void Patch()
		{
			HelperClass.SetLogger(logger);

			MethodDefinition initChunkCluster = HelperClass.findMember<MethodDefinition>(module, "ChunkCluster", false,
				HelperClass.MemberNameComparer<MethodDefinition>("Init"),
				HelperClass.MethodParametersComparer("EnumChunkProviderId"));
			if (initChunkCluster == null)
				return;
			{
				int[] jumpsToDefault = HelperClass.FindOPCodePattern(initChunkCluster, new OpCode[]{OpCodes.Switch,OpCodes.Br}, 1);
				if (jumpsToDefault.Length == 0)
				{
					logger.Error("ChunkCluster.Init has no switch (am I outdated?)!");
					return;
				}
				if (jumpsToDefault.Length > 1)
				{
					logger.Error("ChunkCluster.Init has multiple switches (am I outdated?)!");
					return;
				}
				MethodBody body = initChunkCluster.Body;
				int targetIndex = body.Instructions.IndexOf((Instruction)body.Instructions[jumpsToDefault[0]].Operand);
				if (!HelperClass.MethodOPCodeComparer(new int[]{targetIndex,targetIndex+1,targetIndex+2}, new OpCode[]{OpCodes.Ldarg_0,OpCodes.Ldnull,OpCodes.Stfld}, null).Execute(initChunkCluster))
				{
					logger.Error("ChunkCluster.Init's default case is unknown (I am outdated!)!");
					return;
				}

				ILProcessor proc = body.GetILProcessor();
				List<Instruction> hook = HookHelper.Instance.prepareEventHook(initChunkCluster, "UnknownChunkProviderEvent", new Instruction[][]{
					new Instruction[]{proc.Create(OpCodes.Ldarg_0)},
					new Instruction[]{proc.Create(OpCodes.Ldc_I4_0), proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Boolean")))},
					new Instruction[]{proc.Create(OpCodes.Ldarg_1), proc.Create(OpCodes.Box, module.Import(mscorlibModule.GetType("System.Int32")))},
					new Instruction[]{proc.Create(OpCodes.Ldnull)},
				}); hook.Insert(0, proc.Create(OpCodes.Ldarg_0));
				hook.Add(proc.Create(OpCodes.Dup));
				hook.Add(proc.Create(OpCodes.Ldc_I4_1));
				hook.Add(proc.Create(OpCodes.Ldelem_Ref));
				hook.Add(proc.Create(OpCodes.Unbox_Any, module.Import(mscorlibModule.GetType("System.Boolean"))));
				int jmp1_sindex = hook.Count; hook.Add(null); //brtrue
				hook.Add(proc.Create(OpCodes.Pop));
				hook.Add(proc.Create(OpCodes.Br, body.Instructions[targetIndex+1]));
				int jmp1_tindex = hook.Count;
				hook.Add(proc.Create(OpCodes.Ldc_I4_2));
				hook.Add(proc.Create(OpCodes.Ldelem_Ref));
				hook.Add(proc.Create(OpCodes.Br, body.Instructions[targetIndex+2]));

				hook.RemoveAt(jmp1_sindex); hook.Insert(jmp1_sindex, proc.Create(OpCodes.Brtrue, hook[jmp1_tindex]));

				HookHelper.insertAt(body, targetIndex, hook.ToArray());
				body.Instructions[jumpsToDefault[0]].Operand = hook[0];
			}
		}
	}
}

