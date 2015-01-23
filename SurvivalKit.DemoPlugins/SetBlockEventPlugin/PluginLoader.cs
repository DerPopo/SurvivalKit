using SurvivalKit.Interfaces;

namespace SetBlockEventPlugin
{
    public class PluginLoader : IPlugin
    {
		private SetBlockListener Listener;

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
			Listener = new SetBlockListener();
		}

		public void onEnable()
		{
		}

		public void onDisable()
		{
		}

		public void Dispose()
		{
			Listener.Dispose();
		}
	}
}
