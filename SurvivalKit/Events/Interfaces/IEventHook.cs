using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Events.Interfaces
{
	public interface IEventHook
	{
		/// <summary>
		///	Method to get the type of the event this hook should listen to.
		/// </summary>
		/// <returns>The <see cref="Type"/> of the event.</returns>
		Type GetEventType();

		/// <summary>
		///	The priority of the hook
		/// </summary>
		SurvivalKit.Events.Priority HookPriority { get; }

		/// <summary>
		///	The method to invoke when the event is triggered.
		/// </summary>
		System.Reflection.MethodInfo MethodToInvoke { get; }
	}
}
