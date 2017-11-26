using System.Threading.Tasks;
using NUnit.Framework;

namespace Common.IO.Tests.TestFileSystem
{
    [TestFixture]
    public class TestStreamToString
    {
        [Test]
        public async Task TestStreamToString_TextString()
        {
            // Arrange
            var before = "message";

            var stream = FileSystem.StringToStream(before);

            // Act
            var after = await FileSystem.StreamToStringAsync(stream);

            // Assert
            Assert.AreEqual(before, after);
            Assert.IsNotNull(stream);
        }
    }
}
