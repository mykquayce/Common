using System;
using System.IO;
using NUnit.Framework;

namespace Common.IO.Tests.TestWeb
{
	[TestFixture]
	public class TestDownloadFile
	{
		[Test, Explicit("Calls a third-party API")]
		public void TestDownloadFile_NzbFile_Live()
		{
			// Arrange
			var uri = new Uri(@"https://nzbfinder.ws/getnzb/4cad23f5c29ed1fadbd2edcd60d52b5f21d830bd.nzb&i=86599&r=");
			var destination = new FileInfo($@"{Path.GetTempPath()}\{Guid.NewGuid()}.nzb");
			var task = Web.DownloadFileAsync(uri, destination);

			// Act
			var success = task.Result;
			destination.Refresh();

			// Assert
			Assert.IsTrue(success);
			Assert.IsTrue(destination.Exists);

			destination.Delete();
		}
	}
}
