using System;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	///	Interface that should be implemented by all plug ins.	
	/// Contains a method for registering <see cref="Abstracts.EventListener"/> instances.
	/// </summary>
	public interface IPlugin : IDisposable
	{
		/// <summary>
		///		Method to register event listener.
		/// </summary>
		/// <param name="eventAggregator">The aggregator the event listeners should register with.</param>
		void RegisterEventListeners(IEventAggregator eventAggregator);

		/// <summary>
		///		Method to register command listener.
		/// </summary>
		/// <param name="eventAggregator">The aggregator the command listeners should register with.</param>
		void RegisterCommandListeners(IEventAggregator eventAggregator);

		/// <summary>
		/// Method to retrieve the name of a plugin.
		/// </summary>
		/// <returns>The name of the plugin.</returns>
		string getPluginName();

		/// <summary>
		/// Gets the authors of this plugin.
		/// </summary>
		/// <returns>A string array of authors.</returns>
		String[] getAuthors();

		/// <summary>
		/// Called when the plug in load finished.
		/// </summary>
		void onLoad();

		/// <summary>
		/// Called when the plug in is enabled.
		/// </summary>
		void onEnable();

		/// <summary>
		/// Called when the plug in is disabled.
		/// </summary>
		void onDisable();
	}
}
