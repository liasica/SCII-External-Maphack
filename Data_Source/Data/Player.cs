namespace Data
{
	using SC2RanksAPI;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;

	[Flags]
	public enum StateFlags : uint
	{
		ShowScore = 0x40,
		NoXPGain = 0x80,
		ShowWorldTip = 0x100,
		FidgetingEnabled = 0x200,
		DisplayInLeaderPanel = 0x400,
		DisplayInViewMenu = 0x800,
		ChargesPaused = 0x1000,
		CooldownsPaused = 0x2000,
		DisplayGameResult = 0x4000,
		FoodIgnored = 0x8000,
		MineralCostIgnored = 0x10000,
		VespeneCostIgnored = 0x20000,
		TerrazineCostIgnored = 0x40000,
		CustomCostIgnored = 0x80000
	}

	[StructLayout(LayoutKind.Sequential)]
	public class Player
	{
		private static Color[] possible_colors = new Color[] { Color.White, Color.FromArgb(179, 20, 30), Color.FromArgb(0, 66, 254), Color.FromArgb(28, 166, 233), Color.FromArgb(84, 0, 129), Color.FromArgb(234, 224, 41), Color.FromArgb(253, 138, 14), Color.FromArgb(22, 128, 0), Color.FromArgb(203, 165, 251), Color.FromArgb(31, 1, 200), Color.FromArgb(82, 84, 148), Color.FromArgb(16, 98, 70), Color.FromArgb(78, 42, 4), Color.FromArgb(150, 254, 145), Color.FromArgb(35, 35, 35), Color.FromArgb(228, 91, 175) };
		public static Color SelectedSelfColor = Color.FromArgb(0, 255, 0);
		public static Color SelfColor = Color.FromArgb(0, 187, 0);
		public static Color AllyColor = Color.Yellow;
		public static Color NeutralColor = Color.White;
		public static Color EnemyColor = Color.Red;

		public static void Reset()
		{
			_LocalPlayer = null;
			_NeutralPlayer = null;
		}

		private static Player _LocalPlayer = null;
		public static Player LocalPlayer
		{
			get
			{
				int Local = GameData.LocalPlayerNumber;
				if (_LocalPlayer == null || _LocalPlayer.number != Local)
					_LocalPlayer = new Player(Local);
				return _LocalPlayer;
			}
		}
		private static Player _NeutralPlayer = null;
		public static Player NeutralPlayer
		{
			get
			{
				if (_NeutralPlayer == null)
					_NeutralPlayer = new Player(0);
				return _NeutralPlayer;
			}
		}


		public static bool UseTeamColors = false;

		private int _number;
		private uint _memoryAddress;
		private string _Handle;
		private int _HandleLength;
		private string _NameWithTag;
		private string _name;
		private int _nameLength;
		private string _ClanTag;
		private int _ClanTagLength;
		private Race _race;

		public Player(int Number)
		{
			_number = Number;

			_memoryAddress = (uint)GameData.offsets.GetArrayElementAddress(ORNames.Players, number);
			
			_nameLength = (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.name_length);
			_name = ((string)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.name)).Substring(0, _nameLength);
			_NameWithTag = _name;

			_ClanTagLength = (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.ClanTagLength);
			_ClanTag = ((string)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.ClanTag)).Substring(0, _ClanTagLength);
			if (_ClanTagLength > 0)
				_NameWithTag = "[" + _ClanTag + "] " + _name;

			_HandleLength = (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.HandleLength);
			_Handle = ((string)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.Handle)).Substring(0, _HandleLength);
			

			uint num = (uint)GameData.mem.ReadMemory((uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.racePointer) + 4, typeof(uint));
			byte[] buffer;
			GameData.mem.ReadMemory((uint)num, 4, out buffer);
			if (buffer == null || buffer.Length <= 0 || buffer[0] == 0)
				_race = Race.Neutral;
			else
			{
				string str2 =  Encoding.UTF8.GetString(buffer).Trim();
				if (str2 != null)
				{
					if (str2 == "Zerg")
						_race = Race.Zerg;
					else if (str2 == "Prot")
						_race = Race.Protoss;
					else if (str2 == "Terr")
						_race = Race.Terran;
					else
						_race = Race.Neutral;
				}
			}
		}

		public List<Unit> units
		{
			get
			{
				return GameData.GetPlayerUnits(number);
			}
		}
		public uint memoryAddress
		{
			get
			{
				return _memoryAddress;
			}
		}
		public int number
		{
			get
			{
				return _number;
			}
		}
		public PlayerStatus playerStatus
		{
			get
			{
				return (PlayerStatus)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.active);
			}
		}
		public StateFlags State
		{
			get
			{
				return (StateFlags)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.StateFlags);
			}
		}
		public VictoryStatus victoryStatus
		{
			get
			{
				return (VictoryStatus)(byte)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.status);
			}
		}
		public PlayerDifficulty difficulty
		{
			get
			{
				return (PlayerDifficulty)(byte)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.difficulty);
			}
		}
		public PlayerSelections unit_selections
		{
			get
			{
				return new PlayerSelections(number);
			}
		}
		public int nameLength
		{
			get
			{
				return _nameLength;
			}
		}
		public int colorIndex
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.color_index);
			}
		}
		public int workersCurrent
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.harvesters_current);
			}
		}
		public int workersBuilt
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.harvesters_built);
			}
		}
		public int buildingQueueLength
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.building_queue_length);
			}
		}
		public int buildingsConstructing
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.buildings_constructing);
			}
		}
		public int buildingsCurrent
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.buildings_current);
			}
		}
		public int armySize
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.army_size);
			}
		}
		public Color drawingColor
		{
			get
			{
				if (!UseTeamColors)
				{
					int Index = colorIndex;
					return possible_colors[Index < 16 && Index >= 0 ? Index : 0];
				}
				else
				{
					if (number == 0)
						return NeutralColor;
					else if (number == LocalPlayer.number)
						return SelfColor;
					else if (team == LocalPlayer.team)
						return AllyColor;
					else
						return EnemyColor;
				}
			}
		}
		public PlayerColor playerColor
		{
			get
			{
				return (PlayerColor)colorIndex;
			}
		}
		public string name
		{
			get
			{
				return _NameWithTag;
			}
		}
		public string NameWithoutClanTag
		{
			get
			{
				return _name;
			}
		}
		public string ClanTag
		{ get { return _ClanTag; } }
		public int ClanTagLength
		{ get { return _ClanTagLength; } }

		public Race race
		{
			get
			{
				return _race;
			}
		}
		public int minerals
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.minerals_current);
			}
		}
		public int gas
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.vespene_current);
			}
		}
		public int terrazine
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.terrazine_current);
			}
		}
		public int custom
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.custom_resource_current);
			}
		}
		public int mineralTotal
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.minerals_total);
			}
		}
		public int gasTotal
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.vespene_total);
			}
		}
		public int terrazineTotal
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.terrazine_total);
			}
		}
		public int customTotal
		{
			get
			{
				return (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.custom_resource_total);
			}
		}
		public float supply
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.supply_current);
			}
		}
		public float supplyCap
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.supply_cap);
			}
		}
		public float supplyLimit
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.supply_limit);
			}
		}
		public int unitCount;
		public int team
		{
			get
			{
				return (byte)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.team);
			}
		}
		public PlayerType playerType
		{
			get
			{
				return (PlayerType)(byte)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.player_type);
			}
		}
		public float cameraX
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.camera_x);
			}
		}
		public float cameraY
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.camera_y);
			}
		}
		public float cameraDistance
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.camera_distance);
			}
		}
		public float cameraPitch
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.camera_angle_of_attack);
			}
		}
		public float cameraRotation
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.camera_rotation);
			}
		}
		public string accountNumber
		{
			get
			{
				return _Handle;
			}
		}

		public override string ToString()
		{
			return name;
		}
	}
}

