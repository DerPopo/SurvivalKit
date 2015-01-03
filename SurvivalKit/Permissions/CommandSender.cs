using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Permissions
{
	/// <summary>
	/// Contains information about a command sender. <para/>
	/// Supports EntityPlayer at the moment. To add more features, override its methods.
	/// </summary>
	public abstract class CommandSender
	{


		/*/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Permissions.CommandSender"/> class.
		/// </summary>
		/// <param name="sender">The player this CommandSender instance should represent.</param>
		/// <param name="networkPlayer">The NetworkPlayer used to send messages back.</param> 
		public CommandSender(EntityPlayer sender, UnityEngine.NetworkPlayer networkPlayer)
		{
			this.sender = sender;
			this.networkPlayer = networkPlayer;
			fullName = ("player;steam=" + SingletonMonoBehaviour<Authenticator>.Instance.GetSaveFileName(sender.EntityName) + ";");
		}*/
		/*/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Permissions.CommandSender"/> class.
		/// </summary>
		/// <param name="sender">The player this CommandSender instance should represent.</param> 
		public CommandSender(EntityPlayer sender)
		{
			this.sender = sender;
			fullName = ("player;steam=" + SingletonMonoBehaviour<Authenticator>.Instance.GetSaveFileName(sender.EntityName) + ";");
		}*/

		/// <summary>
		/// Gets a string that identifies this CommandSender.
		/// </summary>
		/// <value>The string identifying this CommandSender.</value>
		public abstract string DefiniteName { get; }

		/// <summary>
		/// Gets the sender.
		/// </summary>
		/// <value>The sender.</value>
		public abstract object Sender { get; }

		/// <summary>
		/// Gets whether this CommandSender has a specific permission.
		/// </summary>
		/// <returns><c>true</c>, if this CommandSender has the permission, <c>false</c> otherwise.</returns>
		/// <param name="permission">The permission.</param>
		public virtual bool isPermitted(string permission) {
			return PermissionManager.Instance.isPermitted(this, permission);
		}

		/// <summary>
		/// Gets whether this CommandSender has operator privileges.
		/// </summary>
		/// <returns><c>true</c>, if this CommandSender has operator rights, <c>false</c> otherwise.</returns>
		public virtual bool isOp() {
			return PermissionManager.Instance.isOp(this);
		}

		/// <summary>
		/// Sends a message to the command sender.
		/// </summary>
		/// <returns><c>true</c>, if the message was sent, <c>false</c> otherwise.</returns>
		/// <param name="msg">The message to send.</param>
		/// <param name="team">The team (may be -1).</param>
		/// <param name="sender">The sender name of the message.</param>
		/// <param name="bOpenWdw"></param>
		public virtual bool sendMessage (string msg, int team, string sender, bool bOpenWdw)
		{
			return false;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="SurvivalKit.Permissions.CommandSender"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="SurvivalKit.Permissions.CommandSender"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is a <see cref="SurvivalKit.Permissions.CommandSender"/> and equal to the current
		/// <see cref="SurvivalKit.Permissions.CommandSender"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return ((obj is CommandSender) && this.DefiniteName.Equals(((CommandSender)obj).DefiniteName));
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
	}
}

