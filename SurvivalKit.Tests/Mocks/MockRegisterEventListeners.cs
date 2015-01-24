using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockPlugin : IPlugin
	{
		private List<EventListener> _collection;
		private List<ICommandListener> _commandCollection;
		private string _command;
		
		public MockPlugin(List<EventListener> input, List<ICommandListener> commandListeners, string command)
		{
			_collection = input;
			_commandCollection = commandListeners;
			_command = command;
		}

		public void RegisterEventListeners(IEventAggregator eventAggregator)
		{
			if (_collection == null)
			{
				EventListener listener = null;
				eventAggregator.RegisterEventListener(listener);
			}

			foreach (var item in _collection)
			{
				eventAggregator.RegisterEventListener(item);
			}
		}


		public string[] getAuthors()
		{
			return new string[] { "Mock" };
		}

		public void onLoad()
		{
		}
		
		public void onShutdown()
		{
		}

		public void onEnable()
		{
		}

		public void onDisable()
		{
		}

		public void Dispose()
		{
		}


		public void RegisterCommandListeners(IEventAggregator eventAggregator)
		{
			if (_commandCollection == null)
			{
				ICommandListener listener = null;
				eventAggregator.RegisterCommandListener(_command,listener);
			}

			foreach (var item in _commandCollection)
			{
				eventAggregator.RegisterCommandListener(_command,item);
			}
		}


		public string getPluginName()
		{
			return "MockPlugin";
		}
	}
}
