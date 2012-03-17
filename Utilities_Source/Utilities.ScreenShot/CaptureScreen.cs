namespace Utilities.ScreenShot
{
	using System;
	using System.Drawing;
	using Utilities.WinControl;

	public static class CaptureScreen
	{
		public static Bitmap GetDesktopImage()
		{
			SIZE size;
			IntPtr dC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow());
			IntPtr hdc = PlatformInvokeGDI32.CreateCompatibleDC(dC);
			size.cx = PlatformInvokeUSER32.GetSystemMetrics(0);
			size.cy = PlatformInvokeUSER32.GetSystemMetrics(1);
			IntPtr bmp = PlatformInvokeGDI32.CreateCompatibleBitmap(dC, size.cx, size.cy);
			if (bmp != IntPtr.Zero)
			{
				IntPtr ptr4 = PlatformInvokeGDI32.SelectObject(hdc, bmp);
				PlatformInvokeGDI32.BitBlt(hdc, 0, 0, size.cx, size.cy, dC, 0, 0, 0xcc0020);
				PlatformInvokeGDI32.SelectObject(hdc, ptr4);
				PlatformInvokeGDI32.DeleteDC(hdc);
				PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), dC);
				Bitmap bitmap = Image.FromHbitmap(bmp);
				PlatformInvokeGDI32.DeleteObject(bmp);
				GC.Collect();
				return bitmap;
			}
			return null;
		}

		public static Bitmap GetDesktopImage(int x, int y, int width, int height)
		{
			IntPtr dC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow());
			IntPtr hdc = PlatformInvokeGDI32.CreateCompatibleDC(dC);
			IntPtr bmp = PlatformInvokeGDI32.CreateCompatibleBitmap(dC, width, height);
			if (bmp != IntPtr.Zero)
			{
				IntPtr ptr4 = PlatformInvokeGDI32.SelectObject(hdc, bmp);
				PlatformInvokeGDI32.BitBlt(hdc, 0, 0, width, height, dC, x, y, 0xcc0020);
				PlatformInvokeGDI32.SelectObject(hdc, ptr4);
				PlatformInvokeGDI32.DeleteDC(hdc);
				PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), dC);
				Bitmap bitmap = Image.FromHbitmap(bmp);
				PlatformInvokeGDI32.DeleteObject(bmp);
				GC.Collect();
				return bitmap;
			}
			return null;
		}

		public static Bitmap GetWindowImage(IntPtr hwnd)
		{
			IntPtr dC = PlatformInvokeUSER32.GetDC(hwnd);
			IntPtr hdc = PlatformInvokeGDI32.CreateCompatibleDC(dC);
			Size size = WC.winGetClientSize(hwnd);
			int width = size.Width;
			int height = size.Height;
			IntPtr bmp = PlatformInvokeGDI32.CreateCompatibleBitmap(dC, width, height);
			if (bmp != IntPtr.Zero)
			{
				IntPtr ptr4 = PlatformInvokeGDI32.SelectObject(hdc, bmp);
				PlatformInvokeGDI32.BitBlt(hdc, 0, 0, width, height, dC, 0, 0, 0xcc0020);
				PlatformInvokeGDI32.SelectObject(hdc, ptr4);
				PlatformInvokeGDI32.DeleteDC(hdc);
				PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), dC);
				Bitmap bitmap = Image.FromHbitmap(bmp);
				PlatformInvokeGDI32.DeleteObject(bmp);
				GC.Collect();
				return bitmap;
			}
			return null;
		}

		public static Bitmap GetWindowImage(IntPtr hwnd, int x, int y, int width, int height)
		{
			IntPtr dC = PlatformInvokeUSER32.GetDC(hwnd);
			IntPtr hdc = PlatformInvokeGDI32.CreateCompatibleDC(dC);
			IntPtr bmp = PlatformInvokeGDI32.CreateCompatibleBitmap(dC, width, height);
			if (bmp != IntPtr.Zero)
			{
				IntPtr ptr4 = PlatformInvokeGDI32.SelectObject(hdc, bmp);
				PlatformInvokeGDI32.BitBlt(hdc, 0, 0, width, height, dC, x, y, 0xcc0020);
				PlatformInvokeGDI32.SelectObject(hdc, ptr4);
				PlatformInvokeGDI32.DeleteDC(hdc);
				PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), dC);
				Bitmap bitmap = Image.FromHbitmap(bmp);
				PlatformInvokeGDI32.DeleteObject(bmp);
				GC.Collect();
				return bitmap;
			}
			return null;
		}
	}
}

