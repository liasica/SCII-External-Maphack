namespace Updater
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ColumnHeader columnHeader3;
			this.listViewUpdates = new System.Windows.Forms.ListView();
			this.buttonGetUpdates = new System.Windows.Forms.Button();
			this.buttonStartSCIIEMH = new System.Windows.Forms.Button();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.progressBarDownload = new System.Windows.Forms.ProgressBar();
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// listViewUpdates
			// 
			this.listViewUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewUpdates.CheckBoxes = true;
			this.listViewUpdates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            columnHeader3,
            this.columnHeader4});
			this.listViewUpdates.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.listViewUpdates.FullRowSelect = true;
			this.listViewUpdates.GridLines = true;
			this.listViewUpdates.Location = new System.Drawing.Point(13, 13);
			this.listViewUpdates.Name = "listViewUpdates";
			this.listViewUpdates.Size = new System.Drawing.Size(683, 176);
			this.listViewUpdates.TabIndex = 2;
			this.listViewUpdates.UseCompatibleStateImageBehavior = false;
			this.listViewUpdates.View = System.Windows.Forms.View.Details;
			// 
			// buttonGetUpdates
			// 
			this.buttonGetUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonGetUpdates.Location = new System.Drawing.Point(13, 195);
			this.buttonGetUpdates.Name = "buttonGetUpdates";
			this.buttonGetUpdates.Size = new System.Drawing.Size(238, 23);
			this.buttonGetUpdates.TabIndex = 0;
			this.buttonGetUpdates.Text = "Get Selected Updates";
			this.buttonGetUpdates.UseVisualStyleBackColor = true;
			this.buttonGetUpdates.Click += new System.EventHandler(this.buttonGetUpdates_Click);
			// 
			// buttonStartSCIIEMH
			// 
			this.buttonStartSCIIEMH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonStartSCIIEMH.Location = new System.Drawing.Point(257, 195);
			this.buttonStartSCIIEMH.Name = "buttonStartSCIIEMH";
			this.buttonStartSCIIEMH.Size = new System.Drawing.Size(238, 23);
			this.buttonStartSCIIEMH.TabIndex = 1;
			this.buttonStartSCIIEMH.Text = "Cancel and Start SCIIEMH";
			this.buttonStartSCIIEMH.UseVisualStyleBackColor = true;
			this.buttonStartSCIIEMH.Click += new System.EventHandler(this.buttonStartSCIIEMH_Click);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "File Name";
			this.columnHeader1.Width = 350;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Action";
			this.columnHeader2.Width = 98;
			// 
			// columnHeader3
			// 
			columnHeader3.Text = "File Size";
			columnHeader3.Width = 81;
			// 
			// progressBarDownload
			// 
			this.progressBarDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarDownload.Enabled = false;
			this.progressBarDownload.Location = new System.Drawing.Point(501, 195);
			this.progressBarDownload.Name = "progressBarDownload";
			this.progressBarDownload.Size = new System.Drawing.Size(195, 23);
			this.progressBarDownload.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBarDownload.TabIndex = 3;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Download Progress";
			this.columnHeader4.Width = 132;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(708, 230);
			this.Controls.Add(this.progressBarDownload);
			this.Controls.Add(this.buttonStartSCIIEMH);
			this.Controls.Add(this.buttonGetUpdates);
			this.Controls.Add(this.listViewUpdates);
			this.MinimumSize = new System.Drawing.Size(710, 156);
			this.Name = "MainForm";
			this.Text = "SCIIEMH Updater";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewUpdates;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonGetUpdates;
		private System.Windows.Forms.Button buttonStartSCIIEMH;
		private System.Windows.Forms.ProgressBar progressBarDownload;
		private System.Windows.Forms.ColumnHeader columnHeader4;
	}
}

