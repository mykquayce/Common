using Common.Tests.Data;
using NUnit.Framework;

namespace Common.Tests
{
	[TestFixture]
	public partial class TestEnumToString
	{
		[Test]
		public void TestEnumToString_NoDescriptions()
		{
			// Arrange
			var value = EnumNoDescriptions.One | EnumNoDescriptions.SixtyFour;

			// Act
			var actual = HelperMethods.EnumToString(value);

			// Assert
			Assert.AreEqual("One, SixtyFour", actual);
		}

		[Test]
		public void TestEnumToString_Descriptions()
		{
			// Arrange
			var value = EnumWithDescriptions.One | EnumWithDescriptions.SixtyFour;

			// Act
			var actual = HelperMethods.EnumToString(value);

			// Assert
			Assert.AreEqual("Ichi, Rockujuyon", actual);
		}

		[Test]
		public void TestEnumToString_EnumMembers()
		{
			// Arrange
			var value = EnumWithEnumMembers.One | EnumWithEnumMembers.SixtyFour;

			// Act
			var actual = HelperMethods.EnumToString(value);

			// Assert
			Assert.AreEqual("Un, SoixanteQuatre", actual);
		}
	}
}
