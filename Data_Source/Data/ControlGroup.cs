namespace Data
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct ControlGroup
	{
		public int groupNumber;
		public int totalSelected;
		public int unitTypesSelected;
		public int unitSubGroupSelected;
		public List<int> selected_unit_ids;
	}
}

