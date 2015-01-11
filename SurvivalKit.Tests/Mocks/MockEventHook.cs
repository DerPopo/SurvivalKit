using SurvivalKit.Events;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockEventHook :IEventHook
	{
		private Type _type;
		private Priority _priority;
		private MethodInfo _methodInfo;

		public MockEventHook(Type type, Priority prio, MethodInfo methodInfo)
		{
			_type = type;
			_priority = prio;
			_methodInfo = methodInfo;
		}

		public Type GetEventType()
		{
			return _type;
		}

		public SurvivalKit.Events.Priority HookPriority
		{
			get { return _priority; }
		}

		public System.Reflection.MethodInfo MethodToInvoke
		{
			get { return _methodInfo; }
		}
	}
}
