namespace SC2RanksAPI
{
	using System;
	using System.IO;
	using System.Net;
	using System.Runtime.InteropServices;
	using System.Text;

	public static class SC2Ranks
	{
		private static string FetchPage(string httpUrl)
		{
			HttpWebResponse response;
			StringBuilder builder = new StringBuilder();
			byte[] buffer = new byte[0x2000];
			if (!httpUrl.Contains("http://"))
			{
				httpUrl.Insert(0, "http://");
			}
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(httpUrl);
			try
			{
				response = (HttpWebResponse) request.GetResponse();
			}
			catch
			{
				return null;
			}
			Stream responseStream = response.GetResponseStream();
			string str = null;
			int count = 0;
			do
			{
				count = responseStream.Read(buffer, 0, buffer.Length);
				if (count != 0)
				{
					str = Encoding.ASCII.GetString(buffer, 0, count);
					builder.Append(str);
				}
			}
			while (count > 0);
			return builder.ToString();
		}

		private static SC2Rank getGameTypeInfo(string playerName, SC2GameType gameType, string rawProfile, string leagueIdentifier, string rankIdentifier, string totalsIdentifier, int searchOffset = 0)
		{
			int startIndex = rawProfile.IndexOf(leagueIdentifier, searchOffset) + leagueIdentifier.Length;
			string str = rawProfile.Substring(startIndex, rawProfile.IndexOf(" ", startIndex) - startIndex);
			str = char.ToUpper(str[0]) + str.Substring(1);
			SC2League league = (SC2League) System.Enum.Parse(typeof(SC2League), str);
			int num2 = rawProfile.IndexOf(rankIdentifier, startIndex) + rankIdentifier.Length;
			int rank = 0;
			if (!int.TryParse(rawProfile.Substring(num2, rawProfile.IndexOf("<", num2) - num2).Trim(), out rank))
				rank = -1;
			int num4 = rawProfile.IndexOf(totalsIdentifier, num2) + totalsIdentifier.Length;
			string[] strArray = rawProfile.Substring(num4, (rawProfile.IndexOf(" ", num4) - num4) + 2).Split(new char[] { ' ' });
			strArray[0] = strArray[0].Replace(",", "");
			int num5 = 0;
			int wins = 0;
			if (strArray[1] == "G")
			{
				num5 = int.Parse(strArray[0]);
				int num8 = rawProfile.IndexOf(totalsIdentifier, num4) + totalsIdentifier.Length;
				wins = int.Parse(rawProfile.Substring(num8, rawProfile.IndexOf(" ", num8) - num8).Replace(",", ""));
			}
			else
			{
				wins = int.Parse(strArray[0]);
				num5 = wins;
			}
			return new SC2Rank(playerName, gameType, league, rank, wins, num5 - wins);
		}

		private static SC2Rank[] getPlayerProfile(SC2Region region, uint bnetID, string playerName, SC2GameType gameType)
		{
			string URL = string.Concat(new object[] { "http://", region, ".battle.net/sc2/en/profile/", bnetID, "/1/", playerName, "/" });
			string rawProfile = FetchPage(string.Concat(new object[] { "http://", region, ".battle.net/sc2/en/profile/", bnetID, "/1/", playerName, "/" }));
			return parsePlayerProfile(playerName, gameType, ref rawProfile);
		}

		public static SC2Rank[] GetRanksForPlayer(SC2Region region, uint bnetID, string playerName, SC2GameType gameType)
		{
			SC2Rank[] rankArray;
			if ((bnetID == 0) || string.IsNullOrWhiteSpace(playerName))
			{
				return null;
			}
			try
			{
				rankArray = getPlayerProfile(region, bnetID, playerName, gameType);
			}
			catch (Exception exception)
			{
				string str = "There was an error getting the ranks for the player.\n\n";
				object obj2 = str;
				object obj3 = string.Concat(new object[] { obj2, "Region: ", region, "\n" });
				throw new Exception((string.Concat(new object[] { obj3, "BNetID: ", bnetID, "\n" }) + "Name: " + playerName + "\n") + "Game: " + gameType, exception);
			}
			return rankArray;
		}

		private static SC2Rank[] parsePlayerProfile(string playerName, SC2GameType gameType, ref string rawProfile)
		{
			if (string.IsNullOrWhiteSpace(rawProfile))
			{
				return null;
			}
			string str = "class=\"module-body snapshot-";
			int startIndex = rawProfile.IndexOf(str, 0) + str.Length;
			if (rawProfile[startIndex] == 'e')
			{
				return null;
			}
			string str2 = "class=\"snapshot ";
			string leagueIdentifier = "class=\"badge badge-";
			string rankIdentifier = "<strong>Rank:</strong> ";
			string totalsIdentifier = "class=\"totals\">";
			int num2 = rawProfile.IndexOf(str2, startIndex) + str2.Length;
			int num3 = rawProfile.IndexOf(str2, num2) + str2.Length;
			int num4 = rawProfile.IndexOf(str2, num3) + str2.Length;
			int searchOffset = rawProfile.IndexOf(str2, num4) + str2.Length;
			bool flag2 = rawProfile[num2] == 'e';
			bool flag3 = rawProfile[num3] == 'e';
			bool flag4 = rawProfile[num4] == 'e';
			bool flag5 = rawProfile[searchOffset] == 'e';

			switch (gameType)
			{
				case SC2GameType.All:
					return new SC2Rank[] { getGameTypeInfo(playerName, SC2GameType.ONEvsONE, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, num2), getGameTypeInfo(playerName, SC2GameType.TWOvsTWO, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, num3), getGameTypeInfo(playerName, SC2GameType.THREEvsTHREE, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, num4), getGameTypeInfo(playerName, SC2GameType.FOURvsFOUR, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, searchOffset) };

				case SC2GameType.ONEvsONE:
					if (!flag2)
					{
						return new SC2Rank[] { getGameTypeInfo(playerName, SC2GameType.ONEvsONE, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, num2) };
					}
					return null;

				case SC2GameType.TWOvsTWO:
					if (!flag3)
					{
						return new SC2Rank[] { getGameTypeInfo(playerName, SC2GameType.TWOvsTWO, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, num3) };
					}
					return null;

				case SC2GameType.THREEvsTHREE:
					if (!flag4)
					{
						return new SC2Rank[] { getGameTypeInfo(playerName, SC2GameType.THREEvsTHREE, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, num4) };
					}
					return null;

				case SC2GameType.FOURvsFOUR:
					if (!flag5)
					{
						return new SC2Rank[] { getGameTypeInfo(playerName, SC2GameType.FOURvsFOUR, rawProfile, leagueIdentifier, rankIdentifier, totalsIdentifier, searchOffset) };
					}
					return null;
			}
			return null;
		}
	}
}

