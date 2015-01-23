using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockClassWithConstructorException
	{
		public MockClassWithConstructorException()
		{
			throw new InvalidOperationException("We need this for a unit test.");
		}

	}
}
