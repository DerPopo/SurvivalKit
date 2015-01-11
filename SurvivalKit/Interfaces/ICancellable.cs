using System;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	/// An interface for cancellable events.
	/// </summary>
	public interface ICancellable
	{
		/// <summary>
		/// Gets or sets whether this event is cancelled.
		/// </summary>
		bool Cancelled { get; set; }
	}
}

