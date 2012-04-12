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
		public float apmCurrent;
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
		public float supply;
		public float supplyCap;
		public float supplyLimit;
		public int unitCount;
		public int team;
		public PlayerType playerType;
		public float cameraX;
		public float cameraY;
		public float cameraDistance;
		public float cameraPitch;
		public float cameraRotation;

		public string accountNumber;
		public string rankIconTooltip;

		public override string ToString()
		{
			return name;
		}

		public void UpdateRankTexture()
		{
			if (this.playerType == PlayerType.User && this.accountNumber.Contains("-S2-"))
			{
				string str2 = this.accountNumber.Substring(this.accountNumber.LastIndexOf('-') + 1);
				uint bnetID = 0;
				if (!string.IsNullOrWhiteSpace(str2))
				{
					try
					{
						bnetID = uint.Parse(str2);
					}
					catch (Exception exception)
					{
						throw new Exception("Couldn't get BNetID from: " + this.accountNumber, exception);
					}
					if (bnetID != 0)
					{
						SC2GameType oNEvsONE;
						string region = GameData.SC2Language.Substring(2);
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
							if(this.rank_texture.Contains("none"))
								this.rank_texture = null;
							this.rankIconTooltip = rankArray[0].GameType.ToString() + ", League: " +  rankArray[0].League + ", Rank: " + rankArray[0].Rank + ", Wins: " + rankArray[0].Wins;
						}
					}
				}
			}
		}
	}
}

