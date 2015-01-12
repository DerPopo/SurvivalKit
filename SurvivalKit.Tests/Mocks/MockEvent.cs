using SurvivalKit.Abstracts;
using SurvivalKit.Events;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockEvent : BaseEvent
	{
		public override object[] getReturnParams()
		{
			return new object[0];
		}
	}
}
