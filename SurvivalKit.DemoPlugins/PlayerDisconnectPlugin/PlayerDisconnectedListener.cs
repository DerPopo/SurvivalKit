using SurvivalKit.Abstracts;
using SurvivalKit.Events;
using SurvivalKit.Events.Player;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerDisconnectPlugin
{
	public class PlayerDisconnectedListener : EventListener
	{
		private Stream WriteStream;

		public void Load()
		{
			var fileInfo = new FileInfo("PlayerDisconnectLog.txt");
			WriteStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
		}

		public override string GetDescription()
		{
			return "Log data regarding the set player disconnect event.";
		}

		public override string GetName()
		{
			return "DemoPlugins.PlayerDisconectPlugin.PlayerDisconnectListener";
		}

		public override IEnumerable<SurvivalKit.Interfaces.IEventHook> GetEventHooks()
		{
			var eventHook = new EventHook<PlayerDisconnectedEvent>(Priority.NORMAL, GetType().GetMethod("HandleEvent"));
			return new List<IEventHook> { eventHook };
		}

		public void HandleEvent(PlayerDisconnectedEvent eventInstance)
		{
			var information = eventInstance.EntityPlayer.EntityName;

			var formatted = string.Format("[{0}] - Player '{1}' disconnected.", DateTime.Now, information, System.Environment.NewLine);
			var bytes = ASCIIEncoding.UTF8.GetBytes(formatted);
			WriteStream.Write(bytes, 0, bytes.Length);
			WriteStream.Flush();
		}

		public void ShutDown()
		{
			WriteStream.Close();
			WriteStream.Dispose();
		}
	}
}
