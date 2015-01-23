using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	/// <summary>
	///	Mock class to test the InstanceResolver.
	/// </summary>
	public class MockClassWithConstructorArguments
	{
		/// <summary>
		///	Constructor with an required argument
		/// </summary>
		/// <param name="myRequiredObject">The required argument.</param>
		public MockClassWithConstructorArguments(object myRequiredObject)
		{
			myRequiredObject.ToString();
		}
	}
}
