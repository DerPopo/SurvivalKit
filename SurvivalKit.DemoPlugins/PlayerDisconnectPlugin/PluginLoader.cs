using SurvivalKit.Interfaces;
using SurvivalKit.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerDisconnectPlugin
{
	public class PluginLoader : IPlugin
	{
		private PlayerDisconnectedListener Listener;

		public PluginLoader()
		{
			Listener = new PlayerDisconnectedListener();
		}

		public void RegisterEventListeners(IEventAggregator eventAggregator)
		{
			eventAggregator.RegisterEventListener(Listener);
		}

		public void RegisterCommandListeners(IEventAggregator eventAggregator)
		{
			return;
		}

		public string getPluginName()
		{
			return "PlayerDisconnectedEvent Demo plugin";
		}

		public string[] getAuthors()
		{
			return new string[] { "DerPopo", "GWM" };
		}

		public void onLoad()
		{
			LogUtility.Out("[SKDP-PDP] Load");
			Listener.Load();
		}

		public void onShutdown()
		{
			LogUtility.Out("[SKDP-PDP] Shutdown");
			Listener.ShutDown();
		}

		public void onEnable()
		{
		}

		public void onDisable()
		{
		}

		public void Dispose()
		{
			Listener = null;
		}
	}
}
