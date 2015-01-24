using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurvivalKit.Utility
{
	/// <summary>
	///	Utility class that will prevent exceptions caused by logging when running unit tests.
	/// </summary>
	public static class LogUtility
	{
		private static bool LogToConsole = false;

		internal static void SetLogToConsole()
		{
			LogToConsole = true;
		}

		/// <summary>
		///	Method to log a message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void Out(string message)
		{
			if (LogToConsole)
			{
				Debug.WriteLine(message);
			}
			else
			{
				Log.Out(message);
			}
		}

		/// <summary>
		///	Log an exception.
		/// </summary>
		/// <param name="exception">The exception to log</param>
		public static void Exception(Exception exception)
		{
			if(LogToConsole)
			{
				if (exception is ISurvivalKitException)
				{
					// custom exception should be formatted using the ToString override.
					Debug.WriteLine(exception.ToString());
				}
				else
				{
					// no exception of ours, just log the default way.
					Debug.WriteLine(exception);
				}
			} else {
				if (exception is ISurvivalKitException)
				{
					// custom exception should be formatted using the ToString override.
					Log.Error(exception.ToString());
				}
				else
				{
					// no exception of ours, just log the default way.
					Log.Exception(exception);
				}
			}
		}

		/// <summary>
		///	Log a warning.
		/// </summary>
		/// <param name="message">The message to log</param>
		public static void Warning(string message)
		{
			if (LogToConsole)
			{
				Debug.WriteLine("Warning: " + message);
			}
			else
			{
				Log.Warning(message);
			}
		}

		/// <summary>
		///	Log an error.
		/// </summary>
		/// <param name="message">The message to log</param>
		public static void Error(string message)
		{
			if (LogToConsole)
			{
				Debug.WriteLine("Error: " + message);
			}
			else
			{
				Log.Error(message);
			}
		}
	}
}
