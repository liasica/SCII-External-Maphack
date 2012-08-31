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

	static class UpdateFile
	{
		public static void Update(string Filename)
		{
			XDocument File = XDocument.Load(Filename);
			File.Root.Attribute("Version").Value = GameData.SC2Version;
			IEnumerable<XElement> Current =
				from el in File.Root.Elements("Array")
				where el.Attribute("Name").Value == "Players"
				select el;
			Current.ElementAt(0).Attribute("Address").Value = "0x" + GameData.ps.PlayerStruct().ToString("X");
			Current =
				from el in File.Root.Elements("Array")
				where el.Attribute("Name").Value == "Units"
				select el;
			Current.ElementAt(0).Attribute("Address").Value = "0x" + GameData.ps.UnitStruct().ToString("X");
			Current =
				from el in File.Root.Elements("Struct")
				where el.Attribute("Name").Value == "Timer"
				select el;
			Current.ElementAt(0).Attribute("Address").Value = "0x" + GameData.ps.Timer().ToString("X");
			Current =
				from el in File.Root.Elements("Struct")
				where el.Attribute("Name").Value == "MapInfo"
				select el;
			Current.ElementAt(0).Attribute("Address").Value = "0x" + GameData.ps.MapInfoPtr().ToString("X");
			Current =
				from el in File.Root.Elements("Struct")
				where el.Attribute("Name").Value == "Player"
				select el;
			Current.ElementAt(0).Attribute("Size").Value = "0x" + GameData.ps.PlayerStructSize().ToString("X");
			Current =
				from el in File.Root.Elements("Struct")
				where el.Attribute("Name").Value == "LocalPlayer"
				select el;
			Current.ElementAt(0).Attribute("Address").Value = "0x" + GameData.ps.LocalPlayerNumber().ToString("X");

			File.Save(Filename);
		}
	}

	public class OffsetReader
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

		XDocument _File;
		XElement _BaseElement;
		Dictionary<string, ORArray> _Arrays;
		Dictionary<string, ORStruct> _Structs;
		string _Filename;
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

		public bool CheckVersion()
		{
			return GameData.SC2Version == Version;
		}

		public void UpdateAddresses()
		{
			UpdateFile.Update(_Filename);
		}

		public int GetStructSize(string Struct)
		{
			return _Structs[Struct].size;
		}
		public int GetStructAddress(string Struct)
		{
			return _Structs[Struct].address;
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

		public int GetStructMemberSize(string StructMember)
		{
			string[] Split = StructMember.Split('.');
			return GetStructMemberSize(Split[0], Split[1]);
		}
		public int GetStructMemberSize(string Struct, string Member)
		{
			return _Structs[Struct].members[Member].size;
		}

		public string GetArrayType(string Array)
		{
			return _Arrays[Array].type;
		}
		public int GetArrayCount(string Array)
		{
			return  _Arrays[Array].size;
		}
		public int GetArrayTotalSize(string Array)
		{
			return GetArrayCount(Array) * GetArrayElementSize(Array);
		}
		public int GetArrayAddress(string Array)
		{
			return _Arrays[Array].address;
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

		public int GetArrayElementSize(string Array)
		{
			return _Structs[GetArrayType(Array)].size;
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

		public int GetArrayElementMemberSize(string ArrayElementMember)
		{
			string[] Split = ArrayElementMember.Split('.');
			return GetArrayElementMemberSize(Split[0], Split[1]);
		}
		public int GetArrayElementMemberSize(string Array, string Member)
		{
			return GetStructMemberSize(GetArrayType(Array), Member);
		}

		public Object ReadArrayElementMember(string ArrayElementMember)
		{
			char[] splitters = new char[] { '[', ']', '.' };
			string[] Split = ArrayElementMember.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
			return ReadArrayElementMember(Split[0], ImprovedParse.Parse(Split[1]), Split[2]);
		}
		public Object ReadArrayElementMember(string Array, int Index, string Member)
		{
			if (GetArrayElementMemberType(Array, Member) == typeof(string))
			{
				int Length = GetArrayElementMemberSize(Array, Member);
				byte[] buffer = new byte[Length];
				mem.ReadMemory((IntPtr)GetArrayElementMemberAddress(Array, Index, Member), Length, out buffer);
				return System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
			}
			else
				return mem.ReadMemory((uint)GetArrayElementMemberAddress(Array, Index, Member), GetArrayElementMemberType(Array, Member));
		}

		public bool WriteArrayElementMember(string ArrayElementMember, Object NewValue)
		{
			char[] splitters = new char[] { '[', ']', '.' };
			string[] Split = ArrayElementMember.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
			return WriteArrayElementMember(Split[0], ImprovedParse.Parse(Split[1]), Split[2], NewValue);
		}
		public bool WriteArrayElementMember(string array, int Index, string Member, Object NewValue)
		{
			if (GetArrayElementMemberType(array, Member) == typeof(string))
			{
				int Length = GetArrayElementMemberSize(array, Member);
				byte[] StringAsBytes = Encoding.UTF8.GetBytes((string)NewValue);
				byte[] buffer = new byte[Length];
				Array.Copy(StringAsBytes, buffer, StringAsBytes.Length < buffer.Length - 1 ? StringAsBytes.Length : buffer.Length - 1);
				buffer[buffer.Length - 1] = 0;

				return mem.WriteMemory((uint)GetArrayElementMemberAddress(array, Index, Member), buffer.Length, ref buffer);
			}
			else
			{
				byte[] buffer = ReadWriteMemory.RawSerialize(NewValue);
				return mem.WriteMemory((uint)GetArrayElementMemberAddress(array, Index, Member), buffer.Length, ref buffer);
			}
		}

		public Object ReadStructMember(string Struct, string Member, int Address = 0)
		{
			if (Address == 0)
				Address = GetStructAddress(Struct);

			if (GetStructMemberType(Struct, Member) == typeof(string))
			{
				int Length = GetStructMemberSize(Struct, Member);
				byte[] buffer = new byte[Length];
				mem.ReadMemory((IntPtr)(GetStructMemberOffset(Struct, Member) + Address), Length, out buffer);
				return System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
			}
			else
				return mem.ReadMemory((uint)(GetStructMemberOffset(Struct, Member) + Address), GetStructMemberType(Struct, Member));
		}

		public bool WriteStructMember(string Struct, string Member, Object NewValue, int Address = 0)
		{
			if (Address == 0 )
				Address = GetStructAddress(Struct);

			if (GetStructMemberType(Struct, Member) == typeof(string))
			{
				int Length = GetStructMemberSize(Struct, Member);
				byte[] StringAsBytes = Encoding.UTF8.GetBytes((string)NewValue);
				byte[] buffer = new byte[Length];
				Array.Copy(StringAsBytes, buffer, StringAsBytes.Length < buffer.Length - 1 ? StringAsBytes.Length : buffer.Length - 1);
				buffer[buffer.Length - 1] = 0;

				return mem.WriteMemory((uint)(GetStructMemberOffset(Struct, Member) + Address), buffer.Length, ref buffer);
			}
			else
			{
				byte[] buffer = ReadWriteMemory.RawSerialize(NewValue);
				return mem.WriteMemory((uint)(GetStructMemberOffset(Struct, Member) + Address), buffer.Length, ref buffer);
			}
		}
		
		public OffsetReader(string Filename)
		{
			Parse(Filename);
		}

		private void Parse(string Filename)
		{
			_Arrays = new Dictionary<string, ORArray>();
			_Structs = new Dictionary<string, ORStruct>();

			_Filename = Filename;

			_File = XDocument.Load(Filename);
			_BaseElement = _File.Root;
			if (_BaseElement.HasAttributes && _BaseElement.Attribute("Version") != null)
				_Version = _BaseElement.Attribute("Version").Value;
			else
				_Version = "-1.-1.-1.-1";

			if (_BaseElement.HasElements && _BaseElement.Element("Array") != null)
			{
				foreach (XElement element in _BaseElement.Elements("Array"))
					_Arrays.Add(element.Attribute("Name").Value, new ORArray(element));
			}
			if (_BaseElement.HasElements && _BaseElement.Element("Struct") != null)
			{
				foreach (XElement element in _BaseElement.Elements("Struct"))
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
		public int size
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

			int Size = ImprovedParse.Parse(data.Attribute("Size").Value);
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
		public int address
		{ get; set; }
		public Dictionary<string, ORStructMember> members
		{ get; set; }

		public ORStruct(XElement data)
		{
			name = data.Attribute("Name").Value;
			size = ImprovedParse.Parse(data.Attribute("Size").Value);
			if (data.Attribute("Address") != null)
				address = ImprovedParse.Parse(data.Attribute("Address").Value);
			else
				address = 0;
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
