using SurvivalKit.Events;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Abstracts
{
	/// <summary>
	/// The base class of all events which are cancellable.
	/// </summary>
	public abstract class CancellableBaseEvent : BaseEvent, ICancellable
	{
		/// <summary>
		/// Gets or sets whether this event is cancelled.
		/// </summary>
		public abstract bool IsCancelled{ get; set;}
	}
}
