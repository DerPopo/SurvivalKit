	using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Utility
{

	/// <summary>
	///	Dictionary class that has a collection of of objects in every value.
	///	These objects have been sorted according to the sorting operator we get in the constructor.
	/// </summary>
	/// <remarks>Inspired by https://github.com/Microsoft/referencesource/blob/master/System/compmod/system/collections/specialized/ordereddictionary.cs </remarks>
	internal class PrioritizedEventListenerDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private Dictionary<TKey, List<TValue>> _internalDictionary;
		private IComparer<TValue> _comparer;

		public PrioritizedEventListenerDictionary(IComparer<TValue> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			_internalDictionary = new Dictionary<TKey, List<TValue>>();
			_comparer = comparer;
		}


		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if (_internalDictionary.ContainsKey(key))
			{
				InsertAtPosition(_internalDictionary[key], value);
			}
			else
			{
				var list = new List<TValue>();
				_internalDictionary.Add(key, list);
				list.Add(value);
			}
		}

		private void InsertAtPosition(List<TValue> collection, TValue itemToInsert)
		{
			var insertAtIndex = collection.Count;

			for (int indexer = 0; indexer < collection.Count; indexer++)
			{
				var collectionItem = collection[indexer];
				var compareResult = _comparer.Compare(collectionItem, itemToInsert);
				if (compareResult == -1)
				{
					insertAtIndex = indexer;
					break;
				}
			}

			collection.Insert(insertAtIndex, itemToInsert);
		}

		public bool ContainsKey(TKey key)
		{
			return _internalDictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get { return _internalDictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			return _internalDictionary.Remove(key);
		}

		[Obsolete("There is no single value to retrieve. Use the overload with List<TValue>", true)]
		public bool TryGetValue(TKey key, out TValue value)
		{
			throw new NotSupportedException("There is no single value to retrieve. Use the overload with List<TValue>");
		}

		public bool TryGetValue(TKey key, out List<TValue> value)
		{
			List<TValue> localCopy;
			var result = _internalDictionary.TryGetValue(key, out localCopy);
			value = localCopy;
			return result;
		}

		[Obsolete("There is no single value to retrieve. Use the overload with List<TValue>", true)]
		public ICollection<TValue> Values
		{
			get { throw new NotSupportedException("There is no single value to retrieve. Use the overload with List<TValue>"); }
		}

		public ICollection<List<TValue>> ValueCollections
		{
			get {return _internalDictionary.Values;}
		}

		public List<TValue> this[TKey key]
		{
			get
			{
				return _internalDictionary[key];
			}
			
		}

		public IEnumerator GetEnumerator()
		{
			var list = new List<TValue>();

			foreach (var item in _internalDictionary)
			{
				list.AddRange(item.Value);
			}

			return list.GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			_internalDictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (_internalDictionary.ContainsKey(item.Key))
			{
				return _internalDictionary[item.Key].Contains(item.Value);
			}

			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return _internalDictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (_internalDictionary.ContainsKey(item.Key))
			{
				return _internalDictionary[item.Key].Remove(item.Value);
			}

			return false;
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			throw new NotSupportedException("There is no single value to retrieve. Use the overload with List<TValue>");
		}

		IEnumerator<KeyValuePair<TKey, List<TValue>>> GetValueCollectionEnumerator()
		{
			return _internalDictionary.GetEnumerator();
		}


		[Obsolete]
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
