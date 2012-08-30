namespace Data
{
	using System.IO;
	using System.Xml;
	using System.Text;
	using Foole.Mpq;
	using System.Collections.Generic;

	public static class Database
	{
		static HashSet<string> IgnoreList = new HashSet<string>();

		public static void Reset()
		{
			IgnoreList = new HashSet<string>();
		}

		public static string GetItemFilename(string DatabaseItem, bool GetNew)
		{
			MpqManager manager;
			string Filename = "Database\\" + DatabaseItem.Replace('/', '\\');

			if(!Directory.Exists(Filename.Remove(Filename.LastIndexOf('\\'))))
				Directory.CreateDirectory(Filename.Remove(Filename.LastIndexOf('\\')));

			if (File.Exists(Filename) && !GetNew)
				return Filename;

			if (IgnoreList.Contains(DatabaseItem))
				return "";

			if (GameData.mapDat == null)
				GameData.mapDat = new MapData(GameData.getMapData().mapInfo.filePath);

			if (File.Exists(GameData.mapDat.FileName))
			{
				manager = new MpqManager(GameData.mapDat.FileName);
				if (manager.MpqArchive.FileExists(DatabaseItem))
				{
					byte[] file = manager.read(DatabaseItem);
					using (FileStream fs = File.Create(Filename))
					{
						fs.Write(file, 0, file.Length);
						fs.Close();
					}
					manager.Close();
					return Filename;
				}
				manager.Close();
			}
			
			for (int i = GameData.mapDat.Dependencies.Count - 1; i >= 0; i--)
			{
				if(!File.Exists(GameData.mapDat.Dependencies[i].FilePath + "\\Base.SC2Assets"))
					continue;

				manager = new MpqManager(GameData.mapDat.Dependencies[i].FilePath + "\\Base.SC2Assets");
				if (manager.MpqArchive.FileExists(DatabaseItem))
				{
					byte[] file = manager.read(DatabaseItem);
					using (FileStream fs = File.Create(Filename))
					{
						fs.Write(file, 0, file.Length);
						fs.Close();
					}
					manager.Close();
					return Filename;
				}
				manager.Close();
			}

			IgnoreList.Add(DatabaseItem);
			return "";
		}
	}
}