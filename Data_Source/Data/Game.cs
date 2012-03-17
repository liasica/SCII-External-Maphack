namespace Data
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Game
	{
		public string version;
		public GameStatus status;
		public Map map;
	}
}

