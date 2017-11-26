using System;
using System.Collections.Generic;

namespace Common.Collections
{
	public class NotifyDictionaryChangedEventArgs<T, TN> : EventArgs
	{
		public NotifyDictionaryChangedAction Action { get; }
		public IDictionary<T, TN> NewItems { get; } = new Dictionary<T, TN>();
		public IDictionary<T, TN> OldItems { get; } = new Dictionary<T, TN>();

		public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action)
		{
			switch (action)
			{
				case NotifyDictionaryChangedAction.Reset:
					Action = action;
					break;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(action),
						"Constructor must be called with NotifyDictionaryChangedAction.Reset")
					{
						Data = { { nameof(action), action } }
					};
			}
		}

		public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, KeyValuePair<T, TN> keyValuePair)
			: this(action, keyValuePair.Key, keyValuePair.Value)
		{ }

		public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, T key, TN value)
		{
			switch (action)
			{
				case NotifyDictionaryChangedAction.Add:
					Action = action;
					NewItems.Add(key, value);
					break;
				case NotifyDictionaryChangedAction.Remove:
					Action = action;
					OldItems.Add(key, value);
					break;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(action),
						"Constructor must be called with NotifyDictionaryChangedAction.Add or NotifyDictionaryChangedAction.Remove")
					{
						Data = { { nameof(action), action } }
					};
			}
		}

		public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, T key, TN newValue, TN oldValue)
		{
			switch (action)
			{
				case NotifyDictionaryChangedAction.Replace:
					Action = action;
					NewItems.Add(key, newValue);
					OldItems.Add(key, oldValue);
					break;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(action),
						"Constructor must be called with NotifyDictionaryChangedAction.Replace");
			}
		}

		public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, T key)
		{
			switch (action)
			{
				case NotifyDictionaryChangedAction.Remove:
					Action = action;
					OldItems.Add(key, default(TN));
					break;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(action),
						"Constructor must be called with NotifyDictionaryChangedAction.Remove");
			}
		}

		public NotifyDictionaryChangedEventArgs(NotifyDictionaryChangedAction action, IDictionary<T, TN> dictionary)
		{
			switch (action)
			{
				case NotifyDictionaryChangedAction.Add:
					Action = action;
					foreach (var item in dictionary)
					{
						NewItems.Add(item.Key, item.Value);
					}
					break;
				case NotifyDictionaryChangedAction.Remove:
					Action = action;
					foreach (var item in dictionary)
					{
						OldItems.Add(item.Key, item.Value);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(
						nameof(action),
						"Constructor must be called with NotifyDictionaryChangedAction.Add or NotifyDictionaryChangedAction.Remove");
			}
		}
	}
}
