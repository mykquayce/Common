using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Common.Collections
{
	public class ObservableList<T> : List<T>, INotifyCollectionChanged
	{
		#region Constructors
		public ObservableList() { }

		public ObservableList(IEnumerable<T> items)
		{
			AddRange(items);
		}
		#endregion Constructors

		#region Overrides
		public new T this[int index]
		{
			get => base[index];
			set
			{
				base[index] = value;

				NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
			}
		}

		public new void Add(T item)
		{
			base.Add(item);

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public new void AddRange(IEnumerable<T> collection)
		{
			var enumerated = collection.ToList();

			base.AddRange(enumerated);

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, enumerated));
		}

		public new void Clear()
		{
			base.Clear();

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public new void Insert(int index, T item)
		{
			base.Insert(index, item);

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public new void InsertRange(int index, IEnumerable<T> collection)
		{
			var enumerated = collection.ToList();

			base.InsertRange(index, enumerated);

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, enumerated, index));
		}

		public new bool Remove(T item)
		{
			var success = base.Remove(item);

			if (success)
			{
				NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}

			return success;
		}

		public new int RemoveAll(Predicate<T> match)
		{
			var items = base.FindAll(match);

			var count = base.RemoveAll(match);

			if (count > 0)
			{
				NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
			}

			return count;
		}

		public new void RemoveAt(int index)
		{
			var item = base[index];

			base.RemoveAt(index);

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
		}

		public new void RemoveRange(int index, int count)
		{
			var items = base.GetRange(index, count);

			base.RemoveRange(index, count);

			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
		}

		public new void Reverse(int index, int count)
		{
			base.Reverse(index, count);

			for (var a = 0; a < count; a++)
			{
				var oldIndex = (index + (count - 1)) - a;
				var newIndex = index + a;

				if (!Equals(this[oldIndex], this[newIndex]))
				{
					NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this[newIndex], newIndex, oldIndex));
				}
			}
		}

		public new void Reverse()
		{
			this.Reverse(0, base.Count);
		}

		public new void Sort(int index, int count, IComparer<T> comparer)
		{
			var before = new T[count];

			base.CopyTo(index, before, 0, count);

			base.Sort(index, count, comparer);

			var after = base.GetRange(index, count);

			for (var a = 0; a < before.Length; a++)
			{
				var item = before[a];
				var oldIndex = index + a;
				var newIndex = index + after.IndexOf(item);

				if (oldIndex != newIndex)
				{
					NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
				}
			}
		}

		public new void Sort(Comparison<T> comparison)
		{
			var before = new T[base.Count];

			base.CopyTo(before);

			base.Sort(comparison);

			var after = this;

			for (var a = 0; a < before.Length; a++)
			{
				var item = before[a];
				var oldIndex = a;
				var newIndex = after.IndexOf(item);

				if (oldIndex != newIndex)
				{
					NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
				}
			}
		}

		public new void Sort()
		{
			var before = new T[base.Count];

			base.CopyTo(before);

			base.Sort();

			var after = this;

			for (var a = 0; a < before.Length; a++)
			{
				var item = before[a];
				var oldIndex = a;
				var newIndex = after.IndexOf(item);

				if (oldIndex != newIndex)
				{
					NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
				}
			}
		}

		public new void Sort(IComparer<T> comparer)
		{
			this.Sort(0, base.Count, comparer);
		}
		#endregion Overrides

		#region INotifyCollectionChanged members
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}
		#endregion INotifyCollectionChanged members
	}
}
