using System;

namespace SurvivalKit.Permissions
{
	/// <summary>
	/// Describes a player that sent a command.
	/// </summary>
	public class PlayerCommandSender : CommandSender
	{
		private string fullName = null;
		private object sender;
		private UnityEngine.NetworkPlayer networkPlayer;
		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Permissions.PlayerCommandSender"/> class able to be used as a <see cref="SurvivalKit.Permissions.CommandSender"/>.
		/// </summary>
		/// <param name="sender">The player this CommandSender instance should represent.</param>
		/// <param name="networkPlayer">The NetworkPlayer used to send messages back.</param> 
		public PlayerCommandSender(EntityPlayer sender, UnityEngine.NetworkPlayer networkPlayer)
		{
			this.sender = sender;
			this.networkPlayer = networkPlayer;
			fullName = ("player;steam=" + SKMain.getClientInfo(networkPlayer).playerId + ";");
		}

		/// <summary>
		/// Makes a DefiniteName of a name of an unknown type (either definite name, player name or steamid).
		/// </summary>
		/// <returns>The definite name. Note that if a steamid is given and another player is named by that id, the wrong name may get returned.</returns>
		/// <param name="name">The name that may be a definite name, a player name or a steamid.</param>
		public static string MakeDefiniteName(string name)
		{
			if (name.StartsWith ("player;steam=") && name.EndsWith (";"))
				return name;
			foreach (EntityPlayer curPlayer in SKMain.SkMain.currentGameManager().World.playerEntities.list) {
				string curSteamId = SKMain.getClientInfo(curPlayer.EntityName).playerId;
				if (curSteamId == null || curSteamId.Length == 0)
					continue;
				if (curPlayer.EntityName.ToLower().Equals(name.ToLower()) || curSteamId.Equals(name))
					return ("player;steam=" + curSteamId + ";");
			}
			return null;
		}

		/// <summary>
		/// Gets a string that identifies this CommandSender.
		/// </summary>
		/// <value>The string identifying this CommandSender.</value>
		public override string DefiniteName {
			get {
				return this.fullName;
			}
		}

		/// <summary>
		/// Gets the sender.
		/// </summary>
		/// <value>The sender.</value>
		public override object Sender {
			get { return this.sender; }
		}

		/// <summary>
		/// Gets the NetworkPlayer.
		/// </summary>
		/// <value>The sender.</value>
		public UnityEngine.NetworkPlayer NetworkPlayer {
			get { return this.networkPlayer; }
		}

		/// <summary>
		/// Sends a chat message to the player.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="msg">The message to send.</param>
		/// <param name="team">The team (may be -1).</param>
		/// <param name="sender">The sender name of the message.</param>
		/// <param name="bOpenWdw"></param>
		public override bool sendMessage (string msg, int team, string sender, bool bOpenWdw)
		{
			object[] RPCPars = new object[] {msg, team, sender, bOpenWdw};
			SKMain.SkMain.currentGameManager().GetRPCNetworkView().RPC("RPC_ChatMessage", this.NetworkPlayer, RPCPars);
			return true;
		}
	}
}

