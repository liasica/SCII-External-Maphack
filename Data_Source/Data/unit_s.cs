namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit, Pack=1)]
	public struct unit_s
	{
		[FieldOffset(0x29)]
		public byte isImmobile;
		[FieldOffset(0x14)]
		public ulong targetFilter_flags;
		[FieldOffset(0x14c)]
		public uint time_scale;
		[FieldOffset(0x128)]
		public int health_regen_bonus;
		[FieldOffset(0x12c)]
		public int shield_regen_bonus;
		[FieldOffset(0x130)]
		public int energy_regen_bonus;

		[FieldOffset(0xcc)]
		public uint abilities_pointer;
		[FieldOffset(0x174)]
		public uint bountyCustom;
		[FieldOffset(360)]
		public uint bountyMinerals;
		[FieldOffset(0x170)]
		public uint bountyTerrazine;
		[FieldOffset(0x16c)]
		public uint bountyVespene;
		[FieldOffset(0x178)]
		public uint bountyXP;
		[FieldOffset(0x17c)]
		public byte cellX_approx;
		[FieldOffset(0x17d)]
		public byte cellY_approx;
		[FieldOffset(0xc4)]
		public uint commandQueue_pointer;
		[FieldOffset(0x69)]
		public uint death_type;
		[FieldOffset(0x74)]
		public uint destination_x;
		[FieldOffset(120)]
		public uint destination_y;
		[FieldOffset(0x7c)]
		public uint destination_z;
		[FieldOffset(0x88)]
		public uint destination2_x;
		[FieldOffset(140)]
		public uint destination2_y;
		[FieldOffset(0x98)]
		public uint destination3_x;
		[FieldOffset(0x9c)]
		public uint destination3_y;
		[FieldOffset(0x10c)]
		public uint energy;
		[FieldOffset(0x124)]
		public uint energy_multiplier;
		[FieldOffset(260)]
		public uint health_damage;
		[FieldOffset(0x11c)]
		public uint health_multiplier;
		[FieldOffset(50)]
		public ushort kills;
		[FieldOffset(0x84)]
		public uint last_order;
		[FieldOffset(0x155)]
		public uint lastAttacked;
		[FieldOffset(0x151)]
		public uint lifespan;
		[FieldOffset(280)]
		public int bonus_max_energy;
		[FieldOffset(0x110)]
		public int bonus_max_health;
		[FieldOffset(0x114)]
		public int bonus_max_shields;
		[FieldOffset(0xb8)]
		public uint move_speed;
		[FieldOffset(0x22)]
		public byte move_state;
		[FieldOffset(6)]
		public ushort next_unit;
		[FieldOffset(0x27)]
		public byte player_owner;
		[FieldOffset(0x34)]
		public byte player_owner2;
		[FieldOffset(0x35)]
		public byte player_owner3;
		[FieldOffset(0x40)]
		public uint position_x;
		[FieldOffset(0x44)]
		public uint position_y;
		[FieldOffset(0x48)]
		public uint position_z;
		[FieldOffset(4)]
		public ushort prev_unit;
		[FieldOffset(0x60)]
		public uint randomTimer;
		[FieldOffset(0x5c)]
		public int rotation;
		[FieldOffset(0x54)]
		public int rotation_x;
		[FieldOffset(0x58)]
		public int rotation_y;
		[FieldOffset(0x108)]
		public uint shield_damage;
		[FieldOffset(0x120)]
		public uint shields_multiplier;
		[FieldOffset(0x90)]
		public uint start_position_x;
		[FieldOffset(0x94)]
		public uint start_position_y;
		[FieldOffset(160)]
		public uint start_position2_x;
		[FieldOffset(0xa4)]
		public uint start_position2_y;
		[FieldOffset(0x23)]
		public byte state;
		[FieldOffset(0x21)]
		public byte sub_move_state;
		[FieldOffset(0)]
		public ushort times_used;
		[FieldOffset(2)]
		public ushort token;
		[FieldOffset(8)]
		public uint unit_model;
	}
}

