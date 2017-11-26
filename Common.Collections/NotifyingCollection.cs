using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Common.Collections
{
	public class NotifyingCollection<T> : ICollection<T>, INotifyCollectionChanged, Interfaces.INotifyingCollection<T>
	{
		#region ICollection<T> implementation
		public int Count => 0;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			if (CollectionChanged is null)
			{
				return;
			}
			var action = NotifyCollectionChangedAction.Add;
			IList newItems = new[] { item };
			var args = new NotifyCollectionChangedEventArgs(action, newItems);
			CollectionChanged(this, args);
		}

		public void Clear()
		{
			if (CollectionChanged is null)
			{
				return;
			}
			var action = NotifyCollectionChangedAction.Reset;
			var args = new NotifyCollectionChangedEventArgs(action);
			CollectionChanged(this, args);
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			if (CollectionChanged is null)
			{
				return false;
			}
			var action = NotifyCollectionChangedAction.Remove;
			IList newItems = new[] { item };
			var args = new NotifyCollectionChangedEventArgs(action, newItems);
			CollectionChanged(this, args);
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
		#endregion ICollection<T> implementation

		#region INotifyCollectionChanged implementation
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		#endregion INotifyCollectionChanged implementation

		public void AddRange(IEnumerable<T> items)
		{
			if (CollectionChanged is null)
			{
				return;
			}
			var action = NotifyCollectionChangedAction.Add;
			IList newItems = items.ToList();
			var args = new NotifyCollectionChangedEventArgs(action, newItems);
			CollectionChanged(this, args);
		}
	}
}
