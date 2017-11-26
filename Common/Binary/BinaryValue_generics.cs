using System;

namespace Common.Binary
{
	public abstract class BinaryValue<T> : BinaryValue
	{
		public new T Value
		{
			get => (T)base.Value;
			set => base.Value = value;
		}

		protected BinaryValue(short size)
			: base(size, Type.GetTypeCode(typeof(T)))
		{ }

		protected BinaryValue(T value, short size)
			: this(size)
		{
			Value = value;
		}
	}
}
