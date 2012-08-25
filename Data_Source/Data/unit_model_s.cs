namespace Data
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public unsafe struct unit_model_s //this whole thing is outdated except for minimap_radius
	{
		[FieldOffset(0x18)]
		public ushort unit_type;
		[FieldOffset(0x6c)]
		public uint pName_address;	
		[FieldOffset(0x3d8)] //3b4 as of 1.4.3
		public fixed32 minimap_radius;
		[FieldOffset(0x508)]
		public ulong default_targetFilter_flags;

		[FieldOffset(0x7c8)]
		public fixed32 starting_health;
		[FieldOffset(0x7cc)]
		public fixed32 max_health;
		[FieldOffset(0x7d0)]
		public uint life_armor;

		[FieldOffset(0x7d8)]
		public uint health_regen_delay;
		[FieldOffset(0x7dc)]
		public uint health_regen_rate;

		[FieldOffset(0x7ec)]
		public fixed32 starting_energy;
		[FieldOffset(0x7f0)]
		public fixed32 max_energy;
		[FieldOffset(0x7f4)]
		public uint energy_regen_delay;
		[FieldOffset(0x7f8)]
		public fixed32 energy_regen_rate;
		[FieldOffset(0x7fc)]
		public fixed32 starting_shield;
		[FieldOffset(0x800)]
		public fixed32 max_shield;
		[FieldOffset(0x804)]
		public uint shield_armor;

		[FieldOffset(0x80c)]
		public uint shield_regen_delay;
		[FieldOffset(0x810)]
		public fixed32 shield_regen_rate;
	}
}

