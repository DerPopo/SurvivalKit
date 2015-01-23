using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalKit.Abstracts;

namespace SurvivalKit.Tests.Abstracts
{
	[TestClass]
	public class EventListenerTests
	{
		[TestMethod]
		public void CompareTo_Equal()
		{
			var instance = new Mocks.MockEventListener(true, false);
			
			Assert.AreEqual(0, instance.CompareTo(instance));
		}

		[TestMethod]
		public void CompareTo_Null()
		{
			var instance = new Mocks.MockEventListener(true, false);

			Assert.AreEqual(-1, instance.CompareTo(null));
		}

		[TestMethod]
		public void CompareTo_Other()
		{
			var instance = new Mocks.MockEventListener(true, false);
			var otherInstance = new Mocks.MockEventListener(true, false);

			Assert.AreEqual(instance.UniqueIdentifier.CompareTo(otherInstance.UniqueIdentifier), instance.CompareTo(otherInstance));
		}

		[TestMethod]
		public void Equals_SingleArgument()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = new Mocks.MockEventListener(true, false);

			Assert.IsFalse(instance.Equals(otherInstance));
		}

		[TestMethod]
		public void Equals_Equal()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = new Mocks.MockEventListener(true, false);

			Assert.IsTrue(instance.Equals(instance, instance));
		}

		[TestMethod]
		public void Equals_NotEqual()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = new Mocks.MockEventListener(true, false);

			Assert.IsFalse(instance.Equals(instance, otherInstance));
		}

		[TestMethod]
		public void Equals_RightNull()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = null;

			Assert.IsFalse(instance.Equals(instance, otherInstance));
		}

		[TestMethod]
		public void Equals_LeftNull()
		{
			EventListener instance = null;
			EventListener otherInstance = new Mocks.MockEventListener(true, false);


			Assert.IsFalse(otherInstance.Equals(instance, otherInstance));
		}
	}
}
