using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Utilities.MemoryHandling;

namespace Data
{
	static class ImprovedParse
	{
		public static int Parse(string value)
		{
			if (value.Contains("0x"))
			{
				value = value.Replace("0x", "");
				return Convert.ToInt32(value, 16);
			}
			else
				return Convert.ToInt32(value, 10);
		}
	}

	public class OffsetReader //obviously not complete. This will be used for reading offsets from a file instead of doing a pattern scan every time.
	{
		private ReadWriteMemory _mem;
		private ReadWriteMemory mem
		{
			get
			{
				if (_mem == null)
				{
					_mem = new ReadWriteMemory(GameData.SC2Handle);
				}
				return _mem;
			}
		}

		XElement _File;
		List<ORArray> _Arrays;
		List<ORStruct> _Structs;
		string _Version;
		string Version
		{
			get
			{
				if (_Version != null)
					return _Version;
				else
					return "Offsets file not parsed.";
			}
		}
		
		public OffsetReader(string Filename)
		{
			_Arrays = new List<ORArray>();
			_Structs = new List<ORStruct>();

			FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader sr = new StreamReader(fs, Encoding.UTF8);
			_File = XElement.Parse(sr.ReadToEnd());
			sr.Close();
			if (_File.HasAttributes && _File.Attribute("Version") != null)
				_Version = _File.Attribute("Version").Value;
			else
				_Version = "-1.-1.-1.-1";

			if (_File.HasElements && _File.Element("Array") != null)
			{
				foreach (XElement element in _File.Elements("Array"))
					_Arrays.Add(new ORArray(element));
			}
			if (_File.HasElements && _File.Element("Struct") != null)
			{
				foreach (XElement element in _File.Elements("Struct"))
					_Structs.Add(new ORStruct(element));
			}
		}
	}

	class ORStructMember
	{
		public string name
		{ get; set; }
		public Type type
		{ get; set; }
		public uint size
		{ get; set; }
		public int offset
		{ get; set; }
		
		public ORStructMember(XElement data, List<ORStructMember> others)
		{
			name = data.Attribute("Name").Value;
			offset = 0;

			string Offset = data.Attribute("Offset").Value;
			if (!Offset.Contains('+'))
				offset = ImprovedParse.Parse(Offset);
			else
			{
				string[] Split = Offset.Split('+');
				if (Split.Length == 2)
				{
					int BaseOffset = 0;
					foreach (ORStructMember member in others)
					{
						if (member.name == Split[0])
						{
							BaseOffset = member.offset;
							break;
						}
					}

					offset = BaseOffset + ImprovedParse.Parse(Split[1]);
				}
			}

			uint Size = (uint)ImprovedParse.Parse(data.Attribute("Size").Value);
			string Type = data.Attribute("Type").Value;
			switch (Type)
			{
				case "Signed":
					size = 1;
					switch (Size)
					{
						case 1:
							type = typeof(sbyte);
							break;
						case 2:
							type = typeof(Int16);
							break;
						case 4:
							type = typeof(Int32);
							break;
						case 8:
							type = typeof(Int64);
							break;
						default:
							type = typeof(byte);
							break;
					}
					break;
				case "Unsigned":
					size = 1;
					switch (Size)
					{
						case 1:
							type = typeof(byte);
							break;
						case 2:
							type = typeof(UInt16);
							break;
						case 4:
							type = typeof(UInt32);
							break;
						case 8:
							type = typeof(UInt64);
							break;
						default:
							type = typeof(byte);
							break;
					}
					break;
				case "Fixed":
					size = 1;
					switch (Size)
					{
						case 1:
							type = typeof(fixed8);
							break;
						case 2:
							type = typeof(fixed16);
							break;
						case 4:
							type = typeof(fixed32);
							break;
						default:
							type = typeof(byte);
							break;
					}
					break;
				case "String":
					type = typeof(String);
					size = Size;
					break;
				case "Bool":
					type = typeof(bool);
					size = 1;
					break;
				default:
					type = typeof(byte);
					size = Size;
					break;
			}
		}
	}

	class ORStruct
	{
		public string name
		{ get; set; }
		public int size
		{ get; set; }
		public List<ORStructMember> members
		{ get; set; }

		public ORStruct(XElement data)
		{
			name = data.Attribute("Name").Value;
			size = ImprovedParse.Parse(data.Attribute("Size").Value);
			members = new List<ORStructMember>();
			foreach (XElement member in data.Elements("Member"))
			{
				members.Add(new ORStructMember(member, members));
			}
		}
	}

	class ORArray
	{
		public string name
		{ get; set; }
		public int size
		{ get; set; }
		public int address
		{ get; set; }
		public string type
		{ get; set; }

		public ORArray(XElement data)
		{
			name = data.Attribute("Name").Value;
			size = ImprovedParse.Parse(data.Attribute("Size").Value);
			address = ImprovedParse.Parse(data.Attribute("Address").Value);
			type = data.Attribute("Type").Value;
		}
	}
}
