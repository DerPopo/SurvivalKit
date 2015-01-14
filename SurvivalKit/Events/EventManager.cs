using System;
using System.Reflection;
using SurvivalKit.Interfaces;
using SurvivalKit.Extensions;
using SurvivalKit.Abstracts;

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
		private static IEventAggregator _aggregator;

		/// <summary>
		///	Constructor to initialize the <see cref="EventAggregator"/>.
		/// </summary>
		static EventManager()
		{
			_aggregator = EventAggregator.GetInstance();
		}
		
		///// <summary>
		///// Fires the given event to the given event plugins.
		///// </summary>
		///// <returns>Returns an object array of parameters to pass to the caller of fireEvent (or null if the plugin isn't enabled).</returns>
		///// <param name="plug">The plugin that should get notificated for the event.</param>
		///// <param name="_event">The event to fire.</param>
		///// <param name="fireSubevents">Also fires subevents if true.</param>
		//public static Object[] FireEvent(Plugin plug, BaseEvent _event, bool fireSubevents = false)
		//{
		//	if (!SKMain.SkMain.getPluginManager().getLoader(plug).isEnabled())
		//	{
		//		return null;
		//	}
			
		//	List<EventMethodContainer> eventMethods = new List<EventMethodContainer>();
		//	if (typeof(NetPlugin).IsAssignableFrom(plug.GetType()))
		//	{
		//		NetPlugin np = plug as NetPlugin;
		//		foreach (Object handler in np.EventHandlers)
		//		{
		//			foreach (MethodInfo mi in handler.GetType().GetMethods())
		//			{
		//				//if (mi.ReturnType != typeof(void))
		//				//	continue;
		//				foreach (Object attr in mi.GetCustomAttributes(true))
		//				{
		//					if (attr is Events.Listener)
		//					{
		//						Events.Listener listenerAttribute = (Events.Listener)attr;
		//						ParameterInfo[] paramInfo = mi.GetParameters();
		//						if (paramInfo.Length == 1 && _event.GetType().IsAssignableFrom(paramInfo[0].ParameterType))
		//						{
		//							if (listenerAttribute.priority < Priority.LOWEST || listenerAttribute.priority > Priority.MONITOR)
		//								Log.Error("The event handler of '" + (SKMain.SkMain.getPluginManager().getLoader(plug) as NetLoader).name + "' has an invalid priority!");
		//							else
		//								eventMethods.Add(new EventMethodContainer(listenerAttribute, mi, handler));
		//						}
		//						break;
		//					}
		//				}
		//			}
		//		}
		//	}
		//	EventMethodContainer[] sortedMethods;
		//	Array.Sort((sortedMethods = eventMethods.ToArray()), (item1, item2) => ((int)item1.listener.priority).CompareTo((int)item2.listener.priority));
		//	for (int i = 0; i < sortedMethods.Length; i++)
		//	{
		//		EventMethodContainer curMethod = sortedMethods[i];
		//		try
		//		{
		//			curMethod.method.Invoke(curMethod.handler, new Object[] { _event });
		//		}
		//		catch (Exception e)
		//		{
		//			Log.Error("An exception occured in the event handler of '" + (SKMain.SkMain.getPluginManager().getLoader(plug) as NetLoader).name + "' : ");
		//			Log.Exception(e);
		//		}
		//	}
		//	if (fireSubevents)
		//	{
		//		foreach (BaseEvent curSubevent in _event.getSubevents())
		//		{
		//			FireEvent(plug, curSubevent, true);
		//		}
		//	}
		//	return _event.getReturnParams();
		//}

		///// <summary>
		///// Fires the given event to enabled plugins.
		///// </summary>
		///// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		///// <param name="_event">The event to fire.</param>
		///// <param name="fireSubevents">Also fires subevents if true.</param>
		//public static Object[] FireEvent(BaseEvent _event, bool fireSubevents = true)
		//{
		//	foreach (Plugin plug in SKMain.SkMain.getPluginManager().getPlugins())
		//	{
		//		FireEvent(plug, _event);
		//	}
		//	if (fireSubevents)
		//	{
		//		foreach (BaseEvent curSubevent in _event.getSubevents())
		//		{
		//			FireEvent(curSubevent, true);
		//		}
		//	}
		//	return _event.getReturnParams();
		//}

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
				if (curEventType.Name.ToLower().Equals(lowerCaseName))
				{
					try
					{
						Activator.CreateInstance(curEventType, new object[] { pars });
					}
					catch (Exception ex)
					{
						throw new Exception("An exception occurred inside the constructor for event " + name, ex);
					}

					if (_event == null)
					{
						// Event dispatched from the game should always be processed. 
						throw new Exceptions.EventNotFoundException("No matching constructor for event " + name + " found.");
					}
					
					if (!_event.IsCancelled())
					{
						_event.Dispatch();
					}
					return _event.getReturnParams();
				}
			}

			throw new Exceptions.EventNotFoundException("Event " + name + " not found.");
		}
	}
}

