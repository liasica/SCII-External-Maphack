namespace Ini
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class IniFile : Dictionary<string, IniSection>
	{
		private string _FileName;

		public IniFile(string file)
		{
			this._FileName = file;
		}

		public string Add(string line)
		{
			if (line.StartsWith("["))
			{
				line = line.TrimStart(new char[] { '[' });
			}
			if (line.EndsWith("]"))
			{
				line = line.TrimEnd(new char[] { ']' });
			}
			base.Add(line, new IniSection());
			return line;
		}

		public bool Delete()
		{
			try
			{
				File.Delete(this._FileName);
				base.Clear();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Exists()
		{
			return File.Exists(this._FileName);
		}

		public string[] GetSections()
		{
			string[] strArray = new string[base.Count];
			byte index = 0;
			foreach (KeyValuePair<string, IniSection> pair in this)
			{
				strArray[index] = pair.Key;
				index = (byte) (index + 1);
			}
			return strArray;
		}

		public bool HasSection(string section)
		{
			foreach (KeyValuePair<string, IniSection> pair in this)
			{
				if (pair.Key == section)
				{
					return true;
				}
			}
			return false;
		}

		public bool Load()
		{
			if (!this.Exists())
			{
				return true;
			}
			try
			{
				StreamReader reader = new StreamReader(this._FileName);
				string str = "";
				while (reader.Peek() != -1)
				{
					string line = reader.ReadLine();
					if (!line.StartsWith("[") || !line.EndsWith("]"))
					{
						if (str.Length == 0)
						{
							throw new Exception("Ini file must start with a section.");
						}
						base[str].Add(line);
					}
					else
					{
						str = this.Add(line);
						continue;
					}
				}
				reader.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Move(string path)
		{
			try
			{
				File.Move(this._FileName, path);
				this._FileName = path;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Save()
		{
			try
			{
				StreamWriter writer = new StreamWriter(this._FileName);
				foreach (string str in base.Keys)
				{
					writer.WriteLine("[" + str + "]");
					foreach (string str2 in base[str].Keys)
					{
						writer.WriteLine(str2 + "=" + base[str][str2]);
					}
					writer.WriteLine();
					writer.Flush();
				}
				writer.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public string FileName
		{
			get
			{
				return this._FileName;
			}
		}
	}
}

