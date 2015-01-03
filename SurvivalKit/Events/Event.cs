using System;

namespace SurvivalKit.Events
{
	/// <summary>
	/// The baseclass of all events.
	/// </summary>
	public abstract class Event
	{
		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>
		/// Returns an object array of parameters to pass to the caller of fireEvent.
		/// </returns>
		public abstract Object[] getReturnParams();

		/// <summary>
		/// Gets whether this event supports clients.
		/// </summary>
		/// <returns><c>true</c>, if clients are supported, <c>false</c> otherwise.</returns>
		public abstract bool supportsClient();

		/// <summary>
		/// A function that gets all subevents.
		/// </summary>
		/// <returns>
		/// Returns an event array containing all direct subevents.
		/// </returns>
		/// <example>A ProcessPacketEvent containing a SetBlock packet returns an instance of SetBlocksEvent.</example>
		public virtual Event[] getSubevents(){return new Event[0];}

		/// <summary>
		/// A function called to make sure an event gets notificated for changes in a subevent.
		/// </summary>
		/// <example> A SetBlocksEvent calls parent.update() (parent most likely is a ProcessPacketEvent) when the SetBlocksEvent gets cancelled. </example>
		public virtual void update(){}

		/// <summary>
		/// Sets the parent of the current Event.
		/// </summary>
		/// <param name="parent">
		/// The new parent event.
		/// </param>
		public virtual void setParent(Event parent){}
	}
}

