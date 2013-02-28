namespace Utilities
{
	partial class CrashReportWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashReportWindow));
			this.textBoxError = new System.Windows.Forms.TextBox();
			this.labelUhOh = new System.Windows.Forms.Label();
			this.labelMessage = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSend = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxAdditionalInfo = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxError
			// 
			this.textBoxError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxError.Location = new System.Drawing.Point(4, 23);
			this.textBoxError.Multiline = true;
			this.textBoxError.Name = "textBoxError";
			this.textBoxError.ReadOnly = true;
			this.textBoxError.Size = new System.Drawing.Size(441, 165);
			this.textBoxError.TabIndex = 0;
			// 
			// labelUhOh
			// 
			this.labelUhOh.AutoSize = true;
			this.labelUhOh.Location = new System.Drawing.Point(13, 13);
			this.labelUhOh.Name = "labelUhOh";
			this.labelUhOh.Size = new System.Drawing.Size(180, 13);
			this.labelUhOh.TabIndex = 1;
			this.labelUhOh.Text = "Uh oh... It appears we have a crash.";
			// 
			// labelMessage
			// 
			this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMessage.Location = new System.Drawing.Point(13, 27);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(447, 54);
			this.labelMessage.TabIndex = 1;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(12, 113);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxAdditionalInfo);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.textBoxError);
			this.splitContainer1.Size = new System.Drawing.Size(448, 300);
			this.splitContainer1.SplitterDistance = 105;
			this.splitContainer1.TabIndex = 2;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(365, 84);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(95, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Do Not Send";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonSend
			// 
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.Location = new System.Drawing.Point(208, 84);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(151, 23);
			this.buttonSend.TabIndex = 3;
			this.buttonSend.Text = "Send Crash Report";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Error message:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(181, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Write any additional information here:";
			// 
			// textBoxAdditionalInfo
			// 
			this.textBoxAdditionalInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxAdditionalInfo.Location = new System.Drawing.Point(4, 20);
			this.textBoxAdditionalInfo.Multiline = true;
			this.textBoxAdditionalInfo.Name = "textBoxAdditionalInfo";
			this.textBoxAdditionalInfo.Size = new System.Drawing.Size(441, 82);
			this.textBoxAdditionalInfo.TabIndex = 0;
			// 
			// CrashReportWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 425);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.labelUhOh);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CrashReportWindow";
			this.Text = "Unexpected Crash";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxError;
		private System.Windows.Forms.Label labelUhOh;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxAdditionalInfo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonSend;
	}
}