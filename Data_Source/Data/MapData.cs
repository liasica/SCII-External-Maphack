namespace Data
{
	using System;
	using System.Xml.Linq;
	using System.Xml;
	using System.Text;
	using System.IO;
	using Foole.Mpq;
	using System.Collections.Generic;

	public class WeaponData
	{
		public string ID;
		public string DisplayEffectID;
		public string EffectID;
		public fixed32 Range;
		public fixed32 Period;
		public TargetFilter Required;
		public TargetFilter Excluded;

		public WeaponData(MapData map, string id)
		{
			ID = id;
			DisplayEffectID = null;
			EffectID = null;
			Range = 5;
			Period = 0.8332;
			Required = (TargetFilter)0;
			Excluded = (TargetFilter)0;

			if (map.ProcessedDataFiles.ContainsKey("WeaponData"))
			{
				foreach (DataFile dataFile in map.ProcessedDataFiles["WeaponData"])
				{
					foreach (XElement element in dataFile.Data)
					{
						if (element.HasAttributes && element.Attribute("id") != null && element.Attribute("id").Value == id) //found
						{
							if (element.Element("Range") != null)
								Range = float.Parse(element.Element("Range").Attribute("value").Value);
							if (element.Element("DisplayEffect") != null)
								EffectID = element.Element("DisplayEffect").Attribute("value").Value;
							if (element.Element("Effect") != null)
								EffectID = element.Element("Effect").Attribute("value").Value;
							if (element.Element("TargetFilters") != null)
							{
								string[] tf = element.Element("TargetFilters").Attribute("value").Value.Split(';');
								string[] req = tf[0].Split(',');
								foreach (string s in req)
								{
									TargetFilter temp;
									if(Enum.TryParse<TargetFilter>(s, out temp))
										Required |= temp;
								}
								if (tf.Length > 1)
								{
									string[] ex = tf[1].Split(',');
									foreach (string s in ex)
									{
										TargetFilter temp;
										if (Enum.TryParse<TargetFilter>(s, out temp))
											Excluded |= temp;
									}
								}

							}
							break;
						}
					}
				}
			}
		}
	}

	public class UnitData
	{
		public string ID;
		public List<WeaponData> Weapons;

		public UnitData(MapData map, string id)
		{
			ID = id;
			Weapons = new List<WeaponData>();

			Dictionary<int, string> WeaponArray = new Dictionary<int, string>();

			if (map.ProcessedDataFiles.ContainsKey("UnitData"))
			{
				foreach (DataFile dataFile in map.ProcessedDataFiles["UnitData"])
				{
					foreach (XElement element in dataFile.Data)
					{
						if (element.HasAttributes && element.Attribute("id") != null && element.Attribute("id").Value == id)
						{
							int index = 0;
							foreach(XElement weapon in element.Elements("WeaponArray"))
							{
								if(weapon.HasAttributes && weapon.Attribute("index") != null)
									index = int.Parse(weapon.Attribute("index").Value);

								if (weapon.HasAttributes && weapon.Attribute("Link") != null)
								{
									if (WeaponArray.ContainsKey(index))
										WeaponArray[index] = weapon.Attribute("Link").Value;
									else
										WeaponArray.Add(index, weapon.Attribute("Link").Value);
								}

								if (weapon.HasAttributes && WeaponArray.ContainsKey(index) && weapon.Attribute("removed") != null && weapon.Attribute("removed").Value != "0")
									WeaponArray.Remove(index);

								index++;
							}
							break;
						}
					}
				}
			}
			foreach (string wep in WeaponArray.Values)
				Weapons.Add(new WeaponData(map, wep));

		}
	}

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

	public class Module
	{
		protected string _FileName;
		protected string _FilePath;
		protected string _SubPath;
		protected List<Module> _Dependencies;
		protected Dictionary<string, DataFile> _DataFiles;
		public string FileName
		{
			get { return _FileName; }
		}
		public string SubPath
		{
			get { return _SubPath; }
		}
		public List<Module> Dependencies
		{
			get { return _Dependencies; }
		}
		public Dictionary<string, DataFile> DataFiles
		{
			get { return _DataFiles; }
		}

		public Module()
		{
			_DataFiles = new Dictionary<string, DataFile>();
			_Dependencies = new List<Module>();
			_FileName = "";
			_SubPath = "";
		}
		public Module(string fileName) : this()
		{
			Parse(fileName);
		}
		public void Parse(string fileName)
		{
			try
			{
				if (fileName == null)
					fileName = string.Empty;

				MpqManager manager = null;

				if (File.Exists(fileName))
				{
					manager = new MpqManager(fileName);
					_FileName = fileName;
				}
				else if (Directory.Exists(fileName))
				{
					_FileName = fileName;
				}
				else
				{
					if (fileName.StartsWith("Mods\\") || fileName.StartsWith("Campaigns\\"))
					{
						fileName = GameData.SC2FilePath.Remove(GameData.SC2FilePath.LastIndexOf("Versions")) + fileName;
						if (File.Exists(fileName))
						{
							manager = new MpqManager(fileName);
							_FileName = fileName;
						}
						else if (Directory.Exists(fileName))
						{
							_FileName = fileName;
						}
					}
					if (fileName.Contains("Maps\\"))
					{
						string Folder = fileName.Remove(fileName.IndexOf("Maps")) + "Campaigns";
						string Path = fileName.Remove(0, fileName.IndexOf("Maps\\")).Replace('/', '\\') + "\\";
						foreach (string s in Directory.GetFiles(Folder, "*.SC2Maps", SearchOption.AllDirectories))
						{
							manager = new MpqManager(s);
							if (manager.MpqArchive.FileExists(Path + "DocumentInfo"))
							{
								_FileName = s;
								_SubPath = Path;
								break;
							}
							else
							{
								manager.Close();
								manager = null;
							}
						}
					}
				}

				if (manager != null)
				{
					string DocumentInfo = System.Text.Encoding.UTF8.GetString(manager.read(_SubPath + "DocumentInfo"));
					XElement DocInfo = XElement.Parse(DocumentInfo);

					foreach (XElement dependency in DocInfo.Element("Dependencies").Elements("Value"))
					{
						string Value = dependency.Value;
						_Dependencies.Add(new Module(Value.Remove(0, Value.IndexOf("file:") + 5).Replace('/', '\\')));
					}
				}
				else if(_FileName.Contains("Swarm") && (File.Exists(_FileName.Replace("Swarm", "Liberty")) || Directory.Exists(_FileName.Replace("Swarm", "Liberty"))))
					_Dependencies.Add(new Module(_FileName.Replace("Swarm", "Liberty")));

				string GameDataPath = _SubPath + "Base.SC2Data\\GameData\\";
				if (manager == null && File.Exists(_FileName + "\\Base.SC2Data"))
				{
					manager = new MpqManager(_FileName + "\\Base.SC2Data");
					GameDataPath = _SubPath + "GameData\\";
				}

				if (manager != null)
				{
					foreach (MpqEntry entry in manager.MpqArchive)
					{
						if (entry.Filename != null && entry.Filename.StartsWith(GameDataPath) && entry.Filename.EndsWith(".xml"))
						{
							string Name = entry.Filename.Replace(GameDataPath, "").Replace(".xml", "");
							string DataFile = System.Text.Encoding.UTF8.GetString(manager.read(entry.Filename));
							_DataFiles.Add(Name, new DataFile(Name, DataFile));
						}
					}

					manager.Close();
				}
			}
			catch (Exception ex)
			{ }
		}

		public Dictionary<string, List<DataFile>> GetPrioritizedDataFiles()
		{
			Dictionary<string, List<DataFile>> ReturnVal = new Dictionary<string, List<DataFile>>();

			for (int i = _Dependencies.Count - 1; i >= 0; i--)
			{
				Dictionary<string, List<DataFile>> DependencyFiles = _Dependencies[i].GetPrioritizedDataFiles();
				foreach (string key in DependencyFiles.Keys)
				{
					if (!ReturnVal.ContainsKey(key))
						ReturnVal.Add(key, new List<DataFile>());
					ReturnVal[key].AddRange(DependencyFiles[key]);
				}
			}

			foreach (KeyValuePair<string, DataFile> dataFile in _DataFiles)
			{
				if (!ReturnVal.ContainsKey(dataFile.Key))
					ReturnVal.Add(dataFile.Key, new List<DataFile>());

				ReturnVal[dataFile.Key].Add(dataFile.Value);
			}

			return ReturnVal;
		}
	}

	public class MapData : Module
	{
		Dictionary<string, List<DataFile>> _ProcessedDataFiles;

		public Dictionary<string, List<DataFile>> ProcessedDataFiles
		{
			get { return _ProcessedDataFiles; }
		}

		public MapData() : base()
		{
			_ProcessedDataFiles = new Dictionary<string, List<DataFile>>();
		}
		public MapData(string fileName) : this()
		{
			Parse(fileName);
		}
		public void Parse(string fileName)
		{
			base.Parse(fileName);
			_ProcessedDataFiles = base.GetPrioritizedDataFiles();
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
				List<DataFile> files = _ProcessedDataFiles["ActorData"];
				for(int i = files.Count - 1; i >= 0; i--)
				{
					foreach (XElement element in files[i].Data)
					{
						if (element.HasAttributes 
							&& (id != "" && element.Attribute("id") != null && element.Attribute("id").Value == id
							|| id == "" && element.Attribute("unitName") != null && element.Attribute("unitName").Value == unit))
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

			if(unit.Contains("Burrowed"))
				return GetUnitPictureFilename(unit.Replace("Burrowed", ""));

			if (parent != "")
				return GetUnitPictureFilename(parent);

			return "Assets\\Textures\\btn-missing-kaeo.dds";
		}
	}

	
}