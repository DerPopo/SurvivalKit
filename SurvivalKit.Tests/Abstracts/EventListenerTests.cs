using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalKit.Abstracts;

namespace SurvivalKit.Tests.Abstracts
{
	[TestClass]
	public class EventListenerTests
	{
		[TestMethod]
		public void EventListenerTests_CompareTo_Equal()
		{
			var instance = new Mocks.MockEventListener(true, false);
			
			Assert.AreEqual(0, instance.CompareTo(instance));
		}

		[TestMethod]
		public void EventListenerTests_CompareTo_Null()
		{
			var instance = new Mocks.MockEventListener(true, false);

			Assert.AreEqual(-1, instance.CompareTo(null));
		}

		[TestMethod]
		public void EventListenerTests_CompareTo_Other()
		{
			var instance = new Mocks.MockEventListener(true, false);
			var otherInstance = new Mocks.MockEventListener(true, false);

			Assert.AreEqual(instance.UniqueIdentifier.CompareTo(otherInstance.UniqueIdentifier), instance.CompareTo(otherInstance));
		}

		[TestMethod]
		public void EventListenerTests_Equals_SingleArgument()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = new Mocks.MockEventListener(true, false);

			Assert.IsFalse(instance.Equals(otherInstance));
		}

		[TestMethod]
		public void EventListenerTests_Equals_Equal()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = new Mocks.MockEventListener(true, false);

			Assert.IsTrue(instance.Equals(instance, instance));
		}

		[TestMethod]
		public void EventListenerTests_Equals_NotEqual()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = new Mocks.MockEventListener(true, false);

			Assert.IsFalse(instance.Equals(instance, otherInstance));
		}

		[TestMethod]
		public void EventListenerTests_Equals_RightNull()
		{
			EventListener instance = new Mocks.MockEventListener(true, false);
			EventListener otherInstance = null;

			Assert.IsFalse(instance.Equals(instance, otherInstance));
		}

		[TestMethod]
		public void EventListenerTests_Equals_LeftNull()
		{
			EventListener instance = null;
			EventListener otherInstance = new Mocks.MockEventListener(true, false);


			Assert.IsFalse(otherInstance.Equals(instance, otherInstance));
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void EventListenerTests_GetHashCode_Null()
		{
			EventListener instance = null;
			EventListener otherInstance = new Mocks.MockEventListener(true, false);


			otherInstance.GetHashCode(instance);
		}

		[TestMethod]
		public void EventListenerTests_GetHashCode_Valid()
		{
			EventListener instance =  new Mocks.MockEventListener(true, false);

			Assert.IsNotNull(instance.GetHashCode(instance));
		}
	}
}
