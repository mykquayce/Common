using System.Collections.Generic;

namespace Common.Binary.Interfaces
{
	public interface IReduceToBinary
	{
		IEnumerable<bool> ToBits();
		IEnumerable<byte> ToBytes();
	}
}
