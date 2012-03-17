namespace Utilities.PixelPower
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;

	public class FastBitmap
	{
		private BitmapData bitmapData;
		private unsafe byte* pBase = null;
		private unsafe PixelData* pixelData = null;
		private int width;
		private Bitmap workingBitmap;

		public FastBitmap(Bitmap inputBitmap)
		{
			this.workingBitmap = inputBitmap;
		}

		public unsafe Color GetPixel(int x, int y)
		{
			this.pixelData = (PixelData*) ((this.pBase + (y * this.width)) + (x * sizeof(PixelData)));
			return Color.FromArgb(this.pixelData->alpha, this.pixelData->red, this.pixelData->green, this.pixelData->blue);
		}

		public unsafe Color GetPixelNext()
		{
			this.pixelData++;
			return Color.FromArgb(this.pixelData->alpha, this.pixelData->red, this.pixelData->green, this.pixelData->blue);
		}

		public unsafe void LockImage()
		{
			Rectangle rect = new Rectangle(Point.Empty, this.workingBitmap.Size);
			this.width = rect.Width * sizeof(PixelData);
			if ((this.width % 4) != 0)
			{
				this.width = 4 * ((this.width / 4) + 1);
			}
			this.bitmapData = this.workingBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			this.pBase = (byte*) this.bitmapData.Scan0.ToPointer();
		}

		public unsafe void SetPixel(int x, int y, Color color)
		{
			PixelData* dataPtr = (PixelData*) ((this.pBase + (y * this.width)) + (x * sizeof(PixelData)));
			dataPtr->alpha = color.A;
			dataPtr->red = color.R;
			dataPtr->green = color.G;
			dataPtr->blue = color.B;
		}

		public unsafe void UnlockImage()
		{
			this.workingBitmap.UnlockBits(this.bitmapData);
			this.bitmapData = null;
			this.pBase = null;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PixelData
		{
			public byte blue;
			public byte green;
			public byte red;
			public byte alpha;
			public override string ToString()
			{
				return ("(" + this.alpha.ToString() + ", " + this.red.ToString() + ", " + this.green.ToString() + ", " + this.blue.ToString() + ")");
			}
		}
	}
}

