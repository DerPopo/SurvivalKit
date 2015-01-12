using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SurvivalKit.Events.Network
{
	/// <summary>
	/// Called before a packet is processed.
	/// </summary>
	public class ProcessPacketEvent : CancellableBaseEvent
	{
		private bool cancelled;
		private object packet;
		private World world;
		private INetConnectionCallbacks connCallbacks;
		private List<BaseEvent> subevents;

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Network.ProcessPacketEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (object) the packet
		/// args[1] (bool) indicates whether the event is cancelled
		/// args[2] (World)	the World the packet targets
		/// args[3] (INetConnectionCallbacks) callbacks to functions in GameManager used for some packets
		/// </param>
		public ProcessPacketEvent(Object[] args)
		{
			if (args == null || args.Length < 4)
				throw new ArgumentNullException();
			packet = args[0];
			cancelled = (bool)args[1];
			world = (World)args[2];
			connCallbacks = (INetConnectionCallbacks)args[3];

			subevents = new List<BaseEvent>();
			MethodInfo miPkType = this.packet.GetType().GetMethod("GetPkType");
			if (miPkType == null || !typeof(PackageType).IsAssignableFrom(miPkType.ReturnType)) {
				Log.Error("A packet of type '" + this.packet.GetType().FullName + "' hasn't got a valid GetPkType() function!");
			}
			PackageType packType = (PackageType)miPkType.Invoke(this.packet, new object[0]);
			switch (packType) {
				case PackageType.EntityPosAndRot:
				case PackageType.EntityTeleport:
					subevents.Add(
						new Entities.EntityMoveEvent(
							new Object[] {
								null,
								false,
								packet.GetType ().GetField ("pos").GetValue (packet),
								packet.GetType ().GetField ("rot").GetValue (packet),
								world,
								packet,
							}
						)
					);
					break;
				case PackageType.SetBlock:

					subevents.Add(
						new Events.Environment.SetBlocksEvent(
							new Object[] {
								null,
								false,
								packet.GetType ().GetField ("fd0000").GetValue (packet),
								world,
							}
						)
					);
					break;
			}
			foreach (BaseEvent curEvent in subevents) {
				curEvent.setParent (this);
			}
		}
		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "ProcessPacket";
		}
		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		public override object[] getReturnParams ()
		{
			this.update();
			return new object[]{ this.IsCancelled };
		}

		/// <summary>
		/// A function that gets all subevents.
		/// </summary>
		/// <returns>Returns an event array containing all direct subevents.</returns>
		/// <example>A ProcessPacketEvent containing a SetBlock packet returns an instance of SetBlocksEvent.</example>
		public override IDispatchableEvent[] getSubevents()
		{
			return subevents.ToArray();
		}
		/// <summary>
		/// A function called to make sure an event gets notificated for changes in a subevent.
		/// </summary>
		/// <example>A SetBlocksEvent calls parent.update() (parent most likely is a ProcessPacketEvent) when the SetBlocksEvent gets cancelled.</example>
		public override void update()
		{
			foreach (BaseEvent _event in this.subevents) {
				_event.update();
				if (_event is ICancellable) {
					if ((_event as ICancellable).IsCancelled == true)
						IsCancelled = true;
				}
				if (_event is Events.Environment.SetBlocksEvent) {
					List<BlockChangeInfo> listBlockPosType = new List<BlockChangeInfo>();
					listBlockPosType.AddRange ((_event as Events.Environment.SetBlocksEvent).BlockChangeInfos);
					packet.GetType().GetField("fd0000").SetValue(packet, listBlockPosType);
				}
				if (_event is Events.Entities.EntityMoveEvent) {
					Events.Entities.EntityMoveEvent __event = (_event as Events.Entities.EntityMoveEvent);
					packet.GetType().GetField("pos").SetValue(packet, __event.Pos);
					packet.GetType().GetField("rot").SetValue(packet, __event.Rot);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this event is cancelled.
		/// </summary>
		/// <value><c>true</c> if this instance cancelled, <c>false</c> otherwise.</value>
		public override bool IsCancelled
		{
			get { return this.cancelled; }
			set { this.cancelled = value; }
		}
		/// <summary>
		/// Gets the packet that belongs to this event.
		/// </summary>
		public object PacketClass {
			get { return this.packet; }
		}
		/// <summary>
		/// Gets the world this event applies for.
		/// </summary>
		public World WorldArg {
			get { return this.world; }
		}
		/// <summary>
		/// Gets the <c>INetConnectionCallbacks</c> (usually a <c>GameManager</c>) that get notificated for some packets.
		/// </summary>
		public INetConnectionCallbacks ConnectionCallbacks {
			get { return this.connCallbacks; }
		}
	}
}

