namespace Utilities.ScreenShot
{
	using System;
	using System.Runtime.InteropServices;

	internal class PlatformInvokeUSER32
	{
		public const int SM_CXSCREEN = 0;
		public const int SM_CYSCREEN = 1;

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr ptr);
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int abc);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(int ptr);
		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
	}
}

