using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Exceptions
{
	/// <summary>
	///	Exception that wraps all exceptions that occurred when dispatching a command.
	/// </summary>
	public class CommandDispatchingException : Exception, ISurvivalKitException
	{
		/// <summary>
		///	The name of the command that has been dispatched.
		/// </summary>
		public string CommandName;

		/// <summary>
		/// The type of the event that was found as a match.
		/// </summary>
		public string CommandListenerType;

		/// <summary>
		///	Constructor to initialize the exception correctly.
		/// </summary>
		/// <param name="commandName">The name of the command that was dispatched.</param>
		/// <param name="commandListener">The type of the command listener that caused the exception.</param>
		/// <param name="message">The message describing the exception.</param>
		/// <param name="innerException">The inner exceptions.</param>
		public CommandDispatchingException(string commandName, string commandListener, string message, Exception innerException)
			: base(message, innerException)
		{
			CommandName = commandName;
			CommandListenerType = commandListener;
		}

		/// <summary>
		///	Custom ToString format so we can log this exception with as much detail as possible.
		/// </summary>
		/// <returns>Returns a formatted string with data.</returns>
		public override string ToString()
		{
			return string.Format(@"
/****** Start SurvivalKit CommandDispatchingException ******/
Message: {0}
Command Name: {1}
Command Listener Type: {2}

===========[ Inner Exception ]===========
Message: {3}
Stacktrace:
{4}

/****** End SurvivalKit Exception ******/
", Message, CommandName, CommandListenerType, InnerException.Message, InnerException.StackTrace);
		}
	}
}
