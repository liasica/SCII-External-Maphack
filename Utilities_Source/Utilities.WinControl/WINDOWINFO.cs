namespace Utilities.WinControl
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWINFO
	{
		public uint cbSize;
		public Utilities.WinControl.RECT rcWindow;
		public Utilities.WinControl.RECT rcClient;
		public uint dwStyle;
		public uint dwExStyle;
		public uint dwWindowStatus;
		public uint cxWindowBorders;
		public uint cyWindowBorders;
		public ushort atomWindowType;
		public ushort wCreatorVersion;
		public WINDOWINFO(bool? filler)
		{
			this = new WINDOWINFO();
			this.cbSize = (uint) Marshal.SizeOf(typeof(WINDOWINFO));
		}
	}
}

