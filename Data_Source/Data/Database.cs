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

		static bool RecursivelyCheckDependencies(string DatabaseItem, string DestinationFilename, Module mod)
		{
			MpqManager manager;
			byte[] file = null;

			if (File.Exists(mod.FileName))
			{
				manager = new MpqManager(mod.FileName);
				if (manager.MpqArchive.FileExists(mod.SubPath + "Base.SC2Assets\\" + DatabaseItem))
					file = manager.read(mod.SubPath + "Base.SC2Assets\\" + DatabaseItem);
				else if (manager.MpqArchive.FileExists("Base.SC2Assets\\" + DatabaseItem))
					file = manager.read("Base.SC2Assets\\" + DatabaseItem);
				
				if(file != null)
				{
					using (FileStream fs = File.Create(DestinationFilename))
					{
						fs.Write(file, 0, file.Length);
						fs.Close();
					}
					manager.Close();
					return true;
				}
				manager.Close();
			}
			else if (File.Exists(mod.FileName + "\\Base.SC2Assets"))
			{
				manager = new MpqManager(mod.FileName + "\\Base.SC2Assets");
				if (manager.MpqArchive.FileExists(DatabaseItem))
				{
					file = manager.read(DatabaseItem);

					using (FileStream fs = File.Create(DestinationFilename))
					{
						fs.Write(file, 0, file.Length);
						fs.Close();
					}
					manager.Close();
					return true;
				}
				manager.Close();
			}

			for (int i = mod.Dependencies.Count - 1; i >= 0; i--)
			{
				if (RecursivelyCheckDependencies(DatabaseItem, DestinationFilename, mod.Dependencies[i]))
					return true;
			}
			return false;
		}

		public static string GetItemFilename(string DatabaseItem, bool GetNew)
		{
			string Filename = GameData.path + "Database\\" + DatabaseItem.Replace('/', '\\');

			if(!Directory.Exists(Filename.Remove(Filename.LastIndexOf('\\'))))
				Directory.CreateDirectory(Filename.Remove(Filename.LastIndexOf('\\')));

			if (File.Exists(Filename) && !GetNew)
				return Filename;

			if (IgnoreList.Contains(DatabaseItem))
				return "";

			if (RecursivelyCheckDependencies(DatabaseItem, Filename, GameData.mapDat))
				return Filename;

			IgnoreList.Add(DatabaseItem);
			return "";
		}
	}
}