namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit)]
	public struct player_s
	{
		[FieldOffset(0x248)]
		public uint actions_total;
		[FieldOffset(0)]
		public uint active;
		[FieldOffset(0x270)]
		public uint apm_current;
		[FieldOffset(0x448)]
		public uint army_size;
		[FieldOffset(0xf8)]
		public uint attack_multiplier;
		[FieldOffset(0x420)]
		public uint building_queue_length;
		[FieldOffset(0x428)]
		public uint buildings_constructing;
		[FieldOffset(0x430)]
		public uint buildings_current;
		[FieldOffset(0x5c0)]
		public uint buildings_lost_mineral_worth;
		[FieldOffset(0x5c8)]
		public uint buildings_lost_vespene_worth;
		[FieldOffset(20)]
		public uint camera_angle_of_attack;
		[FieldOffset(0x10)]
		public uint camera_distance;
		[FieldOffset(0x18)]
		public uint camera_rotation;
		[FieldOffset(8)]
		public uint camera_x;
		[FieldOffset(12)]
		public uint camera_y;
		[FieldOffset(0xa8)]
		public uint credits;
		[FieldOffset(0xa8)]
		public uint credits_spent;
		[FieldOffset(0x4e8)]
		public uint custom_resource_current;
		[FieldOffset(0x568)]
		public uint custom_resource_rate;
		[FieldOffset(0x508)]
		public uint custom_resource_total;
		[FieldOffset(0xfc)]
		public uint damage_multiplier;
		[FieldOffset(0x27)]
		public byte difficulty;
		[FieldOffset(0x400)]
		public uint harvesters_built;
		[FieldOffset(0x3f0)]
		public uint harvesters_current;
		[FieldOffset(0x550)]
		public uint mineral_rate;
		[FieldOffset(0x4d0)]
		public uint minerals_current;
		[FieldOffset(0x4f0)]
		public uint minerals_total;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=50), FieldOffset(0x48)]
		public string name;
		[FieldOffset(0x40)]
		public uint name_length;
		[FieldOffset(0x1d)]
		public byte player_type;
		[FieldOffset(0x98)]
		public uint racePointer;
		[FieldOffset(0xac)]
		public uint research_points;
		[FieldOffset(160)]
		public uint slot_number;
		[FieldOffset(0x588)]
		public uint slow_counting_custom_resource;
		[FieldOffset(0x570)]
		public uint slow_counting_minerals;
		[FieldOffset(0x580)]
		public uint slow_counting_terrazine;
		[FieldOffset(0x578)]
		public uint slow_counting_vespene;
		[FieldOffset(30)]
		public byte status;
		[FieldOffset(0x4a0)]
		public uint supply_cap;
		[FieldOffset(0x4a8)]
		public uint supply_current;
		[FieldOffset(0xa4)]
		public uint supply_limit;
		[FieldOffset(0x1c)]
		public byte team;
		[FieldOffset(0x4e0)]
		public uint terrazine_current;
		[FieldOffset(0x560)]
		public uint terrazine_rate;
		[FieldOffset(0x500)]
		public uint terrazine_total;
		[FieldOffset(60)]
		public uint time_since_resource_changed;
		[FieldOffset(0x440)]
		public uint total_constructing;
		[FieldOffset(0x440)]
		public uint total_constructing_queue_length;
		[FieldOffset(0x2e8)]
		public uint units_betrayed;
		[FieldOffset(0x2d8)]
		public uint units_killed;
		[FieldOffset(680)]
		public uint units_lost;
		[FieldOffset(0x5b8)]
		public uint units_lost_mineral_worth;
		[FieldOffset(0x5d8)]
		public uint units_lost_vespene_worth;
		[FieldOffset(0x4d8)]
		public uint vespene_current;
		[FieldOffset(0x558)]
		public uint vespene_rate;
		[FieldOffset(0x4f8)]
		public uint vespene_total;
	}
}

