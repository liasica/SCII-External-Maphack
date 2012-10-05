namespace Data
{
	using SC2RanksAPI;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;

	[StructLayout(LayoutKind.Sequential)]
	public class Player
	{
		private static Color[] possible_colors = new Color[] { Color.White, Color.FromArgb(179, 20, 30), Color.FromArgb(0, 66, 254), Color.FromArgb(28, 166, 233), Color.FromArgb(84, 0, 129), Color.FromArgb(234, 224, 41), Color.FromArgb(253, 138, 14), Color.FromArgb(22, 128, 0), Color.FromArgb(203, 165, 251), Color.FromArgb(31, 1, 200), Color.FromArgb(82, 84, 148), Color.FromArgb(16, 98, 70), Color.FromArgb(78, 42, 4), Color.FromArgb(150, 254, 145), Color.FromArgb(35, 35, 35), Color.FromArgb(228, 91, 175) };

		private int _number;
		private uint _memoryAddress;
		private string _accountNumber;
		private string _name;
		private int _nameLength;
		private Race _race;

		public Player(int Number)
		{
			_number = Number;

			string AccountNumber = "";
			if (number < 16)
				AccountNumber = (String)GameData.offsets.ReadArrayElementMember(ORNames.AccountNumbers, number, ORNames.id_string);
			if (!AccountNumber.Contains("-S2-"))
				AccountNumber = "(none)";

			_accountNumber = AccountNumber;
			_name = (string)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.name);
			_nameLength = (int)(uint)GameData.offsets.ReadArrayElementMember(ORNames.Players, number, ORNames.name_length);
			_memoryAddress = (uint)GameData.offsets.GetArrayElementAddress(ORNames.Players, number);

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
					if (!(str2 == "Zerg"))
					{
						if (str2 == "Prot")
							_race = Race.Protoss;
						else if (str2 == "Terr")
							_race = Race.Terran;
					}
					else
						_race = Race.Zerg;
				}
			}
			unit_selections = new PlayerSelections();
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
		public PlayerSelections unit_selections;
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
				int Index = colorIndex;
				return possible_colors[Index < 16 && Index >= 0 ? Index : 0];
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
				return _name;
			}
		}
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
		public float cameraPitch;
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
				return _accountNumber;
			}
		}

		public override string ToString()
		{
			return name;
		}
	}
}

