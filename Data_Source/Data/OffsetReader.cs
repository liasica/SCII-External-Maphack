using System;
using System.Diagnostics;
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
		public static long Parse(string value)
		{
			if (value.Contains("0x"))
			{
				value = value.Replace("0x", "");
				return Convert.ToInt64(value, 16);
			}
			else
				return Convert.ToInt64(value, 10);
		}
	}

	static class UpdateFile
	{
		static object Lock = new object();

		public static void Update(string Filename)
		{
			lock (Lock) //There will be significant problems if multiple threads try to write the new file at the same time. Todo: Skip the update on all but the first thread.
			{
				uint BaseAddress = 0;
				foreach (ProcessModule module in GameData.SC2Process.Modules)
				{
					if (module.ModuleName.Equals("SC2.exe", StringComparison.OrdinalIgnoreCase))
					{
						BaseAddress = (uint)module.BaseAddress;
						break;
					}
				}

				XDocument File = XDocument.Load(Filename);
				File.Root.Attribute("Version").Value = GameData.SC2Version;
				IEnumerable<XElement> Current =
					from el in File.Root.Elements("Array")
					where el.Attribute("Name").Value == "Players"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.PlayerStruct() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Array")
					where el.Attribute("Name").Value == "Units"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.UnitStruct() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Array")
					where el.Attribute("Name").Value == "CurrentSelections"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.PlayerSelection() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Array")
					where el.Attribute("Name").Value == "GalaxyDataTable"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.GalaxyDataTable() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "Timer"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.Timer() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "Timer2"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.Timer2() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "CameraBounds"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.MapBounds() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "MapInfo"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.MapInfoPtr() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "Player"
					select el;
				Current.ElementAt(0).Attribute("Size").Value = "0x" + GameData.ps.PlayerStructSize().ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "LocalPlayer"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.LocalPlayerNumber() - BaseAddress).ToString("X");
				Current =
					from el in File.Root.Elements("Struct")
					where el.Attribute("Name").Value == "Selection"
					select el;
				Current.ElementAt(0).Attribute("Address").Value = "SC2.exe+0x" + Math.Max(0, GameData.ps.LocalSelection() - BaseAddress).ToString("X");

				try
				{
					File.Save(Filename);
					OffsetReader temp = new OffsetReader(Filename);
					temp.File.Save(Filename);
				}
				catch (IOException)
				{
					System.Windows.Forms.MessageBox.Show("Failed to save Offsets.xml after updating it. It may be in use by another process, or the program may not have permission to write to that location. \nPlease close any other programs that may be using it and start SCIIEMH again.",
						"Failed to update offsets", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
					System.Windows.Forms.Application.Exit();
				}
			}
		}
	}

	public class OffsetReader
	{
		private Type StringType = typeof(string);
		private ReadWriteMemory mem
		{
			get
			{
				return GameData.mem;
			}
		}

		XDocument _File;
		public XDocument File
		{ get { return _File; } }
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
			return GameData.SC2Version == Version || GameData.SC2Version == "SC2 Closed";
		}

		public void UpdateAddresses()
		{
			string Version = GameData.SC2Version; // for debugging. This seems to trigger randomly, but GameData.SC2Version is always correct by the time I look.
			UpdateFile.Update(_Filename);
		}

		public uint GetAddressOfDataItem(int ItemID)
		{
			byte[] buffer;
			if (ItemID != 0 && ItemID != 65535 && (ushort)ItemID < 0x8000)
			{
				buffer = ReadArrayElement(ORNames.GalaxyDataTable, (ushort)ItemID);
				if ((ushort)ReadStructMember(ORNames.GalaxyTableEntry, ORNames.times_used, buffer) == (ushort)(ItemID >> 16))
					return (uint)ReadStructMember(ORNames.GalaxyTableEntry, ORNames.data_pointer, buffer);
			}
			return 0;
		}

		public string ReadString(uint Address)
		{
			if (Address <= 0)
				return null;

			uint Length = (uint)GameData.mem.ReadMemory(Address, typeof(uint));
			uint Flags = (uint)GameData.mem.ReadMemory(Address + 4, typeof(uint));
			uint StringAddress = Address;
			if (Length < 65536) //it needs an upper limit incase the length is garbage.
			{
				byte[] buffer = new byte[Length];
				if ((Flags & 4) == 0)
					StringAddress += 8;
				else
					StringAddress = (uint)GameData.mem.ReadMemory(StringAddress + 8, typeof(uint));

				if (StringAddress != 0)
				{
					GameData.mem.ReadMemory(StringAddress, (int)Length, out buffer);
					return System.Text.Encoding.UTF8.GetString(buffer);
				}
			}
			return null;
		}

		public int GetStructSize(ORNames Struct)
		{
			return _Structs[Struct].size;
		}
		public uint GetStructAddress(ORNames Struct)
		{
			return _Structs[Struct].address;
		}
		public int GetStructStartOffset(ORNames Struct)
		{
			return _Structs[Struct].startOffset;
		}
		public int GetStructEndOffset(ORNames Struct)
		{
			return _Structs[Struct].endOffset;
		}

		public Type GetStructMemberType(ORNames Struct, ORNames Member)
		{
			return _Structs[Struct].members[Member].type;
		}

		public int GetStructMemberOffset(ORNames Struct, ORNames Member)
		{
			return _Structs[Struct].members[Member].offset;
		}

		public uint GetStructMemberAddress(ORNames Struct, ORNames Member, uint Address = 0)
		{
			if (Address <= 0)
				Address = GetStructAddress(Struct);
			return (uint)(Address + GetStructMemberOffset(Struct, Member));
		}

		public int GetStructMemberSize(ORNames Struct, ORNames Member)
		{
			int Size = _Structs[Struct].members[Member].size;
			if (_Structs[Struct].members[Member].count > 0)
				return Size * _Structs[Struct].members[Member].count;
			return Size;
		}

		public int GetStructMemberCount(ORNames Struct, ORNames Member)
		{
			return _Structs[Struct].members[Member].count;
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
		public uint GetArrayAddress(ORNames Array)
		{
			return _Arrays[Array].address;
		}

		public uint GetArrayElementAddress(ORNames Array, int Index)
		{
			return (uint)(_Arrays[Array].address + Index * _Structs[GetArrayType(Array)].size);
		}

		public int GetArrayElementSize(ORNames Array)
		{
			return _Structs[GetArrayType(Array)].size;
		}

		public uint GetArrayElementMemberAddress(ORNames Array, int Index, ORNames Member)
		{
			return (uint)(GetArrayElementAddress(Array, Index) + GetStructMemberOffset(GetArrayType(Array), Member));
		}

		public Type GetArrayElementMemberType(ORNames Array, ORNames Member)
		{
			return GetStructMemberType(GetArrayType(Array), Member);
		}

		public int GetArrayElementMemberSize(ORNames Array, ORNames Member)
		{
			return GetStructMemberSize(GetArrayType(Array), Member);
		}

		public int GetArrayElementMemberCount(ORNames Array, ORNames Member)
		{
			return GetStructMemberCount(GetArrayType(Array), Member);
		}

		public byte[] ReadArrayElement(ORNames Array, int Index)
		{
			return ReadStruct(GetArrayType(Array), GetArrayElementAddress(Array, Index));
		}

		public bool WriteArrayElement(ORNames Array, int Index, byte[] Data)
		{
			return WriteStruct(GetArrayType(Array), Data, GetArrayElementAddress(Array, Index));
		}

		public Object ReadArrayElementMember(ORNames Array, int Index, ORNames Member)
		{
			return ReadStructMember(GetArrayType(Array), Member, GetArrayElementAddress(Array, Index));
		}

		public bool WriteArrayElementMember(ORNames Array, int Index, ORNames Member, Object NewValue)
		{
			return WriteStructMember(GetArrayType(Array), Member, NewValue, GetArrayElementAddress(Array, Index));
		}

		public byte[] ReadStruct(ORNames Struct, uint Address = 0)
		{
			if(Address <= 0)
				Address = GetStructAddress(Struct);
			int StartOffset = GetStructStartOffset(Struct);
			int EndOffset = GetStructEndOffset(Struct);
			int TotalSize = GetStructSize(Struct);
			int UsedSize = EndOffset - StartOffset;

			if (TotalSize < UsedSize + StartOffset)
				TotalSize = UsedSize + StartOffset;

			byte[] buffer;
			mem.ReadMemory(Address, TotalSize, out buffer);
			return buffer;
		}

		public bool WriteStruct(ORNames Struct, byte[] Data, uint Address = 0)
		{
			if(Address <= 0)
				Address = GetStructAddress(Struct);
			int Size = GetStructSize(Struct);

			if (Data.Length != Size)
				throw new OverflowException("OffsetReader.WriteStruct: Data.Length != Size. Data.Length: " + Data.Length.ToString() + " Size: " + Size.ToString());

			return mem.WriteMemory(Address, Data.Length, ref Data);
		}

		public Object ReadStructMember(ORNames Struct, ORNames Member, uint Address = 0)
		{
			if (Address <= 0)
				Address = GetStructAddress(Struct);
			
			
			Type type = GetStructMemberType(Struct, Member);
			if (type == StringType)
			{
				int Length = GetStructMemberSize(Struct, Member);
				byte[] buffer = new byte[Length];
				mem.ReadMemory((uint)(GetStructMemberOffset(Struct, Member) + Address), buffer.Length, out buffer);
				string ReturnValS = Encoding.UTF8.GetString(buffer);
				int NullPos = ReturnValS.IndexOf('\0');
				if(NullPos >= 0)
					ReturnValS = ReturnValS.Remove(NullPos);
				return ReturnValS;
			}

			int Count = GetStructMemberCount(Struct, Member);
			if (Count <= 0)
				return mem.ReadMemory((uint)(GetStructMemberOffset(Struct, Member) + Address), GetStructMemberType(Struct, Member));

			int Size = GetStructMemberSize(Struct, Member);
			byte[] Data = new byte[Size];
			mem.ReadMemory(Address, Data.Length, out Data);
			Array ReturnVal = Array.CreateInstance(type, Count);

			for (int i = 0; i < Count; i++)
			{
				ReturnVal.SetValue(ReadWriteMemory.RawDeserialize(Data, i * Size, type), i);
			}
			return ReturnVal;
		}

		public Object ReadStructMember(ORNames Struct, ORNames Member, byte[] Data)
		{
			int Offset = GetStructMemberOffset(Struct, Member);
			int Size = GetStructMemberSize(Struct, Member);
			int Count = GetStructMemberCount(Struct, Member);
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
				string ReturnValS = Encoding.UTF8.GetString(Data, Offset, Size);
				int NullPos = ReturnValS.IndexOf('\0');
				if (NullPos >= 0)
					ReturnValS = ReturnValS.Remove(NullPos);
				return ReturnValS;
			}
			if(Count <= 0)
				return ReadWriteMemory.RawDeserialize(Data, Offset, type);

			Array ReturnVal = Array.CreateInstance(type, Count);
			for (int i = 0; i < Count; i++)
			{
				ReturnVal.SetValue(ReadWriteMemory.RawDeserialize(Data, Offset + i * Size / Count, type), i);
			}
			return ReturnVal;
		}

		public bool WriteStructMember(ORNames Struct, ORNames Member, Object NewValue, uint Address = 0)
		{
			if (Address <= 0 )
				Address = GetStructAddress(Struct);
			int Offset = GetStructMemberOffset(Struct, Member);
			int Size = GetStructMemberSize(Struct, Member);
			int Count = GetStructMemberCount(Struct, Member);
			Type type = GetStructMemberType(Struct, Member);
			if (Count <= 0 && NewValue.GetType() != type)
				throw new InvalidCastException("OffsetReader.WriteStructMember: NewValue is not of the expected type. NewValue type: " + NewValue.GetType().ToString() + " Expected type: " + type.ToString());

			if (type == StringType)
			{
				int Length = Size;
				byte[] StringAsBytes = Encoding.UTF8.GetBytes((string)NewValue);
				byte[] buffer = new byte[Length];
				Array.Copy(StringAsBytes, buffer, StringAsBytes.Length < buffer.Length - 1 ? StringAsBytes.Length : buffer.Length - 1);
				buffer[buffer.Length - 1] = 0;
				return mem.WriteMemory((uint)(Offset + Address), buffer.Length, ref buffer);
			}
			if (Count <= 0)
			{
				byte[] buffer = ReadWriteMemory.RawSerialize(NewValue);
				return mem.WriteMemory((uint)(Offset + Address), buffer.Length, ref buffer);
			}
			else
			{
				Array NewArray = (Array)NewValue;
				byte[] buffer = new byte[Size];
				for (int i = 0; i < Count && i < NewArray.Length; i++)
				{
					ReadWriteMemory.RawSerialize(NewArray.GetValue(i)).CopyTo(buffer, i * Size / Count);
				}
				return mem.WriteMemory((uint)(Offset + Address), buffer.Length, ref buffer);
			}
		}

		public void WriteStructMember(ORNames Struct, ORNames Member, Object NewValue, ref byte[] Data)
		{
			int Offset = GetStructMemberOffset(Struct, Member);
			int Size = GetStructMemberSize(Struct, Member);
			int Count = GetStructMemberCount(Struct, Member);
			Type type = GetStructMemberType(Struct, Member);
			if (Count <= 0 && NewValue.GetType() != type)
				throw new InvalidCastException("OffsetReader.WriteStructMember: NewValue is not of the expected type. NewValue type: " + NewValue.GetType().ToString() + " Expected type: " + type.ToString());

			if (type == StringType)
			{
				int Length = Size;
				byte[] StringAsBytes = Encoding.UTF8.GetBytes((string)NewValue);
				int CopyLength = StringAsBytes.Length < Length - 1 ? StringAsBytes.Length : Length - 1;
				Array.Copy(StringAsBytes, 0, Data, Offset, CopyLength);
				Data[Offset + CopyLength] = 0;
				return;
			}
			if (Count <= 0)
			{
				byte[] buffer = ReadWriteMemory.RawSerialize(NewValue);
				buffer.CopyTo(Data, Offset);
				return;
			}
			else
			{
				Array NewArray = (Array)NewValue;
				byte[] buffer = new byte[Size];
				for (int i = 0; i < Count && i < NewArray.Length; i++)
				{
					ReadWriteMemory.RawSerialize(NewArray.GetValue(i)).CopyTo(buffer, i * Size / Count);
				}
				buffer.CopyTo(Data, Offset);
				return;
			}
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

			using (FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				_File = XDocument.Load(fs);
			}

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
		public int count
		{ get; set; }
		public int offset
		{ get; set; }
		
		public ORStructMember(XElement data, ORStruct parent)
		{
			name = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Name").Value);
			offset = 0;

			string Offset = data.Attribute("Offset").Value;
			if (!Offset.Contains('+'))
			{
				offset = (int)ImprovedParse.Parse(Offset);
				data.SetAttributeValue("AbsoluteOffset", null);
			}
			else
			{
				string[] Split = Offset.Split('+');
				if (Split.Length == 2)
				{
					int BaseOffset = 0;
					if (parent.members.ContainsKey((ORNames)Enum.Parse(typeof(ORNames), Split[0])))
						BaseOffset = parent.members[(ORNames)Enum.Parse(typeof(ORNames), Split[0])].offset;

					offset = BaseOffset + (int)ImprovedParse.Parse(Split[1]);
					data.SetAttributeValue("AbsoluteOffset", "0x" + offset.ToString("X"));
				}
			}

			

			int Size = (int)ImprovedParse.Parse(data.Attribute("Size").Value);

			if (offset < parent.startOffset)
				parent.startOffset = offset;
			if (offset + Size > parent.endOffset)
				parent.endOffset = offset + Size;

			string Type = data.Attribute("Type").Value;
			size = Size;
			if (data.Attribute("Count") != null)
				count = (int)ImprovedParse.Parse(data.Attribute("Count").Value);
			else
				count = -1;
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
		public uint address
		{ get; set; }
		public int startOffset
		{ get; set; }
		public int endOffset
		{ get; set; }
		public Dictionary<ORNames, ORStructMember> members
		{ get; set; }

		public ORStruct(XElement data)
		{
			startOffset = int.MaxValue;
			endOffset = int.MinValue;

			name = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Name").Value);
			size = (int)ImprovedParse.Parse(data.Attribute("Size").Value);
			if (data.Attribute("Address") != null)
			{
				string temp = data.Attribute("Address").Value;
				int PlusPos = -1;
				if ((PlusPos = temp.LastIndexOf('+')) >= 0)
				{
					address = (uint)ImprovedParse.Parse(temp.Substring(PlusPos + 1));

					string ModuleName = temp.Substring(0, PlusPos);
					foreach(ProcessModule module in GameData.SC2Process.Modules)
					{
						if (module.ModuleName.Equals(ModuleName, StringComparison.OrdinalIgnoreCase))
						{
							address += (uint)module.BaseAddress;
							break;
						}
					}
				}
				else
					address = (uint)ImprovedParse.Parse(data.Attribute("Address").Value);
			}
			else
				address = 0;
			members = new Dictionary<ORNames, ORStructMember>();
			foreach (XElement member in data.Elements("Member"))
			{
				members.Add((ORNames)Enum.Parse(typeof(ORNames), member.Attribute("Name").Value), new ORStructMember(member, this));
			}
		}
	}

	class ORArray
	{
		public ORNames name
		{ get; set; }
		public int size
		{ get; set; }
		public uint address
		{ get; set; }
		public ORNames type
		{ get; set; }

		public ORArray(XElement data)
		{
			name = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Name").Value);
			size = (int)ImprovedParse.Parse(data.Attribute("Size").Value);
			type = (ORNames)Enum.Parse(typeof(ORNames), data.Attribute("Type").Value);

			string temp = data.Attribute("Address").Value;
			int PlusPos = -1;
			if ((PlusPos = temp.LastIndexOf('+')) >= 0)
			{
				address = (uint)ImprovedParse.Parse(temp.Substring(PlusPos + 1));

				string ModuleName = temp.Substring(0, PlusPos);
				foreach (ProcessModule module in GameData.SC2Process.Modules)
				{
					if (module.ModuleName.Equals(ModuleName, StringComparison.OrdinalIgnoreCase))
					{
						address += (uint)module.BaseAddress;
						break;
					}
				}
			}
			else
				address = (uint)ImprovedParse.Parse(data.Attribute("Address").Value);

		}
	}
}
