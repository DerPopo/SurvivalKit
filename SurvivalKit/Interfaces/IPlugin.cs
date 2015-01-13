using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	///	Interface that should be implemented by all plugins.	
	/// Contains a method for registering <see cref="EventListener"/> instances.
	/// </summary>
	public interface IPlugin : IDisposable
	{
		/// <summary>
		///		Method to register event listener.
		/// </summary>
		/// <param name="eventAggregator">The aggregator the listeners should register with.</param>
		void RegisterEventListeners(IEventAggregator eventAggregator);

		/// <summary>
		/// Gets the authors of this plugin.
		/// </summary>
		/// <returns>A string array of authors.</returns>
		String[] getAuthors();

		/// <summary>
		/// Called when the plugin load finished.
		/// </summary>
		void onLoad();

		/// <summary>
		/// Called when the plugin is enabled.
		/// </summary>
		void onEnable();

		/// <summary>
		/// Called when the plugin is disabled.
		/// </summary>
		void onDisable();
	}
}
