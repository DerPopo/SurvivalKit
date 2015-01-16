using SurvivalKit.Interfaces;

namespace SurvivalKit.Abstracts
{
	/// <summary>
	/// The base class of all events which are cancellable.
	/// </summary>
	public abstract class CancellableBaseEvent : BaseEvent, ICancellableEvent
	{
		/// <summary>
		/// Gets or sets whether this event is cancelled.
		/// </summary>
		public abstract bool IsCancelled{ get; set;}
	}
}
