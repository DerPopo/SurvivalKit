using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Events.Interfaces
{
	/// <summary>
	///		Interface for classes that register <see cref="EventListener"/> instances.
	/// </summary>
	interface IRegisterEventListeners
	{
		/// <summary>
		///		Method to register event listener.
		/// </summary>
		/// <param name="eventAggregator">The aggregator the listeners should register with.</param>
		void RegisterEventListeners(IEventAggregator eventAggregator);
	}
}
