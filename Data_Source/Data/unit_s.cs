namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct unit_s
	{
		[FieldOffset(0x0)]
		public ushort times_used;
		[FieldOffset(0x2)]
		public ushort token;
		[FieldOffset(0x4)]
		public ushort prev_unit;
		[FieldOffset(0x6)]
		public ushort next_unit;
		[FieldOffset(0x8)]
		public uint unit_model;
		[FieldOffset(0x14)]
		public ulong targetFilter_flags;
		[FieldOffset(0x21)]
		public byte sub_move_state;
		[FieldOffset(0x22)]
		public byte move_state;
		[FieldOffset(0x26)]
		public byte state;
		[FieldOffset(0x2a)]
		public byte player_owner;
		[FieldOffset(0x2c)]
		public byte isImmobile;
		[FieldOffset(0x36)]
		public ushort kills;
		[FieldOffset(0x38)]
		public byte player_owner2;
		[FieldOffset(0x39)]
		public byte player_owner3;
		[FieldOffset(0x44)]
		public uint position_x;
		[FieldOffset(0x48)]
		public uint position_y;
		[FieldOffset(0x4c)]
		public uint position_z;
		[FieldOffset(0x58)]
		public int rotation_x;
		[FieldOffset(0x5c)]
		public int rotation_y;
		[FieldOffset(0x60)]
		public int rotation;
		[FieldOffset(0x64)]
		public uint randomTimer;
		[FieldOffset(0x6d)]
		public uint death_type;
		[FieldOffset(0x78)]
		public uint destination_x;
		[FieldOffset(0x7c)]
		public uint destination_y;
		[FieldOffset(0x80)]
		public uint destination_z;
		[FieldOffset(0x88)]
		public uint last_order;
		[FieldOffset(0x8c)]
		public uint destination2_x;
		[FieldOffset(0x90)]
		public uint destination2_y;
		[FieldOffset(0x94)]
		public uint start_position_x;
		[FieldOffset(0x98)]
		public uint start_position_y;
		[FieldOffset(0x9c)]
		public uint destination3_x;
		[FieldOffset(0xa0)]
		public uint destination3_y;
		[FieldOffset(0xa4)]
		public uint start_position2_x;
		[FieldOffset(0xa8)]
		public uint start_position2_y;
		[FieldOffset(0xc0)]
		public uint move_speed;
		[FieldOffset(0xcc)]
		public uint commandQueue_pointer;
		[FieldOffset(0xd4)]
		public uint abilities_pointer;
		[FieldOffset(0x10c)]
		public uint health_damage;
		[FieldOffset(0x110)]
		public uint shield_damage;
		[FieldOffset(0x114)]
		public uint energy;
		[FieldOffset(0x118)]
		public int bonus_max_health;
		[FieldOffset(0x11c)]
		public int bonus_max_shields;
		[FieldOffset(0x120)]
		public int bonus_max_energy;
		[FieldOffset(0x124)]
		public uint health_multiplier;
		[FieldOffset(0x128)]
		public uint shields_multiplier;
		[FieldOffset(0x12c)]
		public uint energy_multiplier;
		[FieldOffset(0x130)]
		public int health_regen_bonus;
		[FieldOffset(0x134)]
		public int shield_regen_bonus;
		[FieldOffset(0x138)]
		public int energy_regen_bonus;
		[FieldOffset(0x154)]
		public uint time_scale;
		[FieldOffset(0x159)]
		public uint lifespan;
		[FieldOffset(0x15d)]
		public uint lastAttacked;
		[FieldOffset(0x170)]
		public uint bountyMinerals;
		[FieldOffset(0x174)]
		public uint bountyVespene;
		[FieldOffset(0x178)]
		public uint bountyTerrazine;
		[FieldOffset(0x17c)]
		public uint bountyCustom;
		[FieldOffset(0x180)]
		public uint bountyXP;
		[FieldOffset(0x184)]
		public byte cellX_approx;
		[FieldOffset(0x185)]
		public byte cellY_approx;
	}
}

