namespace Utilities.PixelPower
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	public class PixelTools
	{
		public static Bitmap CropBitmap(Bitmap bitmap, Rectangle rect)
		{
			return bitmap.Clone(rect, bitmap.PixelFormat);
		}

		public static Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
		{
			Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
			return bitmap.Clone(rect, bitmap.PixelFormat);
		}

		public static Bitmap FixedSize(Image imgPhoto, int Width, int Height)
		{
			int width = imgPhoto.Width;
			int height = imgPhoto.Height;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			num4 = ((float) Width) / ((float) width);
			num5 = ((float) Height) / ((float) height);
			if (num5 < num4)
			{
				num3 = num5;
			}
			else
			{
				num3 = num4;
			}
			int num6 = (int) (width * num3);
			int num7 = (int) (height * num3);
			Bitmap image = new Bitmap(num6, num7);
			Graphics graphics = Graphics.FromImage(image);
			graphics.DrawImage(imgPhoto, new Rectangle(0, 0, num6, num7), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
			graphics.Dispose();
			return image;
		}

		public static Image InvertImage(Image originalImg)
		{
			Bitmap bitmap = null;
			using (Bitmap bitmap2 = new Bitmap(originalImg))
			{
				bitmap = new Bitmap(bitmap2.Width, bitmap2.Height);
				for (int i = 0; i < bitmap2.Width; i++)
				{
					for (int j = 0; j < bitmap2.Height; j++)
					{
						Color pixel = bitmap2.GetPixel(i, j);
						pixel = Color.FromArgb(0xff - pixel.R, 0xff - pixel.G, 0xff - pixel.B);
						bitmap.SetPixel(i, j, pixel);
					}
				}
			}
			return bitmap;
		}

		public static Image InvertImageColorMatrix(Image originalImg)
		{
			Bitmap image = new Bitmap(originalImg.Width, originalImg.Height);
			float[][] newColorMatrix = new float[5][];
			float[] numArray2 = new float[5];
			numArray2[0] = -1f;
			newColorMatrix[0] = numArray2;
			float[] numArray3 = new float[5];
			numArray3[1] = -1f;
			newColorMatrix[1] = numArray3;
			float[] numArray4 = new float[5];
			numArray4[2] = -1f;
			newColorMatrix[2] = numArray4;
			float[] numArray5 = new float[5];
			numArray5[3] = 1f;
			newColorMatrix[3] = numArray5;
			newColorMatrix[4] = new float[] { 1f, 1f, 1f, 0f, 1f };
			ColorMatrix matrix = new ColorMatrix(newColorMatrix);
			using (ImageAttributes attributes = new ImageAttributes())
			{
				attributes.SetColorMatrix(matrix);
				using (Graphics graphics = Graphics.FromImage(image))
				{
					graphics.DrawImage(originalImg, new Rectangle(0, 0, originalImg.Width, originalImg.Height), 0, 0, originalImg.Width, originalImg.Height, GraphicsUnit.Pixel, attributes);
				}
			}
			return image;
		}

		public static unsafe Image InvertImageUnsafe(Image originalImg)
		{
			Bitmap bitmap = new Bitmap(originalImg);
			BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			int stride = bitmapdata.Stride;
			byte* numPtr = (byte*) bitmapdata.Scan0;
			int num2 = stride - (bitmap.Width * 4);
			int width = bitmap.Width;
			for (int i = 0; i < bitmap.Height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					numPtr[0] = (byte) (0xff - numPtr[0]);
					numPtr[1] = (byte) (0xff - numPtr[1]);
					numPtr[2] = (byte) (0xff - numPtr[2]);
					numPtr += 4;
				}
				numPtr += num2;
			}
			bitmap.UnlockBits(bitmapdata);
			return bitmap;
		}

		public static bool isColorMatch(Color color1, Color color2, int tolerance)
		{
			return (((Math.Abs((int) (color1.R - color2.R)) <= tolerance) && (Math.Abs((int) (color1.G - color2.G)) <= tolerance)) && (Math.Abs((int) (color1.B - color2.B)) <= tolerance));
		}

		public static unsafe Bitmap KeepAndReplace(Bitmap img, Color PixelColorToKeep, Color PixelColorNew)
		{
			Bitmap bitmap = img;
			BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColorToKeep.B, PixelColorToKeep.G, PixelColorToKeep.R };
			int[] numArray2 = new int[] { PixelColorNew.B, PixelColorNew.G, PixelColorNew.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if (((numPtr[j * 3] != numArray[0]) || (numPtr[(j * 3) + 1] != numArray[1])) || (numPtr[(j * 3) + 2] != numArray[2]))
					{
						numPtr[j * 3] = Convert.ToByte(numArray2[0]);
						numPtr[(j * 3) + 1] = Convert.ToByte(numArray2[1]);
						numPtr[(j * 3) + 2] = Convert.ToByte(numArray2[2]);
					}
				}
			}
			bitmap.UnlockBits(bitmapdata);
			return bitmap;
		}

		public static unsafe Bitmap KeepAndReplace(Bitmap img, Color PixelColorToKeep, Color PixelColorNew, int Shade_Variation)
		{
			Bitmap bitmap = img;
			BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColorToKeep.B, PixelColorToKeep.G, PixelColorToKeep.R };
			int[] numArray2 = new int[] { PixelColorNew.B, PixelColorNew.G, PixelColorNew.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if ((!((numPtr[j * 3] >= (numArray[0] - Shade_Variation)) & (numPtr[j * 3] <= (numArray[0] + Shade_Variation))) || !((numPtr[(j * 3) + 1] >= (numArray[1] - Shade_Variation)) & (numPtr[(j * 3) + 1] <= (numArray[1] + Shade_Variation)))) || !((numPtr[(j * 3) + 2] >= (numArray[2] - Shade_Variation)) & (numPtr[(j * 3) + 2] <= (numArray[2] + Shade_Variation))))
					{
						numPtr[j * 3] = Convert.ToByte(numArray2[0]);
						numPtr[(j * 3) + 1] = Convert.ToByte(numArray2[1]);
						numPtr[(j * 3) + 2] = Convert.ToByte(numArray2[2]);
					}
				}
			}
			bitmap.UnlockBits(bitmapdata);
			return bitmap;
		}

		public static Bitmap ReplaceColorInImage(Bitmap bmp, Color target, Color replace, int tolerance)
		{
			Bitmap inputBitmap = new Bitmap(bmp.Width, bmp.Height);
			FastBitmap bitmap2 = new FastBitmap(bmp);
			FastBitmap bitmap3 = new FastBitmap(inputBitmap);
			bitmap2.LockImage();
			bitmap3.LockImage();
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					Color pixel = bitmap2.GetPixel(i, j);
					if (isColorMatch(pixel, target, tolerance))
					{
						bitmap3.SetPixel(i, j, replace);
					}
					else
					{
						bitmap3.SetPixel(i, j, pixel);
					}
				}
			}
			bitmap2.UnlockImage();
			bitmap3.UnlockImage();
			return inputBitmap;
		}

		public static unsafe Bitmap SwitchColors(Bitmap img, Color PixelColorOld, int Shade_Variation, Color PixelColorNew)
		{
			Bitmap bitmap = img;
			BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int[] numArray = new int[] { PixelColorOld.B, PixelColorOld.G, PixelColorOld.R };
			int[] numArray2 = new int[] { PixelColorNew.B, PixelColorNew.G, PixelColorNew.R };
			for (int i = 0; i < bitmapdata.Height; i++)
			{
				byte* numPtr = ((byte*) bitmapdata.Scan0) + (i * bitmapdata.Stride);
				for (int j = 0; j < bitmapdata.Width; j++)
				{
					if ((((numPtr[j * 3] >= (numArray[0] - Shade_Variation)) & (numPtr[j * 3] <= (numArray[0] + Shade_Variation))) && ((numPtr[(j * 3) + 1] >= (numArray[1] - Shade_Variation)) & (numPtr[(j * 3) + 1] <= (numArray[1] + Shade_Variation)))) && ((numPtr[(j * 3) + 2] >= (numArray[2] - Shade_Variation)) & (numPtr[(j * 3) + 2] <= (numArray[2] + Shade_Variation))))
					{
						numPtr[j * 3] = Convert.ToByte(numArray2[0]);
						numPtr[(j * 3) + 1] = Convert.ToByte(numArray2[1]);
						numPtr[(j * 3) + 2] = Convert.ToByte(numArray2[2]);
					}
				}
			}
			bitmap.UnlockBits(bitmapdata);
			return bitmap;
		}
	}
}

