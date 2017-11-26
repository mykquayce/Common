using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Collections
{
	public class NotifyingDictionary<T, TN> : IDictionary<T, TN>, Interfaces.INotifyDictionaryChanged<T, TN>, Interfaces.INotifyingDictionary<T, TN>
	{
		#region IDictionary<T, TN> implementation
		public TN this[T key]
		{
			get => throw new NotImplementedException();
			set => Add(key, value);
		}

		public ICollection<T> Keys => throw new NotImplementedException();

		public ICollection<TN> Values => throw new NotImplementedException();

		public int Count => throw new NotImplementedException();

		public bool IsReadOnly => throw new NotImplementedException();

		public void Add(T key, TN value)
		{
			Add(new KeyValuePair<T, TN>(key, value));
		}

		public void Add(KeyValuePair<T, TN> item)
		{
			if (DictionaryChanged is null)
			{
				return;
			}
			var action = NotifyDictionaryChangedAction.Add;
			var args = new NotifyDictionaryChangedEventArgs<T, TN>(action, item);
			DictionaryChanged(this, args);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<T, TN> item)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(T key)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<T, TN>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<T, TN>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool Remove(T key)
		{
			if (DictionaryChanged is null)
			{
				return false;
			}
			var action = NotifyDictionaryChangedAction.Remove;
			var args = new NotifyDictionaryChangedEventArgs<T, TN>(action, key);
			DictionaryChanged(this, args);
			return true;
		}

		public bool Remove(KeyValuePair<T, TN> item)
		{
			if (DictionaryChanged is null)
			{
				return false;
			}
			var action = NotifyDictionaryChangedAction.Remove;
			var args = new NotifyDictionaryChangedEventArgs<T, TN>(action, item.Key, item.Value);
			DictionaryChanged(this, args);
			return true;
		}

		public bool TryGetValue(T key, out TN value)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
		#endregion IDictionary<T, TN> implementation

		#region INotifyDictionaryChanged<T, TN> implementation
		public event NotifyDictionaryChangedEventHandler<T, TN> DictionaryChanged;
		#endregion INotifyDictionaryChanged<T, TN> implementation

		public void AddRange(IDictionary<T, TN> dictionary)
		{
			if (DictionaryChanged is null)
			{
				return;
			}
			var action = NotifyDictionaryChangedAction.Add;
			var args = new NotifyDictionaryChangedEventArgs<T, TN>(action, dictionary);
			DictionaryChanged(this, args);
		}
	}
}