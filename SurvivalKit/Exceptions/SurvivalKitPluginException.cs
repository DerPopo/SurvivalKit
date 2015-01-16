using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Exceptions
{
	/// <summary>
	///	Exception that wraps all exceptions that occurred when delegating an event.
	/// </summary>
	public class SurvivalKitPluginException : Exception
	{
		/// <summary>
		///	The name of the event that has been fired.
		///	This name is provided by the game engine.
		/// </summary>
		public string EventName;
		
		/// <summary>
		/// The type of the event that was found as a match.
		/// </summary>
		public string EventType;

		/// <summary>
		///	Constructor to initialize the exception correctly.
		/// </summary>
		/// <param name="eventName">The name of the event, provided by the game engine.</param>
		/// <param name="eventType">The type of the event that was found matching the name.</param>
		/// <param name="message">The message describing the exception.</param>
		/// <param name="innerException">The inner exceptions.</param>
		public SurvivalKitPluginException(string eventName, string eventType, string message, Exception innerException)
			: base(message, innerException)
		{
			EventName = eventName;
			EventType = eventType;
		}

		/// <summary>
		///	Custom ToString format so we can log this exception with as much detail as possible.
		/// </summary>
		/// <returns>Returns a formatted string with data.</returns>
		public override string ToString()
		{
			return string.Format(@"
/****** Start SurvivalKit Exception ******/
Message: {0}
Event Name: {1}
Event Type: {2}

===========[ Inner Exception ]===========
Message: {3}
Stacktrace:
{4}

/****** End SurvivalKit Exception ******/
", Message, EventName, EventType, InnerException.Message, InnerException.StackTrace);
		}
	}
}
