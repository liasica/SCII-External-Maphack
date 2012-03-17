namespace Data
{
	using System;

	[Flags]
	public enum GameStatus
	{
		InGame = 0x16,
		MemoryUnreadable = 2,
		None = 1,
		NotInGame = 8,
		VersionDifference = 4
	}
}

