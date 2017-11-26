using System;
using System.Linq;
using NUnit.Framework;

namespace Common.IO.Tests.TestWeb
{
	[TestFixture]
	public class TestDownloadString
	{
		[Test, Explicit("Calls a third-party website")]
		public void TestDownloadString_4ChanWebPage()
		{
			// Arrange
			var uri = new Uri("https://boards.4chan.org/hr/thread/2988202", UriKind.Absolute);

			// Act
			var html = Web.DownloadString(uri);

			// Assert
			Assert.IsTrue(html?.Any() ?? false);
		}
	}
}
