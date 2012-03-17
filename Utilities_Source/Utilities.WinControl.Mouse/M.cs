namespace Utilities.WinControl.Mouse
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using Utilities.WinControl;

	public static class M
	{
		[DllImport("user32.dll")]
		public static extern bool ClipCursor(ref Utilities.WinControl.RECT lpRect);
		[DllImport("user32.dll")]
		public static extern IntPtr CreateCursor(IntPtr hInst, int xHotSpot, int yHotSpot, int nWidth, int nHeight, byte[] pvANDPlane, byte[] pvXORPlane);
		[DllImport("user32.dll")]
		public static extern bool GetClipCursor(out Utilities.WinControl.RECT lpRect);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out Point lpPoint);
		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, IDC_STANDARD_CURSORS lpCursorName);
		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursorFromFile(string lpFileName);
		[DllImport("user32.dll")]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
		[DllImport("user32.dll")]
		public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT
		{
			public int uMsg;
			public short wParamL;
			public short wParamH;
		}

		public enum IDC_STANDARD_CURSORS
		{
			IDC_APPSTARTING = 0x7f8a,
			IDC_ARROW = 0x7f00,
			IDC_CROSS = 0x7f03,
			IDC_HAND = 0x7f89,
			IDC_HELP = 0x7f8b,
			IDC_IBEAM = 0x7f01,
			IDC_ICON = 0x7f81,
			IDC_NO = 0x7f88,
			IDC_SIZE = 0x7f80,
			IDC_SIZEALL = 0x7f86,
			IDC_SIZENESW = 0x7f83,
			IDC_SIZENS = 0x7f85,
			IDC_SIZENWSE = 0x7f82,
			IDC_SIZEWE = 0x7f84,
			IDC_UPARROW = 0x7f04,
			IDC_WAIT = 0x7f02
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT
		{
			public M.SendInputEventType type;
			public M.MouseKeybdhardwareInputUnion mkhi;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MouseInputData
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct MouseKeybdhardwareInputUnion
		{
			[FieldOffset(0)]
			public M.HARDWAREINPUT hi;
			[FieldOffset(0)]
			public M.KEYBDINPUT ki;
			[FieldOffset(0)]
			public M.MouseInputData mi;
		}

		public enum SendInputEventType
		{
			InputMouse,
			InputKeyboard,
			InputHardware
		}
	}
}

