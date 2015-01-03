using System;
using System.Collections.Generic;
using System.Reflection;
using SurvivalKit.Events;

namespace SurvivalKit.Plugins
{
	/// <summary>
	/// Represents a plugin.
	/// </summary>
	public abstract class Plugin
	{
		/// <summary>
		/// Called when the plugin load finished.
		/// </summary>
		public abstract void onLoad();

		/// <summary>
		/// Called when the plugin is enabled.
		/// </summary>
		public abstract void onEnable();

		/// <summary>
		/// Called when the plugin is disabled.
		/// </summary>
		public abstract void onDisable();

		/// <summary>
		/// Gets the authors of this plugin.
		/// </summary>
		/// <returns>A string array of authors.</returns>
		public virtual String[] getAuthors(){return new String[0];}
	}
}

