using SurvivalKit.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Events.Abstracts
{
	/// <summary>
	///	Abstract class all instances registered with the <see cref="EventAggregator"/> should register with.
	/// </summary>
	public abstract class EventListener
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
	}
}
