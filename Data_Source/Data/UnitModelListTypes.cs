namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct LinkedListEntry
	{
		[FieldOffset(0x4)]
		public uint EntryNumber;
		[FieldOffset(0xc)]
		public uint NameStartAddress;
		[FieldOffset(0x10)]
		public uint NameEndAddress;
		[FieldOffset(0x14)]
		public uint OriginalFilenameStartAddress;
		[FieldOffset(0x18)]
		public uint OriginalFilenameEndAddress;
		[FieldOffset(0x5c)]
		public uint pModelArray;
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct UnitModel
	{

	}
}