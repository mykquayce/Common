using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Common.Collections.Tests
{
	[TestFixture]
	public class TestEquatableCollection
	{
		[Test]
		public void TestEquatableCollection_FiveIntsAreTheSame()
		{
			var left = new EquatableCollection<int> { 0, 1, 2, 3, 4 };
			var right = new EquatableCollection<int> { 0, 1, 2, 3, 4 };

			Test(left, right);
		}
		[Test]
		public void TestEquatableCollection_FiveIntsInADifferentOrderAreTheSame()
		{
			var left = new EquatableCollection<int> { 0, 1, 2, 3, 4 };
			var right = new EquatableCollection<int> { 3, 1, 0, 2, 4 };

			Test(left, right);
		}

		[Test]
		public void TestEquatableCollection_FiveStringsAreTheSame()
		{
			var left = new EquatableCollection<string> { "zero", "one", "two", "three", "four" };
			var right = new EquatableCollection<string> { "zero", "one", "two", "three", "four" };

			Test(left, right);
		}

		[Test]
		public void TestEquatableCollection_FiveStringsInADifferentOrderAreTheSame()
		{
			var left = new EquatableCollection<string> { "zero", "one", "two", "three", "four" };
			var right = new EquatableCollection<string> { "three", "one", "zero", "two", "four" };

			Test(left, right);
		}

		[Test]
		public void TestEquatableCollection_FiveStringsAreDifferentFromSixStrings()
		{
			var left = new EquatableCollection<string> { "zero", "one", "two", "three", "four" };
			var right = new EquatableCollection<string> { "zero", "one", "two", "three", "four", "five" };

			Assert.IsFalse(object.ReferenceEquals(left, right));
			Assert.IsFalse(left == right);
			Assert.IsTrue(left != right);
			Assert.IsFalse(left.Equals(right));
		}

		[Test]
		public void TestEquatableCollection_FiveDifferentInstancesOfTheSameVersionsAreTheSame()
		{
			var left = new EquatableCollection<Version>
			{
				new Version(1,2,3,4),
				new Version(2,3,4,5),
				new Version(3,4,5,6),
				new Version(4,5,6,7),
				new Version(5,6,7,8),
			};

			var right = new EquatableCollection<Version>
			{
				new Version(1,2,3,4),
				new Version(2,3,4,5),
				new Version(3,4,5,6),
				new Version(4,5,6,7),
				new Version(5,6,7,8),
			};

			Test(left, right);
		}

		[Test]
		public void TestEquatableCollection_FiveDifferentInstancesOfTheSameVersionsInADifferentOrderAreTheSame()
		{
			var left = new EquatableCollection<Version>
			{
				new Version(1,2,3,4),
				new Version(2,3,4,5),
				new Version(3,4,5,6),
				new Version(4,5,6,7),
				new Version(5,6,7,8),
			};

			var right = new EquatableCollection<Version>
			{
				new Version(2,3,4,5),
				new Version(1,2,3,4),
				new Version(4,5,6,7),
				new Version(5,6,7,8),
				new Version(3,4,5,6),
			};

			Test(left, right);
		}

		[Test]
		public void TestEquatableCollection_WorksAsDictionaryKeys()
		{
			// Arrange
			var dict = new Dictionary<EquatableCollection<Type>, XmlSerializer>();

			var stringSerializer = new XmlSerializer(typeof(string));

			dict.Add(
				new EquatableCollection<Type> { typeof(string), },
				stringSerializer
			);

			// Act, Assert
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(string), }));
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(string) }], stringSerializer);

			// Arrange
			var typeAndIntSerializer = new XmlSerializer(typeof(Type), new[] { typeof(int), });

			dict.Add(
				new EquatableCollection<Type> { typeof(Type), typeof(int), },
				typeAndIntSerializer);

			// Act, Assert
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(string), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(Type), typeof(int), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(int), typeof(Type), }));
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(string) }], stringSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(Type), typeof(int), }], typeAndIntSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(int), typeof(Type), }], typeAndIntSerializer);

			// Arrange
			var attributeAndDateTimeAndVersionSerializer = new XmlSerializer(typeof(Attribute), new[] { typeof(DateTime), typeof(Version), });

			dict.Add(
				new EquatableCollection<Type> { typeof(Attribute), typeof(DateTime), typeof(Version), },
				attributeAndDateTimeAndVersionSerializer);

			// Act, Assert
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(string), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(Type), typeof(int), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(int), typeof(Type), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(Attribute), typeof(DateTime), typeof(Version), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(Attribute), typeof(Version), typeof(DateTime), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(DateTime), typeof(Attribute), typeof(Version), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(DateTime), typeof(Version), typeof(Attribute), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(Version), typeof(Attribute), typeof(DateTime), }));
			Assert.IsTrue(dict.ContainsKey(new EquatableCollection<Type> { typeof(Version), typeof(DateTime), typeof(Attribute), }));
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(string) }], stringSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(Type), typeof(int), }], typeAndIntSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(int), typeof(Type), }], typeAndIntSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(Attribute), typeof(DateTime), typeof(Version), }], attributeAndDateTimeAndVersionSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(Attribute), typeof(Version), typeof(DateTime), }], attributeAndDateTimeAndVersionSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(DateTime), typeof(Attribute), typeof(Version), }], attributeAndDateTimeAndVersionSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(DateTime), typeof(Version), typeof(Attribute), }], attributeAndDateTimeAndVersionSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(Version), typeof(Attribute), typeof(DateTime), }], attributeAndDateTimeAndVersionSerializer);
			Assert.AreSame(dict[new EquatableCollection<Type> { typeof(Version), typeof(DateTime), typeof(Attribute), }], attributeAndDateTimeAndVersionSerializer);
		}

		private static void Test<T>(EquatableCollection<T> left, EquatableCollection<T> right)
		{
			Assert.IsFalse(left == null);
			Assert.IsFalse(right == null);

			Assert.IsTrue(left.Any());
			Assert.IsTrue(right.Any());

			Assert.IsFalse(object.ReferenceEquals(left, right));
			Assert.IsFalse(object.ReferenceEquals(right, left));

			if (typeof(T) != typeof(string))
			{
				foreach (var l in left)
				{
					foreach (var r in right)
					{
						Assert.IsFalse(object.ReferenceEquals(l, r));
					}
				}
			}

			Assert.IsTrue(left == right);
			Assert.IsTrue(right == left);

			Assert.IsFalse(left != right);
			Assert.IsFalse(right != left);

			Assert.IsTrue(left.Equals(right));
			Assert.IsTrue(right.Equals(left));

			Assert.IsTrue(left.Equals((object)right));
			Assert.IsTrue(right.Equals((object)left));

			Assert.IsTrue(object.Equals(left, right));
			Assert.IsTrue(object.Equals(right, left));

			Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
		}
	}
}
