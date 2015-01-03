using System;

namespace SurvivalKit.Events.Exceptions
{
	/// <summary>
	/// Throwed by EventManager.fireEvent(String,Object[]) when a event type was not found.
	/// </summary>
	public class EventNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Exceptions.EventNotFoundException"/> class.
		/// </summary>
		public EventNotFoundException() : base() {}
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Exceptions.EventNotFoundException"/> class.
		/// </summary>
		/// <param name="message">An error message to show.</param>
		public EventNotFoundException(string message) : base(message) {}
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Exceptions.EventNotFoundException"/> class.
		/// </summary>
		/// <param name="message">An error message to show.</param>
		/// <param name="innerException">The exception that caused this exception.</param>
		public EventNotFoundException(string message, Exception innerException) : base(message, innerException) {}
	}
}

