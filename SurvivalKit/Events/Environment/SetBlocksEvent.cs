using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;

namespace SurvivalKit.Events.Environment
{
	/// <summary>
	/// Fired before blocks are set.
	/// </summary>
	public class SetBlocksEvent : Event, ICancellable
	{
		private bool cancelled;
		private List<BlockChangeInfo> blockPosTypes;
		private World world;

		private Event parent = null;
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Environment.SetBlocksEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (object) reserved
		/// args[1] (bool) indicates whether the event is cancelled
		/// args[2] (System.Collections.Generic.List, generic parameter = BlockChangeInfo) a list of BlockChangeInfo
		/// args[3] (World) the world
		/// </param>
		public SetBlocksEvent(Object[] args)
		{
			if (args == null || args.Length < 4)
				throw new ArgumentNullException();
			this.cancelled = (bool)args[1];
			this.blockPosTypes = (List<BlockChangeInfo>)args[2];
			this.world = (World)args[3];
		}

		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "SetBlocks";
		}

		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		public override object[] getReturnParams ()
		{
			if (this.Cancelled) {
				System.Diagnostics.StackFrame sf = new System.Diagnostics.StackTrace().GetFrame(1);
				if (sf != null && sf.GetMethod ().Module.Equals (this.GetType ().Module) && this.world != null) {
					List<BlockChangeInfo> blockPosTypes = new List<BlockChangeInfo>(this.blockPosTypes.Count);
					foreach (BlockChangeInfo oldType in this.blockPosTypes) {
						object blockVal = this.world.GetType().GetMethod("GetBlock", new Type[]{typeof(int),typeof(int),typeof(int)}).Invoke(this.world, new object[]{oldType.pos.x, oldType.pos.y, oldType.pos.z});
						blockPosTypes.Add(
							(BlockChangeInfo)(typeof(BlockChangeInfo).GetConstructor(
								new Type[]{ 
									typeof(Vector3i), blockVal.GetType(), typeof(bool)
								}
							).Invoke(new object[]{oldType.pos, blockVal, false}))
						);
					}
					foreach (GameManager gm in SKMain.SkMain.activeGameManagers()) {
						gm.SetBlocksRPC(blockPosTypes);
					}
				}
			}

			return new object[]{ this.Cancelled, this.blockPosTypes, this.world };
		}
		/// <summary>
		/// Gets whether this event supports clients.
		/// </summary>
		/// <returns><c>true</c>, if clients are supported, <c>false</c> otherwise.</returns>
		public override bool supportsClient ()
		{
			return false;
		}
		/// <summary>
		/// Sets the parent of the current Event.
		/// </summary>
		/// <param name="parent">The new parent event.</param>
		public override void setParent(Event parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Gets whether this event supports clients.
		/// </summary>
		/// <returns><c>true</c>, if clients are supported, <c>false</c> otherwise.</returns>
		public bool Cancelled {
			get { return this.cancelled; }
			set { this.cancelled = value; if (parent != null) parent.update(); }
		}
		/// <summary>
		/// Gets or sets the array of <c>BlockChangeInfo</c>.
		/// </summary>
		public BlockChangeInfo[] BlockChangeInfos {
			get { return this.blockPosTypes.ToArray(); }
			set {
				if (value == null)
					throw new ArgumentNullException ("BlockChangeInfo array is null!");
				this.blockPosTypes.Clear();
				this.blockPosTypes.AddRange(value);
				if (parent != null)
					parent.update();
			}
		}
		/// <summary>
		/// Gets the world this event applies to.
		/// </summary>
		public World _World {
			get { return this.world; }
		}
	}
}

