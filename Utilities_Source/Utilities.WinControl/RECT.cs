namespace Utilities.WinControl
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
		public int Width
		{
			get
			{
				return Math.Abs((int) (this.Right - this.Left));
			}
		}
		public int Height
		{
			get
			{
				return Math.Abs((int) (this.Top - this.Bottom));
			}
		}
		public static implicit operator Rectangle(Utilities.WinControl.RECT rect)
		{
			return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		public static implicit operator Utilities.WinControl.RECT(Rectangle rect)
		{
			return new Utilities.WinControl.RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		public RECT(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}
}

