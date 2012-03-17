namespace ScreenAPI
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Margins
	{
		public int top;
		public int bot;
		public int left;
		public int right;
	}
}

