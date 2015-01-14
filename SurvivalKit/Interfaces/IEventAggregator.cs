using SurvivalKit.Abstracts;
using System;
using System.Collections.Generic;

namespace SurvivalKit.Interfaces
{
	public interface IEventAggregator
	{
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
		bool RegisterEventListener<TListener>(TListener eventListener) where TListener : EventListener;

		/// <summary>
		/// Method to remove the registration of an event listener.
		/// </summary>
		/// <typeparam name="TListener">The type of the event listener.</typeparam>
		/// <param name="eventListener">The listener instance.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="eventListener"/> is <c>null</c>.</exception>
		void UnregisterEventListener<TListener>(TListener eventListener) where TListener : EventListener;

		/// <summary>
		///	Method to dispatch an event.
		/// </summary>
		/// <typeparam name="TEventType">The type of the event that will be dispatched.</typeparam>
		/// <param name="eventInstance">The event instance that should be pushed to all modules.</param>
		/// <param name="fireSubEvents">Should we fire sub events</param>
		void DispatchEvent<TEventType>(TEventType eventInstance, bool fireSubEvents) where TEventType : IDispatchableEvent;

		/// <summary>
		/// Method to get all registered event types.
		/// This list will be used to match incoming events.
		/// </summary>
		/// <returns>Returns a list of <see cref="System.Type"/> instances.</returns>
		List<Type> GetRegisteredEventTypes();
	}
}
