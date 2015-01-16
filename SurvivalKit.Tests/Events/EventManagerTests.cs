using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalKit.Events;
using SurvivalKit.Events.Entities;
using SurvivalKit.Events.Network;
using SurvivalKit.Exceptions;
using SurvivalKit.Tests.Mocks;
using SurvivalKit.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalKit.Tests.Events
{
	[TestClass]
	public class EventManagerTests
	{
		/// <summary>
		/// Fire an event which nobody is listening to.
		/// No errors should occur, because registration is not mandator if you do not use it.
		/// </summary>
		[TestMethod]
		public void EventManagerTests_FireEvent_NotFound()
		{
			LogUtility.SetLogToConsole();
			var mockAggregator = new MockEventAggregator();
			EventManager._aggregator =(mockAggregator);

			try
			{
				var result = EventManager.FireEvent("WillNeverBeFoundEvent", new object[0]);
				Assert.IsNotNull(result);
			}
			catch (Exception exception)
			{
				Assert.Fail("Failed to fire an event that nobody is listening to.");
			}
		}

		/// <summary>
		///	Fire an event, and locate the event type.
		///	This method proves the matching by name works.
		///	When invoking the event, the constructor should complain about arguments.
		/// </summary>
		[TestMethod]
		public void EventManagerTests_FireEvent_FoundEvent_InvalidArguments()
		{
			LogUtility.SetLogToConsole();
			var mockAggregator = new MockEventAggregator(new List<Type> { typeof(EntityMoveEvent) });
			EventManager._aggregator = mockAggregator;

			try
			{
				var result = EventManager.FireEvent("EntityMoveEvent", new object[0]);
				Assert.Fail("Code should not reach this part.");
			}
			catch (SurvivalKitPluginException exception)
			{
				// exception should be thrown. constructor arguments are wrong.
				Assert.IsNotNull(exception);
			}
		}

		/// <summary>
		///	Having issues with this test method.
		///	Currently it keeps complaining the Unity library could not be loaded.
		///	This might be because the unit test library runs ons .NET 3.5 and the project itself runs on 3.0
		///	To be investigated.
		/// </summary>
		[TestMethod]
		public void EventManagerTests_FireEvent_FoundEvent_ValidArguments()
		{
			LogUtility.SetLogToConsole();
			var mockAggregator = new MockEventAggregator(new List<Type> { typeof(EntityMoveEvent) });
			EventManager._aggregator = mockAggregator;

			try
			{
				var result = EventManager.FireEvent("EntityMoveEvent", new object[]
				{
					false,
					new UnityEngine.Vector3(),
					new UnityEngine.Vector3(),
					new World(),
					new {
						pos = 12,
						rot = 241
					}
				});
				
			}
			catch (SurvivalKitPluginException exception)
			{
				// exception should not- be thrown. constructor arguments are correct.
				Assert.IsNotNull(exception);
				Assert.Fail("Code should not reach this part.");
			}
		}


		
	}
}
