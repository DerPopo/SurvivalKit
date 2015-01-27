using SurvivalKit.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Events.Player
{
	/// <summary>
	///	Event that is published when a user disconnects.
	/// </summary>
	public class PlayerDisconnectedEvent : BaseEvent
	{
		/// <summary>
		/// The ID of the client of the player.
		/// </summary>
		public readonly int ClientId;

		/// <summary>
		/// The ID of the entity of the player.
		/// </summary>
		public readonly int EntityId;

		/// <summary>
		///	The <see cref="EntityPlayer"/> instance related to the player.
		/// </summary>
		public readonly EntityPlayer EntityPlayer;

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Player.PlayerDisconnectedEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (int) clientId
		/// </param>
		public PlayerDisconnectedEvent(Object[] args)
		{
			if (args == null || args.Length < 3)
				throw new ArgumentNullException();
			
			ClientId = (int)args[0];
			EntityId = (int)args[1];
			EntityPlayer = (EntityPlayer)args[2];

		}

		/// <summary>
		///	Method to get the return params.
		///	This event does not use return params.
		/// </summary>
		/// <returns>Returns an empty object array</returns>
		public override object[] getReturnParams()
		{
			return new object[0];
		}
	}
}
