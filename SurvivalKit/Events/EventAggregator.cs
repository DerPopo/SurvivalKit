using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using SurvivalKit.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
		///	The private instance of the <see cref="IResolveInstances"/> implementation.
		/// </summary>
		private IResolveInstances _instanceResolver;

		/// <summary>
		///	Registry of hooks.
		/// </summary>
		private PrioritizedEventListenerDictionary<Type, EventListenerRegistration> _hookRegistry;

		/// <summary>
		/// Should we prevent calls to the log library?
		/// Used for the testing framework.
		/// </summary>
		private bool _preventLogging = false;

		/// <summary>
		///		Private constructor for the singleton.
		/// </summary>
		private EventAggregator(IResolveInstances instanceResolver, bool preventLogging = false)
		{
			_preventLogging = preventLogging;
			_instanceResolver = instanceResolver;
			_hookRegistry = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(new EventListenerRegistrationComparer());

			// Get all instances and try to gather all event listeners.
			var registerEventListeners = _instanceResolver.ResolveInstances<IPlugin>();

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
		/// <param name="eventListener">The listener intance.</param>
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


			// TODO check if this eventlistener isn't already registered.

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
						eventListenerRegistration.markForDeletion = true;
					}
				}
				// clean all entries.
				list.RemoveAll(item => item.markForDeletion);
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

						if (fireSubEvents)
						{
							//instance.getSub
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
		///	Helper method to see if an event is cancelled.
		/// </summary>
		/// <typeparam name="TEventType">The type of the vent.</typeparam>
		/// <param name="eventInstance">The instance that might be cancelled.</param>
		/// <returns>Returns <c>true</c> if the event is cancelled.</returns>
		private bool IsCancelled<TEventType>(TEventType eventInstance) where TEventType : IDispatchableEvent
		{
			return eventInstance.GetType().IsAssignableFrom(typeof(ICancellable)) && ((ICancellable)eventInstance).IsCancelled;
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
