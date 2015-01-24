using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Exceptions
{
	/// <summary>
	///	Exception wrapping all exceptions that occur during the loading of an assembly.
	/// </summary>
	public class AssemblyLoadException : Exception, ISurvivalKitException
	{
		/// <summary>
		///	The location of the assembly that is loaded
		/// </summary>
		public string AssemblyLocation { get; set; }

		/// <summary>
		///	Constructor to initialize the exception.
		/// </summary>
		/// <param name="assemblyLocation">The location of the assembly that is being loaded.</param>
		/// <param name="message">The message</param>
		/// <param name="exception">The inner exception.</param>
		public AssemblyLoadException(string assemblyLocation, string message, Exception exception) : base(message, exception)
		{
			AssemblyLocation = assemblyLocation;
		}

		/// <summary>
		///	Custom ToString format so we can log this exception with as much detail as possible.
		/// </summary>
		/// <returns>Returns a formatted string with data.</returns>
		public override string ToString()
		{
			return string.Format(@"
/****** Start SurvivalKit AssemblyLoadException ******/
Message: {0}
Assembly location: {1}

===========[ Inner Exception ]===========
Message: {2}
Stacktrace:
{3}

/****** End SurvivalKit Exception ******/
", AssemblyLocation, Message, InnerException.Message, InnerException.StackTrace);
		}
	}
}
