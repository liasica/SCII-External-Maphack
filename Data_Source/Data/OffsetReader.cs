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
		Dictionary<string, ORArray> _Arrays;
		Dictionary<string, ORStruct> _Structs;
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


		public Type GetStructMemberType(string StructDotMember)
		{
			string[] Split = StructDotMember.Split('.');
			return GetStructMemberType(Split[0], Split[1]);
		}
		public Type GetStructMemberType(string Struct, string Member)
		{
			return _Structs[Struct].members[Member].type;
		}

		public int GetStructMemberOffset(string StructMember)
		{
			string[] Split = StructMember.Split('.');
			return GetStructMemberOffset(Split[0], Split[1]);
		}
		public int GetStructMemberOffset(string Struct, string Member)
		{
			return _Structs[Struct].members[Member].offset;
		}

		public string GetArrayType(string Array)
		{
			return _Arrays[Array].type;
		}

		public int GetArrayElementAddress(string ArrayElement)
		{
			string[] Split = ArrayElement.Split('[', ']');
			return GetArrayElementAddress(Split[0], ImprovedParse.Parse(Split[1]));
		}
		public int GetArrayElementAddress(string Array, int Index)
		{
			return _Arrays[Array].address + Index * _Structs[GetArrayType(Array)].size;
		}

		public int GetArrayElementMemberAddress(string ArrayElementMember)
		{
			char[] splitters = new char[] { '[', ']', '.' };
			string[] Split = ArrayElementMember.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
			return GetArrayElementMemberAddress(Split[0], ImprovedParse.Parse(Split[1]), Split[2]);
		}
		public int GetArrayElementMemberAddress(string Array, int Index, string Member)
		{
			return GetArrayElementAddress(Array, Index) + GetStructMemberOffset(GetArrayType(Array), Member);
		}

		public Type GetArrayElementMemberType(string ArrayElementMember)
		{
			string[] Split = ArrayElementMember.Split('.');
			return GetArrayElementMemberType(Split[0], Split[1]);
		}
		public Type GetArrayElementMemberType(string Array, string Member)
		{
			return GetStructMemberType(GetArrayType(Array), Member);
		}

		public Object ReadArrayElementMember(string ArrayElementMember)
		{
			char[] splitters = new char[] { '[', ']', '.' };
			string[] Split = ArrayElementMember.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
			return ReadArrayElementMember(Split[0], ImprovedParse.Parse(Split[1]), Split[2]);
		}
		public Object ReadArrayElementMember(string Array, int Index, string Member)
		{
			return mem.ReadMemory((uint)GetArrayElementMemberAddress(Array, Index, Member), GetArrayElementMemberType(Array, Member));
		}

		public bool WriteArrayElementMember(string ArrayElementMember, Object NewValue)
		{
			char[] splitters = new char[] { '[', ']', '.' };
			string[] Split = ArrayElementMember.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
			return WriteArrayElementMember(Split[0], ImprovedParse.Parse(Split[1]), Split[2], NewValue);
		}
		public bool WriteArrayElementMember(string Array, int Index, string Member, Object NewValue)
		{
			byte[] buffer = ReadWriteMemory.RawSerialize(NewValue);
			return mem.WriteMemory((uint)GetArrayElementMemberAddress(Array, Index, Member), buffer.Length, ref buffer);
		}
		
		public OffsetReader(string Filename)
		{
			_Arrays = new Dictionary<string, ORArray>();
			_Structs = new Dictionary<string, ORStruct>();

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
					_Arrays.Add(element.Attribute("Name").Value, new ORArray(element));
			}
			if (_File.HasElements && _File.Element("Struct") != null)
			{
				foreach (XElement element in _File.Elements("Struct"))
					_Structs.Add(element.Attribute("Name").Value, new ORStruct(element));
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
		
		public ORStructMember(XElement data, Dictionary<string, ORStructMember> others)
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
					if(others.ContainsKey(Split[0]))
						BaseOffset = others[Split[0]].offset;

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
		public Dictionary<string, ORStructMember> members
		{ get; set; }

		public ORStruct(XElement data)
		{
			name = data.Attribute("Name").Value;
			size = ImprovedParse.Parse(data.Attribute("Size").Value);
			members = new Dictionary<string, ORStructMember>();
			foreach (XElement member in data.Elements("Member"))
			{
				members.Add(member.Attribute("Name").Value, new ORStructMember(member, members));
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
