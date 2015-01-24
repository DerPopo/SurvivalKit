using SurvivalKit.Interfaces;
using SurvivalKit.Utility;

namespace SetBlockEventPlugin
{
    public class PluginLoader : IPlugin
    {
		private SetBlockListener Listener;

		public PluginLoader()
		{
			Listener = new SetBlockListener();
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
			return "SetBlockEvent Demo plugin";
		}

		public string[] getAuthors()
		{
			return new string[]{ "DerPopo", "GWM"};
		}

		public void onLoad()
		{
			LogUtility.Out("[SKDP] Load");
			Listener.Load();
		}

		public void onShutdown()
		{
			LogUtility.Out("[SKDP] Shutdown");
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
