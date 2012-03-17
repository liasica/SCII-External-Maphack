namespace Utilities.WinControl
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;

	public class EnumWindows
	{
		private EnumWindowsCollection items;

		public void GetWindows()
		{
			this.items = new EnumWindowsCollection();
			UnManagedMethods.EnumWindows(new EnumWindowsProc(this.WindowEnum), 0);
		}

		public void GetWindows(IntPtr hWndParent)
		{
			this.items = new EnumWindowsCollection();
			UnManagedMethods.EnumChildWindows(hWndParent, new EnumWindowsProc(this.WindowEnum), 0);
		}

		protected virtual bool OnWindowEnum(IntPtr hWnd)
		{
			this.items.Add(hWnd);
			return true;
		}

		private int WindowEnum(IntPtr hWnd, int lParam)
		{
			if (this.OnWindowEnum(hWnd))
			{
				return 1;
			}
			return 0;
		}

		public EnumWindowsCollection Items
		{
			get
			{
				return this.items;
			}
		}

		private delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

		private class UnManagedMethods
		{
			[DllImport("user32")]
			public static extern int EnumChildWindows(IntPtr hWndParent, Utilities.WinControl.EnumWindows.EnumWindowsProc lpEnumFunc, int lParam);
			[DllImport("user32")]
			public static extern int EnumWindows(Utilities.WinControl.EnumWindows.EnumWindowsProc lpEnumFunc, int lParam);
		}
	}
}

