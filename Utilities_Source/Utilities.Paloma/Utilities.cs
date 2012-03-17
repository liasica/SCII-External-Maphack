namespace Utilities.Paloma
{
	using System;
	using System.Drawing;

	internal static class Utilities
	{
		internal static int GetBits(byte b, int offset, int count)
		{
			return ((b >> offset) & ((((int) 1) << count) - 1));
		}

		internal static Color GetColorFrom2Bytes(byte one, byte two)
		{
			int red = GetBits(one, 2, 5) << 3;
			int num4 = GetBits(one, 0, 2) << 6;
			int num5 = GetBits(two, 5, 3) << 3;
			int green = num4 + num5;
			int blue = GetBits(two, 0, 5) << 3;
			int alpha = GetBits(one, 7, 1) * 0xff;
			return Color.FromArgb(alpha, red, green, blue);
		}

		internal static string GetInt16BinaryString(short n)
		{
			char[] chArray = new char[0x10];
			int index = 15;
			for (int i = 0; i < 0x10; i++)
			{
				if ((n & (((int) 1) << i)) != 0)
				{
					chArray[index] = '1';
				}
				else
				{
					chArray[index] = '0';
				}
				index--;
			}
			return new string(chArray);
		}

		internal static string GetIntBinaryString(int n)
		{
			char[] chArray = new char[0x20];
			int index = 0x1f;
			for (int i = 0; i < 0x20; i++)
			{
				if ((n & (((int) 1) << i)) != 0)
				{
					chArray[index] = '1';
				}
				else
				{
					chArray[index] = '0';
				}
				index--;
			}
			return new string(chArray);
		}
	}
}

