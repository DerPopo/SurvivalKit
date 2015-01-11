using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockInstanceResolver : IResolveInstances 
	{
		private List<IRegisterEventListeners> _returnValue;

		public MockInstanceResolver(List<IRegisterEventListeners> returnValue)
		{
			_returnValue = returnValue;
		}

		public List<TInstance> ResolveInstances<TInstance>()
		{
			return _returnValue == null ? null: _returnValue.Cast<TInstance>().ToList();
		}
	}
}
