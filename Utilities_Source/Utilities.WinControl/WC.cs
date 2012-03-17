namespace Utilities.WinControl
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;

	public class WC
	{
		private const int GWL_EXSTYLE = -20;
		private const int GWL_ID = -12;
		private const int GWL_STYLE = -16;
		private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		private static readonly IntPtr HWND_TOP = new IntPtr(0);
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		public const int MOUSEEVENTF_LEFTDOWN = 2;
		public const int MOUSEEVENTF_LEFTUP = 4;
		public const int MOUSEEVENTF_RIGHTDOWN = 8;
		public const int MOUSEEVENTF_RIGHTUP = 0x10;
		public const int SM_CMONITORS = 80;
		public const int SM_CXFRAME = 0x20;
		public const int SM_CXSCREEN = 0;
		public const int SM_CXVIRTUALSCREEN = 0x4e;
		public const int SM_CYCAPTION = 4;
		public const int SM_CYFRAME = 0x21;
		public const int SM_CYSCREEN = 1;
		public const int SM_CYVIRTUALSCREEN = 0x4f;
		public const int SM_SWAPBUTTON = 0x17;
		private const uint SWP_FRAMECHANGED = 0x20;
		private const uint SWP_HIDEWINDOW = 0x80;
		private const uint SWP_NOACTIVATE = 0x10;
		private const uint SWP_NOCOPYBITS = 0x100;
		private const uint SWP_NOMOVE = 2;
		private const uint SWP_NOOWNERZORDER = 0x200;
		private const uint SWP_NOREDRAW = 8;
		private const uint SWP_NOSENDCHANGING = 0x400;
		private const uint SWP_NOSIZE = 1;
		private const uint SWP_NOZORDER = 4;
		private const uint SWP_SHOWWINDOW = 0x40;
		private const uint TOPMOST_FLAGS = 3;
		private const uint WS_BORDER = 0x800000;
		private const uint WS_CAPTION = 0xc00000;
		private const uint WS_CHILD = 0x40000000;
		private const uint WS_CLIPCHILDREN = 0x2000000;
		private const uint WS_CLIPSIBLINGS = 0x4000000;
		private const uint WS_DISABLED = 0x8000000;
		private const uint WS_DLGFRAME = 0x400000;
		private const uint WS_EX_ACCEPTFILES = 0x10;
		private const uint WS_EX_APPWINDOW = 0x40000;
		private const uint WS_EX_CLIENTEDGE = 0x200;
		private const uint WS_EX_COMPOSITED = 0x2000000;
		private const uint WS_EX_CONTEXTHELP = 0x400;
		private const uint WS_EX_CONTROLPARENT = 0x10000;
		private const uint WS_EX_DLGMODALFRAME = 1;
		private const uint WS_EX_LAYERED = 0x80000;
		private const uint WS_EX_LAYOUTRTL = 0x400000;
		private const uint WS_EX_LEFT = 0;
		private const uint WS_EX_LEFTSCROLLBAR = 0x4000;
		private const uint WS_EX_LTRREADING = 0;
		private const uint WS_EX_MDICHILD = 0x40;
		private const uint WS_EX_NOACTIVATE = 0x8000000;
		private const uint WS_EX_NOINHERITLAYOUT = 0x100000;
		private const uint WS_EX_NOPARENTNOTIFY = 4;
		private const uint WS_EX_OVERLAPPEDWINDOW = 0x300;
		private const uint WS_EX_PALETTEWINDOW = 0x188;
		private const uint WS_EX_RIGHT = 0x1000;
		private const uint WS_EX_RIGHTSCROLLBAR = 0;
		private const uint WS_EX_RTLREADING = 0x2000;
		private const uint WS_EX_STATICEDGE = 0x20000;
		private const uint WS_EX_TOOLWINDOW = 0x80;
		private const uint WS_EX_TOPMOST = 8;
		private const uint WS_EX_TRANSPARENT = 0x20;
		private const uint WS_EX_WINDOWEDGE = 0x100;
		private const uint WS_GROUP = 0x20000;
		private const uint WS_HSCROLL = 0x100000;
		private const uint WS_ICONIC = 0x20000000;
		private const uint WS_MAXIMIZE = 0x1000000;
		private const uint WS_MAXIMIZEBOX = 0x10000;
		private const uint WS_MINIMIZE = 0x20000000;
		private const uint WS_MINIMIZEBOX = 0x20000;
		private const uint WS_OVERLAPPED = 0;
		private const uint WS_POPUP = 0x80000000;
		private const uint WS_SIZEBOX = 0x40000;
		private const uint WS_SYSMENU = 0x80000;
		private const uint WS_TABSTOP = 0x10000;
		private const uint WS_THICKFRAME = 0x40000;
		private const uint WS_TILED = 0;
		private const uint WS_VISIBLE = 0x10000000;
		private const uint WS_VSCROLL = 0x200000;

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
		public static Point CenterWindow(Utilities.WinControl.RECT rect1, Utilities.WinControl.RECT rect2)
		{
			int x = (rect1.Left + (rect1.Width / 2)) - (rect2.Width / 2);
			return new Point(x, (rect1.Top + (rect1.Height / 2)) - (rect2.Height / 2));
		}

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
		[DllImport("user32.dll", SetLastError=true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);
		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(Keys vKey);
		public static EnumWindowsItem GetChildWindow(string title, string className)
		{
			if ((title != "") || (className != ""))
			{
				EnumWindows windows = new EnumWindows();
				windows.GetWindows();
				foreach (EnumWindowsItem item in windows.Items)
				{
					if ((className == "") && (item.Text == title))
					{
						return item;
					}
					if ((item.Text == title) && (item.ClassName == className))
					{
						return item;
					}
					if ((title == "") && (item.ClassName == className))
					{
						return item;
					}
				}
			}
			return null;
		}

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out Utilities.WinControl.RECT lpRect);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		public static int GetIDName(string Name)
		{
			foreach (Process process in Process.GetProcesses())
			{
				if (process.ProcessName == Name)
				{
					return process.Id;
				}
			}
			return 0;
		}

		public static int GetIDTitle(string Title)
		{
			foreach (Process process in Process.GetProcesses())
			{
				if (process.MainWindowTitle == Title)
				{
					return process.Id;
				}
			}
			return 0;
		}

		[DllImport("user32.dll")]
		public static extern short GetKeyState(Keys vKey);
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern IntPtr GetParent(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(SystemMetric smIndex);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
		[DllImport("user32.dll")]
		public static extern long GetWindowLong(IntPtr hWnd, int nIndex);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr hWnd, ref Utilities.WinControl.WINDOWPLACEMENT lpwndpl);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out Utilities.WinControl.RECT lpRect);
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hwnd, StringBuilder buf, int nMaxCount);
		[DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32.dll", SetLastError=true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		public static int Hwnd2ID(IntPtr hwnd)
		{
			uint num;
			GetWindowThreadProcessId(hwnd, out num);
			return (int) num;
		}

		public static IntPtr ID2Handle(int ID)
		{
			try
			{
				return Process.GetProcessById(ID).MainWindowHandle;
			}
			catch
			{
				return IntPtr.Zero;
			}
		}

		public static bool IsFullScreen(IntPtr hWnd)
		{
			Utilities.WinControl.RECT rect;
			Utilities.WinControl.RECT rect2;
			GetWindowRect(hWnd, out rect);
			GetClientRect(hWnd, out rect2);
			return (((rect2.Bottom == rect.Bottom) && (rect2.Right == rect.Right)) || (((GetSystemMetrics(SystemMetric.SM_CMONITORS) > 1) && (rect2.Bottom == rect.Bottom)) && (rect2.Right == (rect.Right - rect.Left))));
		}

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern bool IsZoomed(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
		[DllImport("kernel32.dll")]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
		[DllImport("kernel32.dll")]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);
		[DllImport("user32.dll", SetLastError=true)]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);
		[DllImport("User32.dll")]
		public static extern int SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		public static void SetWindowLayeredMode(IntPtr hwnd)
		{
			SetWindowLong(hwnd, -20, GetWindowLong(hwnd, -20) | 0x20L);
		}

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);
		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int posX, int posY, int width, int height, uint uFlags);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
		[DllImport("User32.Dll")]
		private static extern void SetWindowText(IntPtr handle, string s);
		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
		[DllImport("user32.dll")]
		public static extern short VkKeyScan(char ch);
		public static Size winGetClientSize(IntPtr hwnd)
		{
			Utilities.WinControl.RECT rect;
			GetClientRect(hwnd, out rect);
			return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		public static bool winGetIsTopmost(IntPtr Handle)
		{
			return ((GetWindowLong(Handle, -20) & 8L) != 0L);
		}

		public static Point winGetPosition(IntPtr hwnd)
		{
			Utilities.WinControl.WINDOWPLACEMENT windowplacement;
			windowplacement = new Utilities.WinControl.WINDOWPLACEMENT();
			windowplacement.length = Marshal.SizeOf(windowplacement);

			GetWindowPlacement(hwnd, ref windowplacement);
			return new Point(windowplacement.rcNormalPosition.X, windowplacement.rcNormalPosition.Y);
		}

		public static Size winGetSize(IntPtr hwnd)
		{
			Utilities.WinControl.RECT rect;
			GetWindowRect(hwnd, out rect);
			return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		public static string winGetText(int ID)
		{
			StringBuilder buf = new StringBuilder(GetWindowTextLength(ID2Handle(ID)) + 1);
			GetWindowText(ID2Handle(ID), buf, buf.Capacity);
			return buf.ToString();
		}

		public static void winSetHide(int ID)
		{
			SetWindowPos(ID2Handle(ID), HWND_NOTOPMOST, 0, 0, 0, 0, (uint) 0x80);
		}

		public static void winSetNoMove(int ID)
		{
			SetWindowPos(ID2Handle(ID), HWND_TOPMOST, 0, 0, 0, 0, (uint) 2);
		}

		public static void winSetNoSize(int ID)
		{
			SetWindowPos(ID2Handle(ID), HWND_TOPMOST, 0, 0, 0, 0, (uint) 1);
		}

		public static void winSetPosition(int ID, int x, int y)
		{
			SetWindowPos(ID2Handle(ID), HWND_NOTOPMOST, x, y, 0, 0, (uint) 1);
		}

		public static void winSetPosition(IntPtr winHwnd, int x, int y)
		{
			if (winGetIsTopmost(winHwnd))
			{
				SetWindowPos(winHwnd, HWND_TOPMOST, x, y, 0, 0, (uint) 1);
			}
			else
			{
				SetWindowPos(winHwnd, HWND_NOTOPMOST, x, y, 0, 0, (uint) 1);
			}
		}

		public static void winSetShow(IntPtr hwnd)
		{
			SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, (uint) 0x40);
		}

		public static void winSetSize(int ID, int width, int height)
		{
			SetWindowPos(ID2Handle(ID), HWND_NOTOPMOST, 0, 0, width, height, (uint) 2);
		}

		public static void winSetSize(IntPtr hWnd, int width, int height)
		{
			MoveWindow(hWnd, 0, 0, width, height, false);
		}

		public static void winSetText(int ID, string S)
		{
			SetWindowText(ID2Handle(ID), S);
		}

		public static void winSetToFront(IntPtr hwnd)
		{
			SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, (uint) 3);
		}

		public static void winSetTopMost(IntPtr hwnd)
		{
			SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, (uint) 3);
		}

		/*public static object List
		{
			[CompilerGenerated]
			get
			{
				return <List>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<List>k__BackingField = value;
			}
		}*/

		[Flags]
		public enum SetWindowPosFlags : uint
		{
			DeferErase = 0x2000,
			DoNotActivate = 0x10,
			DoNotChangeOwnerZOrder = 0x200,
			DoNotCopyBits = 0x100,
			DoNotRedraw = 8,
			DoNotReposition = 0x200,
			DoNotSendChangingEvent = 0x400,
			DrawFrame = 0x20,
			FrameChanged = 0x20,
			HideWindow = 0x80,
			IgnoreMove = 2,
			IgnoreResize = 1,
			IgnoreZOrder = 4,
			ShowWindow = 0x40,
			SynchronousWindowPosition = 0x4000
		}

		public enum SystemMetric
		{
			SM_ARRANGE = 0x38,
			SM_CLEANBOOT = 0x43,
			SM_CMETRICS_2000 = 0x53,
			SM_CMETRICS_NT = 0x58,
			SM_CMETRICS_OTHER = 0x4c,
			SM_CMONITORS = 80,
			SM_CMOUSEBUTTONS = 0x2b,
			SM_CXBORDER = 5,
			SM_CXCURSOR = 13,
			SM_CXDLGFRAME = 7,
			SM_CXDOUBLECLK = 0x24,
			SM_CXDRAG = 0x44,
			SM_CXEDGE = 0x2d,
			SM_CXFIXEDFRAME = 7,
			SM_CXFOCUSBORDER = 0x53,
			SM_CXFRAME = 0x20,
			SM_CXFULLSCREEN = 0x10,
			SM_CXHSCROLL = 0x15,
			SM_CXHTHUMB = 10,
			SM_CXICON = 11,
			SM_CXICONSPACING = 0x26,
			SM_CXMAXIMIZED = 0x3d,
			SM_CXMAXTRACK = 0x3b,
			SM_CXMENUCHECK = 0x47,
			SM_CXMENUSIZE = 0x36,
			SM_CXMIN = 0x1c,
			SM_CXMINIMIZED = 0x39,
			SM_CXMINSPACING = 0x2f,
			SM_CXMINTRACK = 0x22,
			SM_CXSCREEN = 0,
			SM_CXSIZE = 30,
			SM_CXSIZEFRAME = 0x20,
			SM_CXSMICON = 0x31,
			SM_CXSMSIZE = 0x34,
			SM_CXVIRTUALSCREEN = 0x4e,
			SM_CXVSCROLL = 3,
			SM_CYBORDER = 6,
			SM_CYCAPTION = 4,
			SM_CYCURSOR = 14,
			SM_CYDLGFRAME = 8,
			SM_CYDOUBLECLK = 0x25,
			SM_CYDRAG = 0x45,
			SM_CYEDGE = 0x2e,
			SM_CYFIXEDFRAME = 8,
			SM_CYFOCUSBORDER = 0x54,
			SM_CYFRAME = 0x21,
			SM_CYFULLSCREEN = 0x11,
			SM_CYHSCROLL = 20,
			SM_CYICON = 12,
			SM_CYICONSPACING = 0x27,
			SM_CYKANJIWINDOW = 0x12,
			SM_CYMAXIMIZED = 0x3e,
			SM_CYMAXTRACK = 60,
			SM_CYMENU = 15,
			SM_CYMENUCHECK = 0x48,
			SM_CYMENUSIZE = 0x37,
			SM_CYMIN = 0x1d,
			SM_CYMINIMIZED = 0x3a,
			SM_CYMINSPACING = 0x30,
			SM_CYMINTRACK = 0x23,
			SM_CYSCREEN = 1,
			SM_CYSIZE = 0x1f,
			SM_CYSIZEFRAME = 0x21,
			SM_CYSMCAPTION = 0x33,
			SM_CYSMICON = 50,
			SM_CYSMSIZE = 0x35,
			SM_CYVIRTUALSCREEN = 0x4f,
			SM_CYVSCROLL = 2,
			SM_CYVTHUMB = 9,
			SM_DBCSENABLED = 0x2a,
			SM_DEBUG = 0x16,
			SM_IMMENABLED = 0x52,
			SM_MEDIACENTER = 0x57,
			SM_MENUDROPALIGNMENT = 40,
			SM_MIDEASTENABLED = 0x4a,
			SM_MOUSEPRESENT = 0x13,
			SM_MOUSEWHEELPRESENT = 0x4b,
			SM_NETWORK = 0x3f,
			SM_PENWINDOWS = 0x29,
			SM_REMOTECONTROL = 0x2001,
			SM_REMOTESESSION = 0x1000,
			SM_RESERVED1 = 0x18,
			SM_RESERVED2 = 0x19,
			SM_RESERVED3 = 0x1a,
			SM_RESERVED4 = 0x1b,
			SM_SAMEDISPLAYFORMAT = 0x51,
			SM_SECURE = 0x2c,
			SM_SHOWSOUNDS = 70,
			SM_SHUTTINGDOWN = 0x2000,
			SM_SLOWMACHINE = 0x49,
			SM_SWAPBUTTON = 0x17,
			SM_TABLETPC = 0x56,
			SM_XVIRTUALSCREEN = 0x4c,
			SM_YVIRTUALSCREEN = 0x4d
		}

		public enum TernaryRasterOperations : uint
		{
			BLACKNESS = 0x42,
			CAPTUREBLT = 0x40000000,
			DSTINVERT = 0x550009,
			MERGECOPY = 0xc000ca,
			MERGEPAINT = 0xbb0226,
			NOTSRCCOPY = 0x330008,
			NOTSRCERASE = 0x1100a6,
			PATCOPY = 0xf00021,
			PATINVERT = 0x5a0049,
			PATPAINT = 0xfb0a09,
			SRCAND = 0x8800c6,
			SRCCOPY = 0xcc0020,
			SRCERASE = 0x440328,
			SRCINVERT = 0x660046,
			SRCPAINT = 0xee0086,
			WHITENESS = 0xff0062
		}

		[Flags]
		public enum VK : ushort
		{
			VK_ACCEPT = 30,
			VK_ADD = 0x6b,
			VK_APPS = 0x5d,
			VK_BACK = 8,
			VK_BROWSER_BACK = 0xa6,
			VK_BROWSER_FAVORITES = 0xab,
			VK_BROWSER_FORWARD = 0xa7,
			VK_BROWSER_HOME = 0xac,
			VK_BROWSER_REFRESH = 0xa8,
			VK_BROWSER_SEARCH = 170,
			VK_BROWSER_STOP = 0xa9,
			VK_CANCEL = 3,
			VK_CAPITAL = 20,
			VK_CLEAR = 12,
			VK_CONTROL = 0x11,
			VK_CONVERT = 0x1c,
			VK_DECIMAL = 110,
			VK_DELETE = 0x2e,
			VK_DIVIDE = 0x6f,
			VK_DOWN = 40,
			VK_END = 0x23,
			VK_ESCAPE = 0x1b,
			VK_EXECUTE = 0x2b,
			VK_F1 = 0x70,
			VK_F10 = 0x79,
			VK_F11 = 0x7a,
			VK_F12 = 0x7b,
			VK_F13 = 0x7c,
			VK_F14 = 0x7d,
			VK_F15 = 0x7e,
			VK_F16 = 0x7f,
			VK_F17 = 0x80,
			VK_F18 = 0x81,
			VK_F19 = 130,
			VK_F2 = 0x71,
			VK_F20 = 0x83,
			VK_F21 = 0x84,
			VK_F22 = 0x85,
			VK_F23 = 0x86,
			VK_F24 = 0x87,
			VK_F3 = 0x72,
			VK_F4 = 0x73,
			VK_F5 = 0x74,
			VK_F6 = 0x75,
			VK_F7 = 0x76,
			VK_F8 = 0x77,
			VK_F9 = 120,
			VK_FINAL = 0x18,
			VK_HANGEUL = 0x15,
			VK_HANGUL = 0x15,
			VK_HANJA = 0x19,
			VK_HELP = 0x2f,
			VK_HOME = 0x24,
			VK_INSERT = 0x2d,
			VK_JUNJA = 0x17,
			VK_KANA = 0x15,
			VK_KANJI = 0x19,
			VK_LAUNCH_APP1 = 0xb6,
			VK_LAUNCH_APP2 = 0xb7,
			VK_LAUNCH_MAIL = 180,
			VK_LAUNCH_MEDIA_SELECT = 0xb5,
			VK_LBUTTON = 1,
			VK_LCONTROL = 0xa2,
			VK_LEFT = 0x25,
			VK_LMENU = 0xa4,
			VK_LSHIFT = 160,
			VK_LWIN = 0x5b,
			VK_MBUTTON = 4,
			VK_MEDIA_NEXT_TRACK = 0xb0,
			VK_MEDIA_PLAY_PAUSE = 0xb3,
			VK_MEDIA_PREV_TRACK = 0xb1,
			VK_MEDIA_STOP = 0xb2,
			VK_MENU = 0x12,
			VK_MODECHANGE = 0x1f,
			VK_MULTIPLY = 0x6a,
			VK_NEXT = 0x22,
			VK_NONCONVERT = 0x1d,
			VK_NUMLOCK = 0x90,
			VK_NUMPAD0 = 0x60,
			VK_NUMPAD1 = 0x61,
			VK_NUMPAD2 = 0x62,
			VK_NUMPAD3 = 0x63,
			VK_NUMPAD4 = 100,
			VK_NUMPAD5 = 0x65,
			VK_NUMPAD6 = 0x66,
			VK_NUMPAD7 = 0x67,
			VK_NUMPAD8 = 0x68,
			VK_NUMPAD9 = 0x69,
			VK_OEM_1 = 0xba,
			VK_OEM_2 = 0xbf,
			VK_OEM_3 = 0xc0,
			VK_OEM_4 = 0xdb,
			VK_OEM_5 = 220,
			VK_OEM_6 = 0xdd,
			VK_OEM_7 = 0xde,
			VK_OEM_8 = 0xdf,
			VK_OEM_COMMA = 0xbc,
			VK_OEM_MINUS = 0xbd,
			VK_OEM_PERIOD = 190,
			VK_OEM_PLUS = 0xbb,
			VK_PAUSE = 0x13,
			VK_PRINT = 0x2a,
			VK_PRIOR = 0x21,
			VK_RBUTTON = 2,
			VK_RCONTROL = 0xa3,
			VK_RETURN = 13,
			VK_RIGHT = 0x27,
			VK_RMENU = 0xa5,
			VK_RSHIFT = 0xa1,
			VK_RWIN = 0x5c,
			VK_SCROLL = 0x91,
			VK_SELECT = 0x29,
			VK_SEPARATOR = 0x6c,
			VK_SHIFT = 0x10,
			VK_SLEEP = 0x5f,
			VK_SNAPSHOT = 0x2c,
			VK_SPACE = 0x20,
			VK_SUBTRACT = 0x6d,
			VK_TAB = 9,
			VK_UP = 0x26,
			VK_VOLUME_DOWN = 0xae,
			VK_VOLUME_MUTE = 0xad,
			VK_VOLUME_UP = 0xaf,
			VK_XBUTTON1 = 5,
			VK_XBUTTON2 = 6
		}

		public enum WindowShowStyle
		{
			ForceMinimized = 11,
			Hide = 0,
			Maximize = 3,
			Minimize = 6,
			Restore = 9,
			Show = 5,
			ShowDefault = 10,
			ShowMaximized = 3,
			ShowMinimized = 2,
			ShowMinNoActivate = 7,
			ShowNoActivate = 8,
			ShowNormal = 1,
			ShowNormalNoActivate = 4
		}
	}
}

