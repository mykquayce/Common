using System;
using System.Collections.Generic;

namespace Common.Collections
{
	public class CompoundKeyDictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>
	{
		public TValue this[TKey1 key1, TKey2 key2]
		{
			get => base[new Tuple<TKey1, TKey2>(key1, key2)];
			set
			{
				var key = new Tuple<TKey1, TKey2>(key1, key2);

				if (ContainsKey(key))
				{
					base[key] = value;
				}
				else
				{
					Add(key, value);
				}
			}
		}

		public void Add(TKey1 key1, TKey2 key2, TValue value)
		{
			this[key1, key2] = value;
		}

		public void Add(KeyValuePair<Tuple<TKey1, TKey2>, TValue> kvp)
		{
			this[kvp.Key.Item1, kvp.Key.Item2] = kvp.Value;
		}

		public void Add(CompoundKeyDictionary<TKey1, TKey2, TValue> other)
		{
			using (var enumerator = other.GetEnumerator())
			{
				var kvp = enumerator.Current;

				this[kvp.Key.Item1, kvp.Key.Item2] = kvp.Value;
			}
		}

		public CompoundKeyDictionary()
			: base(new EqualityComparer())
		{ }

		#region IEqualityComparer
		private class EqualityComparer : IEqualityComparer<Tuple<TKey1, TKey2>>
		{
			public bool Equals(Tuple<TKey1, TKey2> x, Tuple<TKey1, TKey2> y)
			{
				return x.Item1.GetHashCode() == y.Item1.GetHashCode()
				       && x.Item2.GetHashCode() == y.Item2.GetHashCode();
			}

			public int GetHashCode(Tuple<TKey1, TKey2> obj)
			{
				unchecked
				{
					return (obj.Item1.GetHashCode() * 357) ^ obj.Item2.GetHashCode();
				}
			}
		}
		#endregion IEqualityComparer
	}
}
