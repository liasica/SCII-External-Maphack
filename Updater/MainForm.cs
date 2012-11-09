using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace Updater
{
	public partial class MainForm : Form
	{
		private bool DoneUpdating = false;
		private ListViewItem CurrentlyUpdating = null;

		public MainForm(bool Done)
		{
			DoneUpdating = Done;
			InitializeComponent();

			UpdateInfo[] AvailableUpdates = UpdateChecker.UpdatesAvailable();

			if (AvailableUpdates.Length == 0)
			{
				DoneUpdating = true;
				buttonGetUpdates.Text = "No updates. Start SCIIEMH";
			}

			foreach (UpdateInfo info in AvailableUpdates)
			{
				ListViewItem item = new ListViewItem(info.LocalFile);
				string action = "";
				if (info.Operations.Contains('D'))
					action = "Delete";
				else if (info.Operations.Contains('C'))
					action = "Create";
				else if (info.Operations.Contains('R'))
					action = "Replace";

				item.SubItems.Add(action);

				decimal NewSize = info.NewSize;
				string size = string.Empty;
				if (NewSize >= 1048576m) // 1 MB
					size = (NewSize / 1048576m).ToString("F2") + " MB";
				else if(NewSize >= 1024m) // 1 KB
					size = (NewSize / 1024m).ToString("F2") + " KB";
				else
					size = NewSize.ToString() + " Bytes";

				item.SubItems.Add(size);
				item.SubItems.Add("Not Started");
				item.Tag = info;
				item.Checked = true;
				listViewUpdates.Items.Add(item);
				Application.DoEvents();
			}
		}

		private void buttonStartSCIIEMH_Click(object sender, EventArgs e)
		{
			Program.StartSCIIEMH();
			Application.Exit();
		}

		private void DownloadProgressHandler(DownloadStatus e)
		{
			progressBarDownload.Value = e.BytesDownloaded * 100 / e.BytesTotal;
			ListViewItem item;
			if ((item = CurrentlyUpdating) != null)
			{
				item.SubItems[3].Text = (e.BytesDownloaded * 100 / e.BytesTotal).ToString() + "%";
			}
			Application.DoEvents();
		}

		private void buttonGetUpdates_Click(object sender, EventArgs e)
		{
			if (DoneUpdating)
			{
				Program.StartSCIIEMH();
				Application.Exit();
			}

			bool UpdateSelf = false;
			ListViewItem SelfItem = null;
			foreach (ListViewItem item in listViewUpdates.CheckedItems)
			{
				if (item.Tag == null)
					continue;
				UpdateInfo info = (UpdateInfo)item.Tag;

				if (Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToLower() == info.LocalFile.ToLower())
				{
					UpdateSelf = true;
					SelfItem = item;
					continue;
				}

				CurrentlyUpdating = item;
				Downloader dl = new Downloader();
				dl.BytesDownloaded += DownloadProgressHandler;

				Application.DoEvents();
				Program.Update(info.LocalFile, info.URL, dl);
				Application.DoEvents();

			}
			CurrentlyUpdating = null;
			if (UpdateSelf)
			{
				if (SelfItem.Tag == null)
					return;
				UpdateInfo info = (UpdateInfo)SelfItem.Tag;

				CurrentlyUpdating = SelfItem;
				Downloader dl = new Downloader();
				dl.BytesDownloaded += DownloadProgressHandler;
				
				Application.DoEvents();
				Program.UpdateSelf(info.URL, dl);
				progressBarDownload.Value = 0;
				Application.DoEvents();
			}
			DoneUpdating = true;
			buttonGetUpdates.Text = "Start SCIIEMH";
		}
	}
}
