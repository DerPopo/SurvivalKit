using SurvivalKit.Abstracts;
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
		///	The singleton instance of the <see cref="IEventAggregator"/>.
		/// </summary>
		private static IEventAggregator _instance;

		/// <summary>
		///	Registry of hooks.
		/// </summary>
		private readonly PrioritizedEventListenerDictionary<Type, EventListenerRegistration> _hookRegistry;

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

			// Get all instances and try to gather all event listeners.
			var registerEventListeners = instanceResolver.ResolveInstances<IPlugin>();

			if (registerEventListeners == null || registerEventListeners.Count == 0)
			{
				LogMessage("No IRegisterEventListeners instances found.",true);
				return;
			}

			foreach (var registerEventListenerInstance in registerEventListeners)
			{
				try
				{
					registerEventListenerInstance.RegisterEventListeners(this);
				}
				catch (Exception exception)
				{
					LogMessage(exception.Message);
					LogMessage(exception.StackTrace);
				}
			}
		}

		/// <summary>
		///	Helper method for logging.
		///	Calling the log library from a unit test project will cause errors.
		///	This method could be moved to a separate helper in order to re-use it in other classes.
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="isWarning">Should we log it as a warning? If not, we will log it as an error.</param>
		private void LogMessage(string message, bool isWarning = false)
		{
			if (_preventLogging)
			{
				Debug.WriteLine(message);
			}
			else
			{
				if(isWarning)
				{
					Log.Warning(message);
				} else {
					Log.Error(message);
				}
				
			}
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
					if(eventListenerRegistration.Instance == eventListener)
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
			// use the type they are giving us, not the actual type. 
			var eventType = typeof(TEventType);


			if (eventInstance.IsCancelled())
			{
				// stop processing if the event is cancelled?
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
						var message = string.Format("SurvivalKit warning: Skipped listener {0} for event type {1} is skipped. Argument mismatch!", instance.GetType(), eventType);
						LogMessage(message, true);
						continue;
					}

					try
					{
						method.Invoke(instance, new object[1] {eventInstance});

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
						LogMessage(exception.Message, true);
						LogMessage(exception.StackTrace, true);
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
	}
}
