namespace Common.Binary
{
	public class UIntValue : BinaryValue<uint>
	{
		public UIntValue(short size = 16) : base(size)
		{ }

		public UIntValue(uint value, short size = 32) : base(value, size)
		{ }
	}
}
