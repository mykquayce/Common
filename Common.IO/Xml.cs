using Common.Collections;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Common.IO
{
	public static class Xml
	{
		public static XmlWriterSettings XmlWriterSettings { get; } = new XmlWriterSettings
		{
			CheckCharacters = true,
			ConformanceLevel = ConformanceLevel.Document,
			Encoding = Encoding.UTF8,
			Indent = true,
			IndentChars = "  ",
			NewLineChars = Environment.NewLine,
			WriteEndDocumentOnClose = true
		};

		public static void Serialize<T>(T value, Stream stream)
		{
			if (stream is null)
			{
				throw new ArgumentNullException { Data = { { nameof(stream), stream } } };
			}

			if (!stream.CanWrite)
			{
				throw new ArgumentOutOfRangeException { Data = { { nameof(stream), stream }, { nameof(stream.CanRead), stream.CanRead } } };
			}

			using (var xmlWriter = XmlWriter.Create(stream, XmlWriterSettings))
			{
				var type = typeof(T);

				var xmlSerializer = XmlSerializerDictionary.Instance[type];

				xmlSerializer.Serialize(xmlWriter, value);
			}
		}

		public static T Deserialize<T>(Stream stream)
		{
			if (stream is null)
			{
				throw new ArgumentNullException { Data = { { nameof(stream), stream } } };
			}

			if (!stream.CanRead)
			{
				throw new ArgumentOutOfRangeException { Data = { { nameof(stream), stream }, { nameof(stream.CanRead), stream.CanRead } } };
			}

			using (var xmlReader = XmlReader.Create(stream))
			{
				var type = typeof(T);

				var xmlSerializer = XmlSerializerDictionary.Instance[type];

				var value = (T)xmlSerializer.Deserialize(xmlReader);

				return value;
			}
		}

		public static T Deserialize<T>(string xmlString)
		{
			if (string.IsNullOrEmpty(xmlString))
			{
				throw new ArgumentNullException { Data = { { nameof(xmlString), xmlString } } };
			}

			using (var stream = FileSystem.StringToStream(xmlString))
			{
				return Deserialize<T>(stream);
			}
		}
	}
}
