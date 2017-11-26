namespace Common.Binary
{
	public class UShortValue : BinaryValue<ushort>
	{
		public UShortValue(short size = 16) : base(size)
		{ }

		public UShortValue(ushort value, short size = 16) : base(value, size)
		{ }
	}
}
