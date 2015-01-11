using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalKit.Utility;
using SurvivalKit.Tests.Mocks;
using SurvivalKit.Events;

namespace SurvivalKit.Tests.Utility
{
	[TestClass]
	public class EventHookPriorityComparerTests
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void EventHookPriorityComparerTests_BothNull()
		{
			var comparer = new EventListenerRegistrationComparer();
			comparer.Compare(null, null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void EventHookPriorityComparerTests_LeftNull()
		{
			var rightHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null);
			var rightMock = new EventListenerRegistration(new MockEventListener(true, false), rightHook);
			

			var comparer = new EventListenerRegistrationComparer();
			comparer.Compare(null, rightMock);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void EventHookPriorityComparerTests_RightNull()
		{
			var leftHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null);
			var leftMock = new EventListenerRegistration(new MockEventListener(true, false), leftHook);

			var comparer = new EventListenerRegistrationComparer();
			comparer.Compare(leftMock, null);
		}

		[TestMethod]
		public void EventHookPriorityComparerTests_LeftGreater()
		{
			var leftHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.HIGHEST, null);
			var leftMock = new EventListenerRegistration(new MockEventListener(true, false), leftHook);
			var rightHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null);
			var rightMock = new EventListenerRegistration(new MockEventListener(true, false), rightHook);

			var comparer = new EventListenerRegistrationComparer();
			var result = comparer.Compare(leftMock, rightMock);
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void EventHookPriorityComparerTests_RightGreater()
		{
			var leftHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null);
			var leftMock = new EventListenerRegistration(new MockEventListener(true, false), leftHook);
			var rightHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.HIGHEST, null);
			var rightMock = new EventListenerRegistration(new MockEventListener(true, false), rightHook);

			var comparer = new EventListenerRegistrationComparer();
			var result = comparer.Compare(leftMock, rightMock);
			Assert.AreEqual(-1, result);
		}

		[TestMethod]
		public void EventHookPriorityComparerTests_Equals()
		{
			var leftHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null);
			var leftMock = new EventListenerRegistration(new MockEventListener(true, false), leftHook);
			var rightHook = new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null);
			var rightMock = new EventListenerRegistration(new MockEventListener(true, false), rightHook);


			var comparer = new EventListenerRegistrationComparer();
			var result = comparer.Compare(leftMock, rightMock);
			Assert.AreEqual(0, result);
		}
	}
}
