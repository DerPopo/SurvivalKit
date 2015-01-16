using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalKit.Events;
using System.Collections.Generic;
using SurvivalKit.Interfaces;
using SurvivalKit.Abstracts;
using System.Runtime.CompilerServices;
using SurvivalKit.Tests.Mocks;
using SurvivalKit.Utility;

namespace SurvivalKit.Tests.Events
{
	
	[TestClass]
	public class EventAggregatorTests
	{
		[TestMethod]
		public void EventAggregator_RegisterEventListener_Null()
		{
			LogUtility.SetLogToConsole();
			var resolver = new Mocks.MockInstanceResolver(null);
			try
			{
				var instance = EventAggregator.GetInstance(resolver, true);
				Assert.IsNotNull(instance);
			}
			catch (Exception ex)
			{
				Assert.Fail("Exception occurred while constructing the event aggregator.", ex);
			}
			
		}

		[TestMethod]
		public void EventAggregator_RegisterEventListener_EmptyList()
		{
			var list = new List<IPlugin>();
			var resolver = new Mocks.MockInstanceResolver(list);

			try
			{
				var instance = EventAggregator.GetInstance(resolver, true);
				Assert.IsNotNull(instance);
			}
			catch (Exception ex)
			{
				Assert.Fail("Exception occurred while constructing the event aggregator.", ex);
			}
		}

		[TestMethod]
		public void EventAggregator_RegisterEventListener_Mocked_Null()
		{
			List<EventListener> eventListeners = null;
			var list = new List<IPlugin>
			{
				new Mocks.MockPlugin(eventListeners, new List<ICommandListener>(), "")
			};
			var resolver = new Mocks.MockInstanceResolver(list);

			try
			{
				var instance = EventAggregator.GetInstance(resolver, true);
				Assert.IsNotNull(instance);
			}
			catch (Exception ex)
			{
				Assert.Fail("Exception occurred while constructing the event aggregator.", ex);
			}
		}

		[TestMethod]
		public void EventAggregator_RegisterEventListener_Mocked_Empty()
		{
			var eventListeners = new List<EventListener>();
			var list = new List<IPlugin>
			{
				new Mocks.MockPlugin(eventListeners, new List<ICommandListener>(), "")
			};
			var resolver = new Mocks.MockInstanceResolver(list);

			try
			{
				var instance = EventAggregator.GetInstance(resolver, true);
				Assert.IsNotNull(instance);
			}
			catch (Exception ex)
			{
				Assert.Fail("Exception occurred while constructing the event aggregator.", ex);
			}
		}

		[TestMethod]
		public void EventAggregator_RegisterEventListener_Mocked_Single()
		{
			var eventListeners = new List<EventListener>();
			eventListeners.Add(new MockEventListener(true,false));
			var list = new List<IPlugin>
			{
				new Mocks.MockPlugin(eventListeners, new List<ICommandListener>(), "")
			};
			var resolver = new Mocks.MockInstanceResolver(list);

			try
			{
				var instance = EventAggregator.GetInstance(resolver, true);
				Assert.IsNotNull(instance);
			}
			catch (Exception ex)
			{
				Assert.Fail("Exception occurred while constructing the event aggregator.", ex);
			}
		}
	}
}
