using System;
using System.IO;
using System.Reflection;
using SurvivalKit.Events;
using System.Collections.Generic;

namespace SurvivalKit.Plugins.Managed
{
	/// <summary>
	/// The loader for .net compatible plugins.
	/// </summary>
	public class NetLoader : PluginLoader
	{

		private string assemblyPath;
		private string mainClass;
		private Assembly pluginAssembly = null;
		//private AppDomain assemblyDomain = null;
		private Type pluginType;
		private NetPlugin pluginInstance;
		private string pluginName;
		private bool enabled = false;

		private List<Object> eventListeners = new List<Object>();

		/// <summary>
		/// Gets whether the plugin is loaded.
		/// </summary>
		/// <value><c>true</c> if the plugin is loaded, <c>false</c> otherwise.</value>
		public override bool isLoaded {
			get { return this.pluginInstance != null; }
		}
		/// <summary>
		/// Gets the plugin instance.
		/// </summary>
		/// <value>The plugin (null if not loaded)</value>
		public override Plugin plugin {
			get { return this.pluginInstance; }
		}
		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		/// <value>The name of the plugin.</value>
		public override string name {
			get { return this.pluginName; }
		}
		/// <summary>
		/// Gets the .net assembly of the plugin.
		/// </summary>
		/// <value>The .net assembly.</value>
		public Assembly assembly {
			get { return this.pluginAssembly; }
		}
		/// <summary>
		/// Gets the main class' full name.
		/// </summary>
		/// <value>The main class' full name.</value>
		public string MainClass {
			get { return this.mainClass; }
		}
		/// <summary>
		/// Gets the path to the plugin assembly.
		/// </summary>
		/// <value>The path to the plugin assembly.</value>
		public string AssemblyPath {
			get { return this.assemblyPath; }
		}

		/// <summary>
		/// Creates a new <see cref="SurvivalKit.Plugins.Managed.NetLoader"/> and loads the plugin assembly.
		/// </summary>
		/// <param name="assemblyPath">The path to the plugin assembly.</param>
		/// <param name="mainClass">The main class' full name.</param>
		/// <param name="name">The name of the plugin.</param>
		public NetLoader(string assemblyPath, string mainClass, string name)
		{
			if (assemblyPath == null || mainClass == null || name == null) {
				throw new ArgumentNullException();
			}
			if (assemblyPath.Length == 0) {
				throw new FileNotFoundException ("AssemblyPath is an empty string!");
			}
			if (mainClass.Length == 0) {
				throw new Exception("The main class of the plugin is an empty string!");
			}
			//this.assemblyDomain = AppDomain.CreateDomain("plugin_managed_" + name, AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
			this.assemblyPath = assemblyPath;
			this.mainClass = mainClass;

			//FileStream asmStream = new FileStream (assemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			//byte[] asmData = new byte[(int)asmStream.Length];
			//asmStream.Read(asmData, 0, asmData.Length);

			this.pluginAssembly = Assembly.LoadFile(assemblyPath);//Assembly.Load(asmData);//this.assemblyDomain.Load(asmData);

			this.pluginType = this.pluginAssembly.GetType(mainClass, true, false);
			this.pluginInstance = null;
			this.pluginName = name;
		}

		/// <summary>
		/// Loads the plugin.
		/// </summary>
		public override void Load()
		{
			if (this.pluginType == null) {
				throw new NullReferenceException("The NetPlugin class reference is null!");
			}
			foreach (ConstructorInfo constructor in this.pluginType.GetConstructors()) {
				if (constructor.GetParameters().Length == 0) {
					this.pluginInstance = constructor.Invoke(new object[0]) as NetPlugin;
					this.pluginInstance.onLoad();
					return;
				}
			}
			throw new TypeLoadException("Unable to find the plugin's constructor!");
		}

		/// <summary>
		/// Enables the plugin.
		/// </summary>
		public override void Enable()
		{
			if (this.enabled)
				return;
			Log.Out("Enabling plugin '" + name + "'");
			this.enabled = true;
			this.pluginInstance.onEnable();
		}

		/// <summary>
		/// Disables the plugin.
		/// </summary>
		public override void Disable()
		{
			if (!this.enabled)
				return;
			Log.Out("Disabling plugin '" + name + "'");
			this.enabled = false;
			this.pluginInstance.clearHandlers();
			this.pluginInstance.clearCommandListeners();
			this.pluginInstance.onDisable();
		}

		/// <summary>
		/// Unload this instance. Don't use this as it doesn't really unload the assembly yet.
		/// </summary>
		public override void Unload()
		{
			if (this.pluginAssembly == null/* || this.assemblyDomain == null*/) {
				throw new NullReferenceException("The NetPlugin reference is null!");
			}
			if (this.enabled)
				this.Disable();
			this.pluginAssembly = null;
			//AppDomain.Unload(this.assemblyDomain);
			//this.assemblyDomain = null;
		}

		/// <summary>
		/// Gets whether the plugin is enabled
		/// </summary>
		/// <returns><c>true</c>, if the plugin is loaded, <c>false</c> otherwise.</returns>
		public override bool isEnabled()
		{
			return this.enabled;
		}
	}
}

