using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Collections
{
	public class TypesCollection : EquatableCollection<Type>
	{
		public new void Add(Type type)
		{
			if (!Contains(type))
			{
				base.Add(type);
			}
		}

		public void Add(params Type[] types)
		{
			foreach (var type in types)
			{
				this.Add(type);
			}
		}

		public new void AddRange(IEnumerable<Type> types)
		{
			this.Add(types.ToArray());
		}

		public TypesCollection() { }

		public TypesCollection(IEnumerable<Type> types)
		{
			this.AddRange(types);
		}

		public TypesCollection(params Type[] types)
		{
			this.Add(types);
		}
	}
}
