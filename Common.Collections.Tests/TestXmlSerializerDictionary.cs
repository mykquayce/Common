using System;
using NUnit.Framework;
using Common.Collections;

namespace Common.Collections.Tests
{
	[TestFixture]
	public class TestXmlSerializerDictionary
	{
		public class Object1
		{
			public int Number { get; set; }
			public string Name { get; set; }
			public Version Version { get; set; }
		}

		public class Object2
		{
			public float Number { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void TestXmlSerializerDictionary_OneType()
		{
			XmlSerializerDictionary.Instance.Dictionary.Clear();

			Assert.AreEqual(0, XmlSerializerDictionary.Instance.Dictionary.Count);

			var serializer = XmlSerializerDictionary.Instance[typeof(Object1)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object2)];

			Assert.AreEqual(2, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object2)];

			Assert.AreEqual(2, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object1)];

			Assert.AreEqual(2, XmlSerializerDictionary.Instance.Dictionary.Count);
		}

		[Test]
		public void TestXmlSerializerDictionary_TwoTypes()
		{
			XmlSerializerDictionary.Instance.Dictionary.Clear();

			Assert.AreEqual(0, XmlSerializerDictionary.Instance.Dictionary.Count);

			var serializer = XmlSerializerDictionary.Instance[typeof(Object1), typeof(Object2)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object1), typeof(Object2)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object2), typeof(Object1)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);
		}

		[Test]
		public void TestXmlSerializerDictionary_MultipleTypesWithDuplicates()
		{
			XmlSerializerDictionary.Instance.Dictionary.Clear();

			Assert.AreEqual(0, XmlSerializerDictionary.Instance.Dictionary.Count);

			var serializer = XmlSerializerDictionary.Instance[typeof(Object1), typeof(Object2)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object1), typeof(Object1), typeof(Object2)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object1), typeof(Object2), typeof(Object2)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);

			serializer = XmlSerializerDictionary.Instance[typeof(Object1), typeof(Object1), typeof(Object2), typeof(Object2)];

			Assert.AreEqual(1, XmlSerializerDictionary.Instance.Dictionary.Count);
		}
	}
}
