namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential, Size=1)]
	public struct control_group_s
	{
		public static uint total_selected;
		public static uint selected_types;
		public static uint sub_group;
		public static uint tabs;
		public static uint selected_units;
		static control_group_s()
		{
			total_selected = 0;
			selected_types = 1;
			sub_group = 2;
			tabs = 4;
			selected_units = 6;
		}
	}
}

