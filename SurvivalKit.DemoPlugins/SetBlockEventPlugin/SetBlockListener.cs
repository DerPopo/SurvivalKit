using SurvivalKit.Abstracts;
using SurvivalKit.Events;
using SurvivalKit.Events.Environment;
using SurvivalKit.Interfaces;
using SurvivalKit.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SetBlockEventPlugin
{
	public class SetBlockListener : EventListener, IDisposable
	{
		private Stream WriteStream;

		public SetBlockListener()
		{
			var fileInfo = new FileInfo("SetBlockLog.txt");
			WriteStream = fileInfo.OpenWrite();
			
		}

		public override string GetDescription()
		{
			return "Log data regarding the set block event.";
		}

		public override string GetName()
		{
			return "DemoPlugins.SetBlockListener.SetBlockListener";
		}

		public override IEnumerable<SurvivalKit.Interfaces.IEventHook> GetEventHooks()
		{
			var eventHook = new EventHook<SetBlocksEvent>(Priority.NORMAL, GetType().GetMethod("HandleEvent"));
			return new List<IEventHook> { eventHook };
		}

		public void HandleEvent(SetBlocksEvent eventInstance)
		{
			var information = eventInstance.BlockChangeInfos;

			foreach (var blockChangeInformation in information)
			{
				var formatted = string.Format("[{0}] - Block modified at [{1},{2},{3}]", DateTime.Now, blockChangeInformation.pos.x,blockChangeInformation.pos.y,blockChangeInformation.pos.z);
				var bytes = ASCIIEncoding.UTF8.GetBytes(formatted);
				WriteStream.Write(bytes, 0, bytes.Length);
				WriteStream.Flush();
			}
		}


		public void Dispose()
		{
			WriteStream.Close();
			WriteStream.Dispose();
		}
	}
}
