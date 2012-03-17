namespace _2cs_API
{
	using Data;
	using ScreenAPI;
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Threading;
	using Utilities.WinControl;
	using Utilities.WinControl.Mouse;

	public class clsMouse
	{
		private Point _clientRectStart;
		private IntPtr _hWnd;
		private bool _isFullWindowed;
		private int _message_delay = 5;
		private uint _MOUSEEVENTF_PRIMARYDOWN;
		private uint _MOUSEEVENTF_PRIMARYUP;
		private uint _MOUSEEVENTF_SECONDARYDOWN;
		private uint _MOUSEEVENTF_SECONDARYUP;
		private Utilities.WinControl.RECT _rctClient;
		private Utilities.WinControl.RECT _rctWindow;
		private const int WM_LBUTTONDBLCLK = 0x203;
		private const int WM_LBUTTONDOWN = 0x201;
		private const int WM_LBUTTONUP = 0x202;
		private const int WM_RBUTTONDBLCLK = 0x206;
		private const int WM_RBUTTONDOWN = 0x204;
		private const int WM_RBUTTONUP = 0x205;

		public clsMouse(IntPtr hWnd)
		{
			this._hWnd = hWnd;
			WC.GetWindowRect(hWnd, out this._rctWindow);
			WC.GetClientRect(hWnd, out this._rctClient);
			this._isFullWindowed = this._rctClient.Equals(this._rctWindow);
			if (WC.GetSystemMetrics(WC.SystemMetric.SM_SWAPBUTTON) == 0)
			{
				this._MOUSEEVENTF_PRIMARYDOWN = 2;
				this._MOUSEEVENTF_PRIMARYUP = 4;
				this._MOUSEEVENTF_SECONDARYDOWN = 8;
				this._MOUSEEVENTF_SECONDARYUP = 0x10;
			}
			else
			{
				this._MOUSEEVENTF_PRIMARYDOWN = 8;
				this._MOUSEEVENTF_PRIMARYUP = 0x10;
				this._MOUSEEVENTF_SECONDARYDOWN = 2;
				this._MOUSEEVENTF_SECONDARYUP = 4;
			}
			int x = this._rctWindow.Left + WC.GetSystemMetrics(WC.SystemMetric.SM_CXFRAME);
			int y = (this._rctWindow.Top + WC.GetSystemMetrics(WC.SystemMetric.SM_CYFRAME)) + WC.GetSystemMetrics(WC.SystemMetric.SM_CYCAPTION);
			this._clientRectStart = new Point(x, y);
		}

		private void Click(uint flags)
		{
			M.INPUT pInputs = new M.INPUT {
				type = M.SendInputEventType.InputMouse
			};
			pInputs.mkhi.mi.dx = 0;
			pInputs.mkhi.mi.dy = 0;
			pInputs.mkhi.mi.mouseData = 0;
			pInputs.mkhi.mi.dwFlags = flags;
			pInputs.mkhi.mi.time = 0;
			pInputs.mkhi.mi.dwExtraInfo = IntPtr.Zero;
			M.SendInput(1, ref pInputs, Marshal.SizeOf(pInputs));
			Thread.Sleep(100);
		}

		public void Drag(Point start, Point finish)
		{
			WC.SetForegroundWindow(this._hWnd);
			int lParam = (start.Y << 0x10) | start.X;
			SendMessage(this._hWnd, 0x201, 0, lParam);
			lParam = (finish.Y << 0x10) | finish.X;
			SendMessage(this._hWnd, 0x202, 0, lParam);
			Thread.Sleep(this._message_delay);
		}

		public void LeftClick(Point pos)
		{
			WC.SetForegroundWindow(this._hWnd);
			int lParam = (pos.Y << 0x10) | pos.X;
			SendMessage(this._hWnd, 0x201, 0, lParam);
			SendMessage(this._hWnd, 0x202, 0, lParam);
			Thread.Sleep(this._message_delay);
		}

		public void LeftDoubleClick(Point pos)
		{
			this.LeftClick(pos);
			this.LeftClick(pos);
		}

		public void Move(Unit u)
		{
			SmartClicker instance = SmartClicker.GetInstance(GameData.SC2Handle);
			if (!instance.CanClick((double) u.locationX, (double) u.locationY))
			{
				instance.MoveCamera((double) u.locationX, (double) u.locationY);
			}
			Point point = instance.Game2Screen((double) u.locationX, (double) u.locationY);
			this.Move(point.X, point.Y);
		}

		public void Move(double x, double y)
		{
			x *= this._rctClient.Right;
			y *= this._rctClient.Bottom;
			if (!this._isFullWindowed)
			{
				x += this._clientRectStart.X;
				y += this._clientRectStart.Y;
			}
			this.Move((int) x, (int) y);
		}

		public void Move(int x, int y)
		{
			Point point;
			M.SetCursorPos(x, y);
			M.GetCursorPos(out point);
			while ((point.X != x) && (point.Y != y))
			{
				M.SetCursorPos(x, y);
				Thread.Sleep(100);
			}
		}

		public void PrimaryClick()
		{
			this.Click(this._MOUSEEVENTF_PRIMARYDOWN | this._MOUSEEVENTF_PRIMARYUP);
		}

		public void PrimaryClick(Unit u)
		{
			SmartClicker.GetInstance(GameData.SC2Handle).GameLeftClick((double) u.locationX, (double) u.locationY, false);
		}

		public void PrimaryClick(double x, double y)
		{
			x *= this._rctClient.Right;
			y *= this._rctClient.Bottom;
			this.LeftClick(new Point((int) x, (int) y));
		}

		public void PrimaryClickDrag(double x1, double y1, double x2, double y2)
		{
			x1 *= this._rctClient.Right;
			y1 *= this._rctClient.Bottom;
			x2 *= this._rctClient.Right;
			y2 *= this._rctClient.Bottom;
			this.Drag(new Point((int) x1, (int) x2), new Point((int) y1, (int) y2));
		}

		public void PrimaryDoubleClick(double x, double y)
		{
			x *= this._rctClient.Right;
			y *= this._rctClient.Bottom;
			this.LeftDoubleClick(new Point((int) x, (int) y));
		}

		public void RightClick(Point pos)
		{
			WC.SetForegroundWindow(this._hWnd);
			int lParam = (pos.Y << 0x10) | pos.X;
			SendMessage(this._hWnd, 0x204, 0, lParam);
			SendMessage(this._hWnd, 0x205, 0, lParam);
			Thread.Sleep(this._message_delay);
		}

		public void SecondaryClick()
		{
			this.Click(this._MOUSEEVENTF_SECONDARYDOWN | this._MOUSEEVENTF_SECONDARYUP);
		}

		public void SecondaryClick(double x, double y)
		{
			x *= this._rctClient.Right;
			y *= this._rctClient.Bottom;
			this.RightClick(new Point((int) x, (int) y));
		}

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
		public void SetState(uint dwFlags)
		{
			M.mouse_event(dwFlags, 0, 0, 0, UIntPtr.Zero);
		}

		public PrecisionPoint Location
		{
			get
			{
				Point point;
				M.GetCursorPos(out point);
				if (this._isFullWindowed)
				{
					return new PrecisionPoint(((double) point.X) / ((double) this._rctClient.Width), ((double) point.Y) / ((double) this._rctClient.Height));
				}
				return new PrecisionPoint(((double) (point.X - this._clientRectStart.X)) / ((double) this._rctClient.Width), ((double) (point.Y - this._clientRectStart.Y)) / ((double) this._rctClient.Height));
			}
		}

		private enum ClickType
		{
			Primary,
			Secondary
		}
	}
}

