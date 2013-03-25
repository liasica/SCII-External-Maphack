namespace Data
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Selection
	{
		private uint _Address;
		private byte[] _Data;

		public void Update()
		{
			_Data = GameData.offsets.ReadStruct(ORNames.Selection, _Address);
		}
		public bool WriteToSC2(uint Address = 0)
		{
			if (Address <= 0)
				Address = _Address;
			return GameData.offsets.WriteStruct(ORNames.Selection, _Data, Address);
		}


		public Selection(uint Address)
		{
			_Address = Address;
			_Data = null;
			Update();
		}


		public ushort TotalUnits
		{
			get
			{
				return (ushort)GameData.offsets.ReadStructMember(ORNames.Selection, ORNames.num_selected, _Data);
			}
			set
			{
				GameData.offsets.WriteStructMember(ORNames.Selection, ORNames.num_selected, value, ref _Data);
			}
		}
		public ushort TotalUnitTypes
		{
			get
			{
				return (ushort)GameData.offsets.ReadStructMember(ORNames.Selection, ORNames.num_types_selected, _Data);
			}
			set
			{
				GameData.offsets.WriteStructMember(ORNames.Selection, ORNames.num_types_selected, value, ref _Data);
			}
		}
		public ushort HighlightedType
		{
			get
			{
				return (ushort)GameData.offsets.ReadStructMember(ORNames.Selection, ORNames.highlighted_type, _Data);
			}
			set
			{
				GameData.offsets.WriteStructMember(ORNames.Selection, ORNames.highlighted_type, value, ref _Data);
			}
		}
		public ushort TotalPages
		{
			get
			{
				return (ushort)GameData.offsets.ReadStructMember(ORNames.Selection, ORNames.num_pages, _Data);
			}
			set
			{
				GameData.offsets.WriteStructMember(ORNames.Selection, ORNames.num_pages, value, ref _Data);
			}
		}

		public List<uint> SelectedUnitIDs
		{
			get
			{
				uint[] RawArray = (uint[])GameData.offsets.ReadStructMember(ORNames.Selection, ORNames.selected_IDs, _Data);
				List<uint> RawList = new List<uint>(RawArray);
				int Total = TotalUnits;
				if (RawList.Count > Total)
					RawList.RemoveRange(Total, RawList.Count - Total);
				RawList.RemoveAll(x => x == 0);
				RawList.TrimExcess();
				return RawList;
			}
			set
			{
				List<uint> temp = value;
				temp.RemoveAll(x => x == 0);
				TotalUnits = (ushort)temp.Count;
				GameData.offsets.WriteStructMember(ORNames.Selection, ORNames.selected_IDs, temp.ToArray(), ref _Data);
			}
		}

		public List<Unit> SelectedUnits
		{
			get
			{
				List<uint> IDList = SelectedUnitIDs;
				List<Unit> ReturnVal = new List<Unit>();
				foreach (uint u in IDList)
				{
					Unit temp = Unit.GetUnit(u);
					if(temp != null)
						ReturnVal.Add(temp);
				}
				return ReturnVal;
			}
			set
			{
				List<Unit> temp = value;
				temp.RemoveAll(x => x == null);

				uint[] IDs = new uint[temp.Count];
				for (int i = 0; i < IDs.Length; i++)
					IDs[i] = temp[i].ID;
				
				TotalUnits = (ushort)IDs.Length;
				GameData.offsets.WriteStructMember(ORNames.Selection, ORNames.selected_IDs, IDs, ref _Data);
			}
		}
	}
}

