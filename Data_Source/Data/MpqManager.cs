namespace Data
{
	using Foole.Mpq;
	using System;
	using System.IO;

	internal class MpqManager
	{
		private string _fileContents;
		private string _fileContentsType;
		private Foole.Mpq.MpqArchive _mpqArchive;
		private Foole.Mpq.MpqArchive _topArchive;

		public MpqManager(string mpqFile)
		{
			//FileInfo info = new FileInfo(mpqFile);
			//string destFileName = Path.Combine(Path.GetTempPath(), info.Name);
			//File.Copy(mpqFile, destFileName, true);
			//this._mpqArchive = new Foole.Mpq.MpqArchive(destFileName);
			if(File.Exists(mpqFile))
			{
				this._mpqArchive = new Foole.Mpq.MpqArchive(mpqFile);
				this._mpqArchive.AddListfileFilenames();
				return;
			}
			
			string[] SplitPath = mpqFile.Split('\\', '/');
			string[] Remaining = null;
			string Path = SplitPath.Length > 0 ? SplitPath[0] : string.Empty;

			for (int i = 1; i < SplitPath.Length; i++)
			{
				Path += '\\' + SplitPath[i];
				if (!Directory.Exists(Path))
				{
					Remaining = new string[SplitPath.Length - i];
					Array.ConstrainedCopy(SplitPath, i, Remaining, 0, SplitPath.Length - i);
					break;
				}
			}

			if (Remaining != null && Remaining.Length > 0 && File.Exists(Path))
			{
				this._topArchive = new Foole.Mpq.MpqArchive(Path);
				this._topArchive.AddListfileFilenames();
			}


			
		}

		public void Close()
		{
			this._mpqArchive.Dispose();
			this._mpqArchive = null;
		}

		public byte[] read(string fileName)
		{
			MpqStream input = this._mpqArchive.OpenFile(fileName);
			BinaryReader reader = new BinaryReader(input);
			byte[] buffer = new byte[reader.BaseStream.Length];
			reader.Read(buffer, 0, buffer.Length);
			reader.Close();
			input.Close();
			return buffer;
		}

		public string readFile(string fileName)
		{
			if (this._fileContentsType != fileName)
			{
				MpqStream input = this._mpqArchive.OpenFile(fileName);
				BinaryReader reader = new BinaryReader(input);
				char[] buffer = new char[reader.BaseStream.Length];
				reader.Read(buffer, 0, buffer.Length);
				reader.Close();
				input.Close();
				this._fileContents = new string(buffer);
				this._fileContentsType = fileName;
			}
			return this._fileContents;
		}

		public static string ButtonGameHotkeysFile
		{
			get
			{
				return (@"Mods\Liberty.SC2Mod\" + GameData.SC2Language + @".SC2Data\LocalizedData\GameHotkeys.txt");
			}
		}

		public static string MapGameStringsFile
		{
			get
			{
				return (GameData.SC2Language + @".SC2Data\LocalizedData\GameStrings.txt");
			}
		}

		public Foole.Mpq.MpqArchive MpqArchive
		{
			get
			{
				return this._mpqArchive;
			}
			set
			{
				this._mpqArchive = value;
			}
		}

		public static string PatchArchive
		{
			get
			{
				return Path.Combine(GameData.SC2FilePath, "patch-" + GameData.SC2Language + ".SC2Archive");
			}
		}

		public static string UIGameHotkeysFile
		{
			get
			{
				return (@"Mods\Core.SC2Mod\" + GameData.SC2Language + @".SC2Data\LocalizedData\GameHotkeys.txt");
			}
		}

		public static string UnitData
		{
			get
			{
				return (@"Mods\Liberty.SC2Mod\" + GameData.SC2Language + @".SC2Data\LocalizedData\UnitData.txt");
			}
		}
	}
}

