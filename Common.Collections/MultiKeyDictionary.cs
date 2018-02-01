using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Collections
{
	public class MultiKeyDictionary<TKey1, TKey2, TValue> : Dictionary<(TKey1, TKey2), TValue>
	{
		public TValue this[TKey1 key1]
		{
			get
			{
				var key = base.Keys.Single(k => object.Equals(k.Item1, key1));

				return this[key];
			}
			set
			{
				var key = base.Keys.Single(k => object.Equals(k.Item1, key1));

				this[key] = value;
			}
		}

		public TValue this[TKey2 key2]
		{
			get
			{
				var key = base.Keys.Single(k => object.Equals(k.Item1, key2));

				return this[key];
			}
			set
			{
				var key = base.Keys.Single(k => object.Equals(k.Item1, key2));

				this[key] = value;
			}
		}

		public void Add(TKey1 key1, TValue value)
		{
			if(ContainsKey(key1))
			{

			}
		}

		public bool ContainsKey(TKey1 key1) => this.Keys.Any(t => object.Equals(t.Item1, key1));
		public bool ContainsKey(TKey2 key2) => this.Keys.Any(t => object.Equals(t.Item1, key2));
	}
}
