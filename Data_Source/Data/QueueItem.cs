namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct QueueItem
	{
		public int queuePosition;
		public string name;
		public int duration;
		public int startTime;
	}
}

