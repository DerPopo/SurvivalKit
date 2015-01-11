using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalKit.Utility;
using SurvivalKit.Interfaces;
using SurvivalKit.Tests.Mocks;
using SurvivalKit.Events;

namespace SurvivalKit.Tests.Utility
{
	[TestClass]
	public class PrioritizedEventListenerDictionaryTests
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_Ctor_Null()
		{
			EventListenerRegistrationComparer comparer = null;
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
		}

		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_Ctor_Valid()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
		}

		/// <summary>
		///  Test the scenario where you do not have a registry in the dictionary yet.
		/// </summary>
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_Add_InsertSingle()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
			var mockListener = new EventListenerRegistration(new MockEventListener(true,false),new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null));
			dictionary.Add(GetType(), mockListener);

			Assert.AreEqual(1, dictionary.Count);
		}

		/// <summary>
		///  Test the scenario where you do have a registry in the dictionary yet, and you add a new hook to the same event.
		/// </summary>
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_Add_InsertTwo()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
			var mockListener = new EventListenerRegistration(new MockEventListener(true, false), new MockEventHook(GetType(), SurvivalKit.Events.Priority.LOW, null));
			dictionary.Add(GetType(), mockListener);
			var mockListener1 = new EventListenerRegistration(new MockEventListener(true, false), new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null));
			dictionary.Add(GetType(), mockListener1);

			// count the keys int he dictionary
			Assert.AreEqual(1, dictionary.Count);
			var collection = dictionary.ValueCollections;
			var iterator = 0;
			foreach (var item in collection)
			{
				foreach (var registration in item)
				{
					if (iterator == 0)
					{
						Assert.AreEqual(SurvivalKit.Events.Priority.NORMAL, registration.EventHook.HookPriority);
					}
					else
					{
						Assert.AreEqual(SurvivalKit.Events.Priority.LOW, registration.EventHook.HookPriority);
					}
					iterator++;
				}
			}
		}

		/// <summary>
		///	Test the ValueCollections property while its empty
		/// </summary>
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_ValueCollections_Empty()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
			var collections = dictionary.ValueCollections;
			Assert.IsNotNull(collections);
			Assert.AreEqual(0, collections.Count);
		}

		/// <summary>
		///	Test the ValueCollections property while its has only one key
		/// </summary>
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_ValueCollections_Single()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
			
			var mockListener = new EventListenerRegistration(new MockEventListener(true, false), new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null));
			dictionary.Add(GetType(), mockListener);

			var collections = dictionary.ValueCollections;

			Assert.IsNotNull(collections);
			Assert.AreEqual(1, collections.Count);
			foreach (var collection in collections)
			{
				Assert.AreEqual(GetType(), collection[0].EventHook.GetEventType());
			}
		}

		/// <summary>
		///	Test the ValueCollections property while its has only two keys
		/// </summary>
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_ValueCollections_Multiple()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);
			
			var mockListener = new EventListenerRegistration(new MockEventListener(true, false), new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null));
			dictionary.Add(GetType(), mockListener);

			dictionary.Add(typeof(IEventHook), mockListener);

			var collections = dictionary.ValueCollections;
			Assert.IsNotNull(collections);
			Assert.AreEqual(2, collections.Count);
			foreach (var collection in collections)
			{
				Assert.AreEqual(GetType(), collection[0].EventHook.GetEventType());
			}
		}

		/// <summary>
		///	Test the ValueCollections property while its has only two keys and several entries
		/// </summary>
		[TestMethod]
		public void PrioritizedEventListenerDictionaryTests_ValueCollections_Multiple_MultipleEntries()
		{
			EventListenerRegistrationComparer comparer = new EventListenerRegistrationComparer();
			var dictionary = new PrioritizedEventListenerDictionary<Type, EventListenerRegistration>(comparer);

			var mockListener = new EventListenerRegistration(new MockEventListener(true, false), new MockEventHook(GetType(), SurvivalKit.Events.Priority.NORMAL, null));
			dictionary.Add(GetType(), mockListener);

			for (var iterator = 5; iterator > 0; iterator--)
			{
				dictionary.Add(typeof(IEventHook), mockListener);
			}
			

			var collections = dictionary.ValueCollections;
			Assert.IsNotNull(collections);
			Assert.AreEqual(2, collections.Count);
			var indexer = 0;
			foreach (var item in collections)
			{
				if (indexer == 0)
				{
					Assert.AreEqual(1, item.Count);
				}
				else
				{
					Assert.AreEqual(5, item.Count);
				}

				indexer++;
			}
		}
	}
}
