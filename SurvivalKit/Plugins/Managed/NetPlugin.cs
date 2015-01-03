using System;
using System.Collections.Generic;

using SurvivalKit;

namespace SurvivalKit.Plugins.Managed
{
	/// <summary>
	/// Represents a managed .net compatible plugin.
	/// </summary>
	public abstract class NetPlugin : Plugin
	{
		/// <summary>
		/// Gets the event handlers.
		/// </summary>
		public Object[] EventHandlers {
			get { return this.eventHandlers.ToArray(); }
		}
		private List<Object> eventHandlers = new List<Object>();

		private Dictionary<string,Commands.CommandListener> listenerByCommand = new Dictionary<string, SurvivalKit.Commands.CommandListener>();

		/// <summary>
		/// Gets the command listeners.
		/// </summary>
		/// <value>The dictionary containing the commands as keys and the listeners as values.</value>
		public Dictionary<string,Commands.CommandListener> ListenerByCommand {
			get { return new Dictionary<string,Commands.CommandListener>(this.listenerByCommand); }
		}

		/// <summary>
		/// Registers an event handler.
		/// </summary>
		/// <param name="handler">
		/// An instance of an event handler containing methods with the Listener attribute.
		/// </param>
		public void registerEvents(Object handler)
		{
			if (!this.getLoader().isEnabled())
				return;
			foreach (Object curEvent in eventHandlers)
				if (curEvent == handler)
					return;
			eventHandlers.Add(handler);
		}
		/// <summary>
		/// Clears the event handlers.
		/// </summary>
		public void clearHandlers()
		{
			this.eventHandlers.Clear();
		}

		/// <summary>
		/// Clears the command listeners.
		/// </summary>
		public void clearCommandListeners()
		{
			this.listenerByCommand.Clear();
		}

		/// <summary>
		/// Registers a command to a <see cref="SurvivalKit.Commands.CommandListener"/>.
		/// </summary>
		/// <param name="command">Command.</param>
		/// <param name="listener">Listener.</param>
		public void registerCommand(string command, Commands.CommandListener listener)
		{
			if (!this.getLoader().isEnabled())
				return;
			if (!this.listenerByCommand.ContainsKey(command.ToLower())) {
				this.listenerByCommand.Add(command.ToLower(), listener);
			}
		}

		/// <summary>
		/// Gets the SurvivalKit main class instance.
		/// </summary>
		/// <returns>The instance of SKMain.</returns>
		public SKMain getMain()
		{
			return SKMain.SkMain;
		}

		/// <summary>
		/// Gets the PluginLoader of this plugin.
		/// </summary>
		/// <returns>The PluginLoader of this plugin.</returns>
		public NetLoader getLoader()
		{
			return SKMain.SkMain.getPluginManager().getLoader(this) as NetLoader;
		}
	}
}

