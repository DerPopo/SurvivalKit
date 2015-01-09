using SurvivalKit.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SurvivalKit.Events
{
	/// <summary>
	/// The hook to an event a method is subscribing to.
	/// </summary>
	/// <typeparam name="TDispatchableEventType">The type of the event that we subscribe to</typeparam>
	public class EventHook<TDispatchableEventType> : IEventHook where TDispatchableEventType : IDispatchableEvent
	{
		/// <summary>
		///		Constructor setting the <see cref="HookPriority"/> and <see cref="MethodToInvoke"/> property.
		/// </summary>
		/// <param name="hookPriority">The priority.</param>
		/// <param name="methodToInvoke">The method.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="methodToInvoke"/> is empty.</exception>
		public EventHook(Priority hookPriority, MethodInfo methodToInvoke)
		{
			if (methodToInvoke == null)
			{
				throw new ArgumentNullException("methodToInvoke", "You need to supply a method to invoke.");
			}

			HookPriority = hookPriority;
			MethodToInvoke = methodToInvoke;
		}

		/// <summary>
		///		Method to get the type of the event this hook should listen to.
		/// </summary>
		/// <returns>The <see cref="Type"/> of the event.</returns>
		public Type GetEventType()
		{
			return typeof(TDispatchableEventType);
		}

		/// <summary>
		///		The priority of the hook
		/// </summary>
		public Priority HookPriority { get; private set; }

		/// <summary>
		///		The method to invoke when the event is triggered.
		/// </summary>
		public MethodInfo MethodToInvoke { get; private set; }
	}
}
