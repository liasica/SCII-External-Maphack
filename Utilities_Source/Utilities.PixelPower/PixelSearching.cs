namespace Utilities.PixelPower
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using Utilities.ScreenShot;

	public class PixelSearching
	{
		public static Point PixelSearch(Bitmap bmp, Color PixelColor)
		{
			return PixelSearch(bmp, PixelColor, 0);
		}

		public static unsafe Point PixelSearch(Color PixelColor, int Shade_Variation)
		{
			Point point = new Point(-1, -1);
			Bitmap desktopImage = CaptureScreen.GetDesktopImage();
			BitmapData bitmapdata = desktopImage.LockBits(new Rectangle(0, 0, desktopImage.Width, desktopImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColor.B, PixelColor.G, PixelColor.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if ((((numPtr[j * 3] >= (numArray[0] - Shade_Variation)) & (numPtr[j * 3] <= (numArray[0] + Shade_Variation))) && ((numPtr[(j * 3) + 1] >= (numArray[1] - Shade_Variation)) & (numPtr[(j * 3) + 1] <= (numArray[1] + Shade_Variation)))) && ((numPtr[(j * 3) + 2] >= (numArray[2] - Shade_Variation)) & (numPtr[(j * 3) + 2] <= (numArray[2] + Shade_Variation))))
					{
						point = new Point(j, i);
						goto Label_0135;
					}
				}
			}
		Label_0135:
			desktopImage.UnlockBits(bitmapdata);
			return point;
		}

		public static Point PixelSearch(Bitmap bmp, Color PixelColor, Point point)
		{
			return PixelSearch(bmp, PixelColor, point.X, point.Y);
		}

		public static unsafe Point PixelSearch(Bitmap bmp, Color PixelColor, int Shade_Variation)
		{
			Point point = new Point(-1, -1);
			BitmapData bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColor.B, PixelColor.G, PixelColor.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if ((((numPtr[j * 3] >= (numArray[0] - Shade_Variation)) & (numPtr[j * 3] <= (numArray[0] + Shade_Variation))) && ((numPtr[(j * 3) + 1] >= (numArray[1] - Shade_Variation)) & (numPtr[(j * 3) + 1] <= (numArray[1] + Shade_Variation)))) && ((numPtr[(j * 3) + 2] >= (numArray[2] - Shade_Variation)) & (numPtr[(j * 3) + 2] <= (numArray[2] + Shade_Variation))))
					{
						point = new Point(j, i);
						goto Label_0129;
					}
				}
			}
		Label_0129:
			bmp.UnlockBits(bitmapdata);
			return point;
		}

		public static unsafe Point PixelSearch(Bitmap bmp, Color PixelColor, int notIncludedX, int notIncludedY)
		{
			Point point = new Point(-1, -1);
			BitmapData bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColor.B, PixelColor.G, PixelColor.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if ((((numPtr[j * 3] >= numArray[0]) & (numPtr[j * 3] <= numArray[0])) && ((numPtr[(j * 3) + 1] >= numArray[1]) & (numPtr[(j * 3) + 1] <= numArray[1]))) && ((numPtr[(j * 3) + 2] >= numArray[2]) & (numPtr[(j * 3) + 2] <= numArray[2])))
					{
						point = new Point(j, i);
						if (((point != new Point(1, 1)) && (point != new Point(0, 0))) && (((point != new Point(2, 0)) && (point != new Point(0, 2))) && (point != new Point(2, 2))))
						{
							goto Label_0169;
						}
					}
				}
			}
		Label_0169:
			bmp.UnlockBits(bitmapdata);
			return point;
		}

		public static unsafe Point PixelSearch(int px, int py, int pwidth, int pheight, Color PixelColor, int Shade_Variation)
		{
			Point point = new Point(-1, -1);
			Bitmap bitmap = CaptureScreen.GetDesktopImage(px, py, pwidth, pheight);
			BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColor.B, PixelColor.G, PixelColor.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if ((((numPtr[j * 3] >= (numArray[0] - Shade_Variation)) & (numPtr[j * 3] <= (numArray[0] + Shade_Variation))) && ((numPtr[(j * 3) + 1] >= (numArray[1] - Shade_Variation)) & (numPtr[(j * 3) + 1] <= (numArray[1] + Shade_Variation)))) && ((numPtr[(j * 3) + 2] >= (numArray[2] - Shade_Variation)) & (numPtr[(j * 3) + 2] <= (numArray[2] + Shade_Variation))))
					{
						point = new Point(j + px, i + py);
						goto Label_0143;
					}
				}
			}
		Label_0143:
			bitmap.UnlockBits(bitmapdata);
			return point;
		}
	}
}

