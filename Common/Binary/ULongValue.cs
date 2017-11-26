namespace Common.Binary
{
	public class ULongValue : BinaryValue<ulong>
	{
		public ULongValue(short size = 16) : base(size)
		{ }

		public ULongValue(ulong value, short size = 64) : base(value, size)
		{ }
	}
}
