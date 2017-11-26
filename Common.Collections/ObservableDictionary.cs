using System.Collections.Generic;

namespace Common.Collections
{
	public class ObservableDictionary<T, TN> : Dictionary<T, TN>, Interfaces.INotifyDictionaryChanged<T, TN>
	{
		#region Constructor
		public ObservableDictionary() { }

		public ObservableDictionary(IEnumerable<KeyValuePair<T, TN>> kvps)
		{
			foreach (var kvp in kvps)
			{
				Add(kvp.Key, kvp.Value);
			}
		}
		#endregion Constructor

		#region Overrides
		public new TN this[T key]
		{
			get => base[key];
			set
			{
				var oldValue = base[key];
				base[key] = value;

				DictionaryChanged?.Invoke(
					this,
					new NotifyDictionaryChangedEventArgs<T, TN>(NotifyDictionaryChangedAction.Replace, key, value, oldValue));
			}
		}

		public new void Add(T key, TN value)
		{
			base.Add(key, value);

			DictionaryChanged?.Invoke(
				this,
				new NotifyDictionaryChangedEventArgs<T, TN>(NotifyDictionaryChangedAction.Add, key, value));
		}

		public new void Clear()
		{
			base.Clear();

			DictionaryChanged?.Invoke(
				this,
				new NotifyDictionaryChangedEventArgs<T, TN>(NotifyDictionaryChangedAction.Reset));
		}

		public new bool Remove(T key)
		{
			var value = base[key];
			var success = base.Remove(key);

			if (success)
			{
				DictionaryChanged?.Invoke(
					this,
					new NotifyDictionaryChangedEventArgs<T, TN>(NotifyDictionaryChangedAction.Remove, key, value));
			}

			return success;
		}
		#endregion Overrides

		public void AddRange(IDictionary<T, TN> dictionary)
		{
			foreach (var kvp in dictionary)
			{
				base.Add(kvp.Key, kvp.Value);

				DictionaryChanged?.Invoke(
					this,
					new NotifyDictionaryChangedEventArgs<T, TN>(NotifyDictionaryChangedAction.Add, dictionary));
			}
		}

		#region INotifyDictionaryChanged members
		public event NotifyDictionaryChangedEventHandler<T, TN> DictionaryChanged;
		#endregion INotifyDictionaryChanged members
	}
}
