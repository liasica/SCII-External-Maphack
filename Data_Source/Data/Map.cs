namespace Data
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Map
	{
		public MapInfo mapInfo;
		public List<Player> players;
		public List<Unit> units;
		public List<Abil> abilities;
	}
}

