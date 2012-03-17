namespace Utilities.ScreenShot
{
	using System;
	using System.Runtime.InteropServices;

	internal class PlatformInvokeGDI32
	{
		public const int SRCCOPY = 0xcc0020;

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteDC(IntPtr hDc);
		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteObject(IntPtr hDc);
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
	}
}

