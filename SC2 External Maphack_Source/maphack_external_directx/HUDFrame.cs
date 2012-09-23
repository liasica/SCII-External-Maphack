namespace maphack_external_directx
{
	using Ini;
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Windows.Forms;

	public class HUDFrame : Form
	{
		private bool _hasMoved;
		private ComboBox cbObserverPanel;
		private IContainer components;
		private DirectX_HUDs hud;

		public HUDFrame(DirectX_HUDs hud)
		{
			this.InitializeComponent();
			if (hud != null)
			{
				if (hud.ContentHUDType != DirectX_HUDs.HUDType.Map)
				{
					base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
				}
				if (hud.ContentHUDType == DirectX_HUDs.HUDType.Observer)
				{
					this.cbObserverPanel = new ComboBox();
					this.cbObserverPanel.Name = "cbObserverPanel";
					this.cbObserverPanel.DropDownStyle = ComboBoxStyle.DropDownList;
					string[] names = Enum.GetNames(typeof(DirectX_HUDs.ObserverPanelTabs));
					foreach (string str in names)
					{
						string str2 = str;
						int index = str2.IndexOf("__");
						if (index != -1)
						{
							str2 = str2.Remove(index);
						}
						this.cbObserverPanel.Items.Add(str2.Replace("_", " "));
					}
					this.cbObserverPanel.SelectedIndex = (int)DirectX_HUDs.CurrentObserverPanelTab;
					this.cbObserverPanel.SelectedIndexChanged += new EventHandler(this.cb_SelectedIndexChanged);
					base.Controls.Add(this.cbObserverPanel);
					int num2 = names.Max<string>((Func<string, int>) (x => x.Length));
					this.cbObserverPanel.Width = num2 * 6;
				}
				this.hud = hud;
				this.updateSize();
				base.Show();
				base.SendToBack();
			}
		}

		private void cb_SelectedIndexChanged(object sender, EventArgs e)
		{
			DirectX_HUDs.ObserverPanelTabs[] values = (DirectX_HUDs.ObserverPanelTabs[]) Enum.GetValues(typeof(DirectX_HUDs.ObserverPanelTabs));
			DirectX_HUDs.CurrentObserverPanelTab = values[this.cbObserverPanel.SelectedIndex];
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void HUDFrame_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.hud != null)
			{
				//this.saveHUDLocation(this.hud); //this will mess up the real position.
				this.hud.Close();
			}
		}

		private void HUDFrame_Move(object sender, EventArgs e)
		{
			this.updateLocation();
			base.SendToBack();
			if (this._hasMoved)
			{
				//this.saveHUDLocation(this.hud); //this will overwrite the real position before it can even load it.
				this._hasMoved = false;
			}
			this._hasMoved = true;
		}

		private void HUDFrame_Resize(object sender, EventArgs e)
		{
			//this.saveHUDLocation(this.hud); //this will overwrite the real position before it can even load it.
			this.updateSize();

		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HUDFrame));
			this.SuspendLayout();
			// 
			// HUDFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Fuchsia;
			this.ClientSize = new System.Drawing.Size(69, 51);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(75, 75);
			this.Name = "HUDFrame";
			this.Text = "HUD";
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.Fuchsia;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HUDFrame_FormClosed);
			this.Move += new System.EventHandler(this.HUDFrame_Move);
			this.Resize += new System.EventHandler(this.HUDFrame_Resize);
			this.ResumeLayout(false);

		}

		public void loadHUDLocation()
		{
			IniFile file = new IniFile(MainWindow.settings_path);
			if (file.Exists())
			{
				file.Load();
				try
				{
					string str = this.hud.ContentHUDType.ToString() + " HUD";
					int x = int.Parse(file[str]["X"]);
					int y = int.Parse(file[str]["Y"]);

                    if (x < 0 || y < 0)
                    {
                        MessageBox.Show("Error",
                            "Settings.ini contains negative position for HUD",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                    }

                    base.Location = new Point(x, y);

				}
				catch
				{
				}
			}
		}

		private void saveHUDLocation(DirectX_HUDs hud)
		{
			if (hud != null)
			{
				IniFile file = new IniFile(MainWindow.settings_path);
				if (file.Exists())
				{
					file.Load();
				}
				IniSection section = new IniSection();
				string str = hud.ContentHUDType.ToString() + " HUD";
				if (file.HasSection(str))
				{
					section = file[str];
					file.Remove(str);
				}

				if (section.ContainsKey("X"))
					section["X"] = base.Location.X.ToString();
				else
					section.Add("X", base.Location.X.ToString());

				if (section.ContainsKey("Y"))
					section["Y"] = base.Location.Y.ToString();
				else
					section.Add("Y", base.Location.Y.ToString());	

				file.Add(str, section);
				file.Save();
			}
		}

		public void updateLocation()
		{
			if (this.hud != null)
			{
				if (this.hud.ContentHUDType == DirectX_HUDs.HUDType.Observer)
				{
					this.hud.Location = base.PointToScreen(new Point(0, this.cbObserverPanel.Height));
				}
				else
				{
					this.hud.Location = base.PointToScreen(Point.Empty);
				}
			}
		}

		public void updateSize()
		{
			if (this.hud != null)
			{
				switch (this.hud.ContentHUDType)
				{
					case DirectX_HUDs.HUDType.Observer:
						switch (DirectX_HUDs.ObserverPanelDrawDirection)
						{
						}
						break;

					case DirectX_HUDs.HUDType.Resources:
						base.ClientSize = new Size((DirectX_HUDs.resouceColumnWidth + DirectX_HUDs.resourceIconFrameSize * 2) * 5,
							MainWindow.no_showing * (DirectX_HUDs.resourceIconSize.Height + DirectX_HUDs.resourceIconFrameSize * 2));

						this.hud.ClientSize = base.ClientSize;
						return;

					default:
						this.hud.ClientSize = base.ClientSize;
						base.Show();
						return;
				}
				base.ClientSize = new Size(this.cbObserverPanel.Width, this.cbObserverPanel.Height + (DirectX_HUDs.observerPlayerLogoHeight * MainWindow.no_showing));
				this.hud.ClientSize = new Size(800, 800);
			}
		}
	}
}

