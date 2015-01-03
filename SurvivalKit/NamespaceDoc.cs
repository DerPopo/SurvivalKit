using System;

namespace SurvivalKit
{
	/// <summary><para/>
	/// <b>SurvivalKit, a plugin system for 7 Days to Die</b><para/>
	/// Version 0.02
	/// <para/>
	/// For making plugins, choose an IDE (preferable one that uses Mono, like <see href="http://monodevelop.com/Download">Xamarin Studio</see>).<para/>
	/// Create a class that overrides <see cref="SurvivalKit.Plugins.Managed.NetPlugin"/> and override the methods onLoad, onEnable, onDisable and getAuthors of <see cref="SurvivalKit.Plugins.Plugin"/>.<para/>
	/// If you want to add a listener to events, call <see cref="SurvivalKit.Plugins.Managed.NetPlugin.registerEvents(object)"/>.<para/>
	/// The object it takes is an instance of a class containing public void methods tagged with the attribute <see cref="SurvivalKit.Events.Listener"/> that have a specific Event.<para/>
	/// You also need to add a xml settings file having the same name (TestPlugin.dll should have a TestPlugin.xml). The base tag is called plugin. 
	/// It contains the tags 'name' (the name displayed while loading the plugin), 'main' (the full name of the class overriding NetPlugin) and 'enabled' (whether the plugin should load).<para/>
	/// Currently supported events are :<para/>
	/// 	<see cref="SurvivalKit.Events.Entity.EntityMoveEvent"/> - [server only] Not checked if it works.<para/>
	/// 	<see cref="SurvivalKit.Events.Environment.SetBlocksEvent"/> - [server only] Fired after a client put or damaged a block. If cancelled, the block gets reset.<para/>
	/// 	<see cref="SurvivalKit.Events.Misc.TorchFlickerUpdateEvent"/> - [client only] Fired before the game updates torch flicker. If cancelled, the torch flicker won't get updated.<para/>
	/// 	<see cref="SurvivalKit.Events.Network.ProcessPacketEvent"/> - [server only] Fired before the server processes a packet. If cancelled, the packet won't get processed.<para/>
	/// 	<see cref="SurvivalKit.Events.Network.ReadPacketFromBufEvent"/> - [server only] Fired after a packet was read from a buffer.<para/>
	/// 	<see cref="SurvivalKit.Events.Network.RPCEvent"/> - [server only] Fired when a client sent a RPC message. If cancelled, the RPC message won't get processed.<para/>
	/// 	<see cref="SurvivalKit.Events.Network.WritePacketToBufEvent"/> [server only] Fired before the server writes a packet to a buffer. If cancelled, the packet won't get written to the buffer.<para/>
	///  <para/>
	/// To add a listener to a command, call <see cref="SurvivalKit.Plugins.Managed.NetPlugin.registerCommand(string,SurvivalKit.Commands.CommandListener)"/>.<para/>
	/// You can check for permissions (the name should begin with your plugin's name) with <see cref="SurvivalKit.Permissions.CommandSender.isPermitted(string)"/> and 
	/// <see cref="SurvivalKit.Permissions.CommandSender.isOp()"/>.
	/// </summary>
	class NamespaceDoc
	{
		public NamespaceDoc ()
		{
		}
	}
}

