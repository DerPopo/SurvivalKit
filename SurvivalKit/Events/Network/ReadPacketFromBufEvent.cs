using SurvivalKit.Abstracts;
using System;

namespace SurvivalKit.Events.Network
{
	/// <summary>
	/// Fired after the game read a packet from a buffer.
	/// </summary>
	public class ReadPacketFromBufEvent : BaseEvent
	{
		private object packet;

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Network.ReadPacketFromBufEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (object) the packet that belongs to this event
		/// </param>
		public ReadPacketFromBufEvent(Object[] args)
		{
			if (args == null || args.Length < 1)
				throw new ArgumentNullException();
			packet = args[0];
		}

		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "ReadPacketFromBuf";
		}
		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		public override object[] getReturnParams ()
		{
			return new object[]{  };
		}

		/// <summary>
		/// Gets the packet that belongs to this event.
		/// </summary>
		public object PacketClass {
			get { return this.packet; }
		}
	}
}

