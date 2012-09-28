using _2cs_API;
using Data;
using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Utilities.WebTools;

namespace maphack_external_directx
{
	public partial class Trainer : Form
	{
		bool Ingame = true;
		string[] PlayerDisplay;
		bool[] PlayerCheck;
		bool[] NewPlayerCheck;

		List<int> CheckedPlayers;
		List<int> NewCheckedPlayers;

		bool FreezeTS = false;
		int FreezeMins = -1;
		int FreezeGas = -1;
		int FreezeTerra = -1;
		int FreezeCustom = -1;
		
		fixed32 FreezeSupply = -1;
		fixed32 DDMultiplier = 1;
		fixed32 DTMultiplier = 1;
		fixed32 TSMultiplier = 1;

		public Trainer()
		{
			InitializeComponent();
			UpdatePlayers();
		}

		public void ApplyChanges()
		{
			PlayerCheck = NewPlayerCheck;
			CheckedPlayers = NewCheckedPlayers;

			int Mins = (int)boxMins.Value;
			int Gas = (int)boxGas.Value;
			int Terra = (int)boxTerra.Value;
			int Custom = (int)boxCustom.Value;
			fixed32 Supply = (float)boxSupply.Value;
			fixed32 MaxSupply = cfMaxSupply.Checked ? (float)Supply : (float)boxMaxSupply.Value;
			FreezeMins = cfMins.Checked ? Mins : -1;
			FreezeGas = cfGas.Checked ? Gas : -1;
			FreezeTerra = cfTerra.Checked ? Terra : -1;
			FreezeCustom = cfCustom.Checked ? Custom : -1;
			FreezeSupply = cfSupply.Checked ? Supply : -1;

			FreezeTS = cbFreezeUnits.Checked;

			bool ChangeDD = false;
			bool ChangeDT = false;
			bool ChangeTS = false;

			if (!rDDNoChange.Checked)
			{
				ChangeDD = true;
				if (rDDNone.Checked)
					DDMultiplier = 0;
				if (rDDNormal.Checked)
					DDMultiplier = 1;
				if (rDDCustom.Checked)
					DDMultiplier = (float)boxDDCustom.Value;
			}

			if (!rDTNoChange.Checked)
			{
				ChangeDT = true;
				if (rDTNone.Checked)
					DTMultiplier = 0;
				if (rDTNormal.Checked)
					DTMultiplier = 1;
				if (rDTCustom.Checked)
					DTMultiplier = (float)boxDTCustom.Value;
			}

			if (!rTSNoChange.Checked)
			{
				ChangeTS = true;
				if (rTSNone.Checked)
					TSMultiplier = 0;
				if (rTSNormal.Checked)
					TSMultiplier = 1;
				if (rTSCustom.Checked)
					TSMultiplier = (float)boxTSCustom.Value;
			}

			for (int i = 0; i < 16; i++)
			{
				if (PlayerCheck[i])
				{
					if (Mins >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.minerals_current, (uint)Mins);
					if (Gas >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.vespene_current, (uint)Gas);
					if (Terra >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.terrazine_current, (uint)Terra);
					if (Custom >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.custom_resource_current, (uint)Custom);
					if (MaxSupply >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.supply_limit, MaxSupply);
					if (Supply >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.supply_cap, Supply);

					if (ChangeDD)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.attack_multiplier, DDMultiplier);
					if (ChangeDT)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.defense_multiplier, DTMultiplier);
				}
			}

			List<Unit> AffectedUnits = GameData.GetPlayersUnits(CheckedPlayers);
			foreach (Unit unit in AffectedUnits)
			{
				if (ChangeTS)
					unit.timeScale = TSMultiplier;
			}
		}

		public void UpdatePlayers()
		{
			for (int i = 0; i < 1000 && MainWindow.players.Count == 0 && _2csAPI.InGame(); i++)
				Thread.Sleep(5);

			PlayerDisplay = new string[16];
			for (int i = 0; i < 16; i++)
				PlayerDisplay[i] = i.ToString() + ": (none)";
			foreach (Player p in MainWindow.players)
			{
				if (p.number < 16)
					PlayerDisplay[p.number] = p.number.ToString() + ": " + p.name;
			}

			PlayerCheck = new bool[16];
			if(MainWindow.localplayer != 0)
				PlayerCheck[MainWindow.localplayer] = true;
			NewPlayerCheck = new bool[16];

			CheckedPlayers = new List<int>();
			CheckedPlayers.Add((int)MainWindow.localplayer);
			NewCheckedPlayers = new List<int>();
			
			PlayerSelectionBox.Items.Clear();
			for (int i = 0; i < 16; i++)
			{
				PlayerSelectionBox.Items.Add(PlayerDisplay[i], PlayerCheck[i]);
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (Ingame && !_2cs_API._2csAPI.InGame())
			{
				Ingame = false;
				buttonApply.Enabled = false;
				buttonApply.Text = "Cannot use out of game.";
				UpdatePlayers();
				return;
			}
			if (!Ingame && _2cs_API._2csAPI.InGame())
			{
				Ingame = true;
				buttonApply.Enabled = true;
				buttonApply.Text = "Apply Changes Now";
				UpdatePlayers();
				return;
			}


			for (int i = 0; i < 16; i++)
			{
				if (PlayerCheck[i])
				{
					if (FreezeMins >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.minerals_current, (uint)FreezeMins);
					if (FreezeGas >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.vespene_current, (uint)FreezeGas);
					if (FreezeTerra >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.terrazine_current, (uint)FreezeTerra);
					if (FreezeCustom >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.custom_resource_current, (uint)FreezeCustom);
					if (FreezeSupply >= 0)
						GameData.offsets.WriteArrayElementMember(ORNames.Players, i, ORNames.supply_cap, FreezeSupply);
				}
			}

			List<Unit> AffectedUnits = GameData.GetPlayersUnits(CheckedPlayers);
			foreach (Unit unit in AffectedUnits)
			{
				if (FreezeTS)
					unit.timeScale = TSMultiplier;
			}
			MainWindow.UpdateRefreshes("Trainer Freeze");
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			ApplyChanges();
		}

		private void PlayerSelectionBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			NewPlayerCheck[e.Index] = e.NewValue == CheckState.Checked;
			if (e.NewValue == CheckState.Checked && !NewCheckedPlayers.Contains(e.Index))
				NewCheckedPlayers.Add(e.Index);
			if (e.NewValue != CheckState.Checked && NewCheckedPlayers.Contains(e.Index))
				NewCheckedPlayers.RemoveAll(it => it == e.Index);
		}

		private void PlayerSelectionBox_SelectedValueChanged(object sender, EventArgs e)
		{
			((CheckedListBox)sender).ClearSelected();
		}

		private void cfMaxSupply_CheckedChanged(object sender, EventArgs e)
		{
			boxMaxSupply.Enabled = !((CheckBox)sender).Checked;
		}

		private void rDDCustom_CheckedChanged(object sender, EventArgs e)
		{
			boxDDCustom.Enabled = ((RadioButton)sender).Checked;
		}

		private void rDTCustom_CheckedChanged(object sender, EventArgs e)
		{
			boxDTCustom.Enabled = ((RadioButton)sender).Checked;
		}

		private void rTSCustom_CheckedChanged(object sender, EventArgs e)
		{
			boxTSCustom.Enabled = ((RadioButton)sender).Checked;
		}
	}
}
