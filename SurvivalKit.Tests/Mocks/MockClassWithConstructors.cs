using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	/// <summary>
	///	Mock class to test the InstanceResolver.
	/// </summary>
	public class MockClassWithConstructors
	{
		/// <summary>
		///	Constructor with a required argument
		/// </summary>
		/// <param name="myRequiredObject">The required argument.</param>
		public MockClassWithConstructors(object myRequiredObject)
		{
			myRequiredObject.ToString();
		}

		/// <summary>
		///	Constructor with an optional argument
		/// </summary>
		/// <param name="myRequiredObject">The required argument.</param>
		public MockClassWithConstructors(bool myOptionalBool = false)
		{
			myOptionalBool.ToString();
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public MockClassWithConstructors() : this(false)
		{

		}
	}
}
