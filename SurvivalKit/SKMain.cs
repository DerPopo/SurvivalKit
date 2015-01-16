using System;
using System.Collections.Generic;
using SurvivalKit.Events;
using SurvivalKit.Utility;

namespace SurvivalKit
{
	/// <summary>
	/// The main class of SurvivalKit.
	/// </summary>
	public class SKMain
	{
		private static SKMain mainInstance = null;
		private static List<GameManager> gamemanagers = new List<GameManager>();
		/// <summary>
		/// Gets the instance of the SurvivalKit main class
		/// </summary>
		/// <value>The SKMain class instance.</value>
		public static SKMain SkMain {
			get { return mainInstance; }
		}

		/// <summary>
		/// Gets the current GameManager
		/// </summary>
		/// <returns>The current GameManager (null if none exists).</returns>
		public GameManager currentGameManager() {
			return ((gamemanagers.Count > 0) ? gamemanagers[0] : null);
		}
		/// <summary>
		/// Gets all active GameManagers
		/// </summary>
		/// <returns>An array of active GameManagers.</returns>
		public GameManager[] activeGameManagers() {
			return gamemanagers.ToArray();
		}
		/// <summary>
		/// Gets whether the game is a client
		/// </summary>
		/// <returns><c>true</c>, if the game is a client, <c>false</c> otherwise.</returns>
		public bool gameIsClient() {
			return UnityEngine.Network.isClient && !GamePrefs.GetBool(EnumGamePrefs.DedicatedServer);
		}

		private static string getVersion()
		{
			return "0.03 dev1";
		}

		private SKMain()
		{
			Log.Out("SurvivalKit 7DTD plugin system v" + getVersion() + " by DerPopo (http://7daystodie.com/forums/) starting up...");
		}

		/// <summary>
		/// Called inside the static constructor of GameManager.
		/// </summary>
		public static void onGameInit()
		{
			if (mainInstance == null) {
				Permissions.PermissionManager.Instance = new Permissions.SimplePermissionManager();
				mainInstance = new SKMain();
				// TODO start loading plugins on a different thread?
			}
		}

		/// <summary>
		/// Called during MainMenuMono.Start() to add the SurvivalKit version to the 7 Days to Die client version string.
		/// </summary>
		public static void onMainMenuInit()
		{
			UILabel lblVersion = UnityEngine.GameObject.Find("lblVersion").GetComponent<UILabel>();
			if (!lblVersion.text.Contains("SurvivalKit"))
				lblVersion.text += " with SurvivalKit Mod v" + getVersion();
		}

		/// <summary>
		/// Called inside Awake() of GameManager. Loads and enables plugins.
		/// </summary>
		/// <param name="gmanager">The GameManager that will get activated during GameManager.Awake().</param>
		public static void onGameEnable(GameManager gmanager)
		{
			if (mainInstance != null) {
				if (gmanager != null && !gamemanagers.Contains(gmanager))
					gamemanagers.Add(gmanager);
				
				PluginLoader.GetInstance().LoadAssemblies();
				EventAggregator.GetInstance().EnableGame();
			}
		}
		/// <summary>
		/// Called inside Finalize() of GameManager. Disables all plugins.
		/// </summary>
		/// <param name="gmanager">The GameManager that will be disactivated during GameManager.Finalize().</param>
		public static void onGameDisable(GameManager gmanager)
		{
			if (mainInstance != null) {
				EventAggregator.GetInstance().DisableGame();	
				if (gmanager != null)
					gamemanagers.Remove(gmanager);
			}
		}
		/// <summary>
		/// Reserved
		/// </summary>
		public static void onGameUninit()
		{
			if (mainInstance != null) {
				EventAggregator.GetInstance().DisableGame();
				mainInstance = null;
			}
		}
		/// <summary>
		/// Reserved, doesn't work properly yet.
		/// </summary>
		public static void doReload()
		{
			// TODO implement the reload method.
			//if (mainInstance == null)
			//	onGameInit();
			//foreach (Plugin plug in mainInstance.plugMgr.getPlugins()) {
			//	PluginLoader loader = mainInstance.plugMgr.getLoader(plug);
			//	try {
			//		if (loader.isEnabled())
			//			loader.Disable();
			//	} catch (Exception){}
			//	try {
			//		loader.Unload();
			//	} catch (Exception){}
			//}
			//mainInstance.pluginsEnabled = false;
			//mainInstance.plugMgr = new PluginManager();
			//mainInstance.plugMgr.loadPlugins();
			//onGameEnable(null);
		}

		/// <summary>
		/// Determines whether a player entityId has the given UnityEngine.NetworkPlayer.
		/// </summary>
		/// <returns><c>true</c>, if the 7dtd instance is a server and if entityId has networkPlayer, <c>false</c> otherwise.</returns>
		/// <param name="player">The player entity to compare networkPlayer with.</param>
		/// <param name="networkPlayer">The NetworkPlayer.</param>
		public static bool playerEquals(EntityPlayer player, UnityEngine.NetworkPlayer networkPlayer)
		{
			if (SKMain.SkMain.gameIsClient())
				return false;
			ConnectionManager cm = SingletonMonoBehaviour<ConnectionManager>.Instance;
			if (cm == null)
				throw new Exception("null ConnectionManager");
			foreach (ClientInfo cinfo in cm.connectedClients.Values)
			{
				if (cinfo.entityId == player.entityId)
					return cinfo.networkPlayer.Equals(networkPlayer);
			}
			return false;
		}

		/// <summary>
		/// Gets the ClientInfo of a NetworkPlayer.
		/// </summary>
		/// <returns>The NetworkPlayer's ClientInfo or null if it doesn't exist.</returns>
		/// <param name="networkPlayer">The NetworkPlayer.</param>
		/// <param name="gmgr">The GameManager to use for searching the ClientInfo (default : <see cref="SurvivalKit.SKMain.currentGameManager"/>).</param>
		public static ClientInfo getClientInfo(UnityEngine.NetworkPlayer networkPlayer, GameManager gmgr = null)
		{
			if (SKMain.SkMain.gameIsClient())
				return null;
			if (gmgr == null)
			{
				gmgr = SkMain.currentGameManager();
				if (gmgr == null)
					throw new Exception("null GameManager");
			}
			ConnectionManager cm = gmgr.connectionManager;
			if (cm == null)
				throw new Exception("null ConnectionManager");
			foreach (ClientInfo cinfo in cm.connectedClients.Values)
			{
				if (cinfo.networkPlayer.Equals(networkPlayer))
				{
					return cinfo;
				}
			}
			return null;
		}
		/// <summary>
		/// Gets the ClientInfo of a NetworkPlayer.
		/// </summary>
		/// <returns>The player's ClientInfo or null if it doesn't exist.</returns>
		/// <param name="playerName">The player name.</param>
		/// <param name="gmgr">The GameManager to use for searching the ClientInfo (default : <see cref="SurvivalKit.SKMain.currentGameManager"/>).</param>
		public static ClientInfo getClientInfo(string playerName, GameManager gmgr = null)
		{
			if (SKMain.SkMain.gameIsClient())
				return null;
			if (gmgr == null)
			{
				gmgr = SkMain.currentGameManager();
				if (gmgr == null)
					throw new Exception("null GameManager");
			}
			ConnectionManager cm = gmgr.connectionManager;
			if (cm == null)
				throw new Exception("null ConnectionManager");
			foreach (ClientInfo cinfo in cm.connectedClients.Values)
			{
				if (cinfo.playerName.Equals(playerName))
				{
					return cinfo;
				}
			}
			return null;
		}
		/// <summary>
		/// Gets the player entity of a NetworkPlayer.
		/// </summary>
		/// <returns>The player entity or null if it doesn't exist.</returns>
		/// <param name="networkPlayer">The NetworkPlayer.</param>
		/// <param name="gmgr">The GameManager to use for searching the EntityPlayer (default : <see cref="SurvivalKit.SKMain.currentGameManager"/>).</param>
		public static EntityPlayer getPlayerEntity(UnityEngine.NetworkPlayer networkPlayer, GameManager gmgr = null)
		{
			if (SKMain.SkMain.gameIsClient())
				return null;
			if (gmgr == null)
			{
				gmgr = SkMain.currentGameManager();
				if (gmgr == null)
					throw new Exception("null GameManager");
			}
			ConnectionManager cm = gmgr.connectionManager;
			if (cm == null)
				throw new Exception("null ConnectionManager");
			foreach (ClientInfo cinfo in cm.connectedClients.Values)
			{
				if (cinfo.networkPlayer.Equals(networkPlayer))
				{
					foreach (EntityPlayer player in gmgr.World.playerEntities.list)
					{
						if (player.entityId == cinfo.entityId)
						{
							return player;
						}
					}
					return null;
				}
			}
			return null;
		}
	}
}

