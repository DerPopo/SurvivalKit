using SurvivalKit.Events;
using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SurvivalKit.Tests.Events.Mocks
{
	public class MockEventListener : EventListener
	{
		private bool _returnData;
		private bool _returnNull;

		public MockEventListener(bool returnData, bool returnNull)
		{
			_returnData = returnData;
			_returnNull = returnNull;
		}

		public override string GetDescription()
		{
			return "MockEventListener";
		}

		public override string GetName()
		{
			return "MockEventListener";
		}

		public override IEnumerable<SurvivalKit.Interfaces.IEventHook> GetEventHooks()
		{
			if (_returnNull)
			{
				return null;
			}

			var list = new List<IEventHook>();
			if (_returnData)
			{
				var method = GetType().GetMethods().First();
				var hook = new EventHook<MockEvent>(Priority.NORMAL, method); 
				list.Add(hook);
			}

			return list;
		}
	}
}
