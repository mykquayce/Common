using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common.IO
{
	public static class FileSystem
	{
		public static string GetFileContents(string filePath, Encoding encoding)
		{
			Contract.Requires(filePath != null);
			Contract.Requires(encoding != null);
			Contract.Requires(filePath != string.Empty);

			return GetFileContents(new FileInfo(filePath), encoding);
		}

		public static string GetFileContents(FileInfo file, Encoding encoding)
		{
			Contract.Requires(file != null);
			Contract.Requires(encoding != null);
			Contract.Requires(file.Exists);

			using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				return GetFileContents(stream, encoding);
			}
		}

		public static string GetFileContents(Stream stream, Encoding encoding)
		{
			Contract.Requires(stream != null);
			Contract.Requires(encoding != null);
			Contract.Requires(stream.CanRead);

			using (var streamReader = new StreamReader(stream, encoding))
			{
				return GetFileContents(streamReader);
			}
		}

		public static string GetFileContents(TextReader streamReader)
		{
			Contract.Requires(streamReader != null);
			return streamReader.ReadToEnd();
		}

		public static Stream StringToStream(string value, Encoding encoding)
		{
			Contract.Requires(value != null);
			Contract.Requires(value != string.Empty);
			Contract.Requires(encoding != null);

			return new MemoryStream(encoding.GetBytes(value)) { Position = 0 };
		}

		public static Stream StringToStream(string value)
		{
			Contract.Requires(value != null);
			Contract.Requires(value != string.Empty);

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(value);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public static async Task SaveAsync(string path, string contents)
		{
			var file = new FileInfo(path);

			using (var writer = file.Exists ? file.AppendText() : file.CreateText())
			{
				await writer.WriteAsync(contents);
				await writer.FlushAsync();
			}
		}

		public static async Task<string> StreamToStringAsync(Stream stream)
		{
			if (stream is null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (!stream.CanRead)
			{
				throw new InvalidOperationException("Stream doesn't support reading.");
			}

			if (stream.Position > 0 && !stream.CanSeek)
			{
				throw new InvalidOperationException("Cannot read from the beginning of the stream.");
			}

			var position = stream.Position;

			if (position > 0)
			{
				stream.Seek(0, SeekOrigin.Begin);
			}

			var buffer = new byte[stream.Length];

			await stream.ReadAsync(buffer, 0, buffer.Length);


			if (stream.CanSeek)
			{
				stream.Seek(position, SeekOrigin.Begin);
			}

			using (var streamReader = new StreamReader(stream))
			{
				var str = await streamReader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false);


				if (!string.IsNullOrEmpty(str))
				{
					return str;
				}
			}

			return null;
		}
	}
}
