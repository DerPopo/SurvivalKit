using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Exceptions
{
	/// <summary>
	///	Exception wrapping all exceptions that occurred when initializing a plugin.
	/// </summary>
	public class PluginInitializationException : Exception, ISurvivalKitException
	{
		/// <summary>
		/// The type of the plugin that failed to initialize.
		/// </summary>
		public string PluginType { get; set; }

		/// <summary>
		///	Constructor to initialize the exception correctly.
		/// </summary>
		/// <param name="pluginType">The plugin that couldn't be loaded.</param>
		/// <param name="message">The message describing the exception.</param>
		/// <param name="innerException">The inner exceptions.</param>
		public PluginInitializationException(string pluginType, string message, Exception innerException)
			: base(message, innerException)
		{
			PluginType = pluginType;
		}

		/// <summary>
		///	Custom ToString format so we can log this exception with as much detail as possible.
		/// </summary>
		/// <returns>Returns a formatted string with data.</returns>
		public override string ToString()
		{
			return string.Format(@"
/****** Start SurvivalKit PluginInitializationException ******/
Plugin Type: {0}

===========[ Inner Exception ]===========
Message: {1}
Stacktrace:
{2}

/****** End SurvivalKit Exception ******/
", PluginType, InnerException.Message, InnerException.StackTrace);
		}
	}
}
