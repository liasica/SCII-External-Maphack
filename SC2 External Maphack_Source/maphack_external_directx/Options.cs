namespace maphack_external_directx
{
	using Ini;
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Windows.Forms;

	public class Options : Form
	{
		private Button btnSave;
		private ComboBox cbNextPanelHotkey;
		private ComboBox cbObserverPanelDrawDirection;
		private ComboBox cbPreviousPanelHotkey;
		private ComboBox cbRepositionHotkey;
		private CheckBox chkCameraAllies;
		private CheckBox chkCameraEnemies;
		private CheckBox chkCameraSelf;
		private CheckBox chkSpectator;
		private CheckBox chkUnitDestinationAllies;
		private CheckBox chkUnitDestinationEnemies;
		private CheckBox chkUnitDestinationEnemiesScreen;
		private CheckBox chkUnitDestinationSelf;
		private CheckBox chkUnitEnemiesScreen;
		private CheckBox chkUnitsAllies;
		private CheckBox chkUnitsEnemies;
		private CheckBox chkUnitsSelf;
		private IContainer components;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private GroupBox groupBox4;
		private GroupBox groupBox5;
		private GroupBox groupBox7;
		private GroupBox groupBox8;
		private Label label1;
		private Label label10;
		private Label label11;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label label8;
		private Label label9;
		private TabPage tbDrawing;
		private TabPage tbHotkeys;
		private TabControl tbOptions;
		private TabPage tbTimers;
		private NumericUpDown udData;
		private NumericUpDown udDrawing;
		private Button button1;
		private CheckBox chk0Radius;
		private CheckBox checkBoxTeamColors;
		private TabPage tbUpdates;
		private Button buttonCheckUpdates;
		private CheckBox chkAutoUpdate;
		private NumericUpDown udGUI;

		public Options()
		{
			this.InitializeComponent();
			this.cbObserverPanelDrawDirection.SelectedIndex = 0;
			this.LoadHotkeys();
			this.LoadSettings();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Data.Player.UseTeamColors = checkBoxTeamColors.Checked;
			this.SaveSettings();
			base.Close();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
			this.tbOptions = new System.Windows.Forms.TabControl();
			this.tbDrawing = new System.Windows.Forms.TabPage();
			this.checkBoxTeamColors = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cbObserverPanelDrawDirection = new System.Windows.Forms.ComboBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.chkUnitDestinationEnemiesScreen = new System.Windows.Forms.CheckBox();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.chkUnitEnemiesScreen = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkSpectator = new System.Windows.Forms.CheckBox();
			this.chkCameraSelf = new System.Windows.Forms.CheckBox();
			this.chkCameraEnemies = new System.Windows.Forms.CheckBox();
			this.chkCameraAllies = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.chkUnitDestinationSelf = new System.Windows.Forms.CheckBox();
			this.chkUnitDestinationEnemies = new System.Windows.Forms.CheckBox();
			this.chkUnitDestinationAllies = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chk0Radius = new System.Windows.Forms.CheckBox();
			this.chkUnitsSelf = new System.Windows.Forms.CheckBox();
			this.chkUnitsEnemies = new System.Windows.Forms.CheckBox();
			this.chkUnitsAllies = new System.Windows.Forms.CheckBox();
			this.tbHotkeys = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.cbRepositionHotkey = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbPreviousPanelHotkey = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbNextPanelHotkey = new System.Windows.Forms.ComboBox();
			this.tbTimers = new System.Windows.Forms.TabPage();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.udData = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.udDrawing = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.udGUI = new System.Windows.Forms.NumericUpDown();
			this.tbUpdates = new System.Windows.Forms.TabPage();
			this.buttonCheckUpdates = new System.Windows.Forms.Button();
			this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.tbOptions.SuspendLayout();
			this.tbDrawing.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tbHotkeys.SuspendLayout();
			this.tbTimers.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.udData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udDrawing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udGUI)).BeginInit();
			this.tbUpdates.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbOptions
			// 
			this.tbOptions.Controls.Add(this.tbDrawing);
			this.tbOptions.Controls.Add(this.tbHotkeys);
			this.tbOptions.Controls.Add(this.tbTimers);
			this.tbOptions.Controls.Add(this.tbUpdates);
			this.tbOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbOptions.Location = new System.Drawing.Point(0, 0);
			this.tbOptions.Name = "tbOptions";
			this.tbOptions.SelectedIndex = 0;
			this.tbOptions.Size = new System.Drawing.Size(327, 336);
			this.tbOptions.TabIndex = 0;
			// 
			// tbDrawing
			// 
			this.tbDrawing.Controls.Add(this.checkBoxTeamColors);
			this.tbDrawing.Controls.Add(this.label4);
			this.tbDrawing.Controls.Add(this.cbObserverPanelDrawDirection);
			this.tbDrawing.Controls.Add(this.groupBox5);
			this.tbDrawing.Controls.Add(this.groupBox4);
			this.tbDrawing.Location = new System.Drawing.Point(4, 22);
			this.tbDrawing.Name = "tbDrawing";
			this.tbDrawing.Padding = new System.Windows.Forms.Padding(3);
			this.tbDrawing.Size = new System.Drawing.Size(319, 310);
			this.tbDrawing.TabIndex = 0;
			this.tbDrawing.Text = "Drawing";
			this.tbDrawing.UseVisualStyleBackColor = true;
			// 
			// checkBoxTeamColors
			// 
			this.checkBoxTeamColors.AutoSize = true;
			this.checkBoxTeamColors.Location = new System.Drawing.Point(226, 183);
			this.checkBoxTeamColors.Name = "checkBoxTeamColors";
			this.checkBoxTeamColors.Size = new System.Drawing.Size(85, 17);
			this.checkBoxTeamColors.TabIndex = 6;
			this.checkBoxTeamColors.Text = "Team Colors";
			this.checkBoxTeamColors.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(28, 267);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(156, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Observer Panel Draw Direction:";
			// 
			// cbObserverPanelDrawDirection
			// 
			this.cbObserverPanelDrawDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbObserverPanelDrawDirection.FormattingEnabled = true;
			this.cbObserverPanelDrawDirection.Items.AddRange(new object[] {
            "Left",
            "Down"});
			this.cbObserverPanelDrawDirection.Location = new System.Drawing.Point(190, 264);
			this.cbObserverPanelDrawDirection.Name = "cbObserverPanelDrawDirection";
			this.cbObserverPanelDrawDirection.Size = new System.Drawing.Size(121, 21);
			this.cbObserverPanelDrawDirection.TabIndex = 8;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.groupBox7);
			this.groupBox5.Controls.Add(this.groupBox8);
			this.groupBox5.Location = new System.Drawing.Point(8, 183);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(199, 75);
			this.groupBox5.TabIndex = 6;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Screen";
			this.groupBox5.Visible = false;
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.chkUnitDestinationEnemiesScreen);
			this.groupBox7.Location = new System.Drawing.Point(93, 19);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(97, 47);
			this.groupBox7.TabIndex = 4;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Unit Destination";
			// 
			// chkUnitDestinationEnemiesScreen
			// 
			this.chkUnitDestinationEnemiesScreen.AutoSize = true;
			this.chkUnitDestinationEnemiesScreen.Checked = true;
			this.chkUnitDestinationEnemiesScreen.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUnitDestinationEnemiesScreen.Location = new System.Drawing.Point(6, 19);
			this.chkUnitDestinationEnemiesScreen.Name = "chkUnitDestinationEnemiesScreen";
			this.chkUnitDestinationEnemiesScreen.Size = new System.Drawing.Size(66, 17);
			this.chkUnitDestinationEnemiesScreen.TabIndex = 1;
			this.chkUnitDestinationEnemiesScreen.Text = "Enemies";
			this.chkUnitDestinationEnemiesScreen.UseVisualStyleBackColor = true;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.chkUnitEnemiesScreen);
			this.groupBox8.Location = new System.Drawing.Point(12, 19);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(75, 47);
			this.groupBox8.TabIndex = 3;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Units";
			// 
			// chkUnitEnemiesScreen
			// 
			this.chkUnitEnemiesScreen.AutoSize = true;
			this.chkUnitEnemiesScreen.Checked = true;
			this.chkUnitEnemiesScreen.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUnitEnemiesScreen.Location = new System.Drawing.Point(6, 19);
			this.chkUnitEnemiesScreen.Name = "chkUnitEnemiesScreen";
			this.chkUnitEnemiesScreen.Size = new System.Drawing.Size(66, 17);
			this.chkUnitEnemiesScreen.TabIndex = 1;
			this.chkUnitEnemiesScreen.Text = "Enemies";
			this.chkUnitEnemiesScreen.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.button1);
			this.groupBox4.Controls.Add(this.groupBox1);
			this.groupBox4.Controls.Add(this.groupBox3);
			this.groupBox4.Controls.Add(this.groupBox2);
			this.groupBox4.Location = new System.Drawing.Point(8, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(303, 171);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Minimap";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(167, 142);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(130, 23);
			this.button1.TabIndex = 5;
			this.button1.Text = "Auto-Position Minimap";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkSpectator);
			this.groupBox1.Controls.Add(this.chkCameraSelf);
			this.groupBox1.Controls.Add(this.chkCameraEnemies);
			this.groupBox1.Controls.Add(this.chkCameraAllies);
			this.groupBox1.Location = new System.Drawing.Point(6, 19);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(81, 146);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Camera";
			// 
			// chkSpectator
			// 
			this.chkSpectator.AutoSize = true;
			this.chkSpectator.Location = new System.Drawing.Point(6, 88);
			this.chkSpectator.Name = "chkSpectator";
			this.chkSpectator.Size = new System.Drawing.Size(72, 17);
			this.chkSpectator.TabIndex = 3;
			this.chkSpectator.Text = "Spectator";
			this.chkSpectator.UseVisualStyleBackColor = true;
			// 
			// chkCameraSelf
			// 
			this.chkCameraSelf.AutoSize = true;
			this.chkCameraSelf.Location = new System.Drawing.Point(6, 65);
			this.chkCameraSelf.Name = "chkCameraSelf";
			this.chkCameraSelf.Size = new System.Drawing.Size(44, 17);
			this.chkCameraSelf.TabIndex = 2;
			this.chkCameraSelf.Text = "Self";
			this.chkCameraSelf.UseVisualStyleBackColor = true;
			// 
			// chkCameraEnemies
			// 
			this.chkCameraEnemies.AutoSize = true;
			this.chkCameraEnemies.Checked = true;
			this.chkCameraEnemies.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkCameraEnemies.Location = new System.Drawing.Point(6, 19);
			this.chkCameraEnemies.Name = "chkCameraEnemies";
			this.chkCameraEnemies.Size = new System.Drawing.Size(66, 17);
			this.chkCameraEnemies.TabIndex = 1;
			this.chkCameraEnemies.Text = "Enemies";
			this.chkCameraEnemies.UseVisualStyleBackColor = true;
			// 
			// chkCameraAllies
			// 
			this.chkCameraAllies.AutoSize = true;
			this.chkCameraAllies.Checked = true;
			this.chkCameraAllies.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkCameraAllies.Location = new System.Drawing.Point(6, 42);
			this.chkCameraAllies.Name = "chkCameraAllies";
			this.chkCameraAllies.Size = new System.Drawing.Size(50, 17);
			this.chkCameraAllies.TabIndex = 0;
			this.chkCameraAllies.Text = "Allies";
			this.chkCameraAllies.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.chkUnitDestinationSelf);
			this.groupBox3.Controls.Add(this.chkUnitDestinationEnemies);
			this.groupBox3.Controls.Add(this.chkUnitDestinationAllies);
			this.groupBox3.Location = new System.Drawing.Point(200, 19);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(97, 117);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Unit Destination";
			// 
			// chkUnitDestinationSelf
			// 
			this.chkUnitDestinationSelf.AutoSize = true;
			this.chkUnitDestinationSelf.Location = new System.Drawing.Point(6, 65);
			this.chkUnitDestinationSelf.Name = "chkUnitDestinationSelf";
			this.chkUnitDestinationSelf.Size = new System.Drawing.Size(44, 17);
			this.chkUnitDestinationSelf.TabIndex = 2;
			this.chkUnitDestinationSelf.Text = "Self";
			this.chkUnitDestinationSelf.UseVisualStyleBackColor = true;
			// 
			// chkUnitDestinationEnemies
			// 
			this.chkUnitDestinationEnemies.AutoSize = true;
			this.chkUnitDestinationEnemies.Checked = true;
			this.chkUnitDestinationEnemies.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUnitDestinationEnemies.Location = new System.Drawing.Point(6, 19);
			this.chkUnitDestinationEnemies.Name = "chkUnitDestinationEnemies";
			this.chkUnitDestinationEnemies.Size = new System.Drawing.Size(66, 17);
			this.chkUnitDestinationEnemies.TabIndex = 1;
			this.chkUnitDestinationEnemies.Text = "Enemies";
			this.chkUnitDestinationEnemies.UseVisualStyleBackColor = true;
			// 
			// chkUnitDestinationAllies
			// 
			this.chkUnitDestinationAllies.AutoSize = true;
			this.chkUnitDestinationAllies.Checked = true;
			this.chkUnitDestinationAllies.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUnitDestinationAllies.Location = new System.Drawing.Point(6, 42);
			this.chkUnitDestinationAllies.Name = "chkUnitDestinationAllies";
			this.chkUnitDestinationAllies.Size = new System.Drawing.Size(50, 17);
			this.chkUnitDestinationAllies.TabIndex = 0;
			this.chkUnitDestinationAllies.Text = "Allies";
			this.chkUnitDestinationAllies.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chk0Radius);
			this.groupBox2.Controls.Add(this.chkUnitsSelf);
			this.groupBox2.Controls.Add(this.chkUnitsEnemies);
			this.groupBox2.Controls.Add(this.chkUnitsAllies);
			this.groupBox2.Location = new System.Drawing.Point(93, 19);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(101, 117);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Units";
			// 
			// chk0Radius
			// 
			this.chk0Radius.AutoSize = true;
			this.chk0Radius.Checked = true;
			this.chk0Radius.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chk0Radius.Location = new System.Drawing.Point(6, 88);
			this.chk0Radius.Name = "chk0Radius";
			this.chk0Radius.Size = new System.Drawing.Size(101, 17);
			this.chk0Radius.TabIndex = 3;
			this.chk0Radius.Text = "Show if 0 radius";
			this.chk0Radius.UseVisualStyleBackColor = true;
			// 
			// chkUnitsSelf
			// 
			this.chkUnitsSelf.AutoSize = true;
			this.chkUnitsSelf.Location = new System.Drawing.Point(6, 65);
			this.chkUnitsSelf.Name = "chkUnitsSelf";
			this.chkUnitsSelf.Size = new System.Drawing.Size(44, 17);
			this.chkUnitsSelf.TabIndex = 2;
			this.chkUnitsSelf.Text = "Self";
			this.chkUnitsSelf.UseVisualStyleBackColor = true;
			// 
			// chkUnitsEnemies
			// 
			this.chkUnitsEnemies.AutoSize = true;
			this.chkUnitsEnemies.Checked = true;
			this.chkUnitsEnemies.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUnitsEnemies.Location = new System.Drawing.Point(6, 19);
			this.chkUnitsEnemies.Name = "chkUnitsEnemies";
			this.chkUnitsEnemies.Size = new System.Drawing.Size(66, 17);
			this.chkUnitsEnemies.TabIndex = 1;
			this.chkUnitsEnemies.Text = "Enemies";
			this.chkUnitsEnemies.UseVisualStyleBackColor = true;
			// 
			// chkUnitsAllies
			// 
			this.chkUnitsAllies.AutoSize = true;
			this.chkUnitsAllies.Location = new System.Drawing.Point(6, 42);
			this.chkUnitsAllies.Name = "chkUnitsAllies";
			this.chkUnitsAllies.Size = new System.Drawing.Size(50, 17);
			this.chkUnitsAllies.TabIndex = 0;
			this.chkUnitsAllies.Text = "Allies";
			this.chkUnitsAllies.UseVisualStyleBackColor = true;
			// 
			// tbHotkeys
			// 
			this.tbHotkeys.Controls.Add(this.label3);
			this.tbHotkeys.Controls.Add(this.cbRepositionHotkey);
			this.tbHotkeys.Controls.Add(this.label2);
			this.tbHotkeys.Controls.Add(this.cbPreviousPanelHotkey);
			this.tbHotkeys.Controls.Add(this.label1);
			this.tbHotkeys.Controls.Add(this.cbNextPanelHotkey);
			this.tbHotkeys.Location = new System.Drawing.Point(4, 22);
			this.tbHotkeys.Name = "tbHotkeys";
			this.tbHotkeys.Padding = new System.Windows.Forms.Padding(3);
			this.tbHotkeys.Size = new System.Drawing.Size(319, 310);
			this.tbHotkeys.TabIndex = 1;
			this.tbHotkeys.Text = "Hotkeys";
			this.tbHotkeys.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(30, 63);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Reposition";
			this.label3.Visible = false;
			// 
			// cbRepositionHotkey
			// 
			this.cbRepositionHotkey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRepositionHotkey.FormattingEnabled = true;
			this.cbRepositionHotkey.Location = new System.Drawing.Point(93, 60);
			this.cbRepositionHotkey.Name = "cbRepositionHotkey";
			this.cbRepositionHotkey.Size = new System.Drawing.Size(175, 21);
			this.cbRepositionHotkey.TabIndex = 4;
			this.cbRepositionHotkey.Visible = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(78, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Previous Panel";
			// 
			// cbPreviousPanelHotkey
			// 
			this.cbPreviousPanelHotkey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPreviousPanelHotkey.FormattingEnabled = true;
			this.cbPreviousPanelHotkey.Location = new System.Drawing.Point(93, 33);
			this.cbPreviousPanelHotkey.Name = "cbPreviousPanelHotkey";
			this.cbPreviousPanelHotkey.Size = new System.Drawing.Size(175, 21);
			this.cbPreviousPanelHotkey.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Next Panel";
			// 
			// cbNextPanelHotkey
			// 
			this.cbNextPanelHotkey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbNextPanelHotkey.FormattingEnabled = true;
			this.cbNextPanelHotkey.Location = new System.Drawing.Point(93, 6);
			this.cbNextPanelHotkey.Name = "cbNextPanelHotkey";
			this.cbNextPanelHotkey.Size = new System.Drawing.Size(175, 21);
			this.cbNextPanelHotkey.TabIndex = 0;
			// 
			// tbTimers
			// 
			this.tbTimers.Controls.Add(this.label11);
			this.tbTimers.Controls.Add(this.label9);
			this.tbTimers.Controls.Add(this.label10);
			this.tbTimers.Controls.Add(this.udData);
			this.tbTimers.Controls.Add(this.label7);
			this.tbTimers.Controls.Add(this.label8);
			this.tbTimers.Controls.Add(this.udDrawing);
			this.tbTimers.Controls.Add(this.label6);
			this.tbTimers.Controls.Add(this.label5);
			this.tbTimers.Controls.Add(this.udGUI);
			this.tbTimers.Location = new System.Drawing.Point(4, 22);
			this.tbTimers.Name = "tbTimers";
			this.tbTimers.Padding = new System.Windows.Forms.Padding(3);
			this.tbTimers.Size = new System.Drawing.Size(319, 310);
			this.tbTimers.TabIndex = 2;
			this.tbTimers.Text = "Timers";
			this.tbTimers.UseVisualStyleBackColor = true;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(17, 184);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(146, 13);
			this.label11.TabIndex = 11;
			this.label11.Text = "1 second = 1000 milliseconds";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(134, 139);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(63, 13);
			this.label9.TabIndex = 10;
			this.label9.Text = "milliseconds";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(5, 116);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(96, 13);
			this.label10.TabIndex = 9;
			this.label10.Text = "Data Refresh Rate";
			// 
			// udData
			// 
			this.udData.Location = new System.Drawing.Point(8, 132);
			this.udData.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
			this.udData.Name = "udData";
			this.udData.Size = new System.Drawing.Size(120, 20);
			this.udData.TabIndex = 8;
			this.udData.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(134, 89);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(63, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "milliseconds";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(5, 66);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(112, 13);
			this.label8.TabIndex = 6;
			this.label8.Text = "Drawing Refresh Rate";
			// 
			// udDrawing
			// 
			this.udDrawing.Location = new System.Drawing.Point(8, 82);
			this.udDrawing.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
			this.udDrawing.Name = "udDrawing";
			this.udDrawing.Size = new System.Drawing.Size(120, 20);
			this.udDrawing.TabIndex = 5;
			this.udDrawing.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(134, 37);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(63, 13);
			this.label6.TabIndex = 4;
			this.label6.Text = "milliseconds";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(5, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(92, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "GUI Refresh Rate";
			// 
			// udGUI
			// 
			this.udGUI.Location = new System.Drawing.Point(8, 30);
			this.udGUI.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
			this.udGUI.Name = "udGUI";
			this.udGUI.Size = new System.Drawing.Size(120, 20);
			this.udGUI.TabIndex = 2;
			this.udGUI.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			// 
			// tbUpdates
			// 
			this.tbUpdates.Controls.Add(this.buttonCheckUpdates);
			this.tbUpdates.Controls.Add(this.chkAutoUpdate);
			this.tbUpdates.Location = new System.Drawing.Point(4, 22);
			this.tbUpdates.Name = "tbUpdates";
			this.tbUpdates.Size = new System.Drawing.Size(319, 310);
			this.tbUpdates.TabIndex = 3;
			this.tbUpdates.Text = "Updates";
			this.tbUpdates.UseVisualStyleBackColor = true;
			// 
			// buttonCheckUpdates
			// 
			this.buttonCheckUpdates.Location = new System.Drawing.Point(8, 37);
			this.buttonCheckUpdates.Name = "buttonCheckUpdates";
			this.buttonCheckUpdates.Size = new System.Drawing.Size(162, 23);
			this.buttonCheckUpdates.TabIndex = 1;
			this.buttonCheckUpdates.Text = "Check for updates";
			this.buttonCheckUpdates.UseVisualStyleBackColor = true;
			this.buttonCheckUpdates.Click += new System.EventHandler(this.buttonCheckUpdates_Click);
			// 
			// chkAutoUpdate
			// 
			this.chkAutoUpdate.AutoSize = true;
			this.chkAutoUpdate.Checked = true;
			this.chkAutoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoUpdate.Location = new System.Drawing.Point(8, 14);
			this.chkAutoUpdate.Name = "chkAutoUpdate";
			this.chkAutoUpdate.Size = new System.Drawing.Size(163, 17);
			this.chkAutoUpdate.TabIndex = 0;
			this.chkAutoUpdate.Text = "Check for updates on startup";
			this.chkAutoUpdate.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnSave.Location = new System.Drawing.Point(0, 313);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(327, 23);
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// Options
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(327, 336);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.tbOptions);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Options";
			this.Text = "Options";
			this.TopMost = true;
			this.tbOptions.ResumeLayout(false);
			this.tbDrawing.ResumeLayout(false);
			this.tbDrawing.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tbHotkeys.ResumeLayout(false);
			this.tbHotkeys.PerformLayout();
			this.tbTimers.ResumeLayout(false);
			this.tbTimers.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.udData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udDrawing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udGUI)).EndInit();
			this.tbUpdates.ResumeLayout(false);
			this.tbUpdates.PerformLayout();
			this.ResumeLayout(false);

		}

		private void LoadHotkeys()
		{
			string[] names = Enum.GetNames(typeof(Keys));
			for (int i = 0; i < names.Length; i++)
			{
				this.cbRepositionHotkey.Items.Add(names[i]);
				this.cbNextPanelHotkey.Items.Add(names[i]);
				this.cbPreviousPanelHotkey.Items.Add(names[i]);
			}
		}

		private void LoadSettings()
		{
			IniFile file = new IniFile(MainWindow.settings_path);
			if (file.Exists())
			{
				file.Load();
				try
				{
					this.chkAutoUpdate.Checked = bool.Parse(file["OptionsUpdates"]["chkAutoUpdate"]);
				}
				catch
				{
				}
				try
				{
					this.udGUI.Value = int.Parse(file["OptionsTimers"]["udGUI"]);
				}
				catch
				{
				}
				try
				{
					this.udDrawing.Value = int.Parse(file["OptionsTimers"]["udDrawing"]);
				}
				catch
				{
				}
				try
				{
					this.udData.Value = int.Parse(file["OptionsTimers"]["udData"]);
				}
				catch
				{
				}
				try
				{
					this.cbObserverPanelDrawDirection.SelectedItem = file["OptionsDrawing"]["cbObserverPanelDrawDirection"];
				}
				catch
				{
				}
				try
				{
					this.checkBoxTeamColors.Checked = bool.Parse(file["OptionsDrawing"]["chkTeamColors"]);
				}
				catch
				{
				}
				try
				{
					this.chkSpectator.Checked = bool.Parse(file["OptionsDrawing"]["chkSpectator"]);
				}
				catch
				{
				}
				try
				{
					this.chkCameraEnemies.Checked = bool.Parse(file["OptionsDrawing"]["chkCameraEnemies"]);
				}
				catch
				{
				}
				try
				{
					this.chkCameraAllies.Checked = bool.Parse(file["OptionsDrawing"]["chkCameraAllies"]);
				}
				catch
				{
				}
				try
				{
					this.chkCameraSelf.Checked = bool.Parse(file["OptionsDrawing"]["chkCameraSelf"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitsEnemies.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitsEnemies"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitsAllies.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitsAllies"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitsSelf.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitsSelf"]);
				}
				catch
				{
				}
				try
				{
					this.chk0Radius.Checked = bool.Parse(file["OptionsDrawing"]["chk0Radius"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitDestinationEnemies.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationEnemies"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitDestinationAllies.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationAllies"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitDestinationSelf.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationSelf"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitEnemiesScreen.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitEnemiesScreen"]);
				}
				catch
				{
				}
				try
				{
					this.chkUnitDestinationEnemiesScreen.Checked = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationEnemiesScreen"]);
				}
				catch
				{
				}
				try
				{
					this.cbRepositionHotkey.SelectedItem = file["OptionsHotkeys"]["cbRepositionHotkey"];
				}
				catch
				{
				}
				try
				{
					this.cbNextPanelHotkey.SelectedItem = file["OptionsHotkeys"]["cbNextPanelHotkey"];
				}
				catch
				{
				}
				try
				{
					this.cbPreviousPanelHotkey.SelectedItem = file["OptionsHotkeys"]["cbPreviousPanelHotkey"];
				}
				catch
				{
				}
			}
		}

		private void SaveSettings()
		{
			IniFile file = new IniFile(MainWindow.settings_path);
			if (file.Exists())
			{
				file.Load();
			}
			else
			{
				if (!Directory.Exists(MainWindow.settings_folder))
				{
					Directory.CreateDirectory(MainWindow.settings_folder);
				}
				File.Create(MainWindow.settings_path).Close();
			}

			IniSection section = new IniSection();
			if (file.HasSection("OptionsUpdates"))
			{
				file.Remove("OptionsUpdates");
			}
			section.Add("chkAutoUpdate", this.chkAutoUpdate.Checked.ToString());
			file.Add("OptionsUpdates", section);

			section = new IniSection();
			if (file.HasSection("OptionsDrawing"))
			{
				file.Remove("OptionsDrawing");
			}
			section.Add("chkTeamColors", this.checkBoxTeamColors.Checked.ToString());
			section.Add("cbObserverPanelDrawDirection", this.cbObserverPanelDrawDirection.SelectedItem.ToString());
			section.Add("chkCameraEnemies", this.chkCameraEnemies.Checked.ToString());
			section.Add("chkCameraAllies", this.chkCameraAllies.Checked.ToString());
			section.Add("chkCameraSelf", this.chkCameraSelf.Checked.ToString());
			section.Add("chkSpectator", this.chkSpectator.Checked.ToString());
			section.Add("chkUnitsEnemies", this.chkUnitsEnemies.Checked.ToString());
			section.Add("chkUnitsAllies", this.chkUnitsAllies.Checked.ToString());
			section.Add("chkUnitsSelf", this.chkUnitsSelf.Checked.ToString());
			section.Add("chk0Radius", this.chk0Radius.Checked.ToString());
			section.Add("chkUnitDestinationEnemies", this.chkUnitDestinationEnemies.Checked.ToString());
			section.Add("chkUnitDestinationAllies", this.chkUnitDestinationAllies.Checked.ToString());
			section.Add("chkUnitDestinationSelf", this.chkUnitDestinationSelf.Checked.ToString());
			section.Add("chkUnitEnemiesScreen", this.chkUnitEnemiesScreen.Checked.ToString());
			section.Add("chkUnitDestinationEnemiesScreen", this.chkUnitDestinationEnemiesScreen.Checked.ToString());
			file.Add("OptionsDrawing", section);
			section = new IniSection();
			if (file.HasSection("OptionsHotkeys"))
			{
				file.Remove("OptionsHotkeys");
			}
			section.Add("cbRepositionHotkey", this.cbRepositionHotkey.Text);
			section.Add("cbNextPanelHotkey", this.cbNextPanelHotkey.Text);
			section.Add("cbPreviousPanelHotkey", this.cbPreviousPanelHotkey.Text);
			file.Add("OptionsHotkeys", section);
			section = new IniSection();
			string str3 = "OptionsTimers";
			if (file.HasSection(str3))
			{
				file.Remove(str3);
			}
			section.Add("udGUI", this.udGUI.Value.ToString());
			section.Add("udDrawing", this.udDrawing.Value.ToString());
			section.Add("udData", this.udData.Value.ToString());
			file.Add(str3, section);
			file.Save();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!_2cs_API._2csAPI.InGame())
			{
				MessageBox.Show("This option only works when in a game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Rectangle MinimapRect = Data.GameData.GetMinimapCoords();
			MainWindow.minimap_location_x = MinimapRect.X;
			MainWindow.minimap_location_y = MinimapRect.Y;
			MainWindow.minimap_size_x = MinimapRect.Width;
			MainWindow.minimap_size_y = MinimapRect.Height;
			DirectX_HUDs.SaveMapSettings(MainWindow.minimap_location_x, MainWindow.minimap_location_y, MainWindow.minimap_size_x, MainWindow.minimap_size_y);
			if (MainWindow.HUDs != null && MainWindow.HUDs[1] != null && !MainWindow.HUDs[1].Disposing && !MainWindow.HUDs[1].IsDisposed
				&& MainWindow.HUDs[1].frame != null && !MainWindow.HUDs[1].frame.Disposing && !MainWindow.HUDs[1].frame.IsDisposed)
			{
				Rectangle WindowRect = MainWindow.HUDs[1].frame.DesktopBounds;
				Rectangle ClientRect = MainWindow.HUDs[1].frame.RectangleToScreen(MainWindow.HUDs[1].frame.ClientRectangle);

				MainWindow.HUDs[1].frame.Location = new Point(MainWindow.minimap_location_x - (ClientRect.Left - WindowRect.Left), MainWindow.minimap_location_y - (ClientRect.Top - WindowRect.Top));
				MainWindow.HUDs[1].frame.ClientSize = new Size(MainWindow.minimap_size_x, MainWindow.minimap_size_y);
			}
		}

		private void buttonCheckUpdates_Click(object sender, EventArgs e)
		{
			Program.CheckUpdates();
		}
	}
}

