using System.Collections.Generic;
using System.Collections.Specialized;

namespace Common.Collections.Interfaces
{
	public interface INotifyingCollection<T>
	{
		event NotifyCollectionChangedEventHandler CollectionChanged;

		void Add(T item);
		void AddRange(IEnumerable<T> items);
		void Clear();
		bool Remove(T item);
	}
}