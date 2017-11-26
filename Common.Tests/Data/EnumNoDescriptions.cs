using System;

namespace Common.Tests.Data
{
	[Flags]
	public enum EnumNoDescriptions : byte
	{
		Zero = 0,
		One = 1,
		Two = 2,
		Four = 4,
		Eight = 8,
		Sixteen = 16,
		ThirtyTwo = 32,
		SixtyFour = 64,
		OneHundredAndTwentyEight = 128
	}
}
