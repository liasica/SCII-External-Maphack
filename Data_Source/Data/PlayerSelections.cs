namespace Data
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct PlayerSelections
	{
		public static PlayerSelections LocalSelections
		{
			get
			{
				return new PlayerSelections(-1);
			}
		}

		public PlayerSelections(int PlayerNumber)
		{
			ControlGroups = new Selection[10];
			if (PlayerNumber == -1)
			{
				CurrentSelection = new Selection(GameData.offsets.GetStructAddress(ORNames.Selection));
				ControlGroups[0] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group1));
				ControlGroups[1] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group2));
				ControlGroups[2] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group3));
				ControlGroups[3] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group4));
				ControlGroups[4] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group5));
				ControlGroups[5] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group6));
				ControlGroups[6] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group7));
				ControlGroups[7] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group8));
				ControlGroups[8] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group9));
				ControlGroups[9] = new Selection(GameData.offsets.GetStructMemberAddress(ORNames.ControlGroup, ORNames.Group10));
				return;
			}

			if (PlayerNumber < 0 || PlayerNumber >= 16)
				throw new IndexOutOfRangeException("PlayerSelections: PlayerNumber was " + PlayerNumber.ToString());

			CurrentSelection = new Selection(GameData.offsets.GetArrayElementAddress(ORNames.CurrentSelections, PlayerNumber));
			ControlGroups[0] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group1));
			ControlGroups[1] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group2));
			ControlGroups[2] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group3));
			ControlGroups[3] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group4));
			ControlGroups[4] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group5));
			ControlGroups[5] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group6));
			ControlGroups[6] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group7));
			ControlGroups[7] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group8));
			ControlGroups[8] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group9));
			ControlGroups[9] = new Selection(GameData.offsets.GetArrayElementMemberAddress(ORNames.PlayerGroups, PlayerNumber, ORNames.Group10));
		}

		public Selection CurrentSelection;
		public Selection[] ControlGroups;
	}
}

