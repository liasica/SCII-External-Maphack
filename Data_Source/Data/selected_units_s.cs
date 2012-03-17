namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential, Size=1)]
	public struct selected_units_s
	{
		public static uint unit_id;
		public static uint next_unit;
		static selected_units_s()
		{
			unit_id = 0;
			next_unit = 2;
		}
	}
}

