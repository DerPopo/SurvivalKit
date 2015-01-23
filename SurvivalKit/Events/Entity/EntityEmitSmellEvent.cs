using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using System;

namespace SurvivalKit.Events.Entities
{
	/// <summary>
	/// Fired when an entity emits a smell.
	/// </summary>
	public class EntityEmitSmellEvent : CancellableBaseEvent
	{
		private bool cancelled;
		private UnityEngine.Vector3 pos;
		private Entity instigator;
		private string smellName;

		private BaseEvent parent = null;
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Entity.EntityMoveEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (bool) indicates whether the event is cancelled
		/// args[1] (Entity) the entity that caused this event
		/// args[2] (Vector3) the position of the smell
		/// args[3] (string) the name of the smell
		/// </param>
		public EntityEmitSmellEvent(Object[] args)
		{
			if (args == null || args.Length < 5)
				throw new ArgumentNullException();
			cancelled = (bool)args[0];
			pos = (UnityEngine.Vector3)args[1];
			instigator = (Entity)args[2];
			smellName = (string)args[3];
		}

		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "EntityEmitSmell";
		}

		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		public override object[] getReturnParams ()
		{
			return new object[]{ this.IsCancelled, this.pos, this.instigator, this.smellName};
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
				this.pos = value; 
				if (this.parent != null)
					this.parent.update();
			}
		}

		/// <summary>
		/// Gets or sets the Entity that caused this event.
		/// </summary>
		public Entity Instigator {
			get { return instigator; }
			set {
				this.instigator = value;
				if (this.parent != null)
					this.parent.update();
			}
		}

		/// <summary>
		/// Gets or sets the smell type.
		/// </summary>
		public string SmellName {
			get { return this.smellName; }
			set { 
				this.smellName = value;
				if (this.parent != null)
					this.parent.update();
			}
		}
	}
}

