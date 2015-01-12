using SurvivalKit.Events;
using SurvivalKit.Interfaces;

namespace SurvivalKit.Extensions
{
	/// <summary>
	///	Extension method for the <see cref="IDispatchableEvent"/> interface.
	/// </summary>
	public static class IDispatchableEventExtensions
	{
		/// <summary>
		///	Dispatch an event.
		/// </summary>
		/// <typeparam name="TEventType">The event type.</typeparam>
		/// <param name="eventInstance">The instance of the event that should be dispatched</param>
		/// <example>
		/// This method lets you dispatch events easily using the following syntax:
		/// <c>
		/// var instance = new MyCustomDispatchableEvent();
		/// instance.Dispatch();
		/// </c>
		/// </example>
		public static void Dispatch<TEventType>(this TEventType eventInstance, bool fireSubEvents = true) where TEventType : IDispatchableEvent
		{
			var eventAggregator = EventAggregator.GetInstance();
			eventAggregator.DispatchEvent(eventInstance, fireSubEvents);
		}

		/// <summary>
		///	Check if an event is cancelled.
		/// </summary>
		/// <typeparam name="TEventType">The event type.</typeparam>
		/// <param name="eventInstance">The instance of the event which might be cancelled.</param>
		/// <example>
		/// This method lets you check whether events are cancelled easily using the following syntax:
		/// <c>
		/// var instance = new MyCustomDispatchableEvent();
		/// if(instance.IsCancelled())
		/// {
		///		// do stuff.
		/// }
		/// </c>
		/// </example>
		public static bool IsCancelled<TEventType>(this TEventType eventInstance) where TEventType : IDispatchableEvent
		{
			var cast = eventInstance as ICancellable;
			if (cast == null)
			{
				return false;
			}
			else
			{
				return cast.IsCancelled;
			}
		}
	}
}
