namespace _2cs_API
{
	using Data;
	using System;
	using System.Runtime.InteropServices;
	using Utilities.MemoryHandling;
	using Utilities.WinControl;

	public static class clsStarCraft
	{
		private static clsMouse _mouse;

		public static bool IsActiveWindow
		{
			get
			{
				return (GameData.SC2Handle == WC.GetForegroundWindow());
			}
		}

		public static clsMouse Mouse
		{
			get
			{
				return _mouse;
			}
			set
			{
				_mouse = value;
			}
		}

		public static bool SubMenu
		{
			get
			{
				byte[] buffer;
				ReadWriteMemory memory = new ReadWriteMemory(GameData.SC2Handle);
				memory.ReadMemory(GameData.Pointers.SubMenu, 4, out buffer);
				uint num = BitConverter.ToUInt32(buffer, 0);
				for (int i = 0; i < (GameData.OffsetsFromPointers.SubMenu.Length - 1); i++)
				{
					memory.ReadMemory((uint) (num + GameData.OffsetsFromPointers.SubMenu[i]), 4, out buffer);
					num = BitConverter.ToUInt32(buffer, 0);
				}
				memory.ReadMemory((uint) (num + GameData.OffsetsFromPointers.SubMenu[GameData.OffsetsFromPointers.SubMenu.Length - 1]), 1, out buffer);
				return BitConverter.ToBoolean(buffer, 0);
			}
		}

		[StructLayout(LayoutKind.Sequential, Size=1)]
		public struct BuildingGrid
		{
			public static int X
			{
				get
				{
					byte[] buffer;
					ReadWriteMemory memory = new ReadWriteMemory(GameData.SC2Handle);
					memory.ReadMemory(GameData.Pointers.BuildGrid, 4, out buffer);
					memory.ReadMemory((uint) (BitConverter.ToUInt32(buffer, 0) + build_grid_s.X), 4, out buffer);
					return (BitConverter.ToInt32(buffer, 0) >> 12);
				}
			}
			public static int Y
			{
				get
				{
					byte[] buffer;
					ReadWriteMemory memory = new ReadWriteMemory(GameData.SC2Handle);
					memory.ReadMemory(GameData.Pointers.BuildGrid, 4, out buffer);
					memory.ReadMemory((uint) (BitConverter.ToUInt32(buffer, 0) + build_grid_s.Y), 4, out buffer);
					return (BitConverter.ToInt32(buffer, 0) >> 12);
				}
			}
			public static bool Active
			{
				get
				{
					byte[] buffer;
					ReadWriteMemory memory = new ReadWriteMemory(GameData.SC2Handle);
					memory.ReadMemory(GameData.Pointers.BuildGrid, 4, out buffer);
					memory.ReadMemory((uint) (BitConverter.ToUInt32(buffer, 0) + build_grid_s.Active), 4, out buffer);
					return BitConverter.ToBoolean(buffer, 0);
				}
			}
			public static bool ValidSpot
			{
				get
				{
					byte[] buffer;
					ReadWriteMemory memory = new ReadWriteMemory(GameData.SC2Handle);
					memory.ReadMemory(GameData.Pointers.BuildGrid, 4, out buffer);
					memory.ReadMemory((uint) (BitConverter.ToUInt32(buffer, 0) + build_grid_s.Valid), 4, out buffer);
					return BitConverter.ToBoolean(buffer, 0);
				}
			}
		}

		public enum ControlGroups : ushort
		{
			Group0 = 0x30,
			Group1 = 0x31,
			Group2 = 50,
			Group3 = 0x33,
			Group4 = 0x34,
			Group5 = 0x35,
			Group6 = 0x36,
			Group7 = 0x37,
			Group8 = 0x38,
			Group9 = 0x39
		}
	}
}

