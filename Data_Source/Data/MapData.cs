namespace Data
{
	using System.Xml.Linq;
	using System.Xml;
	using System.Text;
	using Foole.Mpq;
	using System.Collections.Generic;
	
	public class DataFile
	{
		string _Name;
		List<XElement> _Data;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		public List<XElement> Data
		{
			get { return _Data; }
			set { _Data = value; }
		}

		public DataFile()
		{
			_Name = "";
			_Data = new List<XElement>();
		}
		public DataFile(string name, string fileContents) : this()
		{
			Parse(name, fileContents);
		}
		public void Parse(string name, string fileContents)
		{
			_Name = name;

			try
			{
				XElement File = XElement.Parse(fileContents);
				foreach (XElement element in File.Elements())
				{
					_Data.Add(element);
				}
			}
			catch
			{
				int fail = 0;
			}
		}
	}

	public class Dependency
	{
		string _FileName;
		string _FilePath;
		Dictionary<string, DataFile> _DataFiles;

		public string FileName
		{
			get { return _FileName; }
		}
		public string FilePath
		{
			get { return _FilePath; }
		}
		public Dictionary<string, DataFile> DataFiles
		{
			get { return _DataFiles; }
		}

		public Dependency()
		{
			_FileName = "";
			_FilePath = "";
			_DataFiles = new Dictionary<string, DataFile>();
		}
		public Dependency(string fileName) : this()
		{
			Parse(fileName);
		}
		public void Parse(string fileName)
		{
			_FileName = fileName;
			_FilePath = GameData.SC2FilePath.Remove(GameData.SC2FilePath.LastIndexOf("Versions")) + fileName;
			
			if (_FileName != "Mods\\Liberty.SC2Mod" && _FileName != "Mods\\LibertyMulti.SC2Mod" && _FileName != "Mods\\Core.SC2Mod"
				&& _FileName != "Campaigns\\Liberty.SC2Campaign" && _FileName != "Campaigns\\LibertyStory.SC2Campaign")
			{
				bool oops = true;
			}
			try
			{
				MpqManager manager = new MpqManager(_FilePath + "\\Base.SC2Data");

				foreach (MpqEntry entry in manager.MpqArchive)
				{
					if (entry.Filename != null && entry.Filename.StartsWith("GameData\\") && entry.Filename.EndsWith(".xml"))
					{
						string Name = entry.Filename.Replace("GameData\\", "").Replace(".xml", "");
						string DataFile = System.Text.Encoding.UTF8.GetString(manager.read(entry.Filename));
						_DataFiles.Add(Name, new DataFile(Name, DataFile));
					}
				}
				
				manager.Close();
			}
			catch
			{
				bool oops = true;
			}
		}
	}

	public class MapData
	{
		string _FileName;
		List<Dependency> _Dependencies;
		Dictionary<string, DataFile> _RawDataFiles;
		Dictionary<string, List<DataFile>> _ProcessedDataFiles;

		public string FileName
		{
			get { return _FileName; }
		}
		public List<Dependency> Dependencies
		{
			get { return _Dependencies; }
		}
		public Dictionary<string, DataFile> RawDataFiles
		{
			get { return _RawDataFiles; }
		}
		public Dictionary<string, List<DataFile>> ProcessedDataFiles
		{
			get { return _ProcessedDataFiles; }
		}

		public MapData()
		{
			_RawDataFiles = new Dictionary<string, DataFile>();
			_ProcessedDataFiles = new Dictionary<string, List<DataFile>>();
			_Dependencies = new List<Dependency>();
			_FileName = "";
		}
		public MapData(string fileName) : this()
		{
			Parse(fileName);
		}
		public void Parse(string fileName)
		{
			_FileName = fileName;
			MpqManager manager = new MpqManager(fileName);
			string DocumentInfo = System.Text.Encoding.UTF8.GetString(manager.read("DocumentInfo"));
			XElement DocInfo = XElement.Parse(DocumentInfo);

			foreach (XElement dependency in DocInfo.Element("Dependencies").Elements("Value"))
			{
				string Value = dependency.Value;
				_Dependencies.Add(new Dependency(Value.Remove(0, Value.IndexOf("file:") + 5).Replace('/', '\\')));
			}


			foreach (MpqEntry entry in manager.MpqArchive)
			{
				if (entry.Filename != null && entry.Filename.StartsWith("Base.SC2Data\\GameData\\") && entry.Filename.EndsWith(".xml"))
				{
					string Name = entry.Filename.Replace("Base.SC2Data\\GameData\\", "").Replace(".xml", "");
					string DataFile = System.Text.Encoding.UTF8.GetString(manager.read(entry.Filename));
					_RawDataFiles.Add(Name, new DataFile(Name, DataFile));
				}
			}

			manager.Close();
			foreach (KeyValuePair<string, DataFile> dataFile in _RawDataFiles)
			{
				if (!_ProcessedDataFiles.ContainsKey(dataFile.Key))
					_ProcessedDataFiles.Add(dataFile.Key, new List<DataFile>());

				_ProcessedDataFiles[dataFile.Key].Add(dataFile.Value);
			}

			for(int i = _Dependencies.Count - 1; i >= 0; i--)
			{
				foreach (KeyValuePair<string, DataFile> dataFile in _Dependencies[i].DataFiles)
				{
					if (!_ProcessedDataFiles.ContainsKey(dataFile.Key))
						_ProcessedDataFiles.Add(dataFile.Key, new List<DataFile>());

					_ProcessedDataFiles[dataFile.Key].Add(dataFile.Value);
				}
			}

			
		}

		private static bool SameElement(XElement a, XElement b, bool checkAttributeValues)
		{
			if (a.Name != b.Name || a.HasAttributes != b.HasAttributes)
				return false;

			if (!a.HasAttributes && !b.HasAttributes)
				return true;

			XAttribute AttributeA = a.FirstAttribute;
			XAttribute AttributeB = b.FirstAttribute;
			bool fail = false;
			while (AttributeA != null && AttributeB != null)
			{
				if (AttributeA.Name != AttributeB.Name)
				{
					fail = true;
					break;
				}
				if (checkAttributeValues && AttributeA.Value != AttributeB.Value)
				{
					fail = true;
					break;
				}
				AttributeA = AttributeA.NextAttribute;
				AttributeB = AttributeB.NextAttribute;
			}
			if (!fail && AttributeA == null && AttributeB == null)
				return true;

			return false;
		}

		private static DataFile MergeData(DataFile Base, DataFile Addition)
		{
			for (int i = 0; i < Addition.Data.Count; i++ )
			{
				bool match = false;
				for (int j = 0; j < Base.Data.Count; j++)
				{
					match = SameElement(Addition.Data[i], Base.Data[j], true);
					if (match)
					{
						foreach (XElement element in Addition.Data[i].Elements())
						{
							if (Base.Data[j].Element(element.Name) != null)
							{
								foreach (XElement baseElement in Base.Data[j].Elements(element.Name))
								{
									if (SameElement(baseElement, element, false))
										baseElement.Remove();
								}
							}

							Base.Data[j].Add(element);
						}
						break;
					}
				}
				if (!match)
				{
					Base.Data.Add(Addition.Data[i]);
				}
			}

			return Base;
		}
		
		public string GetUnitPictureFilename(string unit)
		{
			string parent = "";
			string id = "";
			if (_ProcessedDataFiles.ContainsKey("ActorData"))
			{
				foreach (DataFile dataFile in _ProcessedDataFiles["ActorData"])
				{
					foreach (XElement element in dataFile.Data)
					{
						if (element.Name == "CActorUnit" && element.HasAttributes 
							&& (id != "" && element.Attribute("id") != null && element.Attribute("id").Value == id
							|| element.Attribute("unitName") != null && element.Attribute("unitName").Value == unit))
						{
							if (id == "" && element.Attribute("id") != null)
								id = element.Attribute("id").Value;

							if (parent == "" && element.Attribute("parent") != null)
								parent = element.Attribute("parent").Value;

							if (element.HasElements && element.Element("UnitIcon") != null && element.Element("UnitIcon").HasAttributes && element.Element("UnitIcon").Attribute("value") != null)
								return element.Element("UnitIcon").Attribute("value").Value;
							/*else if (element.HasElements && element.Element("GroupIcon") != null && element.Element("GroupIcon").HasElements && element.Element("GroupIcon").Element("Image") != null)
							{
								if (element.Element("GroupIcon").Element("Image").HasAttributes && element.Element("GroupIcon").Element("Image").Attribute("value") != null)
									return element.Element("GroupIcon").Element("Image").Attribute("value").Value;
							}*/
						}
					}
				}
			}

			if (parent != "")
				return GetUnitPictureFilename(parent);

			return "Assets\\Textures\\btn-missing-kaeo.dds";
		}
	}

	
}