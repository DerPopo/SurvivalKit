namespace SurvivalKit.Events
{
	/// <summary>
	/// The enumeration describing the different priority levels.
	/// </summary>
	public enum Priority
	{
		/// <summary>
		/// Event listeners with this priority get called first.
		/// </summary>
		LOWEST = 1,
		/// <summary>
		/// Low priority that is handled after the lowest.
		/// </summary>
		LOW = 2,
		/// <summary>
		/// The default event listener priority.
		/// </summary>
		NORMAL = 3,
		/// <summary>
		/// High priority that has a high influence on the event.
		/// </summary>
		HIGH = 4,
		/// <summary>
		/// The last priority handled that have the highest influence on the event.
		/// </summary>
		HIGHEST = 5,
		/// <summary>
		/// Event listeners with this priority only are allowed to read the results of the event.
		/// </summary>
		MONITOR = 6,
	}
}

