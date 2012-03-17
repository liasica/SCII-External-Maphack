namespace ScreenAPI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Windows.Forms;
	using Utilities.MemoryHandling;
	using Utilities.WinControl;
	using Data;

	public class Utility
	{
		protected uint Camera = GameData.Pointers.Camera;
		protected double camera_angle = 0.97738438111682457;
		protected dPoint camera_center = new dPoint();
		protected double camera_dist = 34.0;
		protected uint CameraInfo = (GameData.Pointers.Camera - 0x19c);
		protected double Cood2PixelRatio;
		public bool debug;
		protected ScreenAPI.Margins margins = new ScreenAPI.Margins();
		protected ReadWriteMemory mem;
		private int message_delay = 5;
		public const int MOUSEEVENTF_LEFTDOWN = 2;
		public const int MOUSEEVENTF_LEFTUP = 4;
		public const int MOUSEEVENTF_RIGHTDOWN = 8;
		public const int MOUSEEVENTF_RIGHTUP = 0x10;
		private Point OldPos;
		private IntPtr OldWindow;
		protected IntPtr SC2hwnd;
		protected static List<ScreenSize> supported = new List<ScreenSize> { (ScreenSize)0, (ScreenSize)3, (ScreenSize)4, (ScreenSize)8,
			(ScreenSize)9, (ScreenSize)10, (ScreenSize)11, (ScreenSize)14, (ScreenSize)0x10, (ScreenSize)0x11, (ScreenSize)0x12 };
		private const int WM_CHAR = 0x105;
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_LBUTTONDBLCLK = 0x203;
		private const int WM_LBUTTONDOWN = 0x201;
		private const int WM_LBUTTONUP = 0x202;
		private const int WM_RBUTTONDBLCLK = 0x206;
		private const int WM_RBUTTONDOWN = 0x204;
		private const int WM_RBUTTONUP = 0x205;
		private const int WM_SYSKEYDOWN = 260;
		private const int WM_SYSKEYUP = 0x105;

		protected Utility(IntPtr hwnd, ScreenSize size)
		{
			this.SC2hwnd = hwnd;
			this.mem = new ReadWriteMemory(this.SC2hwnd);
			this.ChangeSize(size);
			this.debug = /*Debugger.IsAttached*/false;
		}

		public bool CanClick(double x, double y)
		{
			Point point = this.Game2Screen(x, y);
			return (((point.X >= this.margins.left) && (point.X <= this.margins.right)) && ((point.Y >= this.margins.top) && (point.Y <= this.margins.bot)));
		}

		public virtual bool ChangeSize(ScreenSize size)
		{
			if (!supported.Contains(size))
			{
				return false;
			}
			ResEnumHelper helper = new ResEnumHelper(size);
			int num = helper.getWidth();
			int num2 = helper.getHeight();
			int num3 = 15;
			switch (size)
			{
				case ScreenSize.Size1024x768:
					this.margins.top = 30;
					this.margins.bot = 500;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = 512.0;
					this.camera_center.Y = 380.0;
					this.Cood2PixelRatio = 45.91928251121076;
					break;

				case ScreenSize.Size1600x1200:
					this.margins.top = 40;
					this.margins.bot = 840;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = num / 2;
					this.camera_center.Y = num2 / 2;
					this.Cood2PixelRatio = ((double) num) / 22.2;
					break;

				case ScreenSize.Size1280x1024:
					this.margins.top = 30;
					this.margins.bot = 0x2e5;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = num / 2;
					this.camera_center.Y = 509.0;
					this.Cood2PixelRatio = ((double) num) / 20.915;
					break;

				case ScreenSize.Size1280x800:
					this.margins.top = 30;
					this.margins.bot = 0x20d;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = 640.0;
					this.camera_center.Y = 397.0;
					this.Cood2PixelRatio = 47.832585949177876;
					break;

				case ScreenSize.Size1440x900:
					this.margins.top = 30;
					this.margins.bot = 0x253;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = 720.0;
					this.camera_center.Y = 450.0;
					this.Cood2PixelRatio = 53.731343283582085;
					break;

				case ScreenSize.Size1680x1050:
					this.margins.top = 0x21;
					this.margins.bot = 700;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = num / 2;
					this.camera_center.Y = 525.0;
					this.Cood2PixelRatio = ((double) num) / 26.8;
					break;

				case ScreenSize.Size1920x1200:
					this.margins.top = 0x25;
					this.margins.bot = 790;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = num / 2;
					this.camera_center.Y = num2 / 2;
					this.Cood2PixelRatio = ((double) num) / 26.5;
					break;

				case ScreenSize.Size1366x768:
					this.margins.top = 30;
					this.margins.bot = 0x20d;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = num / 2;
					this.camera_center.Y = num2 / 2;
					this.Cood2PixelRatio = ((double) num) / 26.6;
					break;

				case ScreenSize.Size1280x720:
					this.margins.top = 30;
					this.margins.bot = 0x1db;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = 640.0;
					this.camera_center.Y = 358.0;
					this.Cood2PixelRatio = 42.809364548494983;
					break;

				case ScreenSize.Size1600x900:
					this.margins.top = 30;
					this.margins.bot = 600;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = 800.0;
					this.camera_center.Y = 449.0;
					this.Cood2PixelRatio = 53.511705685618729;
					break;

				case ScreenSize.Size1920x1080:
					this.margins.top = 0x21;
					this.margins.bot = 0x2f4;
					this.margins.left = num3;
					this.margins.right = num - num3;
					this.camera_center.X = num / 2;
					this.camera_center.Y = 538.0;
					this.Cood2PixelRatio = ((double) num) / 29.85;
					break;

				default:
					throw new Exception("Selected Resolution Not Yet Supported");
			}
			return true;
		}

		public void Drag(Point start, Point finish)
		{
			SetForegroundWindow(this.SC2hwnd);
			if (this.debug)
			{
				SetCursorPos(start.X, start.Y);
			}
			int lParam = (start.Y << 0x10) | start.X;
			SendMessage(this.SC2hwnd, 0x201, 0, lParam);
			Thread.Sleep(this.message_delay);
			if (this.debug)
			{
				SetCursorPos(finish.X, finish.Y);
			}
			lParam = (finish.Y << 0x10) | finish.X;
			SendMessage(this.SC2hwnd, 0x202, 0, lParam);
			Thread.Sleep(this.message_delay);
		}

		public Point Game2Screen(double x, double y)
		{
			double num6;
			double num7;
			double num8;
			double num9;
			int num = (int) this.mem.ReadMemory(this.CameraInfo, typeof(int));
			double num2 = ((double) num) / 4096.0;
			num = (int) this.mem.ReadMemory((uint) (this.CameraInfo + 4), typeof(int));
			double num3 = ((double) num) / 4096.0;
			double num4 = x - num2;
			double num5 = y - num3;
			bool flag = num4 < 0.0;
			if (flag)
			{
				num4 *= -1.0;
			}
			if (num5 >= 0.0)
			{
				num8 = Math.Sqrt((Math.Pow(num5, 2.0) + Math.Pow(this.camera_dist, 2.0)) - (((2.0 * num5) * this.camera_dist) * Math.Cos(3.1415926535897931 - this.camera_angle)));
				num9 = Math.Asin((Math.Sin(3.1415926535897931 - this.camera_angle) / num8) * num5);
				num7 = this.camera_dist * Math.Tan(num9);
				double num10 = Math.Sqrt(Math.Pow(this.camera_dist, 2.0) + Math.Pow(num7, 2.0));
				num6 = (num4 / num8) * num10;
				return new Point((int) ((((flag ? ((double) (-1)) : ((double) 1)) * num6) * this.Cood2PixelRatio) + this.camera_center.X), (int) ((-num7 * this.Cood2PixelRatio) + this.camera_center.Y));
			}
			num5 *= -1.0;
			num8 = Math.Sqrt((Math.Pow(num5, 2.0) + Math.Pow(this.camera_dist, 2.0)) - (((2.0 * num5) * this.camera_dist) * Math.Cos(this.camera_angle)));
			num9 = Math.Asin((Math.Sin(this.camera_angle) / num8) * num5);
			num7 = this.camera_dist * Math.Tan(num9);
			num6 = ((this.camera_dist / Math.Cos(num9)) / num8) * num4;
			return new Point((int) ((((flag ? ((double) (-1)) : ((double) 1)) * num6) * this.Cood2PixelRatio) + this.camera_center.X), (int) ((num7 * this.Cood2PixelRatio) + this.camera_center.Y));
		}

		public void GameLeftClick(dPoint p, bool doubleclick = false)
		{
			this.GameLeftClick(p.X, p.Y, doubleclick);
		}

		public void GameLeftClick(double x, double y, bool doubleclick = false)
		{
			Point pos = this.Game2Screen(x, y);
			if (((pos.X < this.margins.left) || (pos.X > this.margins.right)) || ((pos.Y < this.margins.top) || (pos.Y > this.margins.bot)))
			{
				this.MoveCamera(x, y);
				pos = this.Game2Screen(x, y);
			}
			if (doubleclick)
			{
				this.LeftDblClick(pos);
			}
			else
			{
				this.LeftClick(pos);
			}
		}

		public void GameRightClick(dPoint p, bool doubleclick = false)
		{
			this.GameRightClick(p.X, p.Y, doubleclick);
		}

		public void GameRightClick(Point p, bool doubleclick = false)
		{
			this.GameRightClick(new dPoint((double) p.X, (double) p.Y), doubleclick);
		}

		public void GameRightClick(double x, double y, bool doubleclick = false)
		{
			Point pos = this.Game2Screen(x, y);
			if (((pos.X < this.margins.left) || (pos.X > this.margins.right)) || ((pos.Y < this.margins.top) || (pos.Y > this.margins.bot)))
			{
				this.MoveCamera(x, y);
				pos = this.Game2Screen(x, y);
			}
			if (doubleclick)
			{
				this.RightDblClick(pos);
			}
			else
			{
				this.RightClick(pos);
			}
		}

		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Point pt);
		[DllImport("user32.dll")]
		private static extern IntPtr GetFocus();
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		public void LeftClick()
		{
			Point point;
			GetCursorPos(out point);
			Size size = WC.winGetClientSize(this.SC2hwnd);
			if (point.X > size.Width)
			{
				point.X -= size.Width;
			}
			if (((point.X >= this.margins.left) && (point.X <= this.margins.right)) && ((point.Y >= this.margins.top) && (point.Y <= this.margins.bot)))
			{
				SetForegroundWindow(this.SC2hwnd);
				if (this.debug)
				{
					SetCursorPos(point.X, point.Y);
				}
				int lParam = (point.Y << 0x10) | point.X;
				SendMessage(this.SC2hwnd, 0x201, 0, lParam);
				SendMessage(this.SC2hwnd, 0x202, 0, lParam);
				Thread.Sleep(this.message_delay);
			}
		}

		public void LeftClick(Point pos)
		{
			this.LeftClick(pos.X, pos.Y);
		}

		public void LeftClick(int x, int y)
		{
			SetForegroundWindow(this.SC2hwnd);
			if (this.debug)
			{
				SetCursorPos(x, y);
			}
			int lParam = (y << 0x10) | x;
			SendMessage(this.SC2hwnd, 0x201, 0, lParam);
			SendMessage(this.SC2hwnd, 0x202, 0, lParam);
			Thread.Sleep(this.message_delay);
		}

		public void LeftDblClick(Point pos)
		{
			SetForegroundWindow(this.SC2hwnd);
			if (this.debug)
			{
				SetCursorPos(pos.X, pos.Y);
			}
			int lParam = (pos.Y << 0x10) | pos.X;
			SendMessage(this.SC2hwnd, 0x201, 0, lParam);
			SendMessage(this.SC2hwnd, 0x202, 0, lParam);
			SendMessage(this.SC2hwnd, 0x201, 0, lParam);
			SendMessage(this.SC2hwnd, 0x202, 0, lParam);
			Thread.Sleep(this.message_delay);
		}

		[DllImport("user32.dll")]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
		public virtual void MoveCamera(dPoint p)
		{
			this.MoveCamera(p.X, p.Y);
		}

		public virtual void MoveCamera(double x, double y)
		{
			uint num = (uint) this.mem.ReadMemory(this.Camera, typeof(uint));
			int num2 = (int) ((12058619.84 * Math.Log(x * 4096.0)) + 964719316.6);
			int num3 = (int) ((12058619.84 * Math.Log(y * 4096.0)) + 964719316.6);
			byte[] bytes = BitConverter.GetBytes(num2);
			byte[] lpBuffer = BitConverter.GetBytes(num3);
			this.mem.WriteMemory(new IntPtr((long) (num + 0x224)), bytes.Length, ref bytes);
			this.mem.WriteMemory(new IntPtr((long) (num + 560)), bytes.Length, ref bytes);
			this.mem.WriteMemory(new IntPtr((long) (num + 0x228)), bytes.Length, ref lpBuffer);
			this.mem.WriteMemory(new IntPtr((long) (num + 0x234)), bytes.Length, ref lpBuffer);
			Thread.Sleep(100);
		}

		public void RightClick(Point pos)
		{
			SetForegroundWindow(this.SC2hwnd);
			if (this.debug)
			{
				SetCursorPos(pos.X, pos.Y);
			}
			int lParam = (pos.Y << 0x10) | pos.X;
			SendMessage(this.SC2hwnd, 0x204, 0, lParam);
			SendMessage(this.SC2hwnd, 0x205, 0, lParam);
			Thread.Sleep(this.message_delay);
		}

		public void RightDblClick(Point pos)
		{
			SetForegroundWindow(this.SC2hwnd);
			if (this.debug)
			{
				SetCursorPos(pos.X, pos.Y);
			}
			int lParam = (pos.Y << 0x10) | pos.X;
			SendMessage(this.SC2hwnd, 0x204, 0, lParam);
			SendMessage(this.SC2hwnd, 0x205, 0, lParam);
			SendMessage(this.SC2hwnd, 0x204, 0, lParam);
			SendMessage(this.SC2hwnd, 0x205, 0, lParam);
			Thread.Sleep(this.message_delay);
		}

		public void SendKey(Keys key)
		{
			this.OldWindow = GetFocus();
			SetFocus(this.SC2hwnd);
			SendMessage(this.SC2hwnd, 0x100, Convert.ToInt32(key), 0);
			SendMessage(this.SC2hwnd, 0x101, Convert.ToInt32(key), 0);
			SetFocus(this.OldWindow);
		}

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		private static extern bool SetCursorPos(int X, int Y);
		[DllImport("user32")]
		internal static extern IntPtr SetFocus(IntPtr hwnd);
		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		public ScreenAPI.Margins Margins
		{
			get
			{
				return this.margins;
			}
		}

		private class ResEnumHelper
		{
			private ScreenSize ss;

			public ResEnumHelper(ScreenSize ss)
			{
				this.ss = ss;
			}

			public int getHeight()
			{
				string name = Enum.GetName(typeof(ScreenSize), this.ss);
				int startIndex = name.IndexOf("x") + 1;
				string s = name.Substring(startIndex, name.Length - startIndex);
				int result = 0;
				int.TryParse(s, out result);
				return result;
			}

			public float getRatio()
			{
				return (float) (this.getHeight() / this.getWidth());
			}

			public int getWidth()
			{
				string name = Enum.GetName(typeof(ScreenSize), this.ss);
				string s = name.Substring(0, name.IndexOf("x")).Replace("Size", "");
				int result = 0;
				int.TryParse(s, out result);
				return result;
			}
		}
	}
}

