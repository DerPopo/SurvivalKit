using SurvivalKit.Events.Abstracts;
using SurvivalKit.Events;
using System;
using System.Collections.Generic;
using System.Text;
using SurvivalKit.Events.Interfaces;

namespace SurvivalKit.Events
{
	/// <summary>
	///		The registration of the first event listener.
	/// </summary>
	internal class EventListenerRegistration
	{
		/// <summary>
		///	Constructor to initialize the an event listener registration.
		/// </summary>
		/// <param name="instance">The instance of the <see cref="EventListener"/></param>
		/// <param name="eventHook">The hook to implement.</param>
		public EventListenerRegistration(EventListener instance, IEventHook eventHook)
		{
			Instance = instance;
			EventHook = eventHook;
		}

		/// <summary>
		///	The instance of the <see cref="EventListener"/>
		/// </summary>
		public EventListener Instance;

		/// <summary>
		///	The event hook to implement.
		/// </summary>
		public IEventHook EventHook;

		/// <summary>
		///	All method arguments, including optional ones.
		/// </summary>
		private int? _methodArguments;

		/// <summary>
		///	The arguments that are required when calling this method.
		/// </summary>
		private int? _nonOptionArguments;

		/// <summary>
		///	Method to retrieve the total amount of arguments of the method, including the optional ones.
		/// </summary>
		/// <returns>Returns the amount of arguments in the <see cref="EventHook.MethodToInvoke"/></returns>
		public int GetMethodArguments()
		{
			if (!_methodArguments.HasValue)
			{
				CountMethodArguments();
			}

			return _methodArguments.Value;
		}

		/// <summary>
		///	Method to retrieve the amount of arguments that are required.
		/// </summary>
		/// <returns>Returns the amount of required arguments in the <see cref="EventHook.MethodToInvoke"/></returns>
		public int GetRequiredMethodArguments()
		{
			if (!_nonOptionArguments.HasValue)
			{
				CountMethodArguments();
			}

			return _nonOptionArguments.Value;
		}

		/// <summary>
		///	Method to process the method arguments for once.
		/// </summary>
		private void CountMethodArguments()
		{
			var optionalArguments = 0;
			var methodArguments = EventHook.MethodToInvoke.GetParameters();
			foreach (var argument in methodArguments)
			{
				if (argument.IsOptional)
				{
					optionalArguments++;
				}
			}

			_methodArguments = methodArguments.Length;
			_nonOptionArguments = methodArguments.Length - optionalArguments;
		}
	}
}
