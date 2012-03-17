namespace Utilities.MemoryHandling
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	public static class Imports
	{
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);
		[DllImport("kernel32.dll")]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, uint lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
		[DllImport("kernel32.dll")]
		public static extern int GetLastError();
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessID);
		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
		[DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
		[DllImport("kernel32.dll")]
		public static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
		[DllImport("kernel32.dll")]
		public static extern bool VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr lpHandle, IntPtr lpAddress, byte[] lpBuffer, int lpSize, out int lpBytesWrote);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

		[Flags]
		public enum AllocationType
		{
			Commit = 0x1000,
			Decommit = 0x4000,
			LargePages = 0x20000000,
			Physical = 0x400000,
			Release = 0x8000,
			Reserve = 0x2000,
			Reset = 0x80000,
			TopDown = 0x100000,
			WriteWatch = 0x200000
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;
			public IntPtr AllocationBase;
			public uint AllocationProtect;
			public IntPtr RegionSize;
			public uint State;
			public uint Protect;
			public uint Type;
		}

		[Flags]
		public enum MemoryProtection
		{
			Execute = 0x10,
			ExecuteRead = 0x20,
			ExecuteReadWrite = 0x40,
			ExecuteWriteCopy = 0x80,
			GuardModifierflag = 0x100,
			NoAccess = 1,
			NoCacheModifierflag = 0x200,
			ReadOnly = 2,
			ReadWrite = 4,
			WriteCombineModifierflag = 0x400,
			WriteCopy = 8
		}

		[Flags]
		public enum ProcessAccessFlags : uint
		{
			All = 0x1f0fff,
			CreateThread = 2,
			DupHandle = 0x40,
			QueryInformation = 0x400,
			SetInformation = 0x200,
			Synchronize = 0x100000,
			Terminate = 1,
			VMOperation = 8,
			VMRead = 0x10,
			VMWrite = 0x20
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		private int _Left;
		private int _Top;
		private int _Right;
		private int _Bottom;

		public RECT(RECT Rectangle)
			: this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
		{
		}
		public RECT(int Left, int Top, int Right, int Bottom)
		{
			_Left = Left;
			_Top = Top;
			_Right = Right;
			_Bottom = Bottom;
		}

		public int X
		{
			get { return _Left; }
			set { _Left = value; }
		}
		public int Y
		{
			get { return _Top; }
			set { _Top = value; }
		}
		public int Left
		{
			get { return _Left; }
			set { _Left = value; }
		}
		public int Top
		{
			get { return _Top; }
			set { _Top = value; }
		}
		public int Right
		{
			get { return _Right; }
			set { _Right = value; }
		}
		public int Bottom
		{
			get { return _Bottom; }
			set { _Bottom = value; }
		}
		public int Height
		{
			get { return _Bottom - _Top; }
			set { _Bottom = value + _Top; }
		}
		public int Width
		{
			get { return _Right - _Left; }
			set { _Right = value + _Left; }
		}
		public Point Location
		{
			get { return new Point(Left, Top); }
			set
			{
				_Left = value.X;
				_Top = value.Y;
			}
		}
		public Size Size
		{
			get { return new Size(Width, Height); }
			set
			{
				_Right = value.Width + _Left;
				_Bottom = value.Height + _Top;
			}
		}

		public static implicit operator Rectangle(RECT Rectangle)
		{
			return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
		}
		public static implicit operator RECT(Rectangle Rectangle)
		{
			return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
		}
		public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
		{
			return Rectangle1.Equals(Rectangle2);
		}
		public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
		{
			return !Rectangle1.Equals(Rectangle2);
		}

		public override string ToString()
		{
			return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool Equals(RECT Rectangle)
		{
			return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
		}

		public override bool Equals(object Object)
		{
			if (Object is RECT)
			{
				return Equals((RECT)Object);
			}
			else if (Object is Rectangle)
			{
				return Equals(new RECT((Rectangle)Object));
			}

			return false;
		}
	}
}

