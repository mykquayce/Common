using System.Collections.Specialized;

namespace Common.Collections.Interfaces
{
	public interface IConcurrentObservableQueue<T>
	{
		event NotifyCollectionChangedEventHandler CollectionChanged;

		void Enqueue(T item);
		bool TryDequeue(out T result);
	}
}
