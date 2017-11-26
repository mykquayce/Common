using Common.Tests.Data;
using NUnit.Framework;

namespace Common.Tests
{
	[TestFixture]
	public class TestToEnum
	{
		[Test]
		[TestCase("One", ExpectedResult = EnumNoDescriptions.One)]
		[TestCase("Two", ExpectedResult = EnumNoDescriptions.Two)]
		[TestCase("Four", ExpectedResult = EnumNoDescriptions.Four)]
		[TestCase("Eight", ExpectedResult = EnumNoDescriptions.Eight)]
		[TestCase("Sixteen", ExpectedResult = EnumNoDescriptions.Sixteen)]
		public EnumNoDescriptions TestToEnum_EnumNoDescriptions_OneValue(string value)
		{
			return value.ToEnum<EnumNoDescriptions>();
		}

		[Test]
		[TestCase("One,Two", ExpectedResult = EnumNoDescriptions.One | EnumNoDescriptions.Two)]
		[TestCase("One, Two", ExpectedResult = EnumNoDescriptions.One | EnumNoDescriptions.Two)]
		public EnumNoDescriptions TestToEnum_EnumNoDescriptions_TwoValues(string value)
		{
			return value.ToEnum<EnumNoDescriptions>();
		}
	}
}
