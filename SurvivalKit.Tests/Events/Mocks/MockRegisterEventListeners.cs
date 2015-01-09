using SurvivalKit.Events.Abstracts;
using SurvivalKit.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Events.Mocks
{
	public class MockRegisterEventListeners : IRegisterEventListeners
	{
		private List<EventListener> _collection;
		
		public MockRegisterEventListeners(List<EventListener> input)
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
	}
}
