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

		public static Player GetPlayer(uint numplayer)
		{
			byte[] buffer;
			Player p = new Player {
				memoryAddress = StructStarts.Players + ((uint)StructSizes.Player * numplayer)
			};
			player_s _s = (player_s) mem.ReadMemory(p.memoryAddress, typeof(player_s));

			string AccountNumber = "";
			if (numplayer < 16)
			{
				byte[] rawAccountNumber = new byte[0x18];
				mem.ReadMemory(StructStarts.BnetIDs + (numplayer - 1) * (uint)StructSizes.BnetID, rawAccountNumber.Length, out rawAccountNumber);
				AccountNumber = Encoding.ASCII.GetString(rawAccountNumber);
				int removeIndex = AccountNumber.IndexOf('\0');
				if(removeIndex > 0)
					AccountNumber = AccountNumber.Remove(removeIndex);
				removeIndex = AccountNumber.IndexOf('\\');
				if (removeIndex > 0)
					AccountNumber = AccountNumber.Remove(removeIndex);
			}

			if (!AccountNumber.Contains("-S2-"))
				AccountNumber = "(none)";

			p.accountNumber = AccountNumber;
			p.rankIconTooltip = "Unranked";

			p.number = numplayer;
			p.playerStatus = (PlayerStatus) _s.active;
			p.victoryStatus = (VictoryStatus) _s.status;
			p.playerType = (PlayerType) _s.player_type;
			p.team = _s.team;
			p.name = _s.name;
			p.playerColor = (PlayerColor) _s.slot_number;
			p.drawingColor = player_colors[numplayer];
			p.minerals = (int) _s.minerals_current;
			p.gas = (int) _s.vespene_current;
			p.terrazine = (int) _s.terrazine_current;
			p.custom = (int) _s.custom_resource_current;
			p.mineralRate = (int) _s.mineral_rate;
			p.gasRate = (int) _s.vespene_rate;
			p.mineralTotal = (int) _s.minerals_total;
			p.gasTotal = (int) _s.vespene_total;
			p.difficulty = (PlayerDifficulty) _s.difficulty;
			p.actionsTotal = (int) _s.actions_total;
			p.apmCurrent = (int) _s.apm_current;
			p.armySize = (int) _s.army_size;
			p.unitsLostMineralWorth = (int) _s.units_lost_mineral_worth;
			p.unitsLostVespeneWorth = (int) _s.units_lost_vespene_worth;
			p.buildingsConstructing = (int) _s.buildings_constructing;
			p.cameraDistance = (int) _s.camera_distance;
			p.cameraX = (int) Math.Round((double) (((double) _s.camera_x) / 4096.0));
			p.cameraY = (int) Math.Round((double) (((double) _s.camera_y) / 4096.0));
			p.cameraRotation = (int) _s.camera_rotation;
			p.workersBuilt = (int) _s.harvesters_built;
			p.workersCurrent = (int) _s.harvesters_current;
			p.unitsKilled = (int) _s.units_killed;
			p.unitsLost = (int) _s.units_lost;
			p.buildingQueueLength = (int) _s.building_queue_length;
			p.slotNumber = (int) _s.slot_number;
			p.supply = _s.supply_current / 4096f;
			p.supplyCap = _s.supply_cap / 4096f;
			p.supplyLimit = _s.supply_limit / 4096f;
			uint num = (uint) mem.ReadMemory((uint) (_s.racePointer + 4), typeof(uint));
			mem.ReadMemory((uint) (num + 8), 4, out buffer);
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
			p.unit_selections = GetPlayerSelections(p);
			p.units = new List<Unit>();
			return p;
		}

		public static List<Player> getPlayersData()
		{
			List<Player> list = new List<Player>();
			for (uint i = 0; i < 0x10; i++)
			{
				Player item = GetPlayer(i);
				if (!(item.name == ""))
				{
					if((int) item.playerColor < 16)
						player_colors[i] = possible_colors[(int) item.playerColor];
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
			for (uint i = 0; i != 0xffff; i++)
			{
				unit_s us = new unit_s();
				uint address = StructStarts.Units + ((int) StructSizes.Units * i);
				us = (unit_s) mem.ReadMemory(address, typeof(unit_s));
				if (us.unit_model == 0)
				{
					//GetUnitModels();
					return list;
				}
				Unit item = ParseUnit(us);
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

		public static Unit ParseUnit(unit_s us)
		{
			unit_model_s ums = (unit_model_s)mem.ReadMemory((uint) (us.unit_model << 5), typeof(unit_model_s));

			string NameAsText = null;
			string UINameAsText = null;
			if (ums.pName_address != 0)
			{
				uint NameDataAddress = (uint)mem.ReadMemory(ums.pName_address, typeof(uint));
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

						if ((uint)mem.ReadMemory(pUINameAddress + 0xc, typeof(uint)) != 0x43) //sometimes the string is right in this struct, other times it's a pointer.
							UINameAddress = (uint)mem.ReadMemory(pUINameAddress + 0x10, typeof(uint));

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

			Unit unit = new Unit { 
				name = UINameAsText,
				textID = NameAsText,
				minimapRadius = (float)ums.minimap_radius / 4096f,
				timeScale = us.time_scale / 4096f,

				ID = us.token,
				playerNumber = us.player_owner,
				unitType = (UnitType) ums.unit_type, 
				state = (UnitStateOld) us.state, 

				isImmobile = us.isImmobile != 0,
				targetFilterFlags = (TargetFilter) us.targetFilter_flags,
				isAlive = (us.targetFilter_flags & (ulong) TargetFilter.Dead) == 0,
				cloaked = (us.targetFilter_flags & (ulong) TargetFilter.Cloaked) != 0,
				detector = (us.targetFilter_flags & (ulong) TargetFilter.Detector) != 0,

				moveState = (UnitMoveState) us.move_state, 
				subMoveState = (UnitSubMoveState) us.sub_move_state, 
				lastOrder = (UnitLastOrder) us.last_order, 
				deathType = (DeathType) us.death_type,

				energyRegenDelay = ums.energy_regen_delay / 65536f,
				energyRegenRate = (ums.energy_regen_rate + us.energy_regen_bonus) / 256f,
				energyDamage = (ums.max_energy * (us.energy_multiplier / 4096f) + us.bonus_max_energy - us.energy) / 4096f,
				currentEnergy = us.energy / 4096f,
				maxEnergy = (ums.max_energy * (us.energy_multiplier / 4096f) + us.bonus_max_energy) / 4096f,

				shieldRegenDelay = ums.shield_regen_delay / 65536f,
				shieldRegenRate = (ums.shield_regen_rate + us.shield_regen_bonus)/ 256f,
				shieldDamage = us.shield_damage / 4096f,
				currentShield = (ums.max_shield * (us.shields_multiplier / 4096f) + us.bonus_max_shields - us.shield_damage) / 4096f,
				maxShield = (ums.max_shield * (us.shields_multiplier / 4096f) + us.bonus_max_shields) / 4096f,

				healthRegenDelay = ums.health_regen_delay / 65536f,
				healthRegenRate = (ums.health_regen_rate + us.energy_regen_bonus) / 256f,
				healthDamage = us.health_damage / 4096f,
				currentHealth = (ums.max_health * (us.health_multiplier / 4096f) + us.bonus_max_health - us.health_damage) / 4096f,
				maxHealth = (ums.max_health * (us.health_multiplier / 4096f) + us.bonus_max_health) / 4096f,

				locationX = ((float) us.position_x) / 4096f, 
				locationY = ((float) us.position_y) / 4096f, 
				locationZ = ((float) us.position_z) / 4096f, 

				destinationX = ((float) us.destination_x) / 4096f, 
				destinationY = ((float) us.destination_y) / 4096f,
				destinationZ = ((float) us.destination_z) / 4096f,

				destination2X = ((float) us.destination2_x) / 4096f,
				destination2Y = ((float) us.destination2_y) / 4096f,

				kills = us.kills,

				/*rotation = (int)(us.rotation * 0.010987669527379678),
				rotationX = (int)us.rotation_x,
				rotationY = (int)us.rotation_y,*/
				rotation = 180 - us.rotation / 4096f * 45,
				rotationX = 180 - us.rotation_x / 4096f * 45,
				rotationY = 180 - us.rotation_y / 4096f * 45,

				moveSpeed = (int)us.move_speed,
				memoryLocation = StructStarts.Units + ((uint) StructSizes.Units * (us.token / 4u)),
				commandQueuePointer = us.commandQueue_pointer
			 };

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
				int MapBottom = ((int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xe0), typeof(int)) + 2048) / 0x1000;
				return MapBottom;
			}
		}

		public static int MapEdgeLeft2
		{
			get
			{
				int MapLeft = ((int) mem.ReadMemory((uint) (Pointers.MapInformation + 0xdc), typeof(int)) + 2048) / 0x1000;
				return MapLeft;
			}
		}

		public static int MapEdgeRight2
		{
			get
			{
				int MapWidth = MapFullWidth;
				int MapRight = ((int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xe4), typeof(int)) + 2048) / 0x1000;
				return MapRight;
			}
		}

		public static int MapEdgeTop2
		{
			get
			{
				int MapHeight = MapFullHeight;
				int MapTop = ((int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xe8), typeof(int)) + 2048) / 0x1000;
				return MapTop;
			}
		}

		public static int MapEdgeBottom
		{
			get
			{
				int MapBottom = (int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xf0), typeof(int));
				return MapBottom;
			}
		}

		public static int MapEdgeLeft
		{
			get
			{
				int MapLeft = (int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xec), typeof(int));
				return MapLeft;
			}
		}

		public static int MapEdgeRight
		{
			get
			{
				int MapWidth = MapFullWidth;
				int MapRight = (int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xf4), typeof(int));
				return MapRight;
			}
		}

		public static int MapEdgeTop
		{
			get
			{
				int MapHeight = MapFullHeight;
				int MapTop = (int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xf8), typeof(int));
				return MapTop;
			}
		}

		public static int MapFullHeight
		{
			get
			{
				return (((int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xb0), typeof(int)) + 2048) / 0x1000);
			}
		}

		public static int MapFullWidth
		{
			get
			{
				return (((int)mem.ReadMemory((uint)(Pointers.MapInformation + 0xac), typeof(int)) + 2048) / 0x1000);
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
				if ((_SC2Handle == IntPtr.Zero) && (SC2Process != null))
				{
					_SC2Handle = SC2Process.MainWindowHandle;
				}
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
				return (SC2Handle != IntPtr.Zero);
			}
		}

		public static Process SC2Process
		{
			get
			{
				if (_SC2Process == null)
				{
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

		public static int SecondsElapsed
		{
			get
			{
				return (int) mem.ReadMemory(Offsets.OFFSET_GAME_TIMER, typeof(int));
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
					return 0x02763864;
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
			Player = 0x910,
			SelectedUnits = 4,
			Selection = 0xcf8,
			Units = 0x1c0
		}

		public static class StructStarts
		{
			public static uint Abilities;
			public static uint BnetIDs = 0x016E5E2C; //0x016CA8B8; //0x3165c08;
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

