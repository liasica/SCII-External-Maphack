namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit)]
	public struct player_s
	{
		[FieldOffset(0x0)]
		public uint active;
		[FieldOffset(0x8)]
		public fixed32 camera_x;
		[FieldOffset(0xc)]
		public fixed32 camera_y;
		[FieldOffset(0x10)]
		public fixed32 camera_distance;
		[FieldOffset(0x14)]
		public fixed32 camera_angle_of_attack;
		[FieldOffset(0x18)]
		public fixed32 camera_rotation;
		[FieldOffset(0x1c)]
		public byte team;
		[FieldOffset(0x1d)]
		public byte player_type;
		[FieldOffset(0x1e)]
		public byte status;
		[FieldOffset(0x27)]
		public byte difficulty;
		[FieldOffset(0x40)]
		public uint time_since_resource_changed;
		[FieldOffset(0x44)]
		public uint name_length;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50), FieldOffset(0x4c)]
		public string name;
		[FieldOffset(0x9c)]
		public uint racePointer;
		[FieldOffset(0xa4)]
		public uint slot_number;
		[FieldOffset(0xa8)]
		public fixed32 supply_limit;
		[FieldOffset(0xac)] //same offset?!
		public uint credits;
		[FieldOffset(0xac)] //same offset?!
		public uint credits_spent;
		[FieldOffset(0xb0)]
		public uint research_points;
		[FieldOffset(0xfc)]
		public fixed32 attack_multiplier;
		[FieldOffset(0x100)]
		public fixed32 defense_multiplier;
		//[FieldOffset(0x380)] //discarding this because I don't know which value is correct.
		//public uint actions_total;
		//[FieldOffset(0x3a8)] //discarding this because I don't know which value is correct.
		//public uint apm_current;
		//[FieldOffset(0x3e0)] //discarding this because I don't know which value is correct.
		//public uint units_lost;
		//[FieldOffset(0x410)] //discarding this because I don't know which value is correct.
		//public uint units_killed;
		//[FieldOffset(0x420)] //discarding this because I don't know which value is correct.
		//public uint units_betrayed;
		[FieldOffset(0x528)]
		public uint harvesters_current;
		[FieldOffset(0x538)]
		public uint harvesters_built;
		[FieldOffset(0x558)]
		public uint building_queue_length;
		[FieldOffset(0x560)]
		public uint buildings_constructing;
		[FieldOffset(0x568)]
		public uint buildings_current;
		[FieldOffset(0x578)] //same offset?!
		public uint total_constructing;
		[FieldOffset(0x578)] //same offset?!
		public uint total_constructing_queue_length;
		[FieldOffset(0x580)]
		public uint army_size;
		[FieldOffset(0x5d8)]
		public fixed32 supply_cap;
		[FieldOffset(0x5f0)]
		public fixed32 supply_current;
		[FieldOffset(0x628)]
		public uint minerals_current;
		[FieldOffset(0x630)]
		public uint vespene_current;
		[FieldOffset(0x638)]
		public uint terrazine_current;
		[FieldOffset(0x640)]
		public uint custom_resource_current;
		[FieldOffset(0x648)]
		public uint minerals_total;
		[FieldOffset(0x650)]
		public uint vespene_total;
		[FieldOffset(0x658)]
		public uint terrazine_total;
		[FieldOffset(0x660)]
		public uint custom_resource_total;
		[FieldOffset(0x6a8)]
		public uint mineral_rate;
		[FieldOffset(0x6b0)]
		public uint vespene_rate;
		[FieldOffset(0x6b8)]
		public uint terrazine_rate;
		[FieldOffset(0x6c0)]
		public uint custom_resource_rate;
		[FieldOffset(0x6c8)]
		public uint slow_counting_minerals;
		[FieldOffset(0x6d0)]
		public uint slow_counting_vespene;
		[FieldOffset(0x6d8)]
		public uint slow_counting_terrazine;
		[FieldOffset(0x6e0)]
		public uint slow_counting_custom_resource;
		[FieldOffset(0x710)] //why are buildings between these?
		public uint units_lost_mineral_worth;
		[FieldOffset(0x718)]
		public uint buildings_lost_mineral_worth;
		[FieldOffset(0x720)]
		public uint buildings_lost_vespene_worth;
		[FieldOffset(0x730)] //why are buildings between these?
		public uint units_lost_vespene_worth;
	}
}