using System;
using System.Reflection;
using SurvivalKit.Interfaces;
using SurvivalKit.Extensions;
using SurvivalKit.Abstracts;
using SurvivalKit.Exceptions;
using SurvivalKit.Utility;

namespace SurvivalKit.Events
{
	/// <summary>
	/// Class that is called by the game.
	/// All events are patches so they push their data to the FireEvent method.
	/// The overload with the string argument is used for calls to SurvivalKit.
	/// </summary>
	public class EventManager
	{
		/// <summary>
		///	Our instance of the <see cref="IEventAggregator"/>.
		/// </summary>
		internal static IEventAggregator _aggregator;

		/// <summary>
		///	Constructor to initialize the <see cref="EventAggregator"/>.
		/// </summary>
		static EventManager()
		{
			_aggregator = EventAggregator.GetInstance();
		}

		/// <summary>
		/// Creates and fires a new event (created by name and pars).
		/// </summary>
		/// <param name="name">
		/// The name of the event to fire (each Event has a static getName() function that returns this name).
		/// </param>
		/// <param name="pars">
		/// An object array of data to pass to the event.
		/// </param>
		/// <returns>
		/// Returns an object array of parameters to pass to the caller of fireEvent.
		/// </returns>
		public static Object[] FireEvent(string name, Object[] pars)
		{
			var lowerCaseName = name.ToLower();
			IDispatchableEvent _event = null;
			var eventTypes = _aggregator.GetRegisteredEventTypes();
			foreach (Type curEventType in eventTypes)
			{
				//Log.Out("Event : " + curEventType.Name);
				if (curEventType.Name.ToLower().Equals(lowerCaseName))
				{
					try
					{
						_event = Activator.CreateInstance(curEventType, new object[] { pars }) as IDispatchableEvent;
						if (_event == null)
							throw new ArgumentException("The event " + name + "is not compatible to IDispatchableEvent!");
					}
					catch (Exception ex)
					{
						var exception = new SurvivalKitPluginException(lowerCaseName, curEventType.AssemblyQualifiedName, "An exception occurred inside the constructor for an event", ex);
						LogUtility.Exception(ex);
						throw exception;
					}
				
					if (!_event.IsCancelled())
					{
						_event.Dispatch();
					}
					return _event.getReturnParams();
				}
			}
			//Log.Out("Event \"" + name + "\" not found!");

			// Removed exception. An event can be implemented, but there could be nobody to listen to it.
			return pars;
		}
	}
}

