using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Common.Collections
{
	[Serializable]
	[XmlRoot("Dictionary")]
	public class ObservableSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializable, IXmlSerializable, INotifyCollectionChanged
	{
		private readonly XmlSerializer _xmlSerializer;

		public new TValue this[TKey key]
		{
			get { return base[key]; }
			set
			{
				var newItems = new List<KeyValuePair<TKey, TValue>> { new KeyValuePair<TKey, TValue>(key, value) };

				if (base.ContainsKey(key))
				{
					var oldItems = new List<KeyValuePair<TKey, TValue>> { new KeyValuePair<TKey, TValue>(key, base[key]) };

					NotifyCollectionChanged(NotifyCollectionChangedAction.Replace, newItems, oldItems);

					base[key] = value;
				}
				else
				{
					base.Add(key, value);

					var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems);

					NotifyCollectionChanged(eventArgs);
				}
			}
		}

		public ObservableSerializableDictionary()
		{
			_xmlSerializer = new XmlSerializer(typeof(SerializableKeyValuePair));
		}

		public ObservableSerializableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values) : this()
		{
			foreach (var kvp in values)
			{
				base.Add(kvp.Key, kvp.Value);
			}
		}

		public new void Add(TKey key, TValue value)
		{
			this[key] = value;
		}

		public new bool Remove(TKey key)
		{
			if (!base.ContainsKey(key))
			{
				return false;
			}

			var value = base[key];

			var kvp = new KeyValuePair<TKey, TValue>(key, value);

			if (!base.Remove(key))
			{
				return false;
			}

			var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { kvp });

			NotifyCollectionChanged(eventArgs);

			return true;
		}

		public new void Clear()
		{
			NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);

			base.Clear();
		}

		#region ISerializable members
		public ObservableSerializableDictionary(SerializationInfo info, StreamingContext context) : this()
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

		#endregion IXmlSerializable Members

		#region INotifyCollectionChanged Support
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void NotifyCollectionChanged(NotifyCollectionChangedAction action)
		{
			var eventArgs = new NotifyCollectionChangedEventArgs(action);
			NotifyCollectionChanged(eventArgs);
		}
		private void NotifyCollectionChanged(NotifyCollectionChangedAction action, IList<KeyValuePair<TKey, TValue>> items)
		{
			var eventArgs = new NotifyCollectionChangedEventArgs(action, (IList)items);
			NotifyCollectionChanged(eventArgs);
		}

		private void NotifyCollectionChanged(NotifyCollectionChangedAction action, IList<KeyValuePair<TKey, TValue>> newItems, IList<KeyValuePair<TKey, TValue>> oldItems)
		{
			var eventArgs = new NotifyCollectionChangedEventArgs(action, (IList)newItems, (IList)oldItems);
			NotifyCollectionChanged(eventArgs);
		}

		private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
		{
			CollectionChanged?.Invoke(this, eventArgs);
		}
		#endregion INotifyCollectionChanged Support

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
