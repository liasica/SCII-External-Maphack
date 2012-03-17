namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential, Size=1)]
	public struct camera_information_s
	{
		public static uint X;
		public static uint Y;
		public static uint Distance;
		public static uint Angel_of_Attack;
		public static uint Rotation;
		static camera_information_s()
		{
			X = 0;
			Y = 4;
			Distance = 8;
			Angel_of_Attack = 12;
			Rotation = 0x10;
		}
	}
}

