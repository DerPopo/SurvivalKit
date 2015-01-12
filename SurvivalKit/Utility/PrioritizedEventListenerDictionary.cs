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
	internal class PrioritizedEventListenerDictionary<TKey, TValue>
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
			if (key == null)
			{
				return false;
			}

			return _internalDictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get { return _internalDictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				return false;
			}

			return _internalDictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out List<TValue> value)
		{
			List<TValue> localCopy;
			var result = _internalDictionary.TryGetValue(key, out localCopy);
			value = localCopy;
			return result;
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
			if (ContainsKey(item.Key))
			{
				return _internalDictionary[item.Key].Remove(item.Value);
			}

			return false;
		}

		/// <summary>
		/// Remove a value from all keys.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>Returns <c>r</c></returns>
		public bool Remove(TValue value)
		{
			if(value == null)
			{
				return false;
			}

			var foundItem = false;
			foreach (var item in _internalDictionary)
			{
				var list = item.Value;
				if (list.Contains(value))
				{
					foundItem = true;
					list.Remove(value);
					foundItem = true;
				}
			}

			return foundItem;
		}

	}
}
