namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential, Size=1)]
	public struct camera_s
	{
		public static uint current_x;
		public static uint current_y;
		public static uint future_x;
		public static uint future_y;
		static camera_s()
		{
			current_x = 160;
			current_y = 0xa4;
			future_x = 0xac;
			future_y = 0xb0;
		}
	}
}

