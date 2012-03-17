namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct PlayerSelections
	{
		public ControlGroup currentSelection;
		public ControlGroup[] control_groups;
	}
}

