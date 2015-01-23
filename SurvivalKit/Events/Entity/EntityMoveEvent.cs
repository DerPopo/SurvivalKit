using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using System;

namespace SurvivalKit.Events.Entities
{
	/// <summary>
	/// Fired when an entity moves (not working properly right now).
	/// </summary>
	public class EntityMoveEvent : CancellableBaseEvent
	{
		private bool cancelled;
		private UnityEngine.Vector3 pos;
		private UnityEngine.Vector3 rot;
		private object packet;
		private World world;

		private BaseEvent parent = null;
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Entity.EntityMoveEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (bool) indicates whether the event is cancelled
		/// args[1] (UnityEngine.Vector3) the position to move the entity to
		/// args[2] (rot) rotation angle 1
		/// args[3] (World,optional) the World of the entity
		/// args[4] (object,optional) the entity movement packet this event belongs to
		/// </param>
		public EntityMoveEvent(Object[] args)
		{
			if (args == null || args.Length < 5)
				throw new ArgumentNullException();
			cancelled = (bool)args[0];
			pos = (UnityEngine.Vector3)args[1];
			rot = (UnityEngine.Vector3)args[2];
			world = (World)((args.Length > 3) ? args[3] : null);
			packet = ((args.Length > 4) ? args[4] : null );
		}

		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "EntityMove";
		}

		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		public override object[] getReturnParams ()
		{
			return new object[]{ this.IsCancelled, this.pos, this.rot};
		}

		/// <summary>
		/// Sets the parent of the current Event.
		/// </summary>
		/// <param name="parent">The new parent event.</param>
		public override void setParent(BaseEvent parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Gets whether this event supports clients.
		/// </summary>
		/// <returns><c>true</c>, if clients are supported, <c>false</c> otherwise.</returns>
		public override bool IsCancelled {
			get { return this.cancelled; }
			set { this.cancelled = value; }
		}
		/// <summary>
		/// Gets or sets the new position of the entity.
		/// </summary>
		public UnityEngine.Vector3 Pos {
			get { return this.pos; }
			set { 
				if (this.packet != null)
					this.packet.GetType().GetField("pos").SetValue(this.packet, value);
				this.pos = value;
				if (this.parent != null)
					this.parent.update();
			}
		}

		/// <summary>
		/// Gets or sets the new rotation of the entity.
		/// </summary>
		public UnityEngine.Vector3 Rot {
			get { return rot; }
			set {
				if (this.packet != null)
					this.packet.GetType().GetField("rot").SetValue(this.packet, value);
				this.rot = value;
				if (this.parent != null)
					this.parent.update();
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

