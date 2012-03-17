namespace Data
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct unit_button_s
	{
		//[FixedBuffer(typeof(byte), 0x10), FieldOffset(0)]
		public fixed byte Unknown16[16];
		public ushort button_type;
	}
}

