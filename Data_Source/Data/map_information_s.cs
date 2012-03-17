namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit)]
	public struct map_information_s
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=20), FieldOffset(0)]
		public byte[] _reserved;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=80), FieldOffset(680)]
		public byte[] _reserved4;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=80), FieldOffset(0x300)]
		public byte[] _reserved5;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=80), FieldOffset(0x358)]
		public byte[] _reserved6;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40), FieldOffset(760)]
		public string Author;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40), FieldOffset(0x350)]
		public string DescriptionBasic;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x255), FieldOffset(0x3a8)]
		public string DescriptionExtended;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x255), FieldOffset(20)]
		public string FilePath;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x255), FieldOffset(0x130)]
		public string FilePath2;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40), FieldOffset(0x2a0)]
		public string Name;
	}
}

