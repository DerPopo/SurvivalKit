//using System;
//using System.IO;
//using System.Xml;
//using System.Collections.Generic;

//using SurvivalKit.Plugins.Managed;

//namespace SurvivalKit.Plugins
//{
//	/// <summary>
//	/// Provides methods to get plugins and load plugins.
//	/// </summary>
//	public class PluginManager
//	{
//		//private List<PluginLoader> pluginLoaders = new List<PluginLoader>();

//		///// <summary>
//		///// Gets a plugin by name
//		///// </summary>
//		///// <returns>The matching plugin, or null if no plugin matches the criteria.</returns>
//		///// <param name="name">The name of the plugin to search.</param>
//		///// <param name="caseSensitive">If set to <c>false</c>, ignore upper and lower case.</param>
//		//public Plugin getPlugin(string name, bool caseSensitive)
//		//{
//		//	foreach (PluginLoader curPluginLoader in this.pluginLoaders) {
//		//		Plugin curPlugin = curPluginLoader.plugin;
//		//		if (curPlugin == null)
//		//			continue;
//		//		if ((caseSensitive ? 
//		//				curPluginLoader.name.Equals(name) : 
//		//				curPluginLoader.name.ToLower().Equals(name.ToLower())))
//		//			return curPlugin;
//		//	}
//		//	return null;
//		//}
//		///// <summary>
//		///// Gets all currently loaded plugins.
//		///// </summary>
//		///// <returns>An array of plugins.</returns>
//		//public Plugin[] getPlugins()
//		//{
//		//	System.Collections.Generic.List<Plugin> pluginList = new System.Collections.Generic.List<Plugin>();
//		//	foreach (PluginLoader curLoader in this.pluginLoaders) {
//		//		Plugin curPlugin = curLoader.plugin;
//		//		if (curPlugin != null) {
//		//			pluginList.Add(curLoader.plugin);
//		//		}
//		//	}
//		//	return pluginList.ToArray();
//		//}
//		///// <summary>
//		///// Loads a .net compatible plugin.
//		///// </summary>
//		///// <returns>The loaded plugin.</returns>
//		///// <param name="assemblyPath">The path to the .net assembly.</param>
//		///// <param name="mainclass">The main class' full name.</param>
//		///// <param name="name">The name of the plugin.</param>
//		//public NetPlugin loadNetPlugin(string assemblyPath, string mainclass, string name)
//		//{
//		//	foreach (PluginLoader curLoader in this.pluginLoaders) {
//		//		if (curLoader is NetLoader) {
//		//			NetLoader loader = (NetLoader)curLoader;
//		//			if (loader.MainClass.ToLower().Equals (mainclass.ToLower())
//		//				|| loader.AssemblyPath.ToLower().Equals(assemblyPath.ToLower())) {
//		//				Log.Warning("Can't load a plugin twice! Please unload it first.");
//		//				return null;
//		//			}
//		//		}
//		//	}
//		//	NetLoader _loader = new NetLoader(assemblyPath, mainclass, name);
//		//	_loader.Load();
//		//	pluginLoaders.Add(_loader);
//		//	return (_loader.plugin as NetPlugin);
//		//}

//		///// <summary>
//		///// Searches for and loads plugins.
//		///// </summary>
//		//public void loadPlugins()
//		//{
//		//	DirectoryInfo pluginDir = new DirectoryInfo("plugins");
//		//	if (!pluginDir.Exists)
//		//		pluginDir.Create();
//		//	DirectoryInfo managedDir = new DirectoryInfo("plugins"+Path.DirectorySeparatorChar+"managed");
//		//	if (!managedDir.Exists) {
//		//		managedDir.Create();
//		//		return;
//		//	}
//		//	foreach (FileInfo fi in managedDir.GetFiles("*.dll")) {
//		//		try {
//		//			XmlDocument pluginXml= new XmlDocument();
//		//			string dllPath = fi.FullName;
//		//			string xmlPath = dllPath.Substring (0, dllPath.Length - 4) + ".xml";
//		//			if (!new FileInfo (xmlPath).Exists)
//		//				continue;
//		//			pluginXml.Load(xmlPath);
//		//			XmlNode nameNode = pluginXml.SelectSingleNode("/plugin/name[1]");
//		//			XmlNode mainClassNode = pluginXml.SelectSingleNode("/plugin/main[1]");
//		//			XmlNode enabledNode = pluginXml.SelectSingleNode("/plugin/enabled[1]");
//		//			if ((nameNode != null && mainClassNode != null
//		//					&& nameNode.InnerText.Length > 0 && mainClassNode.InnerText.Length > 0)) {
//		//				if (enabledNode == null || enabledNode.InnerText.ToLower().Equals("true")) {
//		//					Log.Out("Loading plugin '" + nameNode.InnerText + "'");
//		//					NetPlugin curPlugin = this.loadNetPlugin(dllPath, mainClassNode.InnerText, nameNode.InnerText);
//		//					if (curPlugin != null) {
//		//						string authorStr = "";
//		//						string[] authors = curPlugin.getAuthors();
//		//						if (authors.Length > 0) {
//		//							authorStr += " (by ";
//		//							int authlen = authors.Length;
//		//							for (int i = 0; i < authlen; i++)
//		//							{
//		//								authorStr += "'" + authors[i] + "'";
//		//								if ((i+3) < authlen) {
//		//									authorStr += ", ";
//		//								}
//		//								else if ((i+2) == authlen) {
//		//									authorStr += "and ";
//		//								}
//		//							}
//		//							authorStr += ")";
//		//						}
//		//						Log.Out("Plugin '" + nameNode.InnerText + "'" + authorStr + " loaded.");
//		//					}
//		//				} else {
//		//					Log.Out("Skipping plugin '" + nameNode.InnerText + "' (disabled by configuration xml)");
//		//				}
//		//			}
//		//			else
//		//			{
//		//				Log.Out("Invalid plugin configuration xml '" + xmlPath + "'!");
//		//			}
//		//		} catch (Exception e) {
//		//			Log.Out("An exception occured while loading a plugin dll ('" + fi.Name + "') : ");
//		//			Log.Exception(e);
//		//		}
//		//	}
//		//}

//		///// <summary>
//		///// Unloads a plugin (doesn't work yet).
//		///// </summary>
//		///// <param name="plugin">The plugin to unload.</param>
//		//public void unloadPlugin(Plugin plugin)
//		//{
//		//	foreach (PluginLoader curPluginLoader in this.pluginLoaders) {
//		//		if (curPluginLoader.plugin == plugin) {
//		//			if (curPluginLoader.isEnabled())
//		//				curPluginLoader.Disable();
//		//			curPluginLoader.Unload();
//		//			this.pluginLoaders.Remove(curPluginLoader);
//		//			break;
//		//		}
//		//	}
//		//}

//		///// <summary>
//		///// Gets the plugin loader of a plugin.
//		///// </summary>
//		///// <returns>The plugin loader.</returns>
//		///// <param name="plugin">The plugin to get the loader of.</param>
//		//public PluginLoader getLoader(Plugin plugin)
//		//{
//		//	foreach (PluginLoader curPluginLoader in this.pluginLoaders) {
//		//		if (curPluginLoader.plugin == plugin) {
//		//			return curPluginLoader;
//		//		}
//		//	}
//		//	return null;
//		//}
//	}
//}

