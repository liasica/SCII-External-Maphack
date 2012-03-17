namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential, Size=1)]
	public struct build_grid_s
	{
		public static uint CurrentStruct;
		public static uint Viewable;
		public static uint Active;
		public static uint X;
		public static uint Y;
		public static uint Z;
		public static uint Valid;
		public static uint ValidSquares;
		public static uint InvalidSquares;
		static build_grid_s()
		{
			CurrentStruct = 0x18;
			Viewable = 40;
			Active = 0x2c;
			X = 0x40;
			Y = 0x44;
			Z = 0x48;
			Valid = 0x4c;
			ValidSquares = 0xa4;
			InvalidSquares = 180;
		}
	}
}

