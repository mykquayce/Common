using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace Common.Collections
{
	public abstract class ConcurrentObservableQueue<T> : ConcurrentQueue<T>, INotifyCollectionChanged, Interfaces.IConcurrentObservableQueue<T>
	{
		#region Overidden Members
		public new void Enqueue(T item)
		{
			base.Enqueue(item);
			NotifyCollectionChanged(NotifyCollectionChangedAction.Add, new[] { item });
		}

		public new bool TryDequeue(out T result)
		{
			var success = base.TryDequeue(out result);

			if (success)
			{
				NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, new[] { result });
			}

			return success;
		}
		#endregion Overidden Members

		#region INotifyCollectionChanged Support
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void NotifyCollectionChanged(NotifyCollectionChangedAction action, IList items)
		{
			var args = new NotifyCollectionChangedEventArgs(action, items);

			NotifyCollectionChanged(args);
		}

		private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}
		#endregion INotifyCollectionChanged Support
	}
}
