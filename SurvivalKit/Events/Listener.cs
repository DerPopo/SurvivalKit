using System;

namespace SurvivalKit.Events
{
	/// <summary>
	/// The event listener attribute which plugin listeners have to use.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class Listener : System.Attribute
	{
		/// <summary>
		/// The priority of the plugin (not implemented yet).
		/// </summary>
		public Priority priority;
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Listener"/> attribute.
		/// </summary>
		public Listener ()
		{
			this.priority = Priority.NORMAL;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Listener"/> attribute and sets a priority (not implemented yet).
		/// </summary>
		/// <param name="priority">The priority of the event listener.</param>
		public Listener(Priority priority)
		{
			this.priority = priority;
		}
	}
}

