namespace Ini
{
	using System;
	using System.Collections.Generic;

	public class IniSection : Dictionary<string, string>
	{
		public void Add(string line)
		{
			if (line.Length != 0)
			{
				int index = line.IndexOf('=');
				if (index == -1)
				{
					throw new Exception("Keys must have an equal sign.");
				}
				base.Add(line.Substring(0, index), line.Substring(index + 1, (line.Length - index) - 1));
			}
		}

		public string[] GetKeys()
		{
			string[] strArray = new string[base.Count];
			byte index = 0;
			foreach (KeyValuePair<string, string> pair in this)
			{
				strArray[index] = pair.Key;
				index = (byte) (index + 1);
			}
			return strArray;
		}

		public bool HasKey(string key)
		{
			foreach (KeyValuePair<string, string> pair in this)
			{
				if (pair.Key == key)
				{
					return true;
				}
			}
			return false;
		}

		public string ToString(string key)
		{
			return (key + "=" + base[key]);
		}
	}
}

