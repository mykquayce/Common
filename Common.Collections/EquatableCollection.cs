using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Collections
{
	public class EquatableCollection<T> : List<T>, IEquatable<EquatableCollection<T>>
	{
		public override int GetHashCode()
		{
			var hash = 0;

			this.Select(i => i.GetHashCode()).ToList().ForEach(i => hash ^= i);

			return hash;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as EquatableCollection<T>);
		}

		public bool Equals(EquatableCollection<T> other)
		{
			return this.GetHashCode() == other?.GetHashCode();
		}

		public static bool operator ==(EquatableCollection<T> left, EquatableCollection<T> right)
		{
			return left?.Equals(right) ?? false;
		}

		public static bool operator !=(EquatableCollection<T> left, EquatableCollection<T> right)
		{
			return !(left == right);
		}
	}
}
