using SurvivalKit.Abstracts;
using SurvivalKit.Exceptions;
using SurvivalKit.Extensions;
using SurvivalKit.Interfaces;
using SurvivalKit.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SurvivalKit.Events
{
	/// <summary>
	/// This is a registry of events.
	/// All <see cref="EventListener"/> instances register a hook so we can call them when an event occurs.
	/// </summary>
	public sealed class EventAggregator : IEventAggregator
	{
		/// <summary>
		///	Boolean to determine whether the game is enabled or not.
		/// </summary>
		private bool GameDisabled = true;

		/// <summary>
		///	The singleton instance of the <see cref="IEventAggregator"/>.
		/// </summary>
		private static IEventAggregator _instance;

		private readonly List<IPlugin> _plugins;

		/// <summary>
		///	Registry of hooks.
		/// </summary>
		private readonly PrioritizedEventListenerDictionary<Type, EventListenerRegistration> _hookRegistry;

		/// <summary>
		///	The registry of commands.
		/// </summary>
		private readonly Dictionary<string, List<ICommandListener>> _commandRegistry;

		/// <summary>
		/// Should we prevent calls to the log library?
		/// Used for the testing framework.
		/// </summary>
		private readonly bool _preventLogging = false;

		/// <summary>
		///		Private constructor for the singleton.
		/// </summary>
		private EventAggregator(IResolveInstances instanceResolver, bool preventLogging = false)
		{
			_preventLogging = preventLogging;
			_hookRegistry = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(new EventListenerRegistrationComparer());
			_commandRegistry = new Dictionary<string, List<ICommandListener>>();

			// Get all instances and try to gather all event listeners.
			_plugins = instanceResolver.ResolveInstances<IPlugin>();

			if (_plugins == null || _plugins.Count == 0)
			{
				LogUtility.Warning("[SK] No IPlugin instances found.");
				return;
			}

			foreach (var plugin in _plugins)
			{
				try
				{
					var authors = plugin.getAuthors();
					var formatted = string.Join(", ", authors);
					var lastIndex = formatted.LastIndexOf(", ");
					if(lastIndex > 0)
					{
						formatted = formatted.Remove(lastIndex, 2);
						formatted = formatted.Insert(lastIndex, " & ");
					}
					LogUtility.Out("[SK] Loading plugin " + plugin.getPluginName() + " created by " + formatted);

					plugin.onLoad();
					plugin.RegisterEventListeners(this);
					plugin.RegisterCommandListeners(this);
				}
				catch (Exception exception)
				{
					var wrappedException = new PluginInitializationException(plugin.GetType().AssemblyQualifiedName, exception.Message, exception);
					LogUtility.Exception(wrappedException);
				}
			}

			LogUtility.Out("[SK] EventAggregator initialized");
		}

		/// <summary>
		///		Getter for the singleton instance.
		/// </summary>
		/// <returns>The only instance of the <see cref="IEventAggregator"/></returns>
		public static IEventAggregator GetInstance()
		{
			if (_instance == null)
			{
				// first call to the event aggregator, lets initialize a new instance with the default resolver..
				_instance = new EventAggregator(new InstanceResolver());
			}

			return _instance;
		}

		/// <summary>
		///		Getter for the singleton instance for testing purposes.
		///		This overload will be used for unit testing.
		/// </summary>
		/// <returns>The instance of the <see cref="IEventAggregator"/></returns>
		internal static IEventAggregator GetInstance(IResolveInstances instanceResolver, bool createNewInstance)
		{
			if (_instance == null || (_instance != null && createNewInstance))
			{
				// first call to the event aggregator, lets initialize a new instance.
				_instance = new EventAggregator(instanceResolver, true);
			}

			return _instance;
		}

		/// <summary>
		/// Method to register an event listener.
		/// </summary>
		/// <typeparam name="TListener">The type of the event listener.</typeparam>
		/// <param name="eventListener">The listener instance.</param>
		/// <returns>
		///	Returns <c>true</c> if the <see cref="TListener"/> was added to the registry of listeners.
		///	Returns <c>false</c> if the <see cref="TListener"/> was already added to the registry of listeners.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="eventListener"/> is <c>null</c>.</exception>
		public bool RegisterEventListener<TListener>(TListener eventListener) where TListener : EventListener
		{
			if (eventListener == null)
			{
				throw new ArgumentNullException("eventListener");
			}

			LogUtility.Out("[SK] Registering " + eventListener.GetType().FullName);

			// TODO check if this event listener isn't already registered.

			var hooks = eventListener.GetEventHooks();
			foreach (var eventHook in hooks)
			{
				var registration = new EventListenerRegistration(eventListener, eventHook);
				var type = eventHook.GetEventType();

				_hookRegistry.Add(type, registration);
			}

			return true;
		}

		/// <summary>
		/// Method to remove the registration of an event listener.
		/// </summary>
		/// <typeparam name="TListener">The type of the event listener.</typeparam>
		/// <param name="eventListener">The listener instance.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="eventListener"/> is <c>null</c>.</exception>
		public void UnregisterEventListener<TListener>(TListener eventListener) where TListener : EventListener
		{
			var keys = _hookRegistry.Keys;
			foreach (var key in keys)
			{
				var list = _hookRegistry[key];

				foreach (var eventListenerRegistration in list)
				{
					// mark all registrations this event listener.
					if (eventListenerRegistration.Instance == eventListener)
					{
						eventListenerRegistration.MarkedForDeletion = true;
					}
				}
				// clean all entries.
				list.RemoveAll(item => item.MarkedForDeletion);
			}
		}

		/// <summary>
		///		Method to dispatch an event.
		/// </summary>
		/// <typeparam name="TEventType">The type of the event that will be dispatched.</typeparam>
		/// <param name="eventInstance">The event instance that should be pushed to all modules.</param>
		/// <param name="fireSubEvents">Should we fire sub events</param>
		public void DispatchEvent<TEventType>(TEventType eventInstance, bool fireSubEvents) where TEventType : IDispatchableEvent
		{
			if (GameDisabled)
			{
				// perhaps cancel the event?
				LogUtility.Out("[SK] Dispatching failed: " + eventInstance.GetType().Name);
				return;
			}

			// use the type they are giving us, not the actual type. 
			var eventType = eventInstance.GetType();

			if (eventInstance.IsCancelled())
			{
				// stop processing if the event is cancelled?
				LogUtility.Out("[SK] Dispatching cancelled: " + eventInstance.GetType().Name);
				return;
			}

			if (_hookRegistry.ContainsKey(eventType))
			{
				var eventHookRegistrations = _hookRegistry[eventType];

				foreach (var eventHookRegistration in eventHookRegistrations)
				{
					var method = eventHookRegistration.EventHook.MethodToInvoke;
					var instance = eventHookRegistration.Instance;

					var tooManyArgumentsRequired = eventHookRegistration.GetRequiredMethodArguments() > 1;
					if (tooManyArgumentsRequired)
					{
						var message = string.Format("[SK] SurvivalKit warning: Skipped listener {0} for event type {1} is skipped. Argument mismatch!", instance.GetType(), eventType);
						LogUtility.Warning(message);
						continue;
					}

					try
					{
						LogUtility.Out("[SK] Invoking listener: " + instance.GetType().Name);
						method.Invoke(instance, new object[1] { eventInstance });

						if (!eventInstance.IsCancelled() && fireSubEvents)
						{
							// The current event was cancelled by the handler. No need to trigger child events?

							var subEvents = eventInstance.getSubevents();
							foreach (var dispatchableEvent in subEvents)
							{
								// might end up in an infinite loop.
								dispatchableEvent.Dispatch();
							}
						}
					}
					catch (Exception exception)
					{
						// TODO Perhaps disable this plugin?
						var wrappedException = new EventDispatchingException(eventType.Name, eventType.AssemblyQualifiedName, "Exception while invoking plugin with listener: " + instance.GetType().FullName, exception);
						LogUtility.Exception(wrappedException);
					}
				}
			}
		}

		/// <summary>
		/// Method to get all registered event types.
		/// This list will be used to match incoming events.
		/// </summary>
		/// <returns>Returns a list of <see cref="System.Type"/> instances.</returns>
		public List<Type> GetRegisteredEventTypes()
		{
			var keys = _hookRegistry.Keys;
			return new List<Type>(keys);
		}

		/// <summary>
		///	Enable the event dispatching.
		/// </summary>
		public void EnableGame()
		{
			GameDisabled = false;
			foreach (var plugin in _plugins)
			{
				plugin.onEnable();
			}
		}

		/// <summary>
		/// Disables the event dispatching.
		/// </summary>
		public void DisableGame()
		{
			GameDisabled = true;
			foreach (var plugin in _plugins)
			{
				plugin.onDisable();
			}
		}

		/// <summary>
		/// Method to register a command.
		/// </summary>
		/// <typeparam name="TListener">The type of the event listener.</typeparam>
		/// <param name="command">The command it listens to.</param>
		/// <param name="commandListener">The listener instance.</param>
		/// <returns>
		///	Returns <c>true</c> if the <see cref="TListener"/> was added to the registry of listeners.
		///	Returns <c>false</c> if the <see cref="TListener"/> was already added to the registry of listeners.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="eventListener"/> is <c>null</c>.</exception>
		public bool RegisterCommandListener(string command, ICommandListener commandListener)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			if (commandListener == null)
			{
				throw new ArgumentNullException("commandListener");
			}

			if (!_commandRegistry.ContainsKey(command))
			{
				_commandRegistry.Add(command, new List<ICommandListener>());
			}

			if (_commandRegistry[command].Contains(commandListener))
			{
				return false;
			}

			_commandRegistry[command].Add(commandListener);
			return true;
		}

		/// <summary>
		///	Method to shut down all plugins.
		/// </summary>
		public void Shutdown()
		{
			foreach (var plugin in _plugins)
			{
				plugin.onShutdown();
			}
		}

		/// <summary>
		///	Method to dispatch an event.
		/// </summary>
		/// <typeparam name="TEventType">The type of the event that will be dispatched.</typeparam>
		/// <param name="eventInstance">The event instance that should be pushed to all modules.</param>
		/// <param name="fireSubEvents">Should we fire sub events</param>
		public bool DispatchCommand(string command, Permissions.CommandSender sender, string alias, string[] arguments)
		{
			var isCancelledCommand = false;
			if (_commandRegistry.ContainsKey(command))
			{
				var listeners = _commandRegistry[command];

				foreach (var item in listeners)
				{
					try
					{
						var result = item.onCommand(sender, command, alias, arguments);
						if (result)
						{
							// command is cancelled
							isCancelledCommand = true;
							break;
						}
					}
					catch (Exception exception)
					{
						var wrappedException = new CommandDispatchingException(command, item.GetType().AssemblyQualifiedName, "An exception occured while processing the command " + command.ToLower(), exception);
						LogUtility.Exception(wrappedException);
					}

				}
			}
			else
			{
				// send the sender a message he is sending us unknown commands?
			}


			return isCancelledCommand;
		}

	}
}
