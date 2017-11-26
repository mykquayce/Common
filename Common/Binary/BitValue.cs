namespace Common.Binary
{
	public class BitValue : BinaryValue<bool>
	{
		public BitValue(short size = 16) : base(size)
		{ }

		public BitValue(bool value, short size = 1) : base(value, size)
		{ }
	}
}
