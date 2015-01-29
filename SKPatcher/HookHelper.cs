using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SKPatcher
{
	public class HookHelper
	{
		public static HookHelper Instance = null;

		private ModuleDefinition csModule, skModule, mscorlibModule;
		public HookHelper(ModuleDefinition csModule, ModuleDefinition skModule, ModuleDefinition mscorlibModule)
		{
			this.csModule = csModule;
			this.skModule = skModule;
			this.mscorlibModule = mscorlibModule;
		}

		private void addToArray(ILProcessor proc, List<Instruction> instructions, int index, Instruction[] loadElement)
		{
			instructions.Add(proc.Create(OpCodes.Dup));
			instructions.Add(proc.Create(OpCodes.Ldc_I4, index));
			instructions.AddRange(loadElement);
			instructions.Add(proc.Create(OpCodes.Stelem_Ref));
		}
		public List<Instruction> prepareEventHook(MethodDefinition mdef, string _event, Instruction[][] argLoaders)
		{
			MethodDefinition fireEvent = HelperClass.findMember<MethodDefinition>(skModule, "SurvivalKit.Events.EventManager", false,
				HelperClass.MemberNameComparer<MethodDefinition>("FireEvent"),
				HelperClass.MethodParameterNamesComparer("name", "pars"));
			if (fireEvent == null)
				throw new Exception("Unable to find SurvivalKit.Events.EventManager.FireEvent(string,object[])!");

			ILProcessor proc = mdef.Body.GetILProcessor();
			List<Instruction> hook = new List<Instruction>();
			hook.Add(proc.Create(OpCodes.Ldstr, _event));
			hook.Add(proc.Create(OpCodes.Ldc_I4, argLoaders.Length));
			hook.Add(proc.Create(OpCodes.Newarr, csModule.Import(mscorlibModule.GetType("System.Object"))));
			int dataIndex = 0;
			//addToArray(proc, hook, dataIndex++, new Instruction[] { proc.Create(OpCodes.Ldarg_0) });
			for (int i = 0; i < argLoaders.Length; i++)
				addToArray(proc, hook, dataIndex++, argLoaders[i]);
			hook.Add(proc.Create(OpCodes.Call, csModule.Import(fireEvent)));
			return hook;
		}

		public static void insertAt(MethodBody body, int index, Instruction[] instructions)
		{
			for (int i = 0; i < instructions.Length; i++)
				body.Instructions.Insert(index++, instructions[i]);
		}
	}
}

