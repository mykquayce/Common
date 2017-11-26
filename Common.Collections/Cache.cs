using System.Collections.Generic;

namespace Common.Collections
{
	public abstract class Cache<TKey, TValue> : ActionableConcurrentObservableQueue<TValue>
	{
		protected SerializableDictionary<TKey, TValue> Items { get; private set; }

		protected abstract SerializableDictionary<TKey, TValue> Load();
		protected abstract void Save();

		protected TValue this[TKey key]
		{
			get
			{
				if (Items.ContainsKey(key))
				{
					return Items[key];
				}

				throw new KeyNotFoundException { Data = { { nameof(key), key } } };
			}
			set
			{
				if (Items.ContainsKey(key))
				{
					Items[key] = value;
				}
				else
				{
					Items.Add(key, value);
				}

				base.Enqueue(value);
			}
		}

		protected Cache(int maxBufferSize, double maxBufferTime)
			: base(maxBufferSize, maxBufferTime)
		{

			Items = Load();
			base.Action = i => Save();
		}

		~Cache()
		{
			Save();
		}
	}
}
