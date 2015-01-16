using SurvivalKit.Events;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;

namespace SurvivalKit.Utility
{
	/// <summary>
	///	Class for comparing <see cref="IEventHook"/> instances using the <see cref="IEventHook.HookPriority"/>.
	/// </summary>
	internal class EventListenerRegistrationComparer : IComparer<EventListenerRegistration>
	{
		/// <summary>
		///	Method to compare the priorities of two event hooks.
		/// </summary>
		/// <param name="leftObject">The left object</param>
		/// <param name="rightObject">The right object.</param>
		/// <returns>The comparison of the event priorities.</returns>
		public int Compare(EventListenerRegistration leftObject, EventListenerRegistration rightObject)
		{
			if (leftObject == null)
			{
				throw new ArgumentNullException("leftObject");
			}

			if (rightObject == null)
			{
				throw new ArgumentNullException("rightObject");
			}

			var leftPriority = (int)leftObject.EventHook.HookPriority;
			var rightPriority = (int)rightObject.EventHook.HookPriority;

			return leftPriority.CompareTo(rightPriority);
		}
	}
}
