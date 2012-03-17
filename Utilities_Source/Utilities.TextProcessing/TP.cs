namespace Utilities.TextProcessing
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.CompilerServices;
	using System.Text;

	public class TP
	{
		private static int ByteIndexOf(byte[] arrayToSearch, byte[] patternToFind)
		{
			if (((patternToFind.Length != 0) && (arrayToSearch.Length != 0)) && (arrayToSearch.Length >= patternToFind.Length))
			{
				List<PartialMatch> list = new List<PartialMatch>();
				for (int i = 0; i < arrayToSearch.Length; i++)
				{
					for (int j = list.Count - 1; j >= 0; j--)
					{
						if (arrayToSearch[i] == patternToFind[list[j].MatchLength])
						{
							PartialMatch local1 = list[j];
							local1.MatchLength++;
							if (list[j].MatchLength == patternToFind.Length)
							{
								return list[j].Index;
							}
						}
						else
						{
							list.Remove(list[j]);
						}
					}
					if (arrayToSearch[i] == patternToFind[0])
					{
						if (patternToFind.Length == 1)
						{
							return i;
						}
						list.Add(new PartialMatch(i));
					}
				}
			}
			return -1;
		}

		public static string HexAsciiConvert(string hex)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i <= (hex.Length - 2); i += 2)
			{
				builder.Append(Convert.ToString(Convert.ToChar(int.Parse(hex.Substring(i, 2), NumberStyles.HexNumber))));
			}
			return builder.ToString();
		}

		public byte[] HexStringToByteArray(string hex)
		{
			if ((hex.Length % 2) != 0)
			{
				throw new Exception("HexStringToByteArray hex parameter must contain an even ammount of characters");
			}
			int length = hex.Length;
			byte[] buffer = new byte[length / 2];
			for (int i = 0; i < length; i += 2)
			{
				if (!this.TestHexChar(hex.Substring(i, 1).ToCharArray()[0]) || !this.TestHexChar(hex.Substring(i + 1, 1).ToCharArray()[0]))
				{
					throw new Exception("HexStringToByteArray hex parameter must contain base16/hex characters");
				}
				buffer[i / 2] = Convert.ToByte(hex.Substring(i, 2), 0x10);
			}
			return buffer;
		}

		public static string Isolate(string source, string start, string end)
		{
			int length = start.Length;
			int num1 = end.Length;
			int index = source.IndexOf(start);
			int num3 = source.IndexOf(end);
			int startIndex = index + length;
			int num5 = num3 - startIndex;
			return source.Substring(startIndex, num5);
		}

		public static string[] IsolateSplit(string source, string start, string end)
		{
			int length = start.Length;
			int num2 = end.Length;
			int index = source.IndexOf(start);
			int num4 = source.IndexOf(end);
			int startIndex = index + length;
			int num6 = num4 - startIndex;
			List<string> list = new List<string>();
			while ((index > -1) && (num4 > -1))
			{
				list.Add(source.Substring(startIndex, num6));
				source = source.Substring(num4 + num2);
				index = source.IndexOf(start);
				num4 = source.IndexOf(end);
			}
			return new string[list.Count];
		}

		public static string RemoveString(string source, string start, string end)
		{
			int length = start.Length;
			int num = end.Length;
			int index = source.IndexOf(start);
			int num3 = source.IndexOf(end);
			int startIndex = index;
			int count = (num3 + num) - startIndex;
			return source.Remove(startIndex, count);
		}

		public bool TestHexChar(char hexChar)
		{
			int num = hexChar;
			if ((((num < 40) || (num > 0x39)) && ((num < 0x41) || (num > 70))) && ((num < 0x61) || (num > 0x66)))
			{
				return false;
			}
			return true;
		}

		private class PartialMatch
		{
			public PartialMatch(int index)
			{
				this.Index = index;
				this.MatchLength = 1;
			}

			public int Index { get; private set; }

			public int MatchLength { get; set; }
		}
	}
}

