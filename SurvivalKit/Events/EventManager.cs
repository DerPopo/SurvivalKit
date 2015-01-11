using System;
using System.Reflection;
using System.Collections.Generic;
using SurvivalKit.Plugins;
using SurvivalKit.Plugins.Managed;
using SurvivalKit.Interfaces;

namespace SurvivalKit.Events
{
	/// <summary>
	/// Fires Events.
	/// </summary>
	public class EventManager
	{
		static EventManager()
		{
			eventTypes = new Dictionary<String, Type>();
			foreach (Type curType in Assembly.GetAssembly(typeof(SKMain)).GetExportedTypes()) {
				if (curType == null)
					continue;
				if (typeof(Event).IsAssignableFrom(curType)) {
					MethodInfo nameMethod;
					if ((nameMethod = curType.GetMethod("getName")) != null) {
						String curEventType = nameMethod.Invoke (null, new object[0]) as String;
						//Log.Out("Event type '" + curEventType + "' added.");
						eventTypes.Add (curEventType, curType);
					}
				}
			}

		}
		private static Dictionary<String,Type> eventTypes;

		//used by FireEvent(Plugin, Event, bool) to sort event handlers by priority
		private class EventMethodContainer
		{
			public Events.Listener listener;
			public MethodInfo method;
			public Object handler;
			public EventMethodContainer(Events.Listener listener, MethodInfo method, Object handler)
			{
				this.listener = listener;
				this.method = method;
				this.handler = handler;
			}
		};

		/// <summary>
		/// Fires the given event to the given event plugins.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent (or null if the plugin isn't enabled).</returns>
		/// <param name="plug">The plugin that should get notificated for the event.</param>
		/// <param name="_event">The event to fire.</param>
		/// <param name="fireSubevents">Also fires subevents if true.</param>
		public static Object[] FireEvent(Plugin plug, Event _event, bool fireSubevents = false)
		{
			if (!SKMain.SkMain.getPluginManager().getLoader(plug).isEnabled()) {
				return null;
			}
			if (!_event.supportsClient () && SKMain.SkMain.gameIsClient ()) {
				return _event.getReturnParams();
			}
			List<EventMethodContainer> eventMethods = new List<EventMethodContainer>();
			if (typeof(NetPlugin).IsAssignableFrom(plug.GetType()))
			{
				NetPlugin np = plug as NetPlugin;
				foreach (Object handler in np.EventHandlers)
				{
					foreach (MethodInfo mi in handler.GetType().GetMethods())
					{
						//if (mi.ReturnType != typeof(void))
						//	continue;
						foreach (Object attr in mi.GetCustomAttributes(true))
						{
							if (attr is Events.Listener)
							{
								Events.Listener listenerAttribute = (Events.Listener)attr;
								ParameterInfo[] paramInfo = mi.GetParameters();
								if (paramInfo.Length == 1 && _event.GetType().IsAssignableFrom(paramInfo[0].ParameterType))
								{
									if (listenerAttribute.priority < Priority.LOWEST || listenerAttribute.priority > Priority.MONITOR)
										Log.Error("The event handler of '" + (SKMain.SkMain.getPluginManager().getLoader(plug) as NetLoader).name + "' has an invalid priority!");
									else
										eventMethods.Add(new EventMethodContainer(listenerAttribute, mi, handler));
								}
								break;
							}
						}
					}
				}
			}
			EventMethodContainer[] sortedMethods;
			Array.Sort((sortedMethods = eventMethods.ToArray()), (item1,item2) => ((int)item1.listener.priority).CompareTo((int)item2.listener.priority));
			for (int i = 0; i < sortedMethods.Length; i++)
			{
				EventMethodContainer curMethod = sortedMethods[i];
				try {
					curMethod.method.Invoke(curMethod.handler, new Object[]{ _event });
				} catch (Exception e) {
					Log.Error("An exception occured in the event handler of '" + (SKMain.SkMain.getPluginManager().getLoader(plug) as NetLoader).name + "' : ");
					Log.Exception(e);
				}
			}
			if (fireSubevents) {
				foreach (Event curSubevent in _event.getSubevents()) {
					FireEvent (plug, curSubevent, true);
				}
			}
			return _event.getReturnParams();
		}

		/// <summary>
		/// Fires the given event to enabled plugins.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		/// <param name="_event">The event to fire.</param>
		/// <param name="fireSubevents">Also fires subevents if true.</param>
		public static Object[] FireEvent(Event _event, bool fireSubevents = true)
		{
			foreach (Plugin plug in SKMain.SkMain.getPluginManager().getPlugins()) {
				FireEvent(plug, _event);
			}
			if (fireSubevents) {
				foreach (Event curSubevent in _event.getSubevents()) {
					FireEvent(curSubevent, true);
				}
			}
			return _event.getReturnParams();
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
			Event _event = null;
			foreach (KeyValuePair<String, Type> curEventType in eventTypes) {
				if (curEventType.Key.ToLower().Equals(name.ToLower())) {
					foreach (ConstructorInfo ci in curEventType.Value.GetConstructors())
					{
						ParameterInfo[] parameters = ci.GetParameters();
						if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(pars.GetType()))
						{
							try {
								_event = ci.Invoke(new Object[]{pars}) as Event;
							} catch (Exception e) {
								throw new Exception("An exception occured inside the constructor for event " + name, e);
							}
							break;
						}
					}
					//_event = curEventType.Value.GetConstructor(new Type[]{ typeof(Object[]) }).Invoke(pars) as Event;
					if (_event == null)
						throw new Events.Exceptions.EventNotFoundException("No matching constructor for event " + name + " found.");
					if (!(_event is ICancellable) || (_event is ICancellable && !((ICancellable)_event).IsCancelled)) {
						FireEvent (_event);
					}
					return _event.getReturnParams();
				}
			}
			throw new Events.Exceptions.EventNotFoundException("Event " + name + " not found.");
		}
	}
}

