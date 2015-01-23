using System;
using System.Collections.Generic;
using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;

namespace SurvivalKit.Events.Network
{
	/// <summary>
	/// Fired when a RPC function gets called.
	/// </summary>
	public class RPCEvent : CancellableBaseEvent
	{
		private bool cancelled;
		private string name;
		private object[] args;

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Network.ReadPacketFromBufEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (bool) indicates whether the event is cancelled
		/// args[1] (string) the name of the fired RPC.
		/// args[2] (object) the first argument of the fired RPC. (following args contain following RPC arguments)
		/// </param>
		public RPCEvent(Object[] args)
		{
			if (args == null || args.Length < 3)
				throw new ArgumentNullException();
			this.cancelled = (bool)args[0];
			this.name = (string)args[1];
			this.args = new object[args.Length-2];
			for (int i = 2; i < args.Length; i++)
				this.args[i - 2] = args[i];

			if (this.name.Equals("RPC_ChatMessage") && this.args[0] != null) { //string _msg, int _teamNo, string _playerName, bool _bOpenWdw
				string message = (string)this.args[0];
				if (message.Length <= 0)
					this.cancelled = true;
				else if (message.Length > 1 && message[0] == '/') {
					string[] splitCmd = message.Split(new char[]{' '});
					string[] cmdArgs = new string[splitCmd.Length-1];
					for (int i = 1; i < splitCmd.Length; i++)
						cmdArgs[i-1] = splitCmd[i];
					splitCmd[0] = splitCmd[0].Substring(1);
					UnityEngine.NetworkPlayer networkPlayer = ((UnityEngine.NetworkMessageInfo)this.args[this.args.Length - 1]).sender;
					foreach (GameManager curGmgr in SKMain.SkMain.activeGameManagers())
					{
						EntityPlayer playerEntity = SKMain.getPlayerEntity(networkPlayer, curGmgr);
						if (playerEntity == null)
							continue;

						string alias = splitCmd[0], command = splitCmd[0];
						var commandSender = new SurvivalKit.Permissions.PlayerCommandSender(playerEntity, networkPlayer);

						var isCancelled = EventAggregator.GetInstance().DispatchCommand(command, commandSender, alias, cmdArgs);
						if (isCancelled)
						{
							cancelled = true;

							// Why do we set these arguments to null? Shouldn't we set the properties 'name' and 'rpcclass' to null?
							this.args[0] = null;
							this.args[1] = null;
							break;
						}
					}
				}
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
			return "RPC";
		}
		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>Returns an object array of parameters to pass to the caller of fireEvent.</returns>
		public override object[] getReturnParams ()
		{
			object[] ret = new object[2+args.Length];
			ret[0] = cancelled;
			ret[1] = this.name;
			for (int i = 0; i < args.Length; i++)
				ret[i + 2] = args[i];
			return ret;
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
		/// Gets the name of the fired RPC.
		/// </summary>
		public string Name {
			get { return this.name; }
		}
		/// <summary>
		/// Gets the arguments of the fired RPC.
		/// </summary>
		public object[] Args {
			get { return this.args; }
		}

		/// <summary>
		/// Sets an argument at the given index.
		/// </summary>
		/// <param name="index">
		/// The index inside the argument list.
		/// </param>
		/// <param name="arg">
		/// The new argument value.
		/// </param>
		/// <exception cref="System.InvalidCastException">Thrown if the new argument type is not compatible with the previous argument type</exception>
		/// <exception cref="System.IndexOutOfRangeException">Thrown if the index is out of the range of the array</exception> 
		public void setArg(int index, object arg)
		{
			if (this.args.Length > index || index < 0) {
				if (this.args[index].GetType().IsAssignableFrom(arg.GetType())) {
					this.args[index] = arg;
				} else {
					throw new InvalidCastException("RPCEvent.setArg : the type of index " + index + " for " + Name + " is not compatible with " + arg.GetType ().Name);
				}
			} else {
				throw new IndexOutOfRangeException("RPCEvent.setArg : index " + index + " is out of range (maximum " + (this.args.Length-1) + ")");
			}
		}
	}
}

