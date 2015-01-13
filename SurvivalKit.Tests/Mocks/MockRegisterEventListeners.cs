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
		
		public MockPlugin(List<EventListener> input)
		{
			_collection = input;
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

		public void onEnable()
		{
		}

		public void onDisable()
		{
		}

		public void Dispose()
		{
		}
	}
}
