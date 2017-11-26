using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Common.Collections
{
	public sealed class XmlSerializerDictionary
	{
		#region Singleton pattern
		private static volatile XmlSerializerDictionary _instance;
		private static readonly object SyncLock = new object();

		public static XmlSerializerDictionary Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}

				lock (SyncLock)
				{
					if (_instance == null)
					{
						_instance = new XmlSerializerDictionary();
					}
				}

				return _instance;
			}
		}

		private XmlSerializerDictionary() { }
		#endregion Singleton pattern

		private readonly Dictionary<TypesCollection, XmlSerializer> _dictionary = new Dictionary<TypesCollection, XmlSerializer>();

#if DEBUG
		internal Dictionary<TypesCollection, XmlSerializer> Dictionary => _dictionary;
#endif

		public XmlSerializer this[Type type] => this[new TypesCollection(type)];

		public XmlSerializer this[IEnumerable<Type> types] => this[new TypesCollection(types)];

		public XmlSerializer this[params Type[] types] => this[new TypesCollection(types)];

		public XmlSerializer this[TypesCollection types]
		{
			get
			{
				if (!_dictionary.ContainsKey(types))
				{
					var type = types.First();
					var extraTypes = types.Skip(1).ToArray();

					var serializer = new XmlSerializer(type, extraTypes);

					_dictionary.Add(types, serializer);
				}

				return _dictionary[types];
			}
		}
	}
}
