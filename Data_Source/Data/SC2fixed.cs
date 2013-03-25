using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Data
{
	//not all of these are implemented yet.
	
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct fixed32
	{
		public static fixed32 MaxValue
		{ get { return new fixed32(Int32.MaxValue, true); } }
		public static fixed32 MinValue
		{ get { return new fixed32(Int32.MinValue, true); } }
		public static fixed32 Zero
		{ get { return new fixed32(0, true); } }
		public static fixed32 Precision
		{ get { return new fixed32(1, true); } }

		[FieldOffset(0)]
		private Int32 _rawData;

		public fixed32(float it)
		{ _rawData = (Int32)Math.Round(it * 4096); } //Rounding is necessary becaus we all know that 1.999999 is really 2.0, not 1.999755.
		public fixed32(double it)
		{ _rawData = (Int32)Math.Round(it * 4096); } //Rounding is necessary becaus we all know that 1.999999 is really 2.0, not 1.999755.
		public fixed32(Int32 it, bool raw = false)
		{
			if (raw)
				_rawData = it;
			else
				_rawData = (Int32)(it * 4096);
		}
		public fixed32(UInt32 it)
		{ _rawData = (Int32)(it * 4096); }
		public fixed32(fixed32 it)
		{ _rawData = it._rawData; }
		/*public fixed32(fixed16 it)
		{ _rawData = it.RawData * 2; }
		public fixed32(fixed8 it)
		{ _rawData = it.RawData * 16; }*/ //These types are not implemented yet.

		public Int32 RawData
		{
			get
			{
				return _rawData;
			}
			set
			{
				_rawData = value;
			}
		}

		public override bool Equals(Object obj)
		{ return obj is fixed32 && this == (fixed32)obj; }
		public override int GetHashCode()
		{ return _rawData.GetHashCode(); }
		public static bool operator ==(fixed32 x, fixed32 y)
		{ return x._rawData == y._rawData; }
		public static bool operator !=(fixed32 x, fixed32 y)
		{ return x._rawData != y._rawData; }
		public static bool operator >(fixed32 x, fixed32 y)
		{ return x._rawData > y._rawData; }
		public static bool operator <(fixed32 x, fixed32 y)
		{ return x._rawData < y._rawData; }
		public static bool operator >=(fixed32 x, fixed32 y)
		{ return x._rawData >= y._rawData; }
		public static bool operator <=(fixed32 x, fixed32 y)
		{ return x._rawData <= y._rawData; }

		public static fixed32 operator +(fixed32 x, fixed32 y)
		{ return new fixed32(x._rawData + y._rawData, true); }
		public static fixed32 operator -(fixed32 x, fixed32 y)
		{ return new fixed32(x._rawData - y._rawData, true); }
		public static fixed32 operator *(fixed32 x, fixed32 y)
		{ return new fixed32((double)x * (double)y); }
		public static fixed32 operator /(fixed32 x, fixed32 y)
		{ return new fixed32((double)x / (double)y); }


		public static implicit operator float(fixed32 it)
		{ return (float)it._rawData / 4096f; }
		public static implicit operator double(fixed32 it)
		{ return (double)it._rawData / 4096d; }
		public static explicit operator Int64(fixed32 it)
		{ return it._rawData / 4096; }
		public static explicit operator Int32(fixed32 it)
		{ return it._rawData / 4096; }
		public static explicit operator Int16(fixed32 it)
		{ return (Int16)(it._rawData / 4096); }
		public static explicit operator SByte(fixed32 it)
		{ return (SByte)(it._rawData / 4096); }
		public static explicit operator UInt64(fixed32 it)
		{ return (UInt64)(it._rawData / 4096); }
		public static explicit operator UInt32(fixed32 it)
		{ return (UInt32)(it._rawData / 4096); }
		public static explicit operator UInt16(fixed32 it)
		{ return (UInt16)(it._rawData / 4096); }
		public static explicit operator Byte(fixed32 it)
		{ return (Byte)(it._rawData / 4096); }

		public static implicit operator fixed32(float it)
		{ return new fixed32(it); }
		public static implicit operator fixed32(double it)
		{ return new fixed32(it); }
		public static implicit operator fixed32(Int32 it)
		{ return new fixed32(it); }
		/*public static implicit operator fixed32(fixed16 it)
		{ return new fixed32(it); }
		public static implicit operator fixed32(fixed8 it)
		{ return new fixed32(it); }*/

		public override string ToString()
		{
			return ((float)this).ToString();
		}
	}
	
	//these are not very common, so I'm not going to do anything with them till I know fixed32 works right.
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct fixed16 
	{
	}
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct fixed8
	{
	}
}
