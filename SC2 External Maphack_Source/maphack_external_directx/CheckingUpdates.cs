namespace maphack_external_directx
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Threading;
	using System.Windows.Forms;
	using Utilities.WebTools;

	public class CheckingUpdates : Form
	{
		private IContainer components;
		private ProgressBar progressBar;
		private bool shouldClose;
		private System.Windows.Forms.Timer tmrMain;

		public CheckingUpdates()
		{
			ThreadStart start = null;
			this.InitializeComponent();
			if (start == null)
			{
				start = delegate {
					this.checkUpdates();
				};
			}
			new Thread(start).Start();
		}

		private void checkUpdates()
		{
			try
			{
				WT.CheckForSourceForgeUpdates("sc2extmaphack", Program.ApplicationVersion);
				this.shouldClose = true;
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception exception)
			{
				WT.ReportCrash(exception, Program.ApplicationTitle + " " + Program.ApplicationVersion, null, null, true);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckingUpdates));
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.tmrMain = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressBar.Location = new System.Drawing.Point(0, 0);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(287, 48);
			this.progressBar.TabIndex = 0;
			this.progressBar.UseWaitCursor = true;
			// 
			// tmrMain
			// 
			this.tmrMain.Enabled = true;
			this.tmrMain.Tick += new System.EventHandler(this.tmrMain_Tick);
			// 
			// CheckingUpdates
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(287, 48);
			this.Controls.Add(this.progressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CheckingUpdates";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Checking for Updates";
			this.TopMost = true;
			this.UseWaitCursor = true;
			this.ResumeLayout(false);

		}

		private void tmrMain_Tick(object sender, EventArgs e)
		{
			if (this.shouldClose)
			{
				base.Close();
			}
			this.progressBar.Value++;
			if (this.progressBar.Value >= 100)
			{
				this.progressBar.Value = 0;
			}
		}
	}
}

