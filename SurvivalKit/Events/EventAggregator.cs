using SurvivalKit.Events.Abstracts;
using SurvivalKit.Events.Interfaces;
using System;
using System.Collections.Generic;
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
		///		The singleton instance of the <see cref="IEventAggregator"/>.
		/// </summary>
		private static IEventAggregator _instance;

		/// <summary>
		///		The private instance of the <see cref="IResolveInstances"/> implementation.
		/// </summary>
		private IResolveInstances _instanceResolver;

		/// <summary>
		///		Registry of hooks.
		/// </summary>
		private Dictionary<Type, List<EventListenerRegistration>> _hookRegistry;

		/// <summary>
		///		Private constructor for the singleton.
		/// </summary>
		private EventAggregator(IResolveInstances instanceResolver)
		{
			_instanceResolver = instanceResolver;
			_hookRegistry = new Dictionary<Type, List<EventListenerRegistration>>();

			// Get all instances and try to gather all event listeners.
			var registerEventListeners = _instanceResolver.ResolveInstances<IRegisterEventListeners>();
			foreach (var registerEventListenerInstance in registerEventListeners)
			{
				try
				{
					registerEventListenerInstance.RegisterEventListeners(this);
				}
				catch (Exception exception)
				{
					Log.Error(exception.Message);
					Log.Error(exception.StackTrace);
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
				_instance = new EventAggregator(instanceResolver);
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

				if (!_hookRegistry.ContainsKey(type))
				{

					_hookRegistry.Add(type, new List<EventListenerRegistration>());
				}

				_hookRegistry[type].Add(registration);
				// TODO implement priority sort
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
			foreach (var keyValue in _hookRegistry)
			{
				// loop all events, and remove all instances.
				keyValue.Value.RemoveAll(item => item.Instance.UniqueIdentifier == eventListener.UniqueIdentifier);
			}
		}

		/// <summary>
		///		Method to dispatch an event.
		/// </summary>
		/// <typeparam name="TEventType">The type of the event that will be dispatched.</typeparam>
		/// <param name="eventInstance">The event instance that should be pushed to all modules.</param>
		public void DispatchEvent<TEventType>(TEventType eventInstance, params object[] arguments) where TEventType : IDispatchableEvent
		{
			var eventType = eventInstance.GetType();

			if (_hookRegistry.ContainsKey(eventType))
			{
				var eventHookRegistrations = _hookRegistry[eventType];

				foreach (var eventHookRegistration in eventHookRegistrations)
				{
					var method = eventHookRegistration.EventHook.MethodToInvoke;
					var instance = eventHookRegistration.Instance;

					var tooManyArguments = arguments.Length > eventHookRegistration.GetMethodArguments();
					var tooLittleArguments = arguments.Length < eventHookRegistration.GetRequiredMethodArguments();
					if (tooManyArguments || tooLittleArguments)
					{
						var message = string.Format("SurvivalKit warning: Skipped listener {0} for event type {1} is skipped. Argument mismatch!", instance.GetType(), eventType);
						Log.Warning(message);
						continue;
					}

					try
					{
						method.Invoke(instance, arguments);
					}
					catch (Exception exception)
					{
						Log.Error(exception.Message);
						Log.Error(exception.StackTrace);
					}
				}
			}
		}
	}
}
