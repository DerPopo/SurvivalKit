using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Abstracts
{
	/// <summary>
	///	Abstract class all instances registered with the <see cref="EventAggregator"/> should register with.
	/// </summary>
	public abstract class EventListener : IComparable<EventListener>, IEqualityComparer<EventListener>
	{
		/// <summary>
		///	Guid identifying an event listener.
		/// </summary>
		internal Guid UniqueIdentifier { get; set; }

		/// <summary>
		///	Constructor to initialize the guid.
		/// </summary>
		protected EventListener()
		{
			UniqueIdentifier = Guid.NewGuid();
		}

		/// <summary>
		/// Get a description of the event listener.
		/// </summary>
		/// <returns>
		/// A description of the event listener
		/// </returns>
		/// <example>
		/// This event listener updates the map when a chunk is updated.
		/// </example>
		public abstract string GetDescription();

		/// <summary>
		/// Get the name of the event listener.
		/// </summary>
		/// <returns>
		/// The name of the event listener.
		/// </returns>
		/// <example>
		/// SurvivalKit - TorchFlicker event listener
		/// </example>
		public abstract string GetName();

		/// <summary>
		///		Method to gather all event hooks the listener will respond to.
		/// </summary>
		/// <returns>An enumerable of <see cref="IEventHook"/> instances.</returns>
		public abstract IEnumerable<IEventHook> GetEventHooks();

		/// <summary>
		///	Implementation of the <see cref="IComparable{EventListener}"/> interface
		/// </summary>
		/// <param name="other">The instance to compare to.</param>
		/// <returns></returns>
		public int CompareTo(EventListener other)
		{
			if (other == null)
			{
				return -1;
			}

			return UniqueIdentifier.CompareTo(other.UniqueIdentifier);
		}

		/// <summary>
		///		Method to compare two <see cref="EventListener"/> instances with eachother.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(EventListener x, EventListener y)
		{
			if (x == null || y == null)
			{
				return false;
			}

			return x.CompareTo(y) == 0;
		}

		/// <summary>
		///		Method to get a hashcode.
		/// </summary>
		/// <param name="obj">The object to get the hashcode from.</param>
		/// <returns>Returns a hashcode.</returns>
		public int GetHashCode(EventListener obj)
		{
			return obj.UniqueIdentifier.GetHashCode();
		}
	}
}
