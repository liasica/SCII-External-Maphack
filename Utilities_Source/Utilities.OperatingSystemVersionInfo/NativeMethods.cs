namespace Utilities.OperatingSystemVersionInfo
{
	using System;
	using System.Runtime.InteropServices;

	internal class NativeMethods
	{
		private NativeMethods()
		{
		}

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("kernel32.dll")]
		internal static extern bool GetProductInfo(int osMajorVersion, int osMinorVersion, int spMajorVersion, int spMinorVersion, ref uint type);
		[DllImport("kernel32.dll")]
		internal static extern int GetSystemMetrics(int index);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("kernel32")]
		internal static extern bool GetVersionEx(ref OSVersionInfoEx osvi);

		[StructLayout(LayoutKind.Sequential)]
		internal struct OSVersionInfoEx
		{
			public int VersionInfoSize;
			public int MajorVersion;
			public int MinorVersion;
			public int BuildNumber;
			public int PlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x80)]
			public string CSDVersion;
			public short ServicePackMajor;
			public short ServicePackMinor;
			public short SuiteMask;
			public byte ProductType;
			public byte Reserved;
		}
	}
}

