namespace Common.Binary
{
	public class ByteValue : BinaryValue<byte>
	{
		public ByteValue(short size = 16) : base(size)
		{ }

		public ByteValue(byte value, short size = 8) : base(value, size)
		{ }
	}
}
