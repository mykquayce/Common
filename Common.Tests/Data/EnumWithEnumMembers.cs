using System;
using System.Runtime.Serialization;

namespace Common.Tests.Data
{
	[Flags]
	public enum EnumWithEnumMembers : byte
	{
		[EnumMember(Value = "Zero")]
		Zero = 0,
		[EnumMember(Value = "Un")]
		One = 1,
		[EnumMember(Value = "Duex")]
		Two = 2,
		[EnumMember(Value = "Quatre")]
		Four = 4,
		[EnumMember(Value = "Huit")]
		Eight = 8,
		[EnumMember(Value = "Seize")]
		Sixteen = 16,
		[EnumMember(Value = "TrenteDeux")]
		ThirtyTwo = 32,
		[EnumMember(Value = "SoixanteQuatre")]
		SixtyFour = 64,
		[EnumMember(Value = "CentEtVingtHuit")]
		OneHundredAndTwentyEight = 128
	}
}
