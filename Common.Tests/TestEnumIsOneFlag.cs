using Common.Tests.Data;
using NUnit.Framework;

namespace Common.Tests
{
	[TestFixture]
	public class TestEnumIsOneFlag
	{
		[Test]
		[TestCase((EnumNoDescriptions)0, ExpectedResult = true)]
		[TestCase((EnumNoDescriptions)1, ExpectedResult = true)]
		[TestCase((EnumNoDescriptions)2, ExpectedResult = true)]
		[TestCase((EnumNoDescriptions)3, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)4, ExpectedResult = true)]
		[TestCase((EnumNoDescriptions)5, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)6, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)7, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)8, ExpectedResult = true)]
		[TestCase((EnumNoDescriptions)9, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)10, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)11, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)12, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)13, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)14, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)15, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)16, ExpectedResult = true)]
		[TestCase((EnumNoDescriptions)17, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)18, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)19, ExpectedResult = false)]
		[TestCase((EnumNoDescriptions)20, ExpectedResult = false)]
		public bool TestEnumIsOneFlag_EnumNoDescriptions(EnumNoDescriptions value)
		{
			return value.IsOneFlag();
		}
	}
}
