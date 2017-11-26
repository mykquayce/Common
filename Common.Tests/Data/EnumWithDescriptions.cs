using System;

namespace Common.Tests.Data
{
	[Flags]
	public enum EnumWithDescriptions : byte
	{
		[System.ComponentModel.Description("Zero")]
		Zero = 0,
		[System.ComponentModel.Description("Ichi")]
		One = 1,
		[System.ComponentModel.Description("Ni")]
		Two = 2,
		[System.ComponentModel.Description("Yon")]
		Four = 4,
		[System.ComponentModel.Description("Hachi")]
		Eight = 8,
		[System.ComponentModel.Description("Jurocku")]
		Sixteen = 16,
		[System.ComponentModel.Description("Sanjuni")]
		ThirtyTwo = 32,
		[System.ComponentModel.Description("Rockujuyon")]
		SixtyFour = 64,
		[System.ComponentModel.Description("Honnijuhachi")]
		OneHundredAndTwentyEight = 128
	}
}
