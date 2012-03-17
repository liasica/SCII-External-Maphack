using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Data.Data
{
	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct unit_name_length_s
	{
		[FieldOffset(0)]
		public int name_length;
	}
}
