using Common.Binary.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Common
{
	public static class HelperMethods
	{
		public static class MergeHelpers
		{
			public static IList Lists(params IList[] lists)
			{
				if (!lists.Any())
				{
					throw new ArgumentNullException { Data = { { nameof(lists), lists } } };
				}

				if (!lists.Skip(1).Any())
				{
					var cloned = DeepClone(lists.Single());

					return cloned;
				}

				var type = lists[0].GetType().GetElementType();

				var count = lists.Sum(l => l?.Count ?? 0);

				var target = Array.CreateInstance(type, count);

				CopyHelpers.Lists(target, lists);

				return target;
			}
		}

		public static class CopyHelpers
		{
			public static void Lists(IList target, params IList[] lists)
			{
				if (target is null)
				{
					throw new ArgumentNullException { Data = { { nameof(target), target } } };
				}

				if (!lists.Any())
				{
					return;
				}

				if (target.IsFixedSize && target.Count < lists.Sum(l => l?.Count ?? 0))
				{
					throw new ArgumentOutOfRangeException
					{
						Data =
						{
							{ nameof(target), target },
							{ nameof(lists), lists }
						}
					};
				}

				var index = 0;

				foreach (var list in lists)
				{
					foreach (var item in list)
					{
						var cloned = DeepClone(item);

						if (target.IsFixedSize)
						{
							while (!(((Array)target).GetValue(index) is null))
							{
								index++;
							}

							((Array)target).SetValue(item, index++);
						}
						else
						{
							target.Add(cloned);
						}
					}
				}
			}
		}

		public static T Merge<T>(params T[] values)
		{
			var type = typeof(T);

			var isCollection = type.GetInterface("ICollection") != null;

			return isCollection
				? (T)MergeLists(values)
				: (T)Merge(type, values.OfType<object>().ToArray());
		}

		public static string MergeStrings(IEnumerable<string> strings)
		{
			return strings.FirstOrDefault(v => !string.IsNullOrEmpty(v))
				   ?? strings.FirstOrDefault(v => !Equals(v, null));
		}

		public static IDictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(IEnumerable<IDictionary<TKey, TValue>> dictionaries)
		{
			return
				(
					from d in dictionaries
					from kvp in d
					group kvp by kvp.Key into g
					let key = g.Key
					let values = g.Select(kvp => kvp.Value).ToArray()
					let value = Merge(values)
					select new { Key = key, Value = value }
				)
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public static object Merge(Type type, params object[] values)
		{
			if (type.IsValueType)
			{
				var defaultValue = Activator.CreateInstance(type);

				if (!type.GenericTypeArguments.Any())
					return values.FirstOrDefault(v => !Equals(v, defaultValue));

				// Nullable value type
				var emptyValue = Activator.CreateInstance(type.GenericTypeArguments[0]);

				return values.FirstOrDefault(v => !Equals(v, emptyValue) && !Equals(v, defaultValue))
					?? values.FirstOrDefault(v => !Equals(v, emptyValue));
			}

			if (type == typeof(string))
			{
				return MergeStrings(values.OfType<string>().ToArray());
			}

			var properties = type
				.GetProperties()
				.Where(pi => pi.CanRead)
				.Where(pi => pi.CanWrite)
				.Where(pi => (pi.MemberType & MemberTypes.Property) == MemberTypes.Property)
				.ToList();

			if (properties.Any())
			{
				return Merge(type, properties, values);
			}

			if (values[0] is Array)
			{
				return MergeHelpers.Lists(values);
			}

			throw new NotImplementedException();
		}

		public static T Merge<T>(IEnumerable<PropertyInfo> properties, params T[] values)
		{
			return (T)Merge(typeof(T), properties, values.OfType<object>().ToArray());
		}

		public static object Merge(Type type, IEnumerable<PropertyInfo> properties, params object[] objects)
		{
			var merged = Activator.CreateInstance(type);

			foreach (var property in properties)
			{
				var values = objects.Select(property.GetValue).ToArray();

				var value = Merge(property.PropertyType, values);

				property.SetValue(merged, value);
			}

			return merged;
		}

		public static IList MergeLists(params IList[] lists)
		{
			if (!lists.Any())
			{
				throw new ArgumentNullException { Data = { { nameof(lists), lists } } };
			}

			if (!lists.Skip(1).Any())
			{
				return DeepClone(lists.Single());
			}

			var type = lists[0].GetType();

			var count = lists.Select(a => a?.Count ?? 0).Sum();

			var elementType = type.GetElementType() ?? type.GenericTypeArguments[0];

			var target = Array.CreateInstance(elementType, count);

			var index = 0;

			foreach (var list in lists)
			{
				foreach (var item in list)
				{
					var cloned = DeepClone(item);

					target.SetValue(cloned, index++);
				}
			}

			return target;
		}

		public static void CopyValues<T>(ref T destination, params T[] sources)
		{
			if (Equals(destination, default(T)))
			{
				throw new ArgumentNullException { Data = { { nameof(destination), destination } } };
			}

			if (!sources.Any())
			{
				throw new ArgumentOutOfRangeException { Data = { { nameof(sources), sources } } };
			}

			var merged = Merge(sources);

			var type = typeof(T);

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty))
			{
				var defaultValue = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;

				var oldValue = property.GetValue(destination);

				var newValue = property.GetValue(merged);

				if (Equals(oldValue, newValue) || !Equals(oldValue, defaultValue))
				{
					continue;
				}

				property.SetValue(destination, newValue);
			}
		}

		public static T DeepClone<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
		}

		public static IEnumerable<IEnumerable<T>> Partition<T>(ICollection<T> values, int numOfPartitions = 2)
		{
			try
			{
				if (values == null) throw new ArgumentNullException(nameof(values));
				if (!values.Any()) throw new ArgumentException(nameof(values));
				if (numOfPartitions <= 0) throw new ArgumentException(nameof(numOfPartitions));
				if (numOfPartitions > values.Count) throw new InvalidOperationException("More partition than values");
			}
			catch (Exception ex)
			{
				ex.Data.Add(nameof(values), values);
				ex.Data.Add(nameof(numOfPartitions), numOfPartitions);

				throw;
			}

			var partitionSize = values.Count / (double)numOfPartitions;

			return values
				.Select((v, i) => new { v, i })
				.GroupBy(a => Math.Floor(a.i / partitionSize))
				.Select(g => g.Select(a => a.v));
		}

		private static readonly Regex ArticlesRegex = new Regex(@"^(A|An|The) (.+)$");

		public static string MoveArticlesToEnd(string sentence)
		{
			var match = ArticlesRegex.Match(sentence);

			if (!match.Success)
			{
				return sentence;
			}

			return $"{match.Groups[2].Value}, {match.Groups[1].Value}";
		}

		public static string DataSizeToString(long bytes)
		{
			var units = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

			for (var a = 0; a < units.Length; a++)
			{
				var max = Math.Pow(2, (a + 1) * 10);

				if (bytes < max)
				{
					var value = bytes / Math.Pow(2, a * 10);

					return value % 1 > 0
						? $"{value:F2}{units[a]}"
						: $"{value:F0}{units[a]}";
				}
			}

			throw new ArgumentOutOfRangeException { Data = { { nameof(bytes), bytes } } };
		}

		public static ushort SwapBytes(ushort value)
		{
			// Swap adjacent 8-bit blocks
			return (ushort)((value >> 8) | (value << 8));
		}

		public static uint SwapBytes(uint value)
		{
			// swap adjacent 16-bit blocks
			value = (value >> 16) | (value << 16);
			// swap adjacent 8-bit blocks
			return ((value & 0xff00ff00) >> 8) | ((value & 0x00ff00ff) << 8);
		}

		public static ulong SwapBytes(ulong value)
		{
			// swap adjacent 32-bit blocks
			value = (value >> 32) | (value << 32);
			// swap adjacent 16-bit blocks
			value = ((value & 0xffff0000ffff0000) >> 16) | ((value & 0x0000ffff0000ffff) << 16);
			// swap adjacent 8-bit blocks
			return ((value & 0xff00ff00ff00ff00) >> 8) | ((value & 0x00ff00ff00ff00ff) << 8);
		}

		public static byte[] SwapBytes(byte[] bytes)
		{
			switch (bytes.Length)
			{
				case 8:
					{
						var bigEndianValue = BitConverter.ToUInt64(bytes, 0);
						var littleEndianValue = HelperMethods.SwapBytes(bigEndianValue);
						return BitConverter.GetBytes(littleEndianValue);
					}
				case 4:
					{
						var bigEndianValue = BitConverter.ToUInt32(bytes, 0);
						var littleEndianValue = HelperMethods.SwapBytes(bigEndianValue);
						return BitConverter.GetBytes(littleEndianValue);
					}
				case 2:
					{
						var bigEndianValue = BitConverter.ToUInt16(bytes, 0);
						var littleEndianValue = HelperMethods.SwapBytes(bigEndianValue);
						return BitConverter.GetBytes(littleEndianValue);
					}
				case 1:
					return bytes;
				default:
					throw new ArgumentOutOfRangeException { Data = { { nameof(bytes), bytes } } };
			}
		}

		public static byte[] ToLittleEndianBytes(IReduceToBinary value)
		{
			var bytes = value.ToBits().ToBytes().Reverse().ToArray();

			return SwapBytes(bytes);
		}

		public static byte[] ToLittleEndianBytes(params IReduceToBinary[] values)
		{
			var bytes = values
				.Select(v => v.ToBits())
				.Select(bb => bb.Reverse())
				.SelectMany(bb => bb)
				.Reverse()
				.ToBytes()
				.ToArray();

			return SwapBytes(bytes);
		}

		public static string EnumToString<T>(T value) where T : struct, IComparable
		{
			var type = typeof(T);

			if (!type.IsEnum)
			{
				throw new ArgumentOutOfRangeException { Data = { { nameof(T), value } } };
			}

			var left = (ulong)Convert.ChangeType(value, TypeCode.UInt64);
			var names = new List<string>();

			foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var right = (ulong)Convert.ChangeType(fieldInfo.GetValue(value), TypeCode.UInt64);

				if (right == 0)
				{
					continue;
				}

				if ((left & right) == right)
				{
					var name =
						(
							from x in from xx in fieldInfo.GetCustomAttributes<EnumMemberAttribute>(inherit: false).DefaultIfEmpty()
									  select xx?.Value
							from y in from yy in fieldInfo.GetCustomAttributes<DescriptionAttribute>(inherit: false).DefaultIfEmpty()
									  select yy?.Description
							from z in new[] { fieldInfo.Name }
							from n in new[] { x, y, z }
							where !(n is null)
							select n
						)
						.First();

					names.Add(name);
				}
			}

			return string.Join(", ", names);
		}

		public static IDictionary<DateTime, List<DateTime>> GranularDateTimes(ICollection<DateTime> dateTimes)
		{
			var earliest = dateTimes.OrderBy(dt => dt).First();

			var query = from dt in dateTimes
						let diff = dt - earliest
						group dt by (int)diff.TotalDays
						into g
						select new
						{
							key = earliest.AddDays(g.Key).Date,
							values = g.Select(dt => dt).ToList()
						};

			return query
				.ToDictionary(a => a.key, a => a.values);
		}
	}
}
