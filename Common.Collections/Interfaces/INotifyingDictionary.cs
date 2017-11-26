using System.Collections.Generic;

namespace Common.Collections.Interfaces
{
	public interface INotifyingDictionary<T, TN>
	{
		TN this[T key] { get; set; }

		event NotifyDictionaryChangedEventHandler<T, TN> DictionaryChanged;

		void Add(KeyValuePair<T, TN> item);
		void Add(T key, TN value);
		void AddRange(IDictionary<T, TN> dictionary);
		void Clear();
		bool Remove(KeyValuePair<T, TN> item);
		bool Remove(T key);
	}
}