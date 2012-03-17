namespace _2cs_API
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct PrecisionPoint
	{
		public double X;
		public double Y;
		public PrecisionPoint(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public static PrecisionPoint Empty()
		{
			return new PrecisionPoint(0.0, 0.0);
		}
	}
}

