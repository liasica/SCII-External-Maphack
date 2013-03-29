using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
	public class Abilities
	{
		static Dictionary<string, uint> _Addresses = null;
		public static Dictionary<string, uint> Addresses
		{
			get
			{
				if (_Addresses == null)
					BuildAddressTable();
				return _Addresses;
			}
		}

		static Dictionary<uint, string> _Names = null;
		public static Dictionary<uint, string> Names
		{
			get
			{
				if (_Names == null)
					BuildAddressTable();
				return _Names;
			}
		}

		public static void Reset()
		{
			_Addresses = null;
			_Names = null;
		}

		public static void BuildAddressTable()
		{
			_Addresses = new Dictionary<string, uint>();
			_Names = new Dictionary<uint, string>();

			uint ptr = (uint)GameData.offsets.ReadStructMember(ORNames.AbilTable, ORNames.Ptr0);
			ptr = (uint)GameData.offsets.ReadStructMember(ORNames.AbilTable, ORNames.Ptr1, ptr);
			ptr = (uint)GameData.offsets.ReadStructMember(ORNames.AbilTable, ORNames.Ptr2, ptr);

			for (uint ptr2 = 0; (ptr2 = (uint)GameData.mem.ReadMemory(ptr, typeof(uint))) != 0; ptr += 4)
			{
				uint StringAddress = 0;
				if ((uint)GameData.mem.ReadMemory(ptr2, typeof(uint)) == 0 
					|| (StringAddress = (uint)GameData.offsets.ReadStructMember(ORNames.AbilTable, ORNames.NameStringOffset0, ptr2)) == 0)
					continue;
				string AbilityName = GameData.offsets.ReadString((GameData.offsets.GetStructMemberAddress(ORNames.AbilTable, ORNames.NameStringOffset1, StringAddress)));
				if(!_Addresses.ContainsKey(AbilityName))
				{
					_Addresses.Add(AbilityName, ptr2);
					_Names.Add(ptr2, AbilityName);
				}
			}
		}
	}
}
