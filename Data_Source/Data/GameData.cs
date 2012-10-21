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
	using System.Threading;
	using System.Text;
	using Utilities.MemoryHandling;
	using Utilities.TextProcessing;
	using Utilities.WinControl;

	public static class Imports
	{
		[DllImport("kernel32.dll")]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
		[DllImport("kernel32.dll")]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);
	}

	public static class Locks
	{
		public static ReaderWriterLockSlim Units = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		public static ReaderWriterLockSlim Players = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		public static ReaderWriterLockSlim Map = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion); //Not used consistantly for performance reasons.
	}

	public static class GameData
	{
		private static string _path = null;
		public static string path
		{
			get
			{
				if (_path == null || _path == string.Empty)
				{
					try
					{
						string modulename = Process.GetCurrentProcess().MainModule.FileName;
						_path = modulename.Remove(modulename.LastIndexOf('\\') + 1); // add 1 so it doesn't remove the \
					}
					catch
					{
						_path = string.Empty;
					}
				}
				return _path;
			}
		}

		private static bool _useTimer2 = false;
		private static OffsetReader _offsets;
		public static OffsetReader offsets
		{
			get
			{
				if (_offsets == null)
				{
					_offsets = new OffsetReader(path + "Offsets.xml");
					if (!_offsets.CheckVersion())
					{
						Thread.Sleep(1000); //Sometimes the version check will fail right when SC2 is started, so waiting a second and checking again should help.
						if (!_offsets.CheckVersion())
						{
							_offsets.UpdateAddresses();
							_offsets = new OffsetReader(path + "Offsets.xml");
						}
					}
					_useTimer2 = _offsets.GetStructAddress(ORNames.Timer) == 0;
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

		public static int LocalPlayerNumber
		{
			get
			{
				byte num = (byte)GameData.offsets.ReadStructMember(ORNames.LocalPlayer, ORNames.LocalPlayer);
				return num < 16 ? num : 0;
			}
		}

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
			/*mem.ReadMemory(Offsets.OFFSET_MAPHACK_WIDTH, 4, out info.width);
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
			info.playableY = info.edgeTop - info.edgeBottom;*/
			num = (uint)offsets.ReadStructMember(ORNames.MapInfo, ORNames.dynamic_pointer);
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

		public static Player GetPlayer(int number)
		{
			return new Player(number);
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

		public static List<Unit> getUnitData()
		{
			Locks.Units.EnterReadLock();
			List<Unit> ReturnVal = Unit.Units.Values.ToList();
			Locks.Units.ExitReadLock();
			return ReturnVal;
		}

		public static List<Unit> GetPlayerUnits(int playerNumber)
		{
			return getUnitData().FindAll(u => u.playerNumber == playerNumber);
		}

		public static List<Unit> GetPlayersUnits(List<int> playerNumbers)
		{
			return getUnitData().FindAll(u => playerNumbers.Contains((int)u.playerNumber));
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
			return new Unit(Index);
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
				return (int)((fixed32)offsets.ReadStructMember(ORNames.MapInfo, ORNames.bottom_precise) + 0.5);
			}
		}

		public static int MapEdgeLeft2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember(ORNames.MapInfo, ORNames.left_precise) + 0.5);
			}
		}

		public static int MapEdgeRight2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember(ORNames.MapInfo, ORNames.right_precise) + 0.5);
			}
		}

		public static int MapEdgeTop2
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember(ORNames.MapInfo, ORNames.top_precise) + 0.5);
			}
		}

		public static int MapEdgeBottom
		{
			get
			{
				return (int)offsets.ReadStructMember(ORNames.MapInfo, ORNames.bottom);
			}
		}

		public static int MapEdgeLeft
		{
			get
			{
				return (int)offsets.ReadStructMember(ORNames.MapInfo, ORNames.left);
			}
		}

		public static int MapEdgeRight
		{
			get
			{
				return (int)offsets.ReadStructMember(ORNames.MapInfo, ORNames.right);
			}
		}

		public static int MapEdgeTop
		{
			get
			{
				return (int)offsets.ReadStructMember(ORNames.MapInfo, ORNames.top);
			}
		}

		public static int MapFullHeight
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember(ORNames.MapInfo, ORNames.full_height) + 0.5);
			}
		}

		public static int MapFullWidth
		{
			get
			{
				return (int)((fixed32)offsets.ReadStructMember(ORNames.MapInfo, ORNames.full_width) + 0.5);
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
				if (_mem == null || _mem.m_lpHandle == IntPtr.Zero)
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
					_SC2FilePath = SC2Process.MainModule.FileName.Replace("SC2.exe", "");
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
					try
					{
						_SC2Handle = SC2Process.MainWindowHandle;
					}
					catch (NullReferenceException)
					{
						_SC2Handle = IntPtr.Zero;
					}
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
				return (SC2Process != null);
			}
		}

		public static void ResetProcess()
		{
			_SC2Process = null;
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
						_SC2Handle = IntPtr.Zero;
						_mem = null;
					}
				}
				return _SC2Process;
			}
		}

		public static string SC2Version
		{
			get
			{
				if (_SC2Version == null && SC2Process != null && !SC2Process.HasExited)
				{
					try
					{
						_SC2Version = SC2Process.MainModule.FileVersionInfo.FileVersion;
					}
					catch(Exception ex)
					{
						_SC2Version = null;
						if (!(ex is System.ComponentModel.Win32Exception) && !(ex is NullReferenceException))
							throw ex;
					}
				}
				if (SC2Process == null)
					_SC2Version = null;

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
				if (SC2Process == null)
					return -1;

				if(_useTimer2)
					return ((uint)offsets.ReadStructMember(ORNames.Timer2, ORNames.timer)) / 832f;
				else
					return (fixed32)offsets.ReadStructMember(ORNames.Timer, ORNames.timer);
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
			Selection = 0xcf8,
			Player = 0xA68,
			SelectedUnits = 4,
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

