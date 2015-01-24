using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Exceptions
{
	/// <summary>
	///	Exception wrapping all exceptions that occur when instantiating an event.
	/// </summary>
	public class EventInstantiationException : Exception, ISurvivalKitException
	{
		/// <summary>
		///	The type we are trying to instantiate.
		/// </summary>
		public string EventType { get; set; }

		/// <summary>
		/// Constructor to initialize the exception.
		/// </summary>
		/// <param name="firedEvent">The event that was fired by the game.</param>
		/// <param name="eventType">The event type.</param>
		/// <param name="message">The message</param>
		/// <param name="innerException">The inner exception.</param>
		public EventInstantiationException(string firedEvent, string eventType, string message, Exception innerException) 
			: base(message, innerException)
		{
			EventType = eventType;
		}

		/// <summary>
		///	Custom ToString format so we can log this exception with as much detail as possible.
		/// </summary>
		/// <returns>Returns a formatted string with data.</returns>
		public override string ToString()
		{
			return string.Format(@"
/****** Start SurvivalKit EventInstantiationException ******/
Event Type: {0}

===========[ Inner Exception ]===========
Message: {1}
Stacktrace:
{2}

/****** End SurvivalKit Exception ******/
", EventType, InnerException.Message, InnerException.StackTrace);
		}
	}
}
