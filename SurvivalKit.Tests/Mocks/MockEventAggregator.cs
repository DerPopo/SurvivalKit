using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockEventAggregator : IEventAggregator
	{
		private List<Type> _eventTypes;

		public MockEventAggregator(List<Type> eventTypes = null)
		{
			if (eventTypes == null)
			{
				_eventTypes = new List<Type>();
			}
			else
			{
				_eventTypes = eventTypes;
			}
		}

		public bool RegisterCommandListener(string command, ICommandListener commandListener)
		{
			throw new NotImplementedException();
		}

		public void DispatchEvent<TEventType>(TEventType eventInstance, bool fireSubEvents) where TEventType : IDispatchableEvent
		{
			throw new NotImplementedException();
		}

		public bool DispatchCommand(string command, Permissions.CommandSender sender, string alias, string[] arguments)
		{
			throw new NotImplementedException();
		}

		public List<Type> GetRegisteredEventTypes()
		{
			return _eventTypes;
		}

		public void EnableGame()
		{
			throw new NotImplementedException();
		}

		public void DisableGame()
		{
			throw new NotImplementedException();
		}

		public bool RegisterEventListener<TListener>(TListener eventListener) where TListener : SurvivalKit.Abstracts.EventListener
		{
			throw new NotImplementedException();
		}

		public void UnregisterEventListener<TListener>(TListener eventListener) where TListener : SurvivalKit.Abstracts.EventListener
		{
			throw new NotImplementedException();
		}
	}
}
