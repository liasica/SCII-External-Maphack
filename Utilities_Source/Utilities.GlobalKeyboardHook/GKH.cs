namespace Utilities.GlobalKeyboardHook
{
	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Windows.Forms;

	public class GKH
	{
		private KeyEventHandler KeyDown;
		private KeyPressEventHandler KeyPress;
		private KeyEventHandler KeyUp;
		private bool m_bHookActive;
		protected HookProc m_hookproc;
		private int m_iHandleToHook;
		private const byte VK_CAPITAL = 20;
		private const byte VK_LALT = 0xa4;
		private const byte VK_LCONTROL = 0xa2;
		private const byte VK_LSHIFT = 160;
		private const byte VK_NUMLOCK = 0x90;
		private const byte VK_RALT = 0xa5;
		private const byte VK_RCONTROL = 3;
		private const byte VK_RSHIFT = 0xa1;
		private const byte VK_SHIFT = 0x10;
		private const int WH_KEYBOARD = 2;
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYDOWN = 260;
		private const int WM_SYSKEYUP = 0x105;

		public event KeyEventHandler eKeyDown
		{
			add
			{
				KeyEventHandler handler2;
				KeyEventHandler keyDown = this.KeyDown;
				do
				{
					handler2 = keyDown;
					KeyEventHandler handler3 = (KeyEventHandler) Delegate.Combine(handler2, value);
					keyDown = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyDown, handler3, handler2);
				}
				while (keyDown != handler2);
			}
			remove
			{
				KeyEventHandler handler2;
				KeyEventHandler keyDown = this.KeyDown;
				do
				{
					handler2 = keyDown;
					KeyEventHandler handler3 = (KeyEventHandler) Delegate.Remove(handler2, value);
					keyDown = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyDown, handler3, handler2);
				}
				while (keyDown != handler2);
			}
		}

		public event KeyPressEventHandler eKeyPress
		{
			add
			{
				KeyPressEventHandler handler2;
				KeyPressEventHandler keyPress = this.KeyPress;
				do
				{
					handler2 = keyPress;
					KeyPressEventHandler handler3 = (KeyPressEventHandler) Delegate.Combine(handler2, value);
					keyPress = Interlocked.CompareExchange<KeyPressEventHandler>(ref this.KeyPress, handler3, handler2);
				}
				while (keyPress != handler2);
			}
			remove
			{
				KeyPressEventHandler handler2;
				KeyPressEventHandler keyPress = this.KeyPress;
				do
				{
					handler2 = keyPress;
					KeyPressEventHandler handler3 = (KeyPressEventHandler) Delegate.Remove(handler2, value);
					keyPress = Interlocked.CompareExchange<KeyPressEventHandler>(ref this.KeyPress, handler3, handler2);
				}
				while (keyPress != handler2);
			}
		}

		public event KeyEventHandler eKeyUp
		{
			add
			{
				KeyEventHandler handler2;
				KeyEventHandler keyUp = this.KeyUp;
				do
				{
					handler2 = keyUp;
					KeyEventHandler handler3 = (KeyEventHandler) Delegate.Combine(handler2, value);
					keyUp = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyUp, handler3, handler2);
				}
				while (keyUp != handler2);
			}
			remove
			{
				KeyEventHandler handler2;
				KeyEventHandler keyUp = this.KeyUp;
				do
				{
					handler2 = keyUp;
					KeyEventHandler handler3 = (KeyEventHandler) Delegate.Remove(handler2, value);
					keyUp = Interlocked.CompareExchange<KeyEventHandler>(ref this.KeyUp, handler3, handler2);
				}
				while (keyUp != handler2);
			}
		}

		[DllImport("user32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto)]
		private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
		~GKH()
		{
			this.Unhook();
		}

		[DllImport("user32.dll")]
		private static extern int GetKeyboardState(byte[] pbKeyState);
		[DllImport("user32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto)]
		private static extern short GetKeyState(int vKey);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);
		public bool Hook()
		{
			if (!this.m_bHookActive)
			{
				this.m_hookproc = new HookProc(this.HookCallbackProcedure);
				IntPtr moduleHandle = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
				this.m_iHandleToHook = SetWindowsHookEx(13, this.m_hookproc, moduleHandle, 0);
				if (this.m_iHandleToHook != 0)
				{
					this.m_bHookActive = true;
				}
			}
			return this.m_bHookActive;
		}

		private int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
		{
			bool handled = false;
			if ((nCode > -1) && (((this.KeyDown != null) || (this.KeyUp != null)) || (this.KeyPress != null)))
			{
				KeyboardHookStruct struct2 = (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
				bool flag2 = ((GetKeyState(0xa2) & 0x80) != 0) || ((GetKeyState(3) & 0x80) != 0);
				bool flag3 = ((GetKeyState(160) & 0x80) != 0) || ((GetKeyState(0xa1) & 0x80) != 0);
				bool flag4 = ((GetKeyState(0xa4) & 0x80) != 0) || ((GetKeyState(0xa5) & 0x80) != 0);
				bool flag5 = GetKeyState(20) != 0;
				KeyEventArgs kea = new KeyEventArgs(((((Keys) struct2.vkCode) | (flag2 ? Keys.Control : Keys.None)) | (flag3 ? Keys.Shift : Keys.None)) | (flag4 ? Keys.Alt : Keys.None));
				if ((wParam == 0x100) || (wParam == 260))
				{
					this.OnKeyDown(kea);
					handled = kea.Handled;
				}
				else if ((wParam == 0x101) || (wParam == 0x105))
				{
					this.OnKeyUp(kea);
					handled = kea.Handled;
				}
				if (((wParam == 0x100) && !handled) && !kea.SuppressKeyPress)
				{
					byte[] pbKeyState = new byte[0x100];
					byte[] lpwTransKey = new byte[2];
					GetKeyboardState(pbKeyState);
					if (ToAscii(struct2.vkCode, struct2.scanCode, pbKeyState, lpwTransKey, struct2.flags) == 1)
					{
						char c = (char) lpwTransKey[0];
						if ((flag5 ^ flag3) && char.IsLetter(c))
						{
							c = char.ToUpper(c);
						}
						KeyPressEventArgs kpea = new KeyPressEventArgs(c);
						this.OnKeyPress(kpea);
						handled = kea.Handled;
					}
				}
			}
			if (handled)
			{
				return 1;
			}
			return CallNextHookEx(this.m_iHandleToHook, nCode, wParam, lParam);
		}

		protected virtual void OnKeyDown(KeyEventArgs kea)
		{
			if (this.KeyDown != null)
			{
				this.KeyDown(this, kea);
			}
		}

		protected virtual void OnKeyPress(KeyPressEventArgs kpea)
		{
			if (this.KeyPress != null)
			{
				this.KeyPress(this, kpea);
			}
		}

		protected virtual void OnKeyUp(KeyEventArgs kea)
		{
			if (this.KeyUp != null)
			{
				this.KeyUp(this, kea);
			}
		}

		[DllImport("user32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto, SetLastError=true)]
		private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);
		[DllImport("user32.dll")]
		private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
		public void Unhook()
		{
			if (this.m_bHookActive)
			{
				UnhookWindowsHookEx(this.m_iHandleToHook);
				this.m_bHookActive = false;
			}
		}

		[DllImport("user32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto, SetLastError=true)]
		private static extern int UnhookWindowsHookEx(int idHook);

		public bool HookActive
		{
			get
			{
				return this.m_bHookActive;
			}
		}

		protected delegate int HookProc(int nCode, int wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential)]
		private class KeyboardHookStruct
		{
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}
	}
}

