using System;
using System.Net;

namespace Common.Exceptions
{
	public abstract class WebResponseException : Exception
	{
		protected WebResponseException(Uri uri, WebResponse webResponse)
		{
			Data.Add(nameof(uri), uri);
			Data.Add(nameof(webResponse), webResponse);
		}
	}

	public class NullWebResponesStreamException : WebResponseException
	{
		public NullWebResponesStreamException(Uri uri, WebResponse webResponse)
			: base(uri, webResponse)
		{ }
	}

	public class UnknownWebResponseException : WebResponseException
	{
		public UnknownWebResponseException(Uri uri, WebResponse webResponse)
			: base(uri, webResponse)
		{ }
	}
}
