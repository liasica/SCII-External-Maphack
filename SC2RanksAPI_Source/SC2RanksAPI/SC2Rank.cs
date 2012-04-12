namespace SC2RanksAPI
{
	using System;

	public class SC2Rank
	{
		private SC2GameType _gameType;
		private string _league;
		private int _losses;
		private string _name;
		private int _rank;
		private string _rankTexture;
		private int _wins;

		public SC2Rank(string name, SC2GameType gameType, string league, int rank, int wins, int losses)
		{
			this._name = name;
			this._gameType = gameType;
			this._league = league;
			this._rank = rank;
			this._wins = wins;
			this._losses = losses;
		}

		public int Games
		{
			get
			{
				return (this.Wins + this.Losses);
			}
		}

		public SC2GameType GameType
		{
			get
			{
				return this._gameType;
			}
		}

		public string League
		{
			get
			{
				return this._league;
			}
		}

		public int Losses
		{
			get
			{
				return this._losses;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public int Rank
		{
			get
			{
				return this._rank;
			}
		}

		public string RankTexture
		{
			get
			{
				if (this._rankTexture == null)
				{
					string str = this._league;
					str = char.ToLower(str[0]) + str.Substring(1) + "-";
					if (str != "none")
					{
						if ((this.Rank >= 1) && (this.Rank <= 8))
						{
							str = str + 1;
						}
						else if ((this.Rank >= 9) && (this.Rank <= 25))
						{
							str = str + 2;
						}
						else if ((this.Rank >= 26) && (this.Rank <= 50))
						{
							str = str + 3;
						}
						else if ((this.Rank >= 51) && (this.Rank <= 100))
						{
							str = str + 4;
						}
					}
					str = str + ".png";
					this._rankTexture = @"Leagues\" + str;
				}
				return this._rankTexture;
			}
		}

		public int Wins
		{
			get
			{
				return this._wins;
			}
		}
	}
}

