using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	///	Interface all events should implement if they want to be dispatched by the <see cref="EventAggregator"/>.
	/// </summary>
	public interface IDispatchableEvent
	{
		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>
		/// Returns an object array of parameters to pass to the caller of fireEvent.
		/// </returns>
		Object[] getReturnParams();

		/// <summary>
		/// A function that gets all subevents.
		/// </summary>
		/// <returns>
		/// Returns an event array containing all direct subevents.
		/// </returns>
		/// <example>A ProcessPacketEvent containing a SetBlock packet returns an instance of SetBlocksEvent.</example>
		IDispatchableEvent[] getSubevents();
	}
}
