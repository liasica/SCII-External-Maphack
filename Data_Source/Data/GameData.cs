namespace Data
{
	using PatternScanner;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;
	using Utilities.MemoryHandling;
	using Utilities.TextProcessing;
	using Utilities.WinControl;

	public static class GameData
	{
		private static OffsetReader _offsets;
		public static OffsetReader offsets
		{
			get
			{
				if (_offsets == null)
				{
					_offsets = new OffsetReader("Offsets.xml");
					if (!_offsets.CheckVersion())
					{
						_offsets.UpdateAddresses();
						_offsets = new OffsetReader("Offsets.xml");
					}
				}
				return _offsets;
			}
		}
		public static MapData mapDat;
		private static ReadWriteMemory _mem;
		private static PatternScan _ps;
		private static string _SC2FilePath;
		private static IntPtr _SC2Handle;
		private static string _SC2Language;
		private static Process _SC2Process;
		private static string _SC2Version;
		private const uint END_OF_UNITS = 0xffff;
		private const uint NUM_OF_CONTROL_GROUPS = 10;
		private const uint NUM_PLAYERS = 0x10;
		//private static Color[] possible_colors = new Color[] { Color.White, Color.Red, Color.Blue, Color.FromArgb(0x1c, 0xa7, 0xea), Color.Purple, Color.Yellow, Color.Orange, Color.Green, Color.LightPink, Color.FromArgb(0x1f, 1, 0xc9), Color.LightGray, Color.DarkGreen, Color.Brown, Color.LightGreen, Color.DarkGray, Color.Pink };
		private static Color[] possible_colors = new Color[] { Color.White, Color.FromArgb(179, 20, 30), Color.FromArgb(0, 66, 254), Color.FromArgb(28, 166, 233), Color.FromArgb(84, 0, 129), Color.FromArgb(234, 224, 41), Color.FromArgb(253, 138, 14), Color.FromArgb(22, 128, 0), Color.FromArgb(203, 165, 251), Color.FromArgb(31, 1, 200), Color.FromArgb(82, 84, 148), Color.FromArgb(16, 98, 70), Color.FromArgb(78, 42, 4), Color.FromArgb(150, 254, 145), Color.FromArgb(35, 35, 35), Color.FromArgb(228, 91, 175) };
		public static Color[] player_colors = (Color[])possible_colors.Clone();

		public static List<Abil> getAbilitiesData()
		{
			List<Abil> list = new List<Abil>();
			int num = 0;
			num = (int) mem.ReadMemory(Offsets.AbilityCount, num.GetType());
			for (int i = 0; i < num; i++)
			{
				Abil ability = new Abil(StructStarts.Abilities + ((uint) (120L * i)), mem).Ability;
				if (ability.ClassName != Abil.AbilClass.Unknown)
				{
					list.Add(ability);
				}
			}
			return list;
		}

		public static Rectangle GetMinimapCoords()
		{	
			Rectangle ClientCoords = ps.MinimapCoords();

			Point WindowLocation = new Point(0, 0);
			if (ClientCoords != Rectangle.Empty && Utilities.MemoryHandling.Imports.ClientToScreen(_SC2Process.MainWindowHandle, ref WindowLocation))
			{
				if(WindowLocation.X >-5000 || WindowLocation.Y > -5000)
					ClientCoords.Offset(WindowLocation);
				return ClientCoords;
			}
			return Rectangle.Empty;
		}

		public static Map getMapData()
		{
			uint num;
			Map map = new Map();
			MapInfo info = new MapInfo {
				secondsElapsed = SecondsElapsed
			};
			mem.ReadMemory(Offsets.OFFSET_MAPHACK_WIDTH, 4, out info.width);
			mem.ReadMemory(Offsets.OFFSET_MAPHACK_HEIGHT, 4, out info.height);
			info.width = (int) Math.Round((double) (((double) info.width) / 4096.0));
			info.height = (int) Math.Round((double) (((double) info.height) / 4096.0));
			mem.ReadMemory(Offsets.OFFSET_MAPHACK_EDGE_LEFT, 4, out info.edgeLeft);
			mem.ReadMemory(Offsets.OFFSET_MAPHACK_EDGE_TOP, 4, out info.edgeTop);
			mem.ReadMemory(Offsets.OFFSET_MAPHACK_EDGE_RIGHT, 4, out info.edgeRight);
			mem.ReadMemory(Offsets.OFFSET_MAPHACK_EDGE_BOTTOM, 4, out info.edgeBottom);
			info.edgeLeft = (int) Math.Round((double) (((double) info.edgeLeft) / 4096.0));
			info.edgeTop = (int) Math.Round((double) (((double) info.edgeTop) / 4096.0));
			info.edgeRight = (int) Math.Round((double) (((double) info.edgeRight) / 4096.0));
			info.edgeBottom = (int) Math.Round((double) (((double) info.edgeBottom) / 4096.0));
			info.playableX = info.edgeRight - info.edgeLeft;
			info.playableY = info.edgeTop - info.edgeBottom;
			mem.ReadMemory(Pointers.MapInformation, 4, out num);
			if (num == 0)
			{
				map.mapInfo = new MapInfo();
				return map;
			}
			map_information_s _s = new map_information_s();
			_s = (map_information_s) mem.ReadMemory(num, typeof(map_information_s));
			info.filePath = _s.FilePath;
			info.filePath2 = _s.FilePath2;
			info.name = _s.Name;
			info.author = _s.Author;
			info.descriptionBasic = _s.DescriptionBasic;
			info.desciptionExtended = _s.DescriptionExtended;
			map.mapInfo = info;
			return map;
		}

		public static void GetMapInformation(Utilities.WinControl.RECT minimapRect, ref Utilities.WinControl.RECT mapRect, ref float ratioWidth, ref float ratioHeight)
		{
			float MapWidth = ((float) MapPlayableWidth);
			float MapHeight = ((float) MapPlayableHeight);
			float MapRatio = RatioPlayable;
			if (MapWidth == 0 || MapHeight == 0 || MapRatio == float.NaN)
			{
				MapWidth = 1;
				MapHeight = 1;
				MapRatio = 1;
			}

			int width = minimapRect.Width;
			int height = minimapRect.Height;
			float aspectRatio = ((float) height) / ((float) width);
			float num3 = width * MapRatio;
			float num4 = ((float) height) / MapRatio;
			int num5 = 0;
			int num6 = 0;
			if (num3 <= height)
			{
				num4 = width;
				num5 = (int) ((height - num3) / 2f);
			}
			else
			{
				num3 = height;
				num6 = (width - ((int) num4)) / 2;
			}
			
			ratioWidth = num4 / MapWidth;
			ratioHeight = num3 / MapHeight;
			mapRect = new Utilities.WinControl.RECT(minimapRect.Left + num6, minimapRect.Top + num5, (minimapRect.Left + num6) + ((int) num4), (minimapRect.Top + num5) + ((int) num3));
		}

		public static Player GetPlayer(int numplayer)
		{
			byte[] buffer;
			Player p = new Player {
				memoryAddress = (uint)offsets.GetArrayElementAddress("Players", numplayer)
			};

			string AccountNumber = "";
			if (numplayer < 16)
			{
				AccountNumber = (String)offsets.ReadArrayElementMember("AccountNumbers", numplayer, "id_string");
			}

			if (!AccountNumber.Contains("-S2-"))
				AccountNumber = "(none)";

			p.accountNumber = AccountNumber;
			p.rankIconTooltip = "Unranked";

			p.number = (uint)numplayer;
			p.slotNumber = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "slot_number");
			p.playerStatus = (PlayerStatus)(uint)offsets.ReadArrayElementMember("Players", numplayer, "active");
			p.victoryStatus = (VictoryStatus)(byte)offsets.ReadArrayElementMember("Players", numplayer, "status");
			p.playerType = (PlayerType)(byte)offsets.ReadArrayElementMember("Players", numplayer, "player_type");
			p.team = (byte)offsets.ReadArrayElementMember("Players", numplayer, "team");
			p.name = (string)offsets.ReadArrayElementMember("Players", numplayer, "name");
			p.nameLength = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "name_length");
			p.playerColor = (PlayerColor)p.slotNumber;
			p.drawingColor = possible_colors[p.slotNumber < 16 ? p.slotNumber : 0];
			p.minerals = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "minerals_current");
			p.gas = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "vespene_current");
			p.terrazine = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "terrazine_current");
			p.custom = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "custom_resource_current");
			p.mineralRate = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "mineral_rate");
			p.gasRate = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "vespene_rate");
			p.mineralTotal = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "minerals_total");
			p.gasTotal = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "vespene_total");
			p.difficulty = (PlayerDifficulty)(byte)offsets.ReadArrayElementMember("Players", numplayer, "difficulty");
			//p.actionsTotal = (int) offsets.ReadArrayElementMember("Players", numplayer, "actions_total");
			//p.apmCurrent = (int) offsets.ReadArrayElementMember("Players", numplayer, "apm_current");
			p.armySize = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "army_size");
			p.unitsLostMineralWorth = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "units_lost_mineral_worth");
			p.unitsLostVespeneWorth = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "units_lost_vespene_worth");
			p.buildingsConstructing = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "buildings_constructing");
			p.cameraDistance = (int)(fixed32)offsets.ReadArrayElementMember("Players", numplayer, "camera_distance");
			p.cameraX = (fixed32)offsets.ReadArrayElementMember("Players", numplayer, "camera_x");
			p.cameraY = (fixed32)offsets.ReadArrayElementMember("Players", numplayer, "camera_y");
			p.cameraRotation = (fixed32)offsets.ReadArrayElementMember("Players", numplayer, "camera_rotation");
			p.workersBuilt = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "harvesters_built");
			p.workersCurrent = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "harvesters_current");
			//p.unitsKilled = (int) offsets.ReadArrayElementMember("Players", numplayer, "units_killed");
			//p.unitsLost = (int) offsets.ReadArrayElementMember("Players", numplayer, "units_lost");
			p.buildingQueueLength = (int)(uint)offsets.ReadArrayElementMember("Players", numplayer, "building_queue_length");
			p.supply = (fixed32)offsets.ReadArrayElementMember("Players", numplayer, "supply_current");
			p.supplyCap = (fixed32)offsets.ReadArrayElementMember("Players", numplayer, "supply_cap");
			p.supplyLimit = (fixed32)offsets.ReadArrayElementMember("Players", numplayer, "supply_limit");
			uint num = (uint) mem.ReadMemory((uint)offsets.ReadArrayElementMember("Players", numplayer, "racePointer") + 4, typeof(uint));
			mem.ReadMemory((uint) num, 4, out buffer);
			if (buffer[0] == 0)
			{
				p.race = Race.Neutral;
			}
			else
			{
				string str2 = TP.HexAsciiConvert(BitConverter.ToString(buffer).Replace("-", ""));
				if (str2 != null)
				{
					if (!(str2 == "Zerg"))
					{
						if (str2 == "Prot")
						{
							p.race = Race.Protoss;
						}
						else if (str2 == "Terr")
						{
							p.race = Race.Terran;
						}
					}
					else
					{
						p.race = Race.Zerg;
					}
				}
			}
			//p.unit_selections = GetPlayerSelections(p);
			//p.units = new List<Unit>();
			return p;
		}

		public static List<Player> getPlayersData()
		{
			List<Player> list = new List<Player>();
			for (int i = 0; i < 0x10; i++)
			{
				Player item = GetPlayer(i);
				if (!(item.name == ""))
				{
					player_colors[i] = item.drawingColor;
					list.Add(item);
				}
			}
			return list;
		}

		public static PlayerSelections GetPlayerSelections()
		{
			uint pNumber = (byte) mem.ReadMemory(Offsets.LocalPlayerNumber, typeof(byte));
			return GetPlayerSelections(pNumber);
		}

		public static PlayerSelections GetPlayerSelections(Player p)
		{
			if (p.name == "")
			{
				return new PlayerSelections();
			}
			return GetPlayerSelections(p.number);
		}

		public static PlayerSelections GetPlayerSelections(uint pNumber)
		{
			//BROKEN! DO NOT USE!

			PlayerSelections selections = new PlayerSelections {
				currentSelection = new ControlGroup()
			};
			selections.currentSelection.selected_unit_ids = new List<int>();
			selections.control_groups = new ControlGroup[10];
			for (uint i = 0; i < 10; i++)
			{
				selections.control_groups[i].selected_unit_ids = new List<int>();
				selections.control_groups[i].groupNumber = (int) i;
			}
			uint num2 = pNumber;
			if (num2 > 0)
			{
				uint num3;
				byte[] buffer;
				selections.currentSelection.groupNumber = 10;
				mem.ReadMemory((StructStarts.PlayerSelections + (num2 * 0xcf8)) + control_group_s.selected_types, 1, out selections.currentSelection.unitTypesSelected);
				mem.ReadMemory((StructStarts.PlayerSelections + (num2 * 0xcf8)) + control_group_s.sub_group, 2, out selections.currentSelection.unitSubGroupSelected);
				mem.ReadMemory((StructStarts.PlayerSelections + (num2 * 0xcf8)) + control_group_s.total_selected, 1, out num3);
				selections.currentSelection.totalSelected = (int) num3;
				mem.ReadMemory((uint) ((StructStarts.PlayerSelections + (num2 * 0xcf8)) + control_group_s.selected_units), (int) (num3 * 4), out buffer);
				for (int j = 0; j < (num3 * 4); j += 4)
				{
					byte[] dst = new byte[2];
					Buffer.BlockCopy(buffer, j, dst, 0, 2);
					selections.currentSelection.selected_unit_ids.Add(BitConverter.ToInt16(dst, 0));
				}
				uint num5 = (StructStarts.PlayerSelections + 0xcf80) + (0x81b0 * num2);
				for (uint k = 0; k < 10; k++)
				{
					mem.ReadMemory((num5 + (0xcf8 * k)) + control_group_s.selected_types, 1, out selections.control_groups[k].unitTypesSelected);
					mem.ReadMemory((num5 + (0xcf8 * k)) + control_group_s.sub_group, 2, out selections.control_groups[k].unitSubGroupSelected);
					mem.ReadMemory((num5 + (0xcf8 * k)) + control_group_s.total_selected, 1, out num3);
					selections.control_groups[k].totalSelected = (int) num3;
					mem.ReadMemory((uint) ((num5 + (0xcf8 * k)) + control_group_s.selected_units), (int) (num3 * 4), out buffer);
					for (int m = 0; m < (num3 * 4); m += 4)
					{
						byte[] buffer3 = new byte[2];
						Buffer.BlockCopy(buffer, m, buffer3, 0, 2);
						selections.control_groups[k].selected_unit_ids.Add(BitConverter.ToInt16(buffer3, 0));
					}
				}
			}
			return selections;
		}

		public static List<Unit> getUnitData()
		{
			List<Unit> list = new List<Unit>();
			int MaxUnits = offsets.GetArrayCount("Units");
			for (int i = 0; i <MaxUnits; i++)
			{
				Unit item = ParseUnit((int)i);
				if (item.modelPtr == 0)
				{
					//GetUnitModels();
					return list;
				}
				
				list.Add(item);
			}
			return list;
		}

		public static Map getUnits(Map map, uint playerNumber)
		{
			if (map.units != null)
			{
				map.units.Clear();
				foreach (Unit unit in map.units)
				{
					if (unit.playerNumber == playerNumber)
					{
						map.units.Add(unit);
					}
				}
			}
			return map;
		}

		public static string[] GetUnitModels()
		{
			string[] ReturnValue = new string[0x581];
			LinkedListEntry UnitsEntry = (LinkedListEntry)mem.ReadMemory(0x06A32CAC, typeof(LinkedListEntry));
			uint[] ModelPtrArray = new uint[0x581];
			//string ListType = new string(mem.ReadMemory(UnitsEntry.NameStartAddress, typeof(byte), );

			for (int i = 0; i < ModelPtrArray.Length; i++)
			{
				ModelPtrArray[i] = (uint) mem.ReadMemory(UnitsEntry.pModelArray + (uint) i * 4, typeof(uint));
				if (ModelPtrArray[i] != 0)
				{
					string NameAsText = null;
					uint pNameDataAddress = (uint)mem.ReadMemory((uint)((ModelPtrArray[i]) + 0x6c), typeof(uint));
					if (pNameDataAddress != 0)
					{
						uint NameDataAddress = (uint)mem.ReadMemory(pNameDataAddress, typeof(uint));
						uint NameLength = (uint)mem.ReadMemory(NameDataAddress, typeof(uint));
						if (NameDataAddress != 0 && NameLength > 10)
						{
							byte[] NameAsBytes = new byte[NameLength];
							mem.ReadMemory((IntPtr)NameDataAddress + 0x24, (int)NameLength, out NameAsBytes);
							NameAsText = System.Text.Encoding.UTF8.GetString(NameAsBytes).Remove(0, 10);
						}
					}
					if (NameAsText == null)
						NameAsText = string.Empty;

					ReturnValue[i] = NameAsText;
				}
			}
			return ReturnValue;
		}

		public static Unit ParseUnit(int Index)
		{
			Unit unit = new Unit();
			unit.modelPtr = (uint)offsets.ReadArrayElementMember("Units", Index, "unit_model");

			if (unit.modelPtr == 0)
				return unit;

			unit_model_s ums = (unit_model_s)mem.ReadMemory((uint)(unit.modelPtr << 5), typeof(unit_model_s));

			string NameAsText = null;
			string UINameAsText = null;
			if ((uint)offsets.ReadStructMember("UnitModel", "pName_address", (int)(unit.modelPtr << 5)) != 0)
			{
				uint NameDataAddress = (uint)mem.ReadMemory((uint)offsets.ReadStructMember("UnitModel", "pName_address", (int)(unit.modelPtr << 5)), typeof(uint));
				uint NameLength = (uint)mem.ReadMemory(NameDataAddress, typeof(uint));
				if (NameDataAddress != 0 && NameLength > 10)
				{
					byte[] NameAsBytes = new byte[NameLength];
					mem.ReadMemory((IntPtr)NameDataAddress + 0x24, (int)NameLength, out NameAsBytes);
					NameAsText = System.Text.Encoding.UTF8.GetString(NameAsBytes).Remove(0, 10);

					uint pUINameAddress = (uint)mem.ReadMemory(NameDataAddress + 0x1c, typeof(uint));
					if (pUINameAddress != 0)
					{
						uint UINameLength = (uint)mem.ReadMemory(pUINameAddress + 0x8, typeof(uint));
						uint UINameAddress = pUINameAddress + 0x10;

						byte fail = 0;
						if (((fail = (byte)mem.ReadMemory(pUINameAddress + 12, typeof(byte))) & 4) != 0) //sometimes the string is right in this struct, other times it's a pointer.
							UINameAddress = (uint)mem.ReadMemory(pUINameAddress + 16, typeof(uint));

						if (UINameAddress != 0 && UINameLength > 0)
						{
							byte[] UINameAsBytes = new byte[UINameLength];
							mem.ReadMemory((IntPtr)UINameAddress, (int)UINameLength, out UINameAsBytes);
							UINameAsText = System.Text.Encoding.UTF8.GetString(UINameAsBytes);
						}
					}

				}
			}
			if (NameAsText == null)
				NameAsText = string.Empty;
			if (UINameAsText == null)
				UINameAsText = string.Empty;

			unit.name = UINameAsText;
			unit.textID = NameAsText;
			unit.minimapRadius = (fixed32)offsets.ReadStructMember("UnitModel", "minimap_radius", (int)(unit.modelPtr << 5));

			unit.timeScale = (fixed32)offsets.ReadArrayElementMember("Units", Index, "time_scale");

			unit.ID = (ushort)offsets.ReadArrayElementMember("Units", Index, "token");
			unit.playerNumber = (byte)offsets.ReadArrayElementMember("Units", Index, "player_owner");
			unit.unitType = (UnitType)(ushort)offsets.ReadStructMember("UnitModel", "unit_type", (int)(unit.modelPtr << 5));
			unit.state = (UnitStateOld)((byte)offsets.ReadArrayElementMember("Units", Index, "state"));

			unit.isImmobile = (byte)offsets.ReadArrayElementMember("Units", Index, "isImmobile") != 0;
			unit.targetFilterFlags = (TargetFilter)offsets.ReadArrayElementMember("Units", Index, "targetFilter_flags");
			unit.isAlive = (unit.targetFilterFlags & TargetFilter.Dead) == 0;
			unit.cloaked = (unit.targetFilterFlags & TargetFilter.Cloaked) != 0;
			unit.detector = (unit.targetFilterFlags & TargetFilter.Detector) != 0;

			unit.moveState = (UnitMoveState)((byte)offsets.ReadArrayElementMember("Units", Index, "move_state"));
			unit.subMoveState = (UnitSubMoveState)((byte)offsets.ReadArrayElementMember("Units", Index, "sub_move_state"));
			unit.lastOrder = (UnitLastOrder)((uint)offsets.ReadArrayElementMember("Units", Index, "last_order"));
			unit.deathType = (DeathType)((uint)offsets.ReadArrayElementMember("Units", Index, "death_type"));

			//unit.energyRegenDelay = offsets.ReadStructMember("UnitModel", "energy_regen_delay", (int)(unit.modelPtr << 5)) / 65536f;
			//unit.energyRegenRate = (offsets.ReadStructMember("UnitModel", "energy_regen_rate", (int)(unit.modelPtr << 5)) + (fixed32)offsets.ReadArrayElementMember("Units", Index, "energy_regen_bonus")) / 256f;
			//unit.energyDamage = offsets.ReadStructMember("UnitModel", "max_energy", (int)(unit.modelPtr << 5)) * (fixed32)offsets.ReadArrayElementMember("Units", Index, "energy_multiplier") + (fixed32)offsets.ReadArrayElementMember("Units", Index, "bonus_max_energy") - (fixed32)offsets.ReadArrayElementMember("Units", Index, "energy");
			unit.currentEnergy = (fixed32)offsets.ReadArrayElementMember("Units", Index, "energy");
			//unit.maxEnergy = offsets.ReadStructMember("UnitModel", "m", (int)(unit.modelPtr << 5))ax_energy * (fixed32)offsets.ReadArrayElementMember("Units", Index, "energy_multiplier") + (fixed32)offsets.ReadArrayElementMember("Units", Index, "bonus_max_energy");

			//unit.shieldRegenDelay = offsets.ReadStructMember("UnitModel", "shield_regen_delay", (int)(unit.modelPtr << 5)) / 65536f;
			//unit.shieldRegenRate = (offsets.ReadStructMember("UnitModel", "shield_regen_rate", (int)(unit.modelPtr << 5)) + (fixed32)offsets.ReadArrayElementMember("Units", Index, "shield_regen_bonus")) / 256f;
			unit.shieldDamage = (fixed32)offsets.ReadArrayElementMember("Units", Index, "shield_damage");
			//unit.currentShield = offsets.ReadStructMember("UnitModel", "max_shield", (int)(unit.modelPtr << 5)) * (fixed32)offsets.ReadArrayElementMember("Units", Index, "shields_multiplier") + (fixed32)offsets.ReadArrayElementMember("Units", Index, "bonus_max_shields") - (fixed32)offsets.ReadArrayElementMember("Units", Index, "shield_damage");
			//unit.maxShield = offsets.ReadStructMember("UnitModel", "max_shield", (int)(unit.modelPtr << 5)) * (fixed32)offsets.ReadArrayElementMember("Units", Index, "shields_multiplier") + (fixed32)offsets.ReadArrayElementMember("Units", Index, "bonus_max_shields");

			//unit.healthRegenDelay = offsets.ReadStructMember("UnitModel", "health_regen_delay", (int)(unit.modelPtr << 5)) / 65536f;
			//unit.healthRegenRate = (offsets.ReadStructMember("UnitModel", "health_regen_rate", (int)(unit.modelPtr << 5)) + (fixed32)offsets.ReadArrayElementMember("Units", Index, "energy_regen_bonus")) / 256f;
			unit.healthDamage = (fixed32)offsets.ReadArrayElementMember("Units", Index, "health_damage");
			//unit.currentHealth = offsets.ReadStructMember("UnitModel", "max_health", (int)(unit.modelPtr << 5)) * (fixed32)offsets.ReadArrayElementMember("Units", Index, "health_multiplier") + (fixed32)offsets.ReadArrayElementMember("Units", Index, "bonus_max_health") - (fixed32)offsets.ReadArrayElementMember("Units", Index, "health_damage");
			//unit.maxHealth = offsets.ReadStructMember("UnitModel", "max_health", (int)(unit.modelPtr << 5)) * (fixed32)offsets.ReadArrayElementMember("Units", Index, "health_multiplier") + (fixed32)offsets.ReadArrayElementMember("Units", Index, "bonus_max_health");

			unit.locationX = (fixed32)offsets.ReadArrayElementMember("Units", Index, "position_x");
			unit.locationY = (fixed32)offsets.ReadArrayElementMember("Units", Index, "position_y");
			unit.locationZ = (fixed32)offsets.ReadArrayElementMember("Units", Index, "position_z");

			unit.destinationX = (fixed32)offsets.ReadArrayElementMember("Units", Index, "destination_x");
			unit.destinationY = (fixed32)offsets.ReadArrayElementMember("Units", Index, "destination_y");
			unit.destinationZ = (fixed32)offsets.ReadArrayElementMember("Units", Index, "destination_z");

			unit.destination2X = (fixed32)offsets.ReadArrayElementMember("Units", Index, "destination2_x");
			unit.destination2Y = (fixed32)offsets.ReadArrayElementMember("Units", Index, "destination2_y");

			unit.kills = (ushort)offsets.ReadArrayElementMember("Units", Index, "kills");

				/*rotation = (int)(offsets.ReadArrayElementMember("Units", Index, "rotation") * 0.010987669527379678),
				rotationX = (int)offsets.ReadArrayElementMember("Units", Index, "rotation_x"),
				rotationY = (int)offsets.ReadArrayElementMember("Units", Index, "rotation_y"),*/
			unit.rotation = 180 - (fixed32)offsets.ReadArrayElementMember("Units", Index, "rotation") * 45;
			unit.rotationX = 180 - (fixed32)offsets.ReadArrayElementMember("Units", Index, "rotation_x") * 45;
			unit.rotationY = 180 - (fixed32)offsets.ReadArrayElementMember("Units", Index, "rotation_y") * 45;

			unit.moveSpeed = (int)((uint)offsets.ReadArrayElementMember("Units", Index, "move_speed"));
			unit.memoryLocation = (uint)offsets.GetArrayElementAddress("Units", Index);
			unit.commandQueuePointer = (uint)offsets.ReadArrayElementMember("Units", Index, "commandQueue_pointer");

			return unit;
		}

		public static string removeBlanksFromByteString(byte[] byteString)
		{
			int count = 0;
			byte[] buffer = byteString;
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == 0)
				{
					return Encoding.ASCII.GetString(byteString, 0, count);
				}
				count++;
			}
			return "FAIL! 00248";
		}

		public static uint localCameraX
		{
			get
			{
				uint num;
				mem.ReadMemory(StructStarts.CameraInfo, 4, out num);
				return (uint) Math.Round((double) (((double) num) / 4096.0));
			}
		}

		public static uint localCameraY
		{
			get
			{
				uint num;
				mem.ReadMemory(StructStarts.CameraInfo + 4, 4, out num);
				return (uint) Math.Round((double) (((double) num) / 4096.0));
			}
		}

		public static int MapEdgeBottom2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember("MapInfo", "bottom_precise") + 0.5);
			}
		}

		public static int MapEdgeLeft2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember("MapInfo", "left_precise") + 0.5);
			}
		}

		public static int MapEdgeRight2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember("MapInfo", "right_precise") + 0.5);
			}
		}

		public static int MapEdgeTop2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember("MapInfo", "top_precise") + 0.5);
			}
		}

		public static int MapEdgeBottom
		{
			get
			{
				return (int)offsets.ReadStructMember("MapInfo", "bottom");
			}
		}

		public static int MapEdgeLeft
		{
			get
			{
				return (int)offsets.ReadStructMember("MapInfo", "left");
			}
		}

		public static int MapEdgeRight
		{
			get
			{
				return (int)offsets.ReadStructMember("MapInfo", "right");
			}
		}

		public static int MapEdgeTop
		{
			get
			{
				return (int)offsets.ReadStructMember("MapInfo", "top");
			}
		}

		public static int MapFullHeight
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember("MapInfo", "full_height") + 0.5);
			}
		}

		public static int MapFullWidth
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember("MapInfo", "full_width") + 0.5);
			}
		}

		public static int MapPlayableHeight2
		{
			get
			{
				return (MapEdgeTop2 - MapEdgeBottom2);
			}
		}

		public static int MapPlayableWidth2
		{
			get
			{
				return (MapEdgeRight2 - MapEdgeLeft2);
			}
		}

		public static int MapPlayableHeight
		{
			get
			{
				return (MapEdgeTop - MapEdgeBottom);
			}
		}

		public static int MapPlayableWidth
		{
			get
			{
				return (MapEdgeRight - MapEdgeLeft);
			}
		}

		public static ReadWriteMemory mem
		{
			get
			{
				if (_mem == null)
				{
					_mem = new ReadWriteMemory(SC2Handle);
				}
				return _mem;
			}
		}

		public static PatternScan ps
		{
			get
			{
				if (_ps == null)
				{
					_ps = new PatternScan();
					_ps.Setup(SC2Handle);
				}
				return _ps;
			}
		}

		public static float RatioPlayable
		{
			get
			{
				return (((float) MapPlayableHeight) / ((float) MapPlayableWidth));
			}
		}

		public static string SC2FilePath
		{
			get
			{
				if (_SC2FilePath == null)
				{
					_SC2FilePath = SC2Process.MainModule.FileVersionInfo.FileName.Replace("SC2.exe", "");
				}
				return _SC2FilePath;
			}
		}

		public static string SC2GameType
		{
			get
			{
				List<Player> list = getPlayersData().Where<Player>(delegate (Player x) {
					if (x.playerType != PlayerType.Computer)
					{
						return (x.playerType == PlayerType.User);
					}
					return true;
				}).ToList<Player>();
				List<int> list2 = (from x in list
					orderby x.team descending
					select x.team).Distinct<int>().ToList<int>();
				List<int> source = new List<int>();
				using (List<int>.Enumerator enumerator = list2.GetEnumerator())
				{
					int team;
					while (enumerator.MoveNext())
					{
						team = enumerator.Current;
						source.Add((from x in list
							where x.team == team
							select x).Count<Player>());
					}
				}
				source = source.Distinct<int>().ToList<int>();
				if ((list2.Count == 2) && (list.Count == 2))
				{
					return "ONEvsONE";
				}
				if (list2.Count == list.Count)
				{
					return "FFA";
				}
				if ((source.Count == 1) && (source[0] <= 4))
				{
					switch (source[0])
					{
						case 2:
							return "TWOvsTWO";

						case 3:
							return "THREEvsTHREE";

						case 4:
							return "FOURvsFOUR";
					}
				}
				return null;
			}
		}

		public static IntPtr SC2Handle
		{
			get
			{
				if ((SC2Process != null))
				{
					_SC2Handle = SC2Process.MainWindowHandle;
				}
				else
					_SC2Handle = IntPtr.Zero;
				return _SC2Handle;
			}
		}

		public static string SC2Language
		{
			get
			{
				if (_SC2Language == null)
				{
					string[] files = Directory.GetFiles(SC2FilePath);
					for (int i = 0; i < files.Length; i++)
					{
						if (files[i].Contains("patch-"))
						{
							int index = files[i].LastIndexOf("-");
							int num3 = files[i].LastIndexOf(".");
							try
							{
								_SC2Language = files[i].Substring(index + 1, (num3 - index) - 1);
							}
							catch (Exception exception)
							{
								throw new Exception("The SC2 language couldn't be parsed: " + files[i], exception);
							}
						}
					}
				}
				return _SC2Language;
			}
		}

		public static bool SC2Opened
		{
			get
			{
				return (SC2Process != null && SC2Handle != IntPtr.Zero);
			}
		}

		public static Process SC2Process
		{
			get
			{
				if (_SC2Process == null || _SC2Process.HasExited || _SC2Process.MainModule == null)
				{
					_SC2Process = null;

					Process[] processesByName = Process.GetProcessesByName("SC2");
					if (processesByName.Length != 0)
					{
						_SC2Process = processesByName[processesByName.Length - 1];
					}
				}
				return _SC2Process;
			}
		}

		public static string SC2Version
		{
			get
			{
				if ((_SC2Version == null) && (SC2Process != null))
				{
					_SC2Version = SC2Process.MainModule.FileVersionInfo.FileVersion;
				}
				if (_SC2Version == null)
				{
					return "SC2 Closed";
				}
				return _SC2Version;
			}
		}

		public static float SecondsElapsed
		{
			get
			{
				if (SC2Process == null || SC2Process.HasExited)
					return 0;
				return ((uint)offsets.ReadStructMember("Timer", "timer")) / 832f;
			}
		}

		public static class Offsets
		{
			public static uint AbilityCount;
			public static uint OFFSET_MAPHACK_EDGE_BOTTOM;
			public static uint OFFSET_MAPHACK_EDGE_LEFT;
			public static uint OFFSET_MAPHACK_EDGE_RIGHT;
			public static uint OFFSET_MAPHACK_EDGE_TOP;
			public static uint OFFSET_MAPHACK_HEIGHT;
			public static uint OFFSET_MAPHACK_WIDTH;
			public static uint StartLocations;

			public static uint LocalPlayerNumber
			{
				get
				{
					return GameData.ps.LocalPlayerNumber();
				}
			}

			public static uint OFFSET_GAME_TIMER
			{
				get
				{
					return 0x033EEF64;
					
					//return (GameData.StructStarts.Units + 0x14e);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential, Size=1)]
		public struct OffsetsFromPointers
		{
			public static uint[] SubMenu;
			static OffsetsFromPointers()
			{
				SubMenu = new uint[] { 0x21d };
			}
		}

		public static class Pointers
		{
			public static uint BuildGrid;
			public static uint Camera;
			public static uint LocalPlayerBnetID = 0x2f99154;
			public static uint SubMenu;

			public static uint MapInformation
			{
				get
				{
					return GameData.ps.MapInfoPtr();
				}
			}
		}

		public enum StructSizes : uint
		{
			Ability = 120,
			BnetID = 0x4b0,
			CameraInfo = 0x24,
			ControlGroup = 0xcf8,
			Player = 0xA68,
			SelectedUnits = 4,
			Selection = 0xcf8,
			Units = 0x1c0
		}

		public static class StructStarts
		{
			public static uint Abilities;
			public static uint BnetIDs = 0x0179EFF8; //0x0176ACF0; //0x016E5E2C; //0x016CA8B8; //0x03165c08;
			public static uint CameraInfo;

			public static uint LocalSelection
			{
				get
				{
					return GameData.ps.LocalSelection();
				}
			}

			public static uint Map
			{
				get
				{
					return (GameData.Pointers.MapInformation - 0x10);
				}
			}

			public static uint Players
			{
				get
				{
					return GameData.ps.PlayerStruct();
				}
			}

			public static uint PlayerSelections
			{
				get
				{
					return GameData.ps.PlayerSelection();
				}
			}

			public static uint Units
			{
				get
				{
					return GameData.ps.UnitStruct();
				}
			}
		}
	}
}

