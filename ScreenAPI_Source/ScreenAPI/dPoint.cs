namespace ScreenAPI
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct dPoint
	{
		public double X;
		public double Y;
		public dPoint(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public static bool operator ==(dPoint a, dPoint b)
		{
			return ((a.X == b.X) && (a.Y == b.Y));
		}

		public static bool operator !=(dPoint a, dPoint b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return string.Concat(new object[] { "X: ", Math.Round(this.X, 2), ", Y: ", Math.Round(this.Y, 2) });
		}
	}
}

