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
				where el.Attribute("Name").Value == "Timer2"
				select el;
			Current.ElementAt(0).Attribute("Address").Value = "0x" + GameData.ps.Timer2().ToString("X");
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

			try
			{
				File.Save(Filename);
			}
			catch (IOException)
			{
				System.Windows.Forms.MessageBox.Show("Failed to save Offsets.xml after updating it. It may be in use by another process, or the program may not have permission to write to that location. \nPlease close any other programs that may be using it and start SCIIEMH again.",
					"Failed to update offsets", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				System.Windows.Forms.Application.Exit();
			}
		}
	}

	public class OffsetReader
	{
		private Type StringType = typeof(string);
		private ReadWriteMemory _mem;
		private ReadWriteMemory mem
		{
			get
			{
				if (!GameData.SC2Opened)
				{
					_mem = new ReadWriteMemory(0);
				}
				else if (_mem == null || _mem.m_lpHandle == IntPtr.Zero)
				{
					_mem = new ReadWriteMemory(GameData.SC2Handle);
				}
				return _mem;
			}
		}

		XDocument _File;
		XElement _BaseElement;
		Dictionary<ORNames, ORArray> _Arrays;
		Dictionary<ORNames, ORStruct> _Structs;
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

		public int GetStructSize(ORNames Struct)
		{
			return _Structs[Struct].size;
		}
		public int GetStructAddress(ORNames Struct)
		{
			return _Structs[Struct].address;
		}

		public Type GetStructMemberType(ORNames Struct, ORNames Member)
		{
			return _Structs[Struct].members[Member].type;
		}

		public int GetStructMemberOffset(ORNames Struct, ORNames Member)
		{
			return _Structs[Struct].members[Member].offset;
		}

		public int GetStructMemberSize(ORNames Struct, ORNames Member)
		{
			return _Structs[Struct].members[Member].size;
		}

		public ORNames GetArrayType(ORNames Array)
		{
			return _Arrays[Array].type;
		}
		public int GetArrayCount(ORNames Array)
		{
			return  _Arrays[Array].size;
		}
		public int GetArrayTotalSize(ORNames Array)
		{
			return GetArrayCount(Array) * GetArrayElementSize(Array);
		}
		public int GetArrayAddress(ORNames Array)
		{
			return _Arrays[Array].address;
		}

		public int GetArrayElementAddress(ORNames Array, int Index)
		{
			return _Arrays[Array].address + Index * _Structs[GetArrayType(Array)].size;
		}

		public int GetArrayElementSize(ORNames Array)
		{
			return _Structs[GetArrayType(Array)].size;
		}

		public int GetArrayElementMemberAddress(ORNames Array, int Index, ORNames Member)
		{
			return GetArrayElementAddress(Array, Index) + GetStructMemberOffset(GetArrayType(Array), Member);
		}

		public Type GetArrayElementMemberType(ORNames Array, ORNames Member)
		{
			return GetStructMemberType(GetArrayType(Array), Member);
		}

		public int GetArrayElementMemberSize(ORNames Array, ORNames Member)
		{
			return GetStructMemberSize(GetArrayType(Array), Member);
		}

		public Object ReadArrayElementMember(ORNames Array, int Index, ORNames Member)
		{
			if (GetArrayElementMemberType(Array, Member) == StringType)
			{
				int Length = GetArrayElementMemberSize(Array, Member);
				byte[] buffer = new byte[Length];
				mem.ReadMemory((IntPtr)GetArrayElementMemberAddress(Array, Index, Member), Length, out buffer);
				return System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
			}
			else
				return mem.ReadMemory((uint)GetArrayElementMemberAddress(Array, Index, Member), GetArrayElementMemberType(Array, Member));
		}

		public bool WriteArrayElementMember(ORNames array, int Index, ORNames Member, Object NewValue)
		{
			if (GetArrayElementMemberType(array, Member) == StringType)
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

		public Object ReadStructMember(ORNames Struct, ORNames Member, int Address = 0)
		{
			if (Address == 0)
				Address = GetStructAddress(Struct);

			if (GetStructMemberType(Struct, Member) == StringType)
			{
				int Length = GetStructMemberSize(Struct, Member);
				byte[] buffer = new byte[Length];
				mem.ReadMemory((IntPtr)(GetStructMemberOffset(Struct, Member) + Address), Length, out buffer);
				return System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
			}
			else
				return mem.ReadMemory((uint)(GetStructMemberOffset(Struct, Member) + Address), GetStructMemberType(Struct, Member));
		}

		public Object ReadStructMember(ORNames Struct, ORNames Member, byte[] Data)
		{
			int Offset = GetStructMemberOffset(Struct, Member);
			int Size = GetStructMemberSize(Struct, Member);
			Type type = GetStructMemberType(Struct, Member);

			if (Data == null)
				throw new NullReferenceException("OffsetReader.ReadStructMember: Data was null. Struct: " + Struct
					+ " Member: " + Member);

			if (Data.Length < Offset + Size)
			{
				char[] hex = new char[16]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
				string HexData = string.Empty;
				foreach (byte b in Data)
				{
					HexData += hex[b >> 4];
					HexData += hex[b & 0x0F] + " ";
				}

				throw new IndexOutOfRangeException("OffsetReader.ReadStructMember: Data was too small to read all data.\nData.Length: "
					+ Data.Length.ToString() + " Offset: " + Offset.ToString() + " Size: " + Size.ToString() + " Struct: " + Struct.ToString()
					+ " Member: " + Member.ToString() + "\n Data: " + HexData);
			}

			if (type == StringType)
			{
				return System.Text.Encoding.UTF8.GetString(Data, Offset, Size).TrimEnd('\0');
			}
			else
				return ReadWriteMemory.RawDeserialize(Data, Offset, type);
		}

		public bool WriteStructMember(ORNames Struct, ORNames Member, Object NewValue, int Address = 0)
		{
			if (Address == 0 )
				Address = GetStructAddress(Struct);

			if (GetStructMemberType(Struct, Member) == StringType)
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

		public byte[] ReadArrayElement(ORNames Array, int Index)
		{
			int Address = GetArrayElementAddress(Array, Index);
			int Size = GetArrayElementSize(Array);
			
			byte[] buffer = new byte[Size];
			mem.ReadMemory((IntPtr)Address, Size, out buffer);
			return buffer;
		}
		
		public OffsetReader(string Filename)
		{
			Parse(Filename);
		}

		private void Parse(string Filename)
		{
			_Arrays = new Dictionary<ORNames, ORArray>();
			_Structs = new Dictionary<ORNames, ORStruct>();

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
					_Arrays.Add((ORNames)Enum.Parse(typeof(ORNames), element.Attribute("Name").Value), new ORArray(element));
			}
			if (_BaseElement.HasElements && _BaseElement.Element("Struct") != null)
			{
				foreach (XElement element in _BaseElement.Elements("Struct"))
					_Structs.Add((ORNames)Enum.Parse(typeof(ORNames), element.Attribute("Name").Value), new ORStruct(element));
			}
		}
	}

	class ORStructMember
	{
		public ORNames name
		{ get; set; }
		public Type type
		{ get; set; }
		public int size
		{ get; set; }
		public int offset
		{ get; set; }
		
		public ORStructMember(XElement data, Dictionary<ORNames, ORStructMember> others)
		{
			name = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Name").Value);
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
					if(others.ContainsKey((ORNames)Enum.Parse(typeof(ORNames), Split[0])))
						BaseOffset = others[(ORNames)Enum.Parse(typeof(ORNames), Split[0])].offset;

					offset = BaseOffset + ImprovedParse.Parse(Split[1]);
				}
			}

			int Size = ImprovedParse.Parse(data.Attribute("Size").Value);
			string Type = data.Attribute("Type").Value;
			size = Size;
			switch (Type)
			{
				case "Signed":
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
					type = typeof(string);
					break;
				case "Bool":
					type = typeof(bool);
					break;
				default:
					type = typeof(byte);
					break;
			}
		}
	}

	class ORStruct
	{
		public ORNames name
		{ get; set; }
		public int size
		{ get; set; }
		public int address
		{ get; set; }
		public Dictionary<ORNames, ORStructMember> members
		{ get; set; }

		public ORStruct(XElement data)
		{
			name = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Name").Value);
			size = ImprovedParse.Parse(data.Attribute("Size").Value);
			if (data.Attribute("Address") != null)
				address = ImprovedParse.Parse(data.Attribute("Address").Value);
			else
				address = 0;
			members = new Dictionary<ORNames, ORStructMember>();
			foreach (XElement member in data.Elements("Member"))
			{
				members.Add((ORNames)Enum.Parse(typeof(ORNames), member.Attribute("Name").Value), new ORStructMember(member, members));
			}
		}
	}

	class ORArray
	{
		public ORNames name
		{ get; set; }
		public int size
		{ get; set; }
		public int address
		{ get; set; }
		public ORNames type
		{ get; set; }

		public ORArray(XElement data)
		{
			name = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Name").Value);
			size = ImprovedParse.Parse(data.Attribute("Size").Value);
			address = ImprovedParse.Parse(data.Attribute("Address").Value);
			type = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Type").Value);
		}
	}
}
