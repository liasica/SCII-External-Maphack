namespace maphack_external_directx
{
	partial class Trainer
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
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.PlayerSelectionBox = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cfMaxSupply = new System.Windows.Forms.CheckBox();
			this.cfSupply = new System.Windows.Forms.CheckBox();
			this.cfCustom = new System.Windows.Forms.CheckBox();
			this.cfTerra = new System.Windows.Forms.CheckBox();
			this.cfGas = new System.Windows.Forms.CheckBox();
			this.cfMins = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.boxMaxSupply = new System.Windows.Forms.NumericUpDown();
			this.boxSupply = new System.Windows.Forms.NumericUpDown();
			this.boxCustom = new System.Windows.Forms.NumericUpDown();
			this.boxTerra = new System.Windows.Forms.NumericUpDown();
			this.boxGas = new System.Windows.Forms.NumericUpDown();
			this.boxMins = new System.Windows.Forms.NumericUpDown();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonApply = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.rDDNormal = new System.Windows.Forms.RadioButton();
			this.rDDCustom = new System.Windows.Forms.RadioButton();
			this.boxDDCustom = new System.Windows.Forms.NumericUpDown();
			this.rDDNoChange = new System.Windows.Forms.RadioButton();
			this.rDDNone = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.rDTNone = new System.Windows.Forms.RadioButton();
			this.rDTNoChange = new System.Windows.Forms.RadioButton();
			this.boxDTCustom = new System.Windows.Forms.NumericUpDown();
			this.rDTCustom = new System.Windows.Forms.RadioButton();
			this.rDTNormal = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.boxMaxSupply)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxSupply)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxCustom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxTerra)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxGas)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.boxMins)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.boxDDCustom)).BeginInit();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.boxDTCustom)).BeginInit();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 25;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// PlayerSelectionBox
			// 
			this.PlayerSelectionBox.CheckOnClick = true;
			this.PlayerSelectionBox.FormattingEnabled = true;
			this.PlayerSelectionBox.Items.AddRange(new object[] {
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?",
            "?"});
			this.PlayerSelectionBox.Location = new System.Drawing.Point(12, 25);
			this.PlayerSelectionBox.Name = "PlayerSelectionBox";
			this.PlayerSelectionBox.Size = new System.Drawing.Size(140, 244);
			this.PlayerSelectionBox.TabIndex = 0;
			this.PlayerSelectionBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.PlayerSelectionBox_ItemCheck);
			this.PlayerSelectionBox.SelectedValueChanged += new System.EventHandler(this.PlayerSelectionBox_SelectedValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Affected Players:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cfMaxSupply);
			this.groupBox1.Controls.Add(this.cfSupply);
			this.groupBox1.Controls.Add(this.cfCustom);
			this.groupBox1.Controls.Add(this.cfTerra);
			this.groupBox1.Controls.Add(this.cfGas);
			this.groupBox1.Controls.Add(this.cfMins);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.boxMaxSupply);
			this.groupBox1.Controls.Add(this.boxSupply);
			this.groupBox1.Controls.Add(this.boxCustom);
			this.groupBox1.Controls.Add(this.boxTerra);
			this.groupBox1.Controls.Add(this.boxGas);
			this.groupBox1.Controls.Add(this.boxMins);
			this.groupBox1.Location = new System.Drawing.Point(159, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(207, 181);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Resources";
			// 
			// cfMaxSupply
			// 
			this.cfMaxSupply.AutoSize = true;
			this.cfMaxSupply.Checked = true;
			this.cfMaxSupply.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cfMaxSupply.Location = new System.Drawing.Point(178, 150);
			this.cfMaxSupply.Name = "cfMaxSupply";
			this.cfMaxSupply.Size = new System.Drawing.Size(15, 14);
			this.cfMaxSupply.TabIndex = 2;
			this.toolTip1.SetToolTip(this.cfMaxSupply, "Change this according to current supply. This is different than the freeze option" +
        "s above!");
			this.cfMaxSupply.UseVisualStyleBackColor = true;
			this.cfMaxSupply.CheckedChanged += new System.EventHandler(this.cfMaxSupply_CheckedChanged);
			// 
			// cfSupply
			// 
			this.cfSupply.AutoSize = true;
			this.cfSupply.Location = new System.Drawing.Point(178, 124);
			this.cfSupply.Name = "cfSupply";
			this.cfSupply.Size = new System.Drawing.Size(15, 14);
			this.cfSupply.TabIndex = 2;
			this.toolTip1.SetToolTip(this.cfSupply, "Freze at this number instead of a one-time change.");
			this.cfSupply.UseVisualStyleBackColor = true;
			// 
			// cfCustom
			// 
			this.cfCustom.AutoSize = true;
			this.cfCustom.Location = new System.Drawing.Point(178, 97);
			this.cfCustom.Name = "cfCustom";
			this.cfCustom.Size = new System.Drawing.Size(15, 14);
			this.cfCustom.TabIndex = 2;
			this.toolTip1.SetToolTip(this.cfCustom, "Freze at this number instead of a one-time change.");
			this.cfCustom.UseVisualStyleBackColor = true;
			// 
			// cfTerra
			// 
			this.cfTerra.AutoSize = true;
			this.cfTerra.Location = new System.Drawing.Point(178, 72);
			this.cfTerra.Name = "cfTerra";
			this.cfTerra.Size = new System.Drawing.Size(15, 14);
			this.cfTerra.TabIndex = 2;
			this.toolTip1.SetToolTip(this.cfTerra, "Freze at this number instead of a one-time change.");
			this.cfTerra.UseVisualStyleBackColor = true;
			// 
			// cfGas
			// 
			this.cfGas.AutoSize = true;
			this.cfGas.Location = new System.Drawing.Point(178, 46);
			this.cfGas.Name = "cfGas";
			this.cfGas.Size = new System.Drawing.Size(15, 14);
			this.cfGas.TabIndex = 2;
			this.toolTip1.SetToolTip(this.cfGas, "Freze at this number instead of a one-time change.");
			this.cfGas.UseVisualStyleBackColor = true;
			// 
			// cfMins
			// 
			this.cfMins.AutoSize = true;
			this.cfMins.Location = new System.Drawing.Point(178, 20);
			this.cfMins.Name = "cfMins";
			this.cfMins.Size = new System.Drawing.Size(15, 14);
			this.cfMins.TabIndex = 2;
			this.toolTip1.SetToolTip(this.cfMins, "Freze at this number instead of a one-time change.");
			this.cfMins.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(7, 150);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(65, 13);
			this.label7.TabIndex = 1;
			this.label7.Text = "Max Supply:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 124);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(42, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "Supply:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 98);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(45, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Custom:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "Terrazine:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Vespene:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Minerals:";
			// 
			// boxMaxSupply
			// 
			this.boxMaxSupply.DecimalPlaces = 2;
			this.boxMaxSupply.Enabled = false;
			this.boxMaxSupply.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.boxMaxSupply.Location = new System.Drawing.Point(75, 148);
			this.boxMaxSupply.Maximum = new decimal(new int[] {
            524287,
            0,
            0,
            0});
			this.boxMaxSupply.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.boxMaxSupply.Name = "boxMaxSupply";
			this.boxMaxSupply.Size = new System.Drawing.Size(97, 20);
			this.boxMaxSupply.TabIndex = 0;
			this.toolTip1.SetToolTip(this.boxMaxSupply, "This is the highest supply can go. If you try to change supply to a number higher" +
        " than this, it will be capped at this value. Set at -1 to not change this value." +
        "");
			this.boxMaxSupply.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// boxSupply
			// 
			this.boxSupply.DecimalPlaces = 2;
			this.boxSupply.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.boxSupply.Location = new System.Drawing.Point(75, 122);
			this.boxSupply.Maximum = new decimal(new int[] {
            524287,
            0,
            0,
            0});
			this.boxSupply.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.boxSupply.Name = "boxSupply";
			this.boxSupply.Size = new System.Drawing.Size(97, 20);
			this.boxSupply.TabIndex = 0;
			this.toolTip1.SetToolTip(this.boxSupply, "Set at -1 to not change this resource.");
			this.boxSupply.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// boxCustom
			// 
			this.boxCustom.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.boxCustom.Location = new System.Drawing.Point(75, 96);
			this.boxCustom.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.boxCustom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.boxCustom.Name = "boxCustom";
			this.boxCustom.Size = new System.Drawing.Size(97, 20);
			this.boxCustom.TabIndex = 0;
			this.toolTip1.SetToolTip(this.boxCustom, "Set at -1 to not change this resource.");
			this.boxCustom.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// boxTerra
			// 
			this.boxTerra.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.boxTerra.Location = new System.Drawing.Point(75, 70);
			this.boxTerra.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.boxTerra.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.boxTerra.Name = "boxTerra";
			this.boxTerra.Size = new System.Drawing.Size(97, 20);
			this.boxTerra.TabIndex = 0;
			this.toolTip1.SetToolTip(this.boxTerra, "Set at -1 to not change this resource.");
			this.boxTerra.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// boxGas
			// 
			this.boxGas.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.boxGas.Location = new System.Drawing.Point(75, 44);
			this.boxGas.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.boxGas.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.boxGas.Name = "boxGas";
			this.boxGas.Size = new System.Drawing.Size(97, 20);
			this.boxGas.TabIndex = 0;
			this.toolTip1.SetToolTip(this.boxGas, "Set at -1 to not change this resource.");
			this.boxGas.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// boxMins
			// 
			this.boxMins.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.boxMins.Location = new System.Drawing.Point(75, 18);
			this.boxMins.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.boxMins.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.boxMins.Name = "boxMins";
			this.boxMins.Size = new System.Drawing.Size(97, 20);
			this.boxMins.TabIndex = 0;
			this.toolTip1.SetToolTip(this.boxMins, "Set at -1 to not change this resource.");
			this.boxMins.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 250;
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 250;
			this.toolTip1.ReshowDelay = 50;
			// 
			// buttonApply
			// 
			this.buttonApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.buttonApply.Location = new System.Drawing.Point(12, 276);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(140, 23);
			this.buttonApply.TabIndex = 3;
			this.buttonApply.Text = "Apply Changes Now";
			this.buttonApply.UseVisualStyleBackColor = false;
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.groupBox4);
			this.groupBox2.Controls.Add(this.groupBox3);
			this.groupBox2.Location = new System.Drawing.Point(372, 9);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(232, 181);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Combat";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.rDDNone);
			this.groupBox3.Controls.Add(this.rDDNoChange);
			this.groupBox3.Controls.Add(this.boxDDCustom);
			this.groupBox3.Controls.Add(this.rDDCustom);
			this.groupBox3.Controls.Add(this.rDDNormal);
			this.groupBox3.Location = new System.Drawing.Point(6, 19);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(106, 155);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Damage Dealt";
			// 
			// rDDNormal
			// 
			this.rDDNormal.AutoSize = true;
			this.rDDNormal.Location = new System.Drawing.Point(6, 65);
			this.rDDNormal.Name = "rDDNormal";
			this.rDDNormal.Size = new System.Drawing.Size(58, 17);
			this.rDDNormal.TabIndex = 0;
			this.rDDNormal.Text = "Normal";
			this.toolTip1.SetToolTip(this.rDDNormal, "Units will do 100% damage.");
			this.rDDNormal.UseVisualStyleBackColor = true;
			// 
			// rDDCustom
			// 
			this.rDDCustom.AutoSize = true;
			this.rDDCustom.Location = new System.Drawing.Point(6, 88);
			this.rDDCustom.Name = "rDDCustom";
			this.rDDCustom.Size = new System.Drawing.Size(63, 17);
			this.rDDCustom.TabIndex = 0;
			this.rDDCustom.Text = "Custom:";
			this.toolTip1.SetToolTip(this.rDDCustom, "Set a custom multiplier for damage dealt.");
			this.rDDCustom.UseVisualStyleBackColor = true;
			this.rDDCustom.CheckedChanged += new System.EventHandler(this.rDDCustom_CheckedChanged);
			// 
			// boxDDCustom
			// 
			this.boxDDCustom.DecimalPlaces = 4;
			this.boxDDCustom.Enabled = false;
			this.boxDDCustom.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.boxDDCustom.Location = new System.Drawing.Point(6, 111);
			this.boxDDCustom.Maximum = new decimal(new int[] {
            524287,
            0,
            0,
            0});
			this.boxDDCustom.Minimum = new decimal(new int[] {
            524287,
            0,
            0,
            -2147483648});
			this.boxDDCustom.Name = "boxDDCustom";
			this.boxDDCustom.Size = new System.Drawing.Size(79, 20);
			this.boxDDCustom.TabIndex = 1;
			this.toolTip1.SetToolTip(this.boxDDCustom, "Custom multiplier for damage dealt.");
			this.boxDDCustom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// rDDNoChange
			// 
			this.rDDNoChange.AutoSize = true;
			this.rDDNoChange.Checked = true;
			this.rDDNoChange.Location = new System.Drawing.Point(6, 19);
			this.rDDNoChange.Name = "rDDNoChange";
			this.rDDNoChange.Size = new System.Drawing.Size(79, 17);
			this.rDDNoChange.TabIndex = 2;
			this.rDDNoChange.TabStop = true;
			this.rDDNoChange.Text = "No Change";
			this.toolTip1.SetToolTip(this.rDDNoChange, "Will not change anything when changes are applied.");
			this.rDDNoChange.UseVisualStyleBackColor = true;
			// 
			// rDDNone
			// 
			this.rDDNone.AutoSize = true;
			this.rDDNone.Location = new System.Drawing.Point(6, 42);
			this.rDDNone.Name = "rDDNone";
			this.rDDNone.Size = new System.Drawing.Size(51, 17);
			this.rDDNone.TabIndex = 2;
			this.rDDNone.Text = "None";
			this.toolTip1.SetToolTip(this.rDDNone, "Units will do no damage.");
			this.rDDNone.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.rDTNone);
			this.groupBox4.Controls.Add(this.rDTNoChange);
			this.groupBox4.Controls.Add(this.boxDTCustom);
			this.groupBox4.Controls.Add(this.rDTCustom);
			this.groupBox4.Controls.Add(this.rDTNormal);
			this.groupBox4.Location = new System.Drawing.Point(119, 19);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(106, 155);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Damage Taken";
			// 
			// rDTNone
			// 
			this.rDTNone.AutoSize = true;
			this.rDTNone.Location = new System.Drawing.Point(6, 42);
			this.rDTNone.Name = "rDTNone";
			this.rDTNone.Size = new System.Drawing.Size(51, 17);
			this.rDTNone.TabIndex = 2;
			this.rDTNone.Text = "None";
			this.toolTip1.SetToolTip(this.rDTNone, "Units will take no damage (Invincibility).");
			this.rDTNone.UseVisualStyleBackColor = true;
			// 
			// rDTNoChange
			// 
			this.rDTNoChange.AutoSize = true;
			this.rDTNoChange.Checked = true;
			this.rDTNoChange.Location = new System.Drawing.Point(6, 19);
			this.rDTNoChange.Name = "rDTNoChange";
			this.rDTNoChange.Size = new System.Drawing.Size(79, 17);
			this.rDTNoChange.TabIndex = 2;
			this.rDTNoChange.TabStop = true;
			this.rDTNoChange.Text = "No Change";
			this.toolTip1.SetToolTip(this.rDTNoChange, "Will not change anything when changes are applied.");
			this.rDTNoChange.UseVisualStyleBackColor = true;
			// 
			// boxDTCustom
			// 
			this.boxDTCustom.DecimalPlaces = 4;
			this.boxDTCustom.Enabled = false;
			this.boxDTCustom.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.boxDTCustom.Location = new System.Drawing.Point(6, 111);
			this.boxDTCustom.Maximum = new decimal(new int[] {
            524287,
            0,
            0,
            0});
			this.boxDTCustom.Minimum = new decimal(new int[] {
            524287,
            0,
            0,
            -2147483648});
			this.boxDTCustom.Name = "boxDTCustom";
			this.boxDTCustom.Size = new System.Drawing.Size(79, 20);
			this.boxDTCustom.TabIndex = 1;
			this.toolTip1.SetToolTip(this.boxDTCustom, "Custom multiplier for damage taken.");
			this.boxDTCustom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// rDTCustom
			// 
			this.rDTCustom.AutoSize = true;
			this.rDTCustom.Location = new System.Drawing.Point(6, 88);
			this.rDTCustom.Name = "rDTCustom";
			this.rDTCustom.Size = new System.Drawing.Size(63, 17);
			this.rDTCustom.TabIndex = 0;
			this.rDTCustom.Text = "Custom:";
			this.toolTip1.SetToolTip(this.rDTCustom, "Set a custom multiplier for damage taken.");
			this.rDTCustom.UseVisualStyleBackColor = true;
			this.rDTCustom.CheckedChanged += new System.EventHandler(this.rDTCustom_CheckedChanged);
			// 
			// rDTNormal
			// 
			this.rDTNormal.AutoSize = true;
			this.rDTNormal.Location = new System.Drawing.Point(6, 65);
			this.rDTNormal.Name = "rDTNormal";
			this.rDTNormal.Size = new System.Drawing.Size(58, 17);
			this.rDTNormal.TabIndex = 0;
			this.rDTNormal.Text = "Normal";
			this.toolTip1.SetToolTip(this.rDTNormal, "Units will take 100% damage.");
			this.rDTNormal.UseVisualStyleBackColor = true;
			// 
			// Trainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(610, 306);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.PlayerSelectionBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Trainer";
			this.Text = "SCIIEMH Trainer";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.boxMaxSupply)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxSupply)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxCustom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxTerra)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxGas)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.boxMins)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.boxDDCustom)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.boxDTCustom)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.CheckedListBox PlayerSelectionBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cfSupply;
		private System.Windows.Forms.CheckBox cfCustom;
		private System.Windows.Forms.CheckBox cfTerra;
		private System.Windows.Forms.CheckBox cfGas;
		private System.Windows.Forms.CheckBox cfMins;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown boxSupply;
		private System.Windows.Forms.NumericUpDown boxCustom;
		private System.Windows.Forms.NumericUpDown boxTerra;
		private System.Windows.Forms.NumericUpDown boxGas;
		private System.Windows.Forms.NumericUpDown boxMins;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.CheckBox cfMaxSupply;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown boxMaxSupply;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton rDTNone;
		private System.Windows.Forms.RadioButton rDTNoChange;
		private System.Windows.Forms.NumericUpDown boxDTCustom;
		private System.Windows.Forms.RadioButton rDTCustom;
		private System.Windows.Forms.RadioButton rDTNormal;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rDDNone;
		private System.Windows.Forms.RadioButton rDDNoChange;
		private System.Windows.Forms.NumericUpDown boxDDCustom;
		private System.Windows.Forms.RadioButton rDDCustom;
		private System.Windows.Forms.RadioButton rDDNormal;

	}
}