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
		///		Dispatch an event.
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
		public static void Dispatch<TEventType>(this TEventType eventInstance) where TEventType : IDispatchableEvent
		{
			var eventAggregator = EventAggregator.GetInstance();
			eventAggregator.DispatchEvent(eventInstance);
		}
	}
}
