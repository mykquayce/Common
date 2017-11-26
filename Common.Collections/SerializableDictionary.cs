using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Common.Collections
{
	[Serializable]
	[XmlRoot("Dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializable, IXmlSerializable
	{
		private readonly XmlSerializer _xmlSerializer;

		public SerializableDictionary()
		{
			_xmlSerializer = new XmlSerializer(typeof(SerializableKeyValuePair));
		}

		public SerializableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values) : this()
		{
			foreach (var kvp in values)
			{
				base.Add(kvp.Key, kvp.Value);
			}
		}

		#region ISerializable members
		public SerializableDictionary(SerializationInfo info, StreamingContext context) : this()
		{
			var count = (int)info.GetValue("Count", typeof(int));

			for (var a = 0; a < count; a++)
			{
				var key = (TKey)info.GetValue($"Key{a}", typeof(TKey));
				var value = (TValue)info.GetValue($"Value{a}", typeof(TValue));

				base.Add(key, value);
			}
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Count", base.Count, typeof(int));

			var index = 0;

			foreach (var key in base.Keys)
			{
				info.AddValue($"Key{index}", key, typeof(TKey));
				info.AddValue($"Value{index}", base[key], typeof(TValue));

				index++;
			}

			base.GetObjectData(info, context);
		}
		#endregion ISerializable members

		#region IXmlSerializable Members
		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			XDocument document;

			using (var subtreeReader = reader.ReadSubtree())
			{
				document = XDocument.Load(subtreeReader);
			}

			foreach (var item in document.Descendants(XName.Get("Item")))
			{
				using (var itemReader = item.CreateReader())
				{
					var kvp = (SerializableKeyValuePair)_xmlSerializer.Deserialize(itemReader);
					base.Add(kvp.Key, kvp.Value);
				}
			}

			if (!reader.IsEmptyElement)
				reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			var ns = new XmlSerializerNamespaces();

			ns.Add("", "");

			foreach (var key in base.Keys)
			{
				var value = this[key];
				var kvp = new SerializableKeyValuePair(key, value);
				_xmlSerializer.Serialize(writer, kvp, ns);
			}
		}
		#endregion

		[XmlRoot("Item")]
		public class SerializableKeyValuePair
		{
			[XmlAttribute("Key")]
			public TKey Key;

			[XmlAttribute("Value")]
			public TValue Value;

			public SerializableKeyValuePair() { }

			public SerializableKeyValuePair(TKey key, TValue value)
			{
				Key = key;
				Value = value;
			}
		}
	}
}
