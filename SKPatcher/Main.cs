using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;

namespace SKPatcher
{
	class MainClass
	{
		private static AssemblyPath ownFolder;
		private static Logger mainLogger = null;
		class AssemblyPath
		{
			public string path;
			public string filename;

			public AssemblyPath(string _path, string _filename)
			{
				path = _path;
				filename = _filename;
			}
		}

		private static AssemblyPath GetContainingFolder (string fullpath)
		{
			string path = null;
			string file = null;
			int lastSlash = fullpath.LastIndexOf ("\\");
			if (lastSlash == -1)
				lastSlash = fullpath.LastIndexOf ("/");
			if (lastSlash != -1) {
				path = fullpath.Substring (0, lastSlash);
				file = fullpath.Substring (lastSlash + 1);
			} else {
				if (ownFolder == null)
					return null;
				path = ownFolder.path;
				file = fullpath;
			}

			return new AssemblyPath (path, file);
		}

		private static void ErrorExit (string message, int returnCode = 1)
		{
			Console.WriteLine ();
			Logger.Level logLevel = (returnCode == 0) ? Logger.Level.KEYINFO : Logger.Level.ERROR;
			if (mainLogger != null)
			{
				if (message.Length > 0)
					mainLogger.Log(logLevel, message);
				mainLogger.Close ();
			}
			else
				Console.WriteLine(Logger.Level_ToString(logLevel) + message);

			Console.WriteLine ();
			Console.WriteLine ("Press any key to exit");
			Console.ReadKey ();
			Environment.Exit (returnCode);
		}

		public static void Main (string[] args)
		{
			//args = new string[]{"P:\\Programme\\SteamLibrary\\steamapps\\common\\7 Days to Die Dedicated Server\\7DaysToDie_Data\\Managed\\Assembly-CSharp.o.dll"};
			Console.WriteLine("RandomGen world limit patcher for 7dtd's Assembly-CSharp.dll [by DerPopo]");

			ownFolder = GetContainingFolder(Assembly.GetEntryAssembly().Location);
			if (ownFolder == null) {
				ErrorExit("Unable to retrieve my executable location!");
			}

			mainLogger = new Logger(ownFolder.path + Path.DirectorySeparatorChar + "mainlog.txt", null, (int)Logger.Level.KEYINFO);
			mainLogger.Info("Started logging to mainlog.txt.");

			if ( args.Length == 0 || !args[0].ToLower().EndsWith(".dll") )
			{
				mainLogger.Write("Usage : rglimitpatcher \"<path to Assembly-CSharp.dll>\"");
				mainLogger.Write("Alternatively, you can drag and drop Assembly-CSharp.dll into rglimitpatcher.");
				ErrorExit("", 2);
			}
			AssemblyPath acsharpSource = GetContainingFolder(args[0]);
			if (!File.Exists(acsharpSource.path + Path.DirectorySeparatorChar + acsharpSource.filename)) {
				ErrorExit("Unable to retrieve the folder containing Assembly-CSharp.dll or Assembly-CSharp.dll doesn't exist!");
			}
			if (!File.Exists(acsharpSource.path + Path.DirectorySeparatorChar + "mscorlib.dll")) {
				ErrorExit("Unable to locate mscorlib.dll!");
			}
			if (!File.Exists(acsharpSource.path + Path.DirectorySeparatorChar + "UnityEngine.dll")) {
				ErrorExit("Unable to locate UnityEngine.dll!");
			}
			if (!File.Exists(ownFolder.path + Path.DirectorySeparatorChar + "SurvivalKit.dll")) {
				ErrorExit("Unable to locate SurvivalKit.dll!");
			}

			DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
			resolver.AddSearchDirectory (acsharpSource.path);

			AssemblyDefinition csharpDef = null;
			AssemblyDefinition mscorlibDef = null;
			AssemblyDefinition unityDef = null;
			AssemblyDefinition survivalKitDef = null;
			try
			{
				csharpDef = AssemblyDefinition.ReadAssembly(args[0], new ReaderParameters { AssemblyResolver = resolver, });
				mscorlibDef = AssemblyDefinition.ReadAssembly(acsharpSource.path + Path.DirectorySeparatorChar + "mscorlib.dll", new ReaderParameters { AssemblyResolver = resolver, });
				unityDef = AssemblyDefinition.ReadAssembly(acsharpSource.path + Path.DirectorySeparatorChar + "UnityEngine.dll", new ReaderParameters { AssemblyResolver = resolver, });
				survivalKitDef = AssemblyDefinition.ReadAssembly(ownFolder.path + Path.DirectorySeparatorChar + "SurvivalKit.dll", new ReaderParameters { AssemblyResolver = resolver, });
			}
			catch (Exception e)
			{
				ErrorExit("Unable to load Assembly-CSharp.dll, mscorlib.dll, UnityEngine.dll or SurvivalKit.dll :" + e.ToString());
			}
			if (csharpDef.Modules.Count < 1)
			{
				ErrorExit("Assembly-CSharp.dll is invalid : it has no modules!");
			}
			if (mscorlibDef.Modules.Count < 1)
			{
				ErrorExit("mscorlib.dll is invalid : it has no modules!");
			}
			if (unityDef.Modules.Count < 1)
			{
				ErrorExit("UnityEngine.dll is invalid : it has no modules!");
			}
			if (survivalKitDef.Modules.Count < 1)
			{
				ErrorExit("SurvivalKit.dll is invalid : it has no modules!");
			}
			ModuleDefinition csharpModule = csharpDef.Modules[0];
			ModuleDefinition mscorlibModule = mscorlibDef.Modules[0];
			ModuleDefinition unityModule = unityDef.Modules[0];
			ModuleDefinition survivalKitModule = survivalKitDef.Modules[0];
			if (csharpModule.GetType("Deobfuscated") == null)
			{
				ErrorExit("Assembly-CSharp.dll needs to be deobfuscated first!");
			}
			HookHelper.Instance = new HookHelper(csharpModule, survivalKitModule, mscorlibModule);

			new InitHookPatcher(csharpModule, survivalKitModule, unityModule, mscorlibModule, mainLogger).Patch();
			new NetworkPatcher(csharpModule, survivalKitModule, unityModule, mscorlibModule, mainLogger).Patch();
			new RPCPatcher(csharpModule, survivalKitModule, unityModule, mscorlibModule, mainLogger).Patch();

			string outputPath = acsharpSource.path + Path.DirectorySeparatorChar + "Assembly-CSharp.rglimit.dll";
			mainLogger.KeyInfo("Saving the new assembly to " + outputPath + " ...");
			try
			{
				csharpDef.Write(outputPath);
			}
			catch (Exception e)
			{
				ErrorExit ("Unable to save the assembly : " + e.ToString());
			}

			ErrorExit ("Success.", 0);
		}
	}
}
