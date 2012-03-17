namespace Data
{
	using SC2RanksAPI;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;

	[StructLayout(LayoutKind.Sequential)]
	public struct Player
	{
		public List<Unit> units;
		public uint memoryAddress;
		public uint number;
		public PlayerStatus playerStatus;
		public VictoryStatus victoryStatus;
		public PlayerDifficulty difficulty;
		public PlayerSelections unit_selections;
		public int nameLength;
		public string rank_texture;
		public int slotNumber;
		public int apmCurrent;
		public int actionsTotal;
		public int unitsKilled;
		public int unitsLost;
		public int workersCurrent;
		public int workersBuilt;
		public int buildingQueueLength;
		public int buildingsConstructing;
		public int buildingsUpgrading;
		public int buildingsCurrent;
		public int totalConstructionCount;
		public int upgradeCount;
		public int researchCount;
		public int unitsLostMineralWorth;
		public int unitsLostVespeneWorth;
		public int armyMineralWorth;
		public int armyVespeneWorth;
		public int armySize;
		public Color drawingColor;
		public PlayerColor playerColor;
		public string name;
		public Race race;
		public int minerals;
		public int gas;
		public int terrazine;
		public int custom;
		public int mineralRate;
		public int gasRate;
		public int mineralTotal;
		public int gasTotal;
		public int economyWorth;
		public int technologyMineralWorth;
		public int technologyVespeneWorth;
		public int armyMineralWorth2;
		public int armyVespeneWorth2;
		public int technologyMineralWorth2;
		public int technologyVespeneWorth2;
		public int supply;
		public int supplyCap;
		public int supplyLimit;
		public int unitCount;
		public int team;
		public PlayerType playerType;
		public int cameraX;
		public int cameraY;
		public int cameraDistance;
		public int cameraPitch;
		public int cameraRotation;

		public override string ToString()
		{
			return name;
		}

		public void UpdateRankTexture()
		{
			if (this.playerType == PlayerType.User)
			{
				List<Player> list = (from x in GameData.getPlayersData()
					where x.playerType == PlayerType.User
					select x).ToList<Player>();
				byte[] bytes = Encoding.ASCII.GetBytes("1-S2-"); //TODO: make it work outside of region 1
				string mask = "";
				byte[] buffer2 = bytes;
				for (int i = 0; i < buffer2.Length; i++)
				{
					byte num1 = buffer2[i];
					mask = mask + "x";
				}
				List<uint> list2 = GameData.ps.FindDataRegionPatterns(GameData.StructStarts.BnetIDs, bytes, mask).Take<uint>(list.Count).ToList<uint>();
				if (list2.Count != 0)
				{
					List<uint> source = new List<uint>();
					for (int j = 0; (j < list2.Count) && ((list2.Count - j) >= 2); j++)
					{
						source.Add(list2[j + 1] - list2[j]);
					}
					if (source.Take<uint>((list.Count - 1)).Distinct<uint>().ToList<uint>().Count == 1)
					{
						bytes = new byte[0x18];
						GameData.mem.ReadMemory(list2[((int) this.number) - 1], bytes.Length, out bytes);
						string str2 = Encoding.ASCII.GetString(bytes);
						int index = str2.IndexOf('\0');
						int startIndex = str2.IndexOf('\\');
						if (index > 0)
						{
							str2 = str2.Remove(index);
						}
						if (startIndex > 0)
						{
							str2 = str2.Remove(startIndex);
						}
						str2 = str2.Substring(str2.LastIndexOf('-') + 1);
						uint bnetID = 0;
						if (!string.IsNullOrWhiteSpace(str2))
						{
							try
							{
								bnetID = uint.Parse(str2);
							}
							catch (Exception exception)
							{
								throw new Exception("Couldn't get BNetID from: " + Encoding.ASCII.GetString(bytes), exception);
							}
							if (bnetID != 0)
							{
								SC2GameType oNEvsONE;
								SC2Region region = (SC2Region) Enum.Parse(typeof(SC2Region), GameData.SC2Language.Substring(2));
								string str4 = GameData.SC2GameType;
								switch (str4)
								{
									case null:
									case "FFA":
										oNEvsONE = SC2GameType.ONEvsONE;
										break;

									default:
										oNEvsONE = (SC2GameType) Enum.Parse(typeof(SC2GameType), str4);
										break;
								}
								SC2Rank[] rankArray = SC2Ranks.GetRanksForPlayer(region, bnetID, this.name, oNEvsONE);
								if (rankArray != null)
								{
									this.rank_texture = rankArray[0].RankTexture;
								}
							}
						}
					}
				}
			}
		}
	}
}

