using System;

namespace SurvivalKit.Plugins
{
	/// <summary>
	/// An abstract plugin loader.
	/// </summary>
	public abstract class PluginLoader
	{
		/// <summary>
		/// Gets whether the plugin is loaded.
		/// </summary>
		/// <value><c>true</c> if the plugin is loaded, <c>false</c> otherwise.</value>
		public abstract bool isLoaded { get; }

		/// <summary>
		/// Gets the plugin.
		/// </summary>
		/// <value>The plugin (null if not loaded)</value>
		public abstract Plugin plugin { get; }

		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		/// <value>The name of the plugin.</value>
		public abstract string name { get; }

		/// <summary>
		/// Loads the plugin.
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// Enables the plugin.
		/// </summary>
		public abstract void Enable();

		/// <summary>
		/// Disables the plugin.
		/// </summary>
		public abstract void Disable();

		/// <summary>
		/// Unloads the plugin.
		/// </summary>
		public abstract void Unload();

		/// <summary>
		/// Gets whether the plugin is enabled
		/// </summary>
		/// <returns><c>true</c>, if the plugin is loaded, <c>false</c> otherwise.</returns>
		public abstract bool isEnabled();
	}
}

