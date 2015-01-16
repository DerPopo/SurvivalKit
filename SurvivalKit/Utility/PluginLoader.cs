using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SurvivalKit.Utility
{
	/// <summary>
	/// Synchronous loader of plugins.
	/// </summary>
	internal class PluginLoader : ILoadAssemblies
	{
		/// <summary>
		///	Switch allowing us to see if we have loaded plugins already
		/// </summary>
		private static bool LoadedPlugins;

		/// <summary>
		///	Collection of assemblies we have loaded.
		/// </summary>
		private static List<Assembly> _loadedAssemblies;

		/// <summary>
		///	The singleton instance.
		/// </summary>
		private static PluginLoader _instance;

		/// <summary>
		///	Private constructor for the singleton.
		/// </summary>
		private PluginLoader()
		{
		}

		/// <summary>
		///	Method to get the singleton instance.
		/// </summary>
		/// <returns>Returns the singleton as a <see cref="ILoadAssemblies"/> instance</returns>
		public static ILoadAssemblies GetInstance()
		{
			if (_instance == null)
			{
				_loadedAssemblies = new List<Assembly>();
				_instance = new PluginLoader();
			}

			return _instance;
		}

		/// <summary>
		///	Try to load the assemblies.
		/// </summary>
		public void LoadAssemblies()
		{
			if (!LoadedPlugins)
			{
				// only load the plugins once
				loadPlugins();
				LoadedPlugins = true;
			}
		}

		/// <summary>
		///	Internal method containing the logic to actually load plugins.
		/// </summary>
		/// <remarks>Todo: implement a registry that will keep track of which plugins are enabled or not.</remarks>
		private void loadPlugins()
		{
			DirectoryInfo pluginDir = new DirectoryInfo("plugins");
			if (!pluginDir.Exists)
				pluginDir.Create();

			foreach (FileInfo fi in pluginDir.GetFiles("*.dll"))
			{
				try
				{
					// just load all assemblies, the resolver locate all IPlugin implementations
					var assembly = Assembly.LoadFrom(fi.FullName);
					_loadedAssemblies.Add(assembly);
				}
				catch (Exception e)
				{
					Log.Out("An exception occured while loading a plugin dll ('" + fi.Name + "') : ");
					Log.Exception(e);
				}
			}
		}
	}
}
