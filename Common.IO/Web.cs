using Common.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Common.IO
{
	public static class Web
	{
		private const int RequestTimeout = 60_000;
		private const string UserAgentString = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322)";

		public static async Task<WebResponse> GetResponseAsync(Uri uri)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(uri);

			webRequest.Timeout = RequestTimeout;
			webRequest.UserAgent = UserAgentString;
			webRequest.Accept = "*/*";

			var webResponse = await webRequest.GetResponseAsync().ConfigureAwait(continueOnCapturedContext: true);

			return webResponse;
		}

		public static WebResponse GetResponse(Uri uri)
		{
			return GetResponseAsync(uri).Result;
		}

		public static async Task<Stream> GetResponseStreamAsync(Uri uri)
		{
			var webResponse = await GetResponseAsync(uri).ConfigureAwait(continueOnCapturedContext: true);

			var httpWebResponse = (HttpWebResponse)webResponse;

			switch (httpWebResponse.StatusCode)
			{
				case HttpStatusCode.OK:
					var stream = httpWebResponse.GetResponseStream();

					if (stream == null)
					{
						throw new NullWebResponesStreamException(uri, webResponse);
					}

					return stream;
				default:
					throw new UnknownWebResponseException(uri, webResponse);
			}
		}

		public static Stream GetResponseStream(Uri uri)
		{
			return GetResponseStreamAsync(uri).Result;
		}

		public static async Task<string> DownloadStringAsync(Uri uri)
		{
			using (var stream = await GetResponseStreamAsync(uri).ConfigureAwait(continueOnCapturedContext: true))
			{
				using (var streamReader = new StreamReader(stream))
				{
					return await streamReader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: true);
				}
			}
		}

		public static string DownloadString(Uri uri)
		{
			return DownloadStringAsync(uri).Result;
		}

		public static async Task<bool> DownloadFileAsync(Uri uri, FileInfo destination)
		{
			if (uri is null) throw new ArgumentNullException { Data = { { nameof(uri), uri } } };
			if (!uri.IsAbsoluteUri) throw new ArgumentOutOfRangeException { Data = { { nameof(uri), uri } } };

			if (destination is null) throw new ArgumentNullException { Data = { { nameof(destination), destination } } };
			if (destination.Exists) throw new ArgumentOutOfRangeException { Data = { { nameof(destination), destination } } };

			using (var stream = await GetResponseStreamAsync(uri).ConfigureAwait(continueOnCapturedContext: true))
			{
				using (var fileStream = destination.OpenWrite())
				{
					var buffer = new byte[32 * 1024];
					int read;

					do
					{
						read = stream.Read(buffer, 0, buffer.Length);
						fileStream.Write(buffer, 0, read);
					} while (read > 0);
				}
			}

			return true;
		}

		public static bool DownloadFile(Uri uri, FileInfo destination)
		{
			return DownloadFileAsync(uri, destination).Result;
		}
	}
}
