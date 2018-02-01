using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Common
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Converts a string to an enum using the name or description
		/// </summary>
		/// <typeparam name="T">The enum to convert to</typeparam>
		/// <param name="value">The string to convert from</param>
		/// <returns></returns>
		public static T ToEnum<T>(this string value) where T : struct, IConvertible
		{
			var type = typeof(T);

			if (!type.IsEnum)
			{
				throw new ArgumentOutOfRangeException(nameof(T), type, $"{nameof(T)} must be an enum")
				{
					Data = { { nameof(T), type } }
				};
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentNullException(nameof(value), $"{nameof(value)} cannot be null or whitespace");
			}

			value = value.Trim();

			if (value.Contains(','))
			{
				var combined = 0L;

				foreach (var result in value.Split(',').ToEnums<T>())
				{
					combined |= Convert.ChangeType(result, typeof(long)) as long? ?? 0L;
				}

				return Enum.ToObject(type, combined) as T? ?? default(T);
			}

			try
			{
				return
				(
					from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
					let fieldName = field.Name
					from attribute in field.GetCustomAttributes<DescriptionAttribute>().DefaultIfEmpty()
					let description = attribute?.Description ?? string.Empty
					from member in field.GetCustomAttributes<EnumMemberAttribute>().DefaultIfEmpty()
					let memberName = member?.Value ?? string.Empty
					let names = new[] { fieldName, description, memberName }
					from name in names
					where string.Equals(value, name, StringComparison.InvariantCultureIgnoreCase)
					select (T)field.GetValue(obj: null)
				).First();
			}
			catch (Exception ex)
			{
				throw new Exception("Error converting string to enum.", ex) { Data = { { nameof(T), type }, { nameof(value), value } } };
			}
		}

		public static IEnumerable<T> ToEnums<T>(this IEnumerable<string> values) where T : struct, IConvertible
		{
			return values.Select(v => v.ToEnum<T>());
		}

		public static bool IsOneFlag<T>(this T value) where T : struct, IConvertible
		{
			var type = typeof(T);

			if (!type.IsEnum)
			{
				throw new ArgumentOutOfRangeException(nameof(T), type, $"{nameof(T)} must be an enum")
				{
					Data = { { nameof(T), type } }
				};
			}

			var intValue = Convert.ToInt32(value);

			return (intValue & (intValue - 1)) == 0;
		}

		public static string GetEnumMemberValue<T>(this T value)
		{
			return GetEnumMemberAttribute(value).Value;
		}

		public static EnumMemberAttribute GetEnumMemberAttribute<T>(this T value)
		{
			return GetAttributes<T, EnumMemberAttribute>(value)[0];
		}

		public static string GetDescription<T>(this T value)
		{
			Contract.Requires(value != null);

			return GetDescriptionAttribute(value).Description;
		}

		public static DescriptionAttribute GetDescriptionAttribute<T>(this T value)
		{
			Contract.Requires(value != null);

			return GetAttributes<T, DescriptionAttribute>(value)[0];
		}

		public static TN[] GetAttributes<T, TN>(this T value) where TN : Attribute
		{
			Contract.Requires(value != null);

			var fieldInfo = typeof(T).GetField(value.ToString());

			var attributes = (TN[])fieldInfo.GetCustomAttributes(typeof(TN), inherit: false);

			if (attributes.Any())
				return attributes;

			throw new ArgumentException("Field doesn't have an attribute of that type")
			{
				Data =
				{
					{ "Attribute type", typeof(TN) },
					{ "Argument type", typeof(T) },
					{ nameof(value), value }
				}
			};
		}

		public static IEnumerable<object> GetValues(this IEnumerable values)
		{
			var enumerator = values.GetEnumerator();

			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		public static IEnumerable<T> GetValues<T>(this IEnumerable values)
		{
			var enumerator = values.GetEnumerator();

			while (enumerator.MoveNext())
			{
				yield return (T)enumerator.Current;
			}
		}

		public static IEnumerable<bool> ToBits(this IEnumerable<byte> bytes)
		{
			return new BitArray(bytes.ToArray()).Cast<bool>();
		}

		public static IEnumerable<byte> ToBytes(this IEnumerable<bool> bits)
		{
			var array = bits.ToArray();
			var bytes = new byte[array.Length / 8];
			new BitArray(array).CopyTo(bytes, 0);
			return bytes;
		}

		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
		{
			key = keyValuePair.Key;
			value = keyValuePair.Value;
		}
	}
}
