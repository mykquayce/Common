using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Common.Binary
{
	public abstract class BinaryValue : INotifyPropertyChanged, Interfaces.IReduceToBinary
	{
		#region Private members
		private static readonly TypeCode[] ValidTypeCodes = { TypeCode.Boolean, TypeCode.Byte, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64 };
		private short _size;
		private TypeCode _typeCode;
		private object _value;
		#endregion Private members

		#region Properties
		public short Size
		{
			get => _size;
			set
			{
				if (_size == value) return;
				_size = value;
				NotifyPropertyChanged();
			}
		}

		public TypeCode TypeCode
		{
			get => _typeCode;
			set
			{
				if (_typeCode == value) return;
				_typeCode = value;
				NotifyPropertyChanged();
			}
		}

		public object Value
		{
			get => _value;
			set
			{
				if (_value == value) return;
				_value = value;
				NotifyPropertyChanged();
			}
		}
		#endregion Properties

		#region Constructor / destructor
		protected BinaryValue(short size, TypeCode typeCode)
		{
			Size = size;
			TypeCode = typeCode;
			PropertyChanged += BinaryValue_PropertyChanged;
		}

		~BinaryValue()
		{
			PropertyChanged -= BinaryValue_PropertyChanged;

		}
		#endregion Constructor / destructor

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void BinaryValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(Size):
					if (Size <= 0 || Size >= 64)
					{
						throw new ArgumentOutOfRangeException { Data = { { nameof(Size), Size } } };
					}
					break;
				case nameof(TypeCode):
					if (!ValidTypeCodes.Contains(TypeCode))
					{
						throw new ArgumentOutOfRangeException { Data = { { nameof(TypeCode), TypeCode } } };
					}
					break;
			}
		}
		#endregion INotifyPropertyChanged

		#region IReduceToBinary
		public IEnumerable<bool> ToBits()
		{
			var bytes = ToBytes().ToArray();
			return new BitArray(bytes).Cast<bool>().Take(Size).ToArray();
		}

		public IEnumerable<byte> ToBytes()
		{
			object converted;

			try
			{
				converted = Convert.ChangeType(Value, TypeCode);

				if (converted is null)
				{
					throw new ArgumentOutOfRangeException();
				}
			}
			catch (Exception ex)
			{
				ex.Data.Add(nameof(Value), Value);
				ex.Data.Add(nameof(TypeCode), TypeCode);
				throw;
			}

			switch (TypeCode)
			{
				case TypeCode.Boolean:
					return BitConverter.GetBytes((bool)converted);
				case TypeCode.Byte:
					return new[] { (byte)converted };
				case TypeCode.UInt16:
					return BitConverter.GetBytes((ushort)converted);
				case TypeCode.UInt32:
					return BitConverter.GetBytes((uint)converted);
				case TypeCode.UInt64:
					return BitConverter.GetBytes((ulong)converted);
				default:
					throw new ArgumentOutOfRangeException { Data = { { nameof(TypeCode), TypeCode } } };
			}
		}
		#endregion IReduceToBinary
	}
}
