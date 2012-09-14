namespace maphack_external_directx
{
	using _2cs_API;
	using Data;
	using Ini;
	using Microsoft.CSharp.RuntimeBinder;
	using Microsoft.DirectX;
	using Microsoft.DirectX.Direct3D;
	using ScreenAPI;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;
	using Utilities.KeyboardHook;
	using Utilities.WebTools;

	public class DirectX_HUDs : Form
	{
		private List<KeyValuePair<Rectangle, string>> Tooltips = new List<KeyValuePair<Rectangle, string>>();
		private Rectangle DesiredClientRect = new Rectangle(0,0,0,0);
		private Rectangle DefaultClientRect = new Rectangle(0, 0, 0, 0);

		private HUDType _HUDType;
		private List<Abil> abilities = new List<Abil>();
		private System.Drawing.Font arial = new System.Drawing.Font("Arial", 12f, FontStyle.Bold);
		private System.Drawing.Font arial2 = new System.Drawing.Font("Arial", 10f, FontStyle.Bold);
		private System.Drawing.Font arial3 = new System.Drawing.Font("Arial", 10f, FontStyle.Regular);
		private System.Drawing.Font arial4 = new System.Drawing.Font("Arial", 9f, FontStyle.Regular);
		private IContainer components;
		public static ObserverPanelTabs CurrentObserverPanelTab = ObserverPanelTabs.Buildings_and_Units_Same_Line;
		public Device device;
		private bool drawCameraAllies = true;
		private bool drawCameraEnemies = true;
		private bool drawCameraSelf;
		private bool drawCameraSpectator;
		private bool drawUnitDestinationAllies = true;
		private bool drawUnitDestinationEnemies = true;
		private bool drawUnitDestinationSelf;
		private bool drawUnitEnemiesDestinationScreen = true;
		private bool drawUnitEnemiesScreen = true;
		private bool drawUnitsAllies;
		private bool drawUnitsEnemies = true;
		private bool drawUnitsSelf;
		private SortedDictionary<int, Microsoft.DirectX.Direct3D.Font> fonts = new SortedDictionary<int, Microsoft.DirectX.Direct3D.Font>();
		public HUDFrame frame;
		public const int GWL_EXSTYLE = -20;
		public const int LWA_ALPHA = 2;
		public const int LWA_COLORKEY = 1;
		private Margins marg;
		private Utilities.KeyboardHook.KeyboardHook nextObserverPanelHotkey;
		public static ObserverPanelDirection ObserverPanelDrawDirection = ObserverPanelDirection.Down;
		public static int observerPlayerLogoHeight = 40;
		public static int observerPlayerLogoWidth = 40;
		public bool pause;
		private Utilities.KeyboardHook.KeyboardHook previousObserverPanelHotkey;
		public static int resouceColumnWidth = 100;
		public static Size resourceIconSize = new Size(24, 24);
		public static int resourceIconFrameSize = 2;
		public static int Size1x1 = 40;
		public static int Size2x2 = 80;
		public static int Size3x3 = 180;
		public static int Size5x5 = 250;
		private int TaskBarHeight = (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom);
		private Sprite textSprite;
		private SortedDictionary<string, Texture> textures = new SortedDictionary<string, Texture>();
		private Sprite textureSprite;
		public Timer tmrRefreshRate;
		
		public KeyValuePair<string, int>[] unit_textures = new KeyValuePair<string, int>[] { 
			new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-colossus.dds", 0x1d), new KeyValuePair<string, int>(@"Assets\btn-building-terran-techlab.dds", 30), new KeyValuePair<string, int>(@"Assets\btn-building-terran-reactor.dds", 0x1f), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-baneling.dds", 0x22), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-baneling.dds", 0x23), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-mothership.dds", 0x24), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-changeling.dds", 0x26), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-changeling.dds", 0x27), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-changeling.dds", 40), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-changeling.dds", 0x29), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-changeling.dds", 0x2a), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-changeling.dds", 0x2b), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-infestedmarine.dds", 0x2c), new KeyValuePair<string, int>(@"Assets\btn-building-terran-commandcenter.dds", 0x2d), new KeyValuePair<string, int>(@"Assets\btn-building-terran-supplydepot.dds", 0x2e), new KeyValuePair<string, int>(@"Assets\btn-building-terran-refinery.dds", 0x2f), 
			new KeyValuePair<string, int>(@"Assets\btn-building-terran-barracks.dds", 0x30), new KeyValuePair<string, int>(@"Assets\btn-building-terran-engineeringbay.dds", 0x31), new KeyValuePair<string, int>(@"Assets\btn-building-terran-missileturret.dds", 50), new KeyValuePair<string, int>(@"Assets\btn-building-terran-bunker.dds", 0x33), new KeyValuePair<string, int>(@"Assets\btn-building-terran-sensordome.dds", 0x34), new KeyValuePair<string, int>(@"Assets\btn-building-terran-ghostacademy.dds", 0x35), new KeyValuePair<string, int>(@"Assets\btn-building-terran-factory.dds", 0x36), new KeyValuePair<string, int>(@"Assets\btn-building-terran-starport.dds", 0x37), new KeyValuePair<string, int>(@"Assets\btn-building-terran-armory.dds", 0x39), new KeyValuePair<string, int>(@"Assets\btn-building-terran-fusioncore.dds", 0x3a), new KeyValuePair<string, int>(@"Assets\btn-building-terran-autoturret.dds", 0x3b), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-siegetank.dds", 60), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-siegetank.dds", 0x3d), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-vikingassault.dds", 0x3e), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-vikingfighter.dds", 0x3f), new KeyValuePair<string, int>(@"Assets\btn-building-terran-commandcenter.dds", 0x40), 
			new KeyValuePair<string, int>(@"Assets\btn-building-terran-techlab.dds", 0x41), new KeyValuePair<string, int>(@"Assets\btn-building-terran-reactor.dds", 0x42), new KeyValuePair<string, int>(@"Assets\btn-building-terran-techlab.dds", 0x43), new KeyValuePair<string, int>(@"Assets\btn-building-terran-reactor.dds", 0x44), new KeyValuePair<string, int>(@"Assets\btn-building-terran-techlab.dds", 0x45), new KeyValuePair<string, int>(@"Assets\btn-building-terran-reactor.dds", 70), new KeyValuePair<string, int>(@"Assets\btn-building-terran-factory.dds", 0x47), new KeyValuePair<string, int>(@"Assets\btn-building-terran-starport.dds", 0x48), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-scv.dds", 0x49), new KeyValuePair<string, int>(@"Assets\btn-building-terran-barracks.dds", 0x4a), new KeyValuePair<string, int>(@"Assets\btn-building-terran-supplydepot.dds", 0x4b), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-marine.dds", 0x4c), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-reaper.dds", 0x4d), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-ghost.dds", 0x4e), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-marauder.dds", 0x4f), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-thor.dds", 80), 
			new KeyValuePair<string, int>(@"Assets\btn-unit-terran-hellion.dds", 0x51), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-medivac.dds", 0x52), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-banshee.dds", 0x53), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-raven.dds", 0x54), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-battlecruiser.dds", 0x55), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-nexus.dds", 0x57), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-pylon.dds", 0x58), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-assimilator.dds", 0x59), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-gateway.dds", 90), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-forge.dds", 0x5b), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-fleetbeacon.dds", 0x5c), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-twilightcouncil.dds", 0x5d), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-photoncannon.dds", 0x5e), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-stargate.dds", 0x5f), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-templararchives.dds", 0x60), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-darkshrine.dds", 0x61), 
			new KeyValuePair<string, int>(@"Assets\btn-building-protoss-roboticssupportbay.dds", 0x62), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-roboticsfacility.dds", 0x63), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-cyberneticscore.dds", 100), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-zealot.dds", 0x65), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-stalker.dds", 0x66), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-hightemplar.dds", 0x67), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-darktemplar.dds", 0x68), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-sentry.dds", 0x69), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-phoenix.dds", 0x6a), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-carrier.dds", 0x6b), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-warpray.dds", 0x6c), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-warpprism.dds", 0x6d), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-observer.dds", 110), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-immortal.dds", 0x6f), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-probe.dds", 0x70), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-hatchery.dds", 0x72), 
			new KeyValuePair<string, int>(@"Assets\btn-building-zerg-creeptumor.dds", 0x73), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-extractor.dds", 0x74), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-spawningpool.dds", 0x75), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-evolutionchamber.dds", 0x76), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-hydraliskden.dds", 0x77), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-spire.dds", 120), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-ultraliskcavern.dds", 0x79), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-infestationpit.dds", 0x7a), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-nydusnetwork.dds", 0x7b), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-banelingnest.dds", 0x7c), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-roachwarren.dds", 0x7d), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-spinecrawler.dds", 0x7e), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-sporecrawler.dds", 0x7f), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-lair.dds", 0x80), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-hive.dds", 0x81), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-greaterspire.dds", 130), 
			new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-drone.dds", 0x84), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-zergling.dds", 0x85), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-overlord.dds", 0x86), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-hydralisk.dds", 0x87), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-mutalisk.dds", 0x88), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-ultralisk.dds", 0x89), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-roach.dds", 0x8a), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-infestor.dds", 0x8b), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-corruptor.dds", 140), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-broodlord.dds", 0x8d), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-broodlord.dds", 0x8e), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-baneling.dds", 0x8f), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-drone.dds", 0x90), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-hydralisk.dds", 0x91), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-roach.dds", 0x92), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-zergling.dds", 0x93), 
			new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-queen.dds", 0x99), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-queen.dds", 0x9a), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-infestor.dds", 0x9b), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-overlord.dds", 0x9c), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-overseer.dds", 0x9d), new KeyValuePair<string, int>(@"Assets\btn-building-terran-planetaryfortress.dds", 0x9e), new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-ultralisk.dds", 0x9f), new KeyValuePair<string, int>(@"Assets\btn-techupgrade-terran-orbitalcommand.dds", 160), new KeyValuePair<string, int>(@"Assets\btn-building-protoss-warpgate.dds", 0xa1), new KeyValuePair<string, int>(@"Assets\btn-techupgrade-terran-orbitalcommand.dds", 0xa2), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-warpprism.dds", 0xa4), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-creeptumor.dds", 0xa5), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-spinecrawler.dds", 0xa6), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-sporecrawler.dds", 0xa7), new KeyValuePair<string, int>(@"Assets\btn-unit-protoss-archon.dds", 0xa8), new KeyValuePair<string, int>(@"Assets\btn-building-zerg-nydusworm.dds", 0xa9), 
			new KeyValuePair<string, int>(@"Assets\btn-unit-zerg-larva.dds", 0xb1), new KeyValuePair<string, int>(@"Assets\btn-unit-terran-mule.dds", 0xb9)
		 };
		public KeyValuePair<int, int>[] unit_textures_sizes = new KeyValuePair<int, int>[] { 
			new KeyValuePair<int, int>(0x2d, Size5x5), new KeyValuePair<int, int>(0x2e, Size2x2), new KeyValuePair<int, int>(0x2f, Size3x3), new KeyValuePair<int, int>(0x30, Size3x3), new KeyValuePair<int, int>(0x31, Size3x3), new KeyValuePair<int, int>(50, Size2x2), new KeyValuePair<int, int>(0x29, Size3x3), new KeyValuePair<int, int>(0x34, Size1x1), new KeyValuePair<int, int>(0x35, Size3x3), new KeyValuePair<int, int>(0x36, Size3x3), new KeyValuePair<int, int>(0x37, Size3x3), new KeyValuePair<int, int>(0x38, Size3x3), new KeyValuePair<int, int>(0x39, Size3x3), new KeyValuePair<int, int>(0x3a, Size3x3), new KeyValuePair<int, int>(0x3b, Size3x3), new KeyValuePair<int, int>(60, Size3x3), 
			new KeyValuePair<int, int>(0x3d, Size3x3), new KeyValuePair<int, int>(0x3e, Size3x3), new KeyValuePair<int, int>(0x3f, Size3x3), new KeyValuePair<int, int>(0x40, Size3x3), new KeyValuePair<int, int>(0x41, Size2x2), new KeyValuePair<int, int>(0x42, Size2x2), new KeyValuePair<int, int>(0x43, Size2x2), new KeyValuePair<int, int>(0x44, Size2x2), new KeyValuePair<int, int>(0x45, Size2x2), new KeyValuePair<int, int>(70, Size2x2), new KeyValuePair<int, int>(0x47, Size2x2), new KeyValuePair<int, int>(0x48, Size2x2), new KeyValuePair<int, int>(0x4b, Size2x2), new KeyValuePair<int, int>(0x57, Size5x5), new KeyValuePair<int, int>(0x58, Size2x2), new KeyValuePair<int, int>(0x59, Size3x3), 
			new KeyValuePair<int, int>(90, Size3x3), new KeyValuePair<int, int>(0x5b, Size3x3), new KeyValuePair<int, int>(0x5c, Size3x3), new KeyValuePair<int, int>(0x5d, Size3x3), new KeyValuePair<int, int>(0x5e, Size2x2), new KeyValuePair<int, int>(0x5f, Size3x3), new KeyValuePair<int, int>(0x60, Size3x3), new KeyValuePair<int, int>(0x61, Size2x2), new KeyValuePair<int, int>(0x62, Size3x3), new KeyValuePair<int, int>(0x63, Size3x3), new KeyValuePair<int, int>(100, Size3x3), new KeyValuePair<int, int>(0x72, Size5x5), new KeyValuePair<int, int>(0x73, Size3x3), new KeyValuePair<int, int>(0x74, Size3x3), new KeyValuePair<int, int>(0x75, Size3x3), new KeyValuePair<int, int>(0x76, Size3x3), 
			new KeyValuePair<int, int>(0x77, Size3x3), new KeyValuePair<int, int>(120, Size2x2), new KeyValuePair<int, int>(0x79, Size3x3), new KeyValuePair<int, int>(0x7a, Size3x3), new KeyValuePair<int, int>(0x7b, Size3x3), new KeyValuePair<int, int>(0x7c, Size3x3), new KeyValuePair<int, int>(0x7d, Size3x3), new KeyValuePair<int, int>(0x7e, Size2x2), new KeyValuePair<int, int>(0x7f, Size2x2), new KeyValuePair<int, int>(0x80, Size3x3), new KeyValuePair<int, int>(0x81, Size3x3), new KeyValuePair<int, int>(130, Size3x3), new KeyValuePair<int, int>(0xa9, Size3x3)
		 };
		public const int WS_EX_LAYERED = 0x80000;
		public const long WS_EX_TOPMOST = 8L;
		public const int WS_EX_TRANSPARENT = 0x20;

		public DirectX_HUDs(HUDType hudType)
		{
			try
			{
				switch (hudType)
				{
					case HUDType.Map:
						this.DefaultClientRect = new Rectangle(50, 50, 150, 150); // Totally arbitrary.
						break;
					case HUDType.Observer:
						this.DefaultClientRect = new Rectangle(200, 200, 800, 800); // I think this is the default size...
						break;
					case HUDType.Resources:
						this.DefaultClientRect = new Rectangle(200, 200,
							(DirectX_HUDs.resouceColumnWidth + DirectX_HUDs.resourceIconFrameSize * 2) * 5,   // I Haven't looked at this much, but that's about what HUDFrame.updateSize() does,
							(DirectX_HUDs.resourceIconSize.Height + DirectX_HUDs.resourceIconFrameSize * 2)); // except that it's for 1 player instead of the current number.
						break;
					case HUDType.Info:
						this.DefaultClientRect = new Rectangle(200, 200, 800, 800); // I think this is the default size...
						break;
					default:
						this.DefaultClientRect = new Rectangle(50, 50, 150, 150); // Same as map.
						break;
				}

				this.InitializeComponent();
				this._HUDType = hudType;
				this.initWindow();
				this.initDirectX();
				this.initVariables();
				this.initFrame();
				this.LoadSettings();

				/*if (_HUDType == HUDType.Map)
				{
					Rectangle ScreenRect = Screen.GetBounds(new Point(MainWindow.minimap_location_x, MainWindow.minimap_location_y));
					if (MainWindow.minimap_location_x >= ScreenRect.Right
					|| MainWindow.minimap_location_y >= ScreenRect.Bottom
					|| MainWindow.minimap_size_x >= ScreenRect.Width
					|| MainWindow.minimap_size_y >= ScreenRect.Height)
					{

						Rectangle MinimapRect = GameData.GetMinimapCoords();
						MainWindow.minimap_location_x = MinimapRect.X;
						MainWindow.minimap_location_y = MinimapRect.Y;
						MainWindow.minimap_size_x = MinimapRect.Width;
						MainWindow.minimap_size_y = MinimapRect.Height;

						if (MainWindow.minimap_location_x >= ScreenRect.Right
						|| MainWindow.minimap_location_y >= ScreenRect.Bottom
						|| MainWindow.minimap_size_x >= ScreenRect.Width
						|| MainWindow.minimap_size_y >= ScreenRect.Height)
						{
							MainWindow.minimap_location_x = ScreenRect.Width / 2;
							MainWindow.minimap_location_y = ScreenRect.Height / 2;
							MainWindow.minimap_size_x = 262;
							MainWindow.minimap_size_y = 258;
						}

						SaveMapSettings(MainWindow.minimap_location_x, MainWindow.minimap_location_y, MainWindow.minimap_size_x, MainWindow.minimap_size_y);
					}

					Rectangle WindowRect = this.frame.DesktopBounds;
					Rectangle ClientRect = this.frame.RectangleToScreen(this.frame.ClientRectangle);

					this.frame.Location = new Point(MainWindow.minimap_location_x - (ClientRect.Left - WindowRect.Left), MainWindow.minimap_location_y - (ClientRect.Top - WindowRect.Top));
					this.frame.ClientSize = new Size(MainWindow.minimap_size_x, MainWindow.minimap_size_y);
				}*/

				this.tmrRefreshRate.Interval = MainWindow.HUDRefreshRate;
				this.tmrRefreshRate.Enabled = true;

			}
			catch (Exception ex)
			{
				WT.ReportCrash(ex, null);
			}
		}

		private void DirectX_HUDs_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this._HUDType == HUDType.Map)
			{
				Rectangle MinimapRect = base.RectangleToScreen(base.ClientRectangle);
				//SaveMapSettings(MinimapRect.X, MinimapRect.Y, MinimapRect.Width, MinimapRect.Height); //SavePositionAndSize() below should cover it.
			}
			this.SavePositionAndSize();
			this.device.Dispose();
			base.Dispose();
			this.frame.Close();
			this.frame.Dispose();
			DirectX_HUDs hud = this;
			Program.MainWindow.closeHUD(ref hud);
		}

		private void DirectX_HUDs_Paint(object sender, PaintEventArgs e)
		{
			this.marg.Left = 0;
			this.marg.Top = 0;
			this.marg.Right = base.Width;
			this.marg.Bottom = base.Height;
			if (AeroEnabled)
			{
				DwmExtendFrameIntoClientArea(base.Handle, ref this.marg);
			}
		}

		private void DirectX_HUDs_Shown(object sender, EventArgs e)
		{
			this.frame.Show();
			//if (this._HUDType != HUDType.Map)
				//this.frame.loadHUDLocation();
		}

		private void DirectX_HUDs_SizeChanged(object sender, EventArgs e)
		{
			if (this.device != null)
			{
				PresentParameters parameters = new PresentParameters
				{
					EnableAutoDepthStencil = true,
					AutoDepthStencilFormat = DepthFormat.D16,
					Windowed = true,
					SwapEffect = SwapEffect.Discard,
					BackBufferFormat = Microsoft.DirectX.Direct3D.Format.A8R8G8B8
				};
				try
				{
					this.device.Reset(new PresentParameters[] { parameters });
				}
				catch
				{
				}
			}
			if (this._HUDType == HUDType.Map)
			{
				Rectangle NewClientRect = base.RectangleToScreen(base.ClientRectangle);
				//MainWindow.minimap_size_x = NewClientRect.Width;
				//MainWindow.minimap_size_y = NewClientRect.Height;
			}
		}

		private void DirectX_HUDs_PositionChanged(object sender, EventArgs e)
		{
			if (this._HUDType == HUDType.Map)
			{
				Rectangle NewClientRect = base.RectangleToScreen(base.ClientRectangle);
				//MainWindow.minimap_location_x = NewClientRect.X;
				//MainWindow.minimap_location_y = NewClientRect.Y;
			}
		}

		private void DirectX_HUDs_VisibleChanged(object sender, EventArgs e)
		{
			this.frame.Hide();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void DrawCamera(float x, float y, float w, float h, Color col, bool PositionRatio = true)
		{
			if (PositionRatio)
			{
				w *= MainWindow.minimap_scale;
				h *= MainWindow.minimap_scale;
				x = (x - MainWindow.playable_map_left) * MainWindow.minimap_scale + MainWindow.minimap_offset_x;
				y = (y - (MainWindow.map_height - MainWindow.playable_map_top)) * MainWindow.minimap_scale + MainWindow.minimap_offset_y;
			}
			int num3 = (int)w;
			int num4 = (int)h;
			int num5 = (int)(10f * (w / h));
			this.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
			CustomVertex.TransformedColored[] vertexStreamZeroData = new CustomVertex.TransformedColored[] { new CustomVertex.TransformedColored(x - num5, y, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored((x + num3) + num5, y, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored((x + num3) + num5, y, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + num3, y + num4, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + num3, y + num4, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x, y + num4, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x, y + num4, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x - num5, y, 0f, 0f, col.ToArgb()) };
			this.device.DrawUserPrimitives(PrimitiveType.LineList, 4, vertexStreamZeroData);
		}

		private void drawCellFlags()
		{
			MpqManager manager = new MpqManager(GameData.getMapData().mapInfo.filePath);
			byte[] src = manager.read("t3CellFlags");
			manager.Close();
			byte[] dst = new byte[src.Length - 0x20];
			Buffer.BlockCopy(src, 0x20, dst, 0, dst.Length);
			int mapFullWidth = GameData.MapFullWidth;
			int mapFullHeight = GameData.MapFullHeight;
			for (int i = 0; i < dst.Length; i++)
			{
				Color pink = Color.Pink;
				switch (dst[i])
				{
					case 0:
						pink = Color.Black;
						break;

					case 1:
						pink = Color.Red;
						break;

					case 2:
						pink = Color.Yellow;
						break;

					case 3:
						pink = Color.LimeGreen;
						break;
				}
				int num3 = i / mapFullWidth;
				int num4 = i - (num3 * mapFullWidth);
				int num5 = 4;
				this.DrawRectangle((float)(num4 * num5), (float)((base.Height - num5) - (num3 * num5)), (float)num5, (float)num5, pink, true);
			}
			this.pause = true;
		}

		private void drawCliffLevel()
		{
			MpqManager manager = new MpqManager(GameData.getMapData().mapInfo.filePath);
			byte[] src = manager.read("t3SyncCliffLevel");
			manager.Close();
			byte[] dst = new byte[src.Length - 0x20];
			Buffer.BlockCopy(src, 0x20, dst, 0, dst.Length);
			int mapFullWidth = GameData.MapFullWidth;
			int mapFullHeight = GameData.MapFullHeight;
			for (int i = 0; i < dst.Length; i += 2)
			{
				Color col = Color.FromArgb(dst[i], 0, 0);
				int num3 = (i / 2) / mapFullWidth;
				int num4 = (i / 2) - (num3 * mapFullWidth);
				int num5 = 4;
				this.DrawRectangle((float)(num4 * num5), (float)((base.Height - num5) - (num3 * num5)), (float)num5, (float)num5, col, true);
			}
			this.pause = true;
		}

		public void DrawLine(float x1, float y1, float x2, float y2, Color col, bool PositionRatio = true)
		{
			float z = 0;
			if ((x1 != x2) || (y1 != y2))
			{
				if (PositionRatio)
				{
					x1 = (x1 - MainWindow.playable_map_left) * MainWindow.minimap_scale + MainWindow.minimap_offset_x;
					y1 = (y1 - (MainWindow.map_height - MainWindow.playable_map_top)) * MainWindow.minimap_scale + MainWindow.minimap_offset_y;
					x2 = (x2 - MainWindow.playable_map_left) * MainWindow.minimap_scale + MainWindow.minimap_offset_x;
					y2 = (y2 - (MainWindow.map_height - MainWindow.playable_map_top)) * MainWindow.minimap_scale + MainWindow.minimap_offset_y;
				}

				this.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
				CustomVertex.TransformedColored[] vertexStreamZeroData = new CustomVertex.TransformedColored[] { new CustomVertex.TransformedColored(x1, y1, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x2, y2, z, 0f, col.ToArgb()) };
				this.device.DrawUserPrimitives(PrimitiveType.LineList, 1, vertexStreamZeroData);
			}
		}

		private void DrawMap()
		{


			if (((float)base.ClientRectangle.Height) / ((float)base.ClientRectangle.Width) >= MainWindow.playable_map_height / MainWindow.playable_map_width)
			{
				MainWindow.minimap_scale = (float)base.ClientRectangle.Width / MainWindow.playable_map_width;
				MainWindow.minimap_offset_x = 0;
				MainWindow.minimap_offset_y = ((float)base.ClientRectangle.Height - MainWindow.minimap_scale * MainWindow.playable_map_height) / 2;
			}
			else
			{
				MainWindow.minimap_scale = (float)base.ClientRectangle.Height / MainWindow.playable_map_height;
				MainWindow.minimap_offset_y = 0;
				MainWindow.minimap_offset_x = ((float)base.ClientRectangle.Width - MainWindow.minimap_scale * MainWindow.playable_map_width) / 2;
			}

			Color OutlineColor = Color.FromArgb(0, 255, 0, 0);
			Color OutlineColor2 = Color.FromArgb(0, 0, 255, 0);

			this.DrawRectangleOutline(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1, OutlineColor, false);
			this.DrawRectangleOutline(MainWindow.minimap_offset_x, MainWindow.minimap_offset_y, this.ClientSize.Width - MainWindow.minimap_offset_x * 2 - 1
				, this.ClientSize.Height - MainWindow.minimap_offset_y * 2 - 1, OutlineColor2, false);

			this.DrawUnitDestinations();

			this.device.RenderState.ZBufferEnable = true;
			this.device.RenderState.ZBufferFunction = Compare.GreaterEqual;
			this.device.RenderState.ZBufferWriteEnable = true;
			this.device.Clear(ClearFlags.ZBuffer, Color.FromArgb(0, 1, 1, 1), 0.0f, 0);

			this.DrawUnitPositionsOnMap();

			this.device.RenderState.ZBufferWriteEnable = false;
			this.device.RenderState.ZBufferEnable = false;

			this.DrawPlayerCameras();
			MainWindow.UpdateRefreshes("Map");
		}

		private void drawPerimeterWithRamps()
		{
			int scale = 4;

			MpqManager manager = new MpqManager(GameData.getMapData().mapInfo.filePath);
			byte[] src = manager.read("t3SyncCliffLevel");
			manager.Close();
			byte[] dst = new byte[src.Length - 0x20];
			Buffer.BlockCopy(src, 0x20, dst, 0, dst.Length);
			int mapFullWidth = GameData.MapFullWidth;
			int mapFullHeight = GameData.MapFullHeight;
			object[,] heights = new object[mapFullWidth, mapFullHeight];
			for (int i = 0; i < dst.Length; i += 2)
			{
				Color color = Color.FromArgb(dst[i], 0, 0);
				int row = (i / 2) / mapFullWidth;
				int column = (i / 2) % mapFullWidth;
				heights[column, row] = new { x = column, y = row, h = color.R };
			}

			for (int j = 1; j < (mapFullWidth - 1); j++)
			{
				for (int k = 1; k < (mapFullHeight - 1); k++)
				{
					dynamic baseCell = heights[j, k];

					if (baseCell.h % 64 != 0)
					{
						this.DrawRectangle(baseCell.x * scale, (base.Height - scale) - (baseCell.y * scale), scale, scale, Color.Blue);
					}
					else
					{
						dynamic tallestCell = this.getTallest(heights, j, k);
						this.DrawRectangle(tallestCell.x * scale, (base.Height - scale) - (tallestCell.y * scale), scale, scale, Color.Red);
					}
				}
			}
			this.pause = true;
		}

		private void DrawPlayerCameras()
		{
			lock (MainWindow.players)
			{
				foreach (Player player in MainWindow.players)
				{
					if (player.team != MainWindow.localteam)
					{
						if (this.drawCameraEnemies)
						{
							goto Label_0042;
						}
						continue;
					}
					if (player.number == MainWindow.localplayer)
					{
						if (this.drawCameraSelf)
						{
							goto Label_0042;
						}
						continue;
					}
					if (!this.drawCameraAllies)
					{
						continue;
					}
				Label_0042:
					if (this.drawCameraSpectator || ((MainWindow.player_playing[player.number] && (player.playerType != PlayerType.Spectator)) && (player.playerType != PlayerType.Referee)))
					{
						if (player.victoryStatus != VictoryStatus.Playing)
						{
							return;
						}
						float x = player.cameraX - 10;
						float y = player.cameraY+ 15;
						this.DrawCamera(x, MainWindow.map_height - y, 20f, 20f, player.drawingColor, true);
					}
				}
			}
		}

		private void drawInfo()
		{
			long freq;
			Imports.QueryPerformanceFrequency(out freq);
			
			MainWindow.UpdateRefreshes("Info");
			lock (MainWindow.Refreshes)
			{
				int y = 0;
				foreach (KeyValuePair<string, Queue<long>> pair in MainWindow.Refreshes)
				{
					double FPS = 0;
					if(pair.Value.Count >= 2)
						FPS = (double)pair.Value.Count / ((double)(pair.Value.Last() - pair.Value.First()) / (double)freq);
					string text = pair.Key + ": " + FPS.ToString("F1");

					if (!this.fonts.ContainsKey(this.arial4.GetHashCode()))
						this.fonts.Add(this.arial4.GetHashCode(), new Microsoft.DirectX.Direct3D.Font(this.device, this.arial4));

					Rectangle rectangle = this.fonts[this.arial4.GetHashCode()].MeasureString(this.textSprite, text, DrawTextFormat.None, Color.White);

					int X = 5;
					int Y = 5 + y * rectangle.Height;

					this.DrawRectangle(X - 2, Y, rectangle.Width + 4, rectangle.Height, Color.Black, false);
					this.DrawText(X, Y, text, this.arial4, Color.White, false);

					y++;
				}
			}
		}

		private void drawPlayerResources()
		{
			bool showMins = false;
			bool showGas = false;
			bool showTerrazine = false;
			bool showCustom = false;
			bool showSupply = false;

			int index = 0;
			lock (MainWindow.players)
			{
				foreach (Player player in MainWindow.players)
				{
					if (MainWindow.show_window[player.number])
					{
						if (player.minerals != 0)
							showMins = true;
						if (player.gas != 0)
							showGas = true;
						if (player.terrazine != 0)
							showTerrazine = true;
						if (player.custom != 0)
							showCustom = true;
						if (player.supply != 0 && player.supplyCap != 0)
							showSupply = true;
					}
				}

				int f = resourceIconFrameSize;
				int f2 = resourceIconFrameSize * 2;
				foreach (Player player in MainWindow.players)
				{
					int pNumber = player.number;
					string pMins = player.minerals.ToString();
					string pGas = player.gas.ToString();
					string pTerra = player.terrazine.ToString();
					string pCustom = player.custom.ToString();
					string pSupply = player.supply.ToString() + "/" + player.supplyCap.ToString();

					string pName = player.name;
					string pRace = player.race.ToString().ToLower();
					Color pColor = player.drawingColor;

					if (MainWindow.show_window[pNumber])
					{
						int x = f;
						int y = (index * (resourceIconSize.Height + f2)) + f;
						int textY = (int)(y + (resourceIconSize.Height - this.arial2.GetHeight()) / 2);

						bool MoreMins = false;

						if (showMins)
						{
							this.DrawRectangle((float)(x - f), (float)(y - f), (float)(resourceIconSize.Width + f2), (float)(resourceIconSize.Height + f2), pColor, false);
							if (MoreMins)
								this.DrawTexture((float)x, (float)y, (float)resourceIconSize.Width, (float)resourceIconSize.Height, Database.GetItemFilename(@"Assets\Textures\icon-highyieldmineral-" + pRace + ".dds", false), false);
							else
								this.DrawTexture((float)x, (float)y, (float)resourceIconSize.Width, (float)resourceIconSize.Height, Database.GetItemFilename(@"Assets\Textures\icon-mineral-" + pRace + ".dds", false), false);

							this.DrawText((x + resourceIconSize.Width + f) + 5, textY, pMins, this.arial2, Color.White, false);
							this.Tooltips.Add(new KeyValuePair<Rectangle, string>(new Rectangle(x - f, y - f, resouceColumnWidth + f2, resourceIconSize.Height + f2), pName + "'s minerals: " + pMins));
							x += resouceColumnWidth + f2;
						}
						if (showGas)
						{
							this.DrawRectangle((float)(x - f), (float)(y - f), (float)(resourceIconSize.Width + f2), (float)(resourceIconSize.Height + f2), pColor, false);
							this.DrawTexture((float)x, (float)y, (float)resourceIconSize.Width, (float)resourceIconSize.Height, Database.GetItemFilename(@"Assets\Textures\icon-gas-" + pRace + ".dds", false), false);
							this.DrawText((x + resourceIconSize.Width + f) + 5, textY, pGas, this.arial2, Color.White, false);
							this.Tooltips.Add(new KeyValuePair<Rectangle, string>(new Rectangle(x - f, y - f, resouceColumnWidth + f2, resourceIconSize.Height + f2), pName + "'s vespene: " + pGas));
							x += resouceColumnWidth + f2;
						}
						if (showTerrazine)
						{
							this.DrawRectangle((float)(x - f), (float)(y - f), (float)(resourceIconSize.Width + f2), (float)(resourceIconSize.Height + f2), pColor, false);
							this.DrawTexture((float)x, (float)y, (float)resourceIconSize.Width, (float)resourceIconSize.Height, Database.GetItemFilename(@"Assets\Textures\icon-energy-" + pRace + ".dds", false), false);
							this.DrawText((x + resourceIconSize.Width + f) + 5, textY, pTerra, this.arial2, Color.White, false);
							this.Tooltips.Add(new KeyValuePair<Rectangle, string>(new Rectangle(x - f, y - f, resouceColumnWidth + f2, resourceIconSize.Height + f2), pName + "'s terrazine: " + pTerra));
							x += resouceColumnWidth + f2;
						}
						if (showCustom)
						{
							this.DrawRectangle((float)(x - f), (float)(y - f), (float)(resourceIconSize.Width + f2), (float)(resourceIconSize.Height + f2), pColor, false);
							this.DrawTexture((float)x, (float)y, (float)resourceIconSize.Width, (float)resourceIconSize.Height, Database.GetItemFilename(@"Assets\Textures\icon-health-" + pRace + ".dds", false), false);
							this.DrawText((x + resourceIconSize.Width + f) + 5, textY, pCustom, this.arial2, Color.White, false);
							this.Tooltips.Add(new KeyValuePair<Rectangle, string>(new Rectangle(x - f, y - f, resouceColumnWidth + f2, resourceIconSize.Height + f2), pName + "'s custom: " + pCustom));
							x += resouceColumnWidth + f2;
						}
						if (showSupply)
						{
							this.DrawRectangle((float)(x - f), (float)(y - f), (float)(resourceIconSize.Width + f2), (float)(resourceIconSize.Height + f2), pColor, false);
							this.DrawTexture((float)x, (float)y, (float)resourceIconSize.Width, (float)resourceIconSize.Height, Database.GetItemFilename(@"Assets\Textures\icon-supply-" + pRace + ".dds", false), false);
							this.DrawText((x + resourceIconSize.Width + f) + 5, textY, pSupply, this.arial2, Color.White, false);
							this.Tooltips.Add(new KeyValuePair<Rectangle, string>(new Rectangle(x - f, y - f, resouceColumnWidth + f2, resourceIconSize.Height + f2), pName + "'s supply: " + pSupply));
						}
						index++;
					}
				}
			}
			MainWindow.UpdateRefreshes("Resources");
		}

		private void DrawPlayerUnitData(UnitDrawStyle drawStyle, int row, int p_no)
		{
			int num3 = 0;
			int num4 = 0;
			if (MainWindow.unit_counts == null || p_no < 0 || p_no >= MainWindow.unit_counts.Length && MainWindow.unit_counts[p_no] != null
				|| MainWindow.unit_pictures == null || MainWindow.unit_names == null)
				return;

			lock (MainWindow.unit_counts)
			{
				if (MainWindow.unit_counts == null || MainWindow.unit_counts[p_no] == null)
					return;
				foreach (KeyValuePair<string, int> pair in MainWindow.unit_counts[p_no])
				{
					if (pair.Key == null)
						continue;

					string picture = "";
					string tooltip = "";
					
					lock (MainWindow.unit_names)
					{
						if (MainWindow.unit_names.ContainsKey(pair.Key) && MainWindow.unit_names[pair.Key] != null)
							tooltip = MainWindow.unit_names[pair.Key];
					}
					if (string.IsNullOrWhiteSpace(tooltip))
						tooltip = "Unable to get unit name.";

					lock(MainWindow.unit_pictures)
					{
						if (pair.Value > 0 && MainWindow.unit_pictures.ContainsKey(pair.Key))
							picture = MainWindow.unit_pictures[pair.Key];
					}
					picture = Database.GetItemFilename(picture, false);

					if(picture != null) //it's acceptable if picture == ""
					{
						int count = pair.Value;
						switch (CurrentObserverPanelTab)
						{
							case ObserverPanelTabs.Buildings_and_Units_Same_Line:
								{
									this.DrawPlayerWindowIndividualUnit(drawStyle, row, num3++, p_no, count, picture, tooltip, observerPlayerLogoWidth, observerPlayerLogoHeight);
									continue;
								}
							case ObserverPanelTabs.Buildings_and_Units_Different_Lines:
								{
									if (MainWindow.buildings.Contains(pair.Key))
										this.DrawPlayerWindowIndividualUnit(drawStyle, row, num3++, p_no, count, picture, tooltip, observerPlayerLogoWidth, observerPlayerLogoHeight);
									else
										this.DrawPlayerWindowIndividualUnit(drawStyle, row + 1, num4++, p_no, count, picture, tooltip, observerPlayerLogoWidth, observerPlayerLogoHeight);
									continue;
								}
							case ObserverPanelTabs.Buildings:
								{
									if (MainWindow.buildings.Contains(pair.Key))
										this.DrawPlayerWindowIndividualUnit(drawStyle, row, num3++, p_no, count, picture, tooltip, observerPlayerLogoWidth, observerPlayerLogoHeight);
									continue;
								}
							case ObserverPanelTabs.Units:
								{
									if (!MainWindow.buildings.Contains(pair.Key))
										this.DrawPlayerWindowIndividualUnit(drawStyle, row, num3++, p_no, count, picture, tooltip, observerPlayerLogoWidth, observerPlayerLogoHeight);
									continue;
								}
						}
						
					}
					continue;

				}
			}
		}

		private void DrawPlayerWindowIndividualUnit(UnitDrawStyle drawStyle, int row, int column, int p_no, int unit_count, string picture_filename, string tooltip, int width, int height)
		{
			if (drawStyle == UnitDrawStyle.ObserverPanel)
			{
				int num = 0;
				int num2 = 0;
				switch (ObserverPanelDrawDirection)
				{
					case ObserverPanelDirection.Down:
						num = row * width;
						num2 = height + (column * height);
						break;

					case ObserverPanelDirection.Left:
						num = width + (column * width);
						num2 = row * height;
						break;
				}
				this.Tooltips.Add(new KeyValuePair<Rectangle, string>(new Rectangle(num, num2, width, height), tooltip));

				this.DrawRectangle((float)num, (float)num2, (float)width, (float)height, Color.Black, false);
				this.DrawRectangleOutline((float)num, (float)num2, (float)width, (float)height, Color.DarkGreen, false);
				this.DrawTexture((float)num, (float)num2, (float)width, (float)height, picture_filename, false);
				if (!this.fonts.ContainsKey(this.arial2.GetHashCode()))
				{
					this.fonts.Add(this.arial2.GetHashCode(), new Microsoft.DirectX.Direct3D.Font(this.device, this.arial2));
				}
				Rectangle rectangle = this.fonts[this.arial2.GetHashCode()].MeasureString(this.textSprite, unit_count.ToString(), DrawTextFormat.None, Color.White);
				this.DrawText((num + width) - rectangle.Width, (num2 + height) - rectangle.Height, unit_count.ToString(), this.arial2, Color.White, false);
			}
		}

		private void DrawPlayerWindows()
		{
			Color black;
			if (AeroEnabled)
			{
				black = Color.Black;
			}
			else
			{
				black = Color.Black;
			}
			int index = 1;
			int num2 = 0;
			while (index < 0x10)
			{
				if (MainWindow.show_window[index])
				{
					Data.Player player = GameData.GetPlayer(index);
					int row = num2++;
					if (CurrentObserverPanelTab == ObserverPanelTabs.Buildings_and_Units_Different_Lines)
					{
						row *= 2;
					}
					int num4 = 0;
					int num5 = 0;
					switch (ObserverPanelDrawDirection)
					{
						case ObserverPanelDirection.Down:
							num4 = row * observerPlayerLogoWidth;
							num5 = 0;
							break;

						case ObserverPanelDirection.Left:
							num4 = 0;
							num5 = row * observerPlayerLogoHeight;
							break;
					}
					this.DrawRectangle((float)num4, (float)num5, (float)observerPlayerLogoWidth, (float)observerPlayerLogoHeight, player.drawingColor, false);
					this.DrawRectangle((float)(num4 + 2), (float)(num5 + 2), (float)(observerPlayerLogoWidth - 4), (float)(observerPlayerLogoHeight - 4), black, false);
					this.Tooltips.Add(new KeyValuePair<Rectangle,string>(new Rectangle(num4, num5, observerPlayerLogoWidth, observerPlayerLogoHeight), player.name + " (" + player.number + ")"));

					this.DrawPlayerUnitData(UnitDrawStyle.ObserverPanel, row, index);
				}
				index++;
			}
			MainWindow.UpdateRefreshes("Observer");
		}

		private void DrawTooltip(string Text, int X, int Y)
		{
			int BorderSize = 2;

			if (!this.fonts.ContainsKey(this.arial2.GetHashCode()))
			{
				this.fonts.Add(this.arial2.GetHashCode(), new Microsoft.DirectX.Direct3D.Font(this.device, this.arial2));
			}
			Rectangle rectangle = this.fonts[this.arial2.GetHashCode()].MeasureString(this.textSprite, Text, DrawTextFormat.None, Color.White);

			this.DrawRectangle(X, Y - (rectangle.Height + BorderSize * 2), rectangle.Width + BorderSize * 4, rectangle.Height + BorderSize * 2, Color.Black, false, false, 0);
			this.DrawRectangleOutline(X, Y - (rectangle.Height + BorderSize * 2), rectangle.Width + BorderSize * 4, rectangle.Height + BorderSize * 2, Color.DarkGray, false, false, 0);
			this.DrawText(X + BorderSize * 2, Y - (rectangle.Height + BorderSize), Text, this.arial2, Color.White, false);
		}

		private void drawRamps()
		{
			MpqManager manager = new MpqManager(GameData.getMapData().mapInfo.filePath);
			byte[] src = manager.read("t3SyncCliffLevel");
			manager.Close();
			byte[] dst = new byte[src.Length - 0x20];
			Buffer.BlockCopy(src, 0x20, dst, 0, dst.Length);
			int mapFullWidth = GameData.MapFullWidth;
			int mapFullHeight = GameData.MapFullHeight;
			for (int i = 0; i < dst.Length; i += 2)
			{
				Color col = Color.FromArgb(dst[i], 0, 0);
				if ((col.R % 0x40) != 0)
				{
					int num3 = (i / 2) / mapFullWidth;
					int num4 = (i / 2) - (num3 * mapFullWidth);
					int num5 = 4;
					this.DrawRectangle((float)(num4 * num5), (float)((base.Height - num5) - (num3 * num5)), (float)num5, (float)num5, col, true);
				}
			}
			this.pause = true;
		}

		public void DrawRectangle(float x, float y, float width, float height, Color col, bool PositionRatio = true, bool? SizeRatio = null, float z = 0)
		{
			if (SizeRatio == null)
				SizeRatio = PositionRatio;

			if ((bool)SizeRatio)
			{
				width *= MainWindow.minimap_scale;
				height *= MainWindow.minimap_scale;
			}
			if (PositionRatio)
			{
				x = (x - MainWindow.playable_map_left) * MainWindow.minimap_scale + MainWindow.minimap_offset_x - width / 2;
				y = (y - (MainWindow.map_height - MainWindow.playable_map_top)) * MainWindow.minimap_scale + MainWindow.minimap_offset_y - height / 2;
			}

			if (z != 0)
				z /= 356.0f;

			this.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
			CustomVertex.TransformedColored[] vertexStreamZeroData = new CustomVertex.TransformedColored[] { new CustomVertex.TransformedColored(x, y, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + width, y, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + width, y + height, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x, y + height, z, 0f, col.ToArgb()) };
			this.device.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, vertexStreamZeroData);
		}

		public void DrawRectangleOutline(float x, float y, float width, float height, Color col, bool PositionRatio = true, bool? SizeRatio = null, float z = 0)
		{
			if (SizeRatio == null)
				SizeRatio = PositionRatio;

			if ((bool)SizeRatio)
			{
				width *= MainWindow.minimap_scale;
				height *= MainWindow.minimap_scale;
			}
			if (PositionRatio)
			{
				x = (x - MainWindow.playable_map_left) * MainWindow.minimap_scale + MainWindow.minimap_offset_x - width / 2;
				y = (y - (MainWindow.map_height - MainWindow.playable_map_top)) * MainWindow.minimap_scale + MainWindow.minimap_offset_y - height / 2;
			}

			if (z != 0)
				z /= 356.0f;

			this.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
			CustomVertex.TransformedColored[] vertexStreamZeroData = new CustomVertex.TransformedColored[] { new CustomVertex.TransformedColored(x, y, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + width, y, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + width, y, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + width, y + height, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x + width, y + height, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x, y + height, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x, y + height, z, 0f, col.ToArgb()), new CustomVertex.TransformedColored(x, y, z, 0f, col.ToArgb()) };
			this.device.DrawUserPrimitives(PrimitiveType.LineList, 4, vertexStreamZeroData);
		}

		private void DrawStuff()
		{
			this.Tooltips = new List<KeyValuePair<Rectangle, string>>();
			if (GameData.SC2Opened)
			{
				switch (this._HUDType)
				{
					case HUDType.Map:
						this.DrawMap();
						break;

					case HUDType.Observer:
						this.DrawPlayerWindows();
						break;

					case HUDType.Resources:
						this.drawPlayerResources();
						break;

					case HUDType.Info:
						this.drawInfo();
						break;

					case HUDType.CellFlags:
						this.drawCellFlags();
						break;

					case HUDType.CliffLevel:
						this.drawCliffLevel();
						break;

					case HUDType.Ramps:
						this.drawRamps();
						break;

					case HUDType.PerimeterWithRamp:
						this.drawPerimeterWithRamps();
						break;
				}
				if (RectangleToScreen(this.ClientRectangle).Contains(Cursor.Position))
				{
					Point ClientMousePos = PointToClient(Cursor.Position);

					foreach (KeyValuePair<Rectangle, string> pair in this.Tooltips)
					{
						if (pair.Key.Contains(ClientMousePos))
						{
							DrawTooltip(pair.Value, ClientMousePos.X, ClientMousePos.Y);
							break;
						}
					}
				}
			}
		}

		public void DrawText(int x, int y, string text, System.Drawing.Font font, Color col, bool PositionRatio = true)
		{
			if (PositionRatio)
			{
				float num = ((float)base.ClientRectangle.Width) / MainWindow.map_width;
				float num2 = ((float)base.ClientRectangle.Height) / MainWindow.map_height;
				x = (int)(x * num);
				y = (int)(y * num2);
			}

			this.textSprite.Begin(SpriteFlags.AlphaBlend);
			if (!this.fonts.ContainsKey(font.GetHashCode()))
			{
				this.fonts.Add(font.GetHashCode(), new Microsoft.DirectX.Direct3D.Font(this.device, font));
			}
			this.fonts[font.GetHashCode()].DrawText(this.textSprite, text, x, y, col);
			this.textSprite.End();
		}

		public void DrawTexture(float x, float y, float width, float height, string texturefile, bool PositionRatio = true, bool? SizeRatio = null)
		{
			if (SizeRatio == null)
				SizeRatio = PositionRatio;

			if (PositionRatio)
			{
				//float MainWindow.minimap_scale = ((float) base.ClientRectangle.Width) / MainWindow.map_width;
				//float MainWindow.minimap_scale = ((float) base.ClientRectangle.Height) / MainWindow.map_height;

				x = (x - MainWindow.playable_map_left) * MainWindow.minimap_scale;
				y = (y - (MainWindow.map_height - MainWindow.playable_map_top)) * MainWindow.minimap_scale;
			}
			if (PositionRatio)
			{
				width *= MainWindow.minimap_scale;
				height *= MainWindow.minimap_scale;
			}
			if (!this.textures.ContainsKey(texturefile))
			{
				if (!File.Exists(texturefile))
				{
					return;
				}
				this.textures.Add(texturefile, TextureLoader.FromFile(this.device, texturefile));
			}
			this.textureSprite.Begin(SpriteFlags.AlphaBlend);
			this.textureSprite.Draw2D(this.textures[texturefile], Rectangle.Empty, new SizeF(width, height), new PointF(x, y), Color.White);
			this.textureSprite.End();
		}

		public void DrawTriangle(Microsoft.DirectX.Vector2 v1, Microsoft.DirectX.Vector2 v2, Microsoft.DirectX.Vector2 v3, Color col)
		{
			this.device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
			CustomVertex.TransformedColored[] vertexStreamZeroData = new CustomVertex.TransformedColored[] { new CustomVertex.TransformedColored(v1.X, v1.Y, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(v2.X, v2.Y, 0f, 0f, col.ToArgb()), new CustomVertex.TransformedColored(v3.X, v3.Y, 0f, 0f, col.ToArgb()) };
			this.device.DrawUserPrimitives(PrimitiveType.TriangleList, 1, vertexStreamZeroData);
		}

		private void DrawUnitDestinations()
		{
			foreach (Unit unit in MainWindow.units)
			{
				uint uOwner = unit.playerNumber;
				if (MainWindow.player_teams[uOwner] != MainWindow.localteam)
				{
					if (this.drawUnitDestinationEnemies)
					{
						goto Label_005A;
					}
					continue;
				}
				if (uOwner == MainWindow.localplayer)
				{
					if (this.drawUnitDestinationSelf)
					{
						goto Label_005A;
					}
					continue;
				}
				if (!this.drawUnitDestinationAllies)
				{
					continue;
				}

			Label_005A:
				float uDX = unit.destinationX;
				float uDY = unit.destinationY;
				if (/*unit.commandQueuePointer != 0 && */(uDX != 0.0) && (uDY != 0.0))
				{
					this.DrawLine(unit.locationX, (MainWindow.map_height - unit.locationY), uDX, (MainWindow.map_height - uDY), Color.Yellow, true);
				}
				/*float x = unit.locationX + (float) Math.Cos((unit.rotation - 90) * (Math.PI / 180)) * 20;
				float y = unit.locationY - (float) Math.Sin((unit.rotation - 90) * (Math.PI / 180)) * 20;
				this.DrawLine(unit.locationX, (MainWindow.map_height - unit.locationY), x, (MainWindow.map_height - y), Color.Lime, true);*/
			}
		}

		private void DrawUnitPositionsOnMap()
		{
			float MinimumRadius = 1f / MainWindow.minimap_scale;
			float AddToRadius = 1f / MainWindow.minimap_scale;

			foreach (Unit unit in MainWindow.units)
			{
				if (!unit.isAlive || (unit.targetFilterFlags & TargetFilter.Missile) != 0)
				{
					continue;
				}

				uint uOwner = unit.playerNumber;
				if (MainWindow.player_teams[uOwner] != MainWindow.localteam && MainWindow.player_teams[uOwner] != 16)
				{
					if (this.drawUnitsEnemies)
					{
						goto Label_005A;
					}
					continue;
				}
				if (uOwner == MainWindow.neutralplayer)
				{
					goto Label_005A;
				}
				if (uOwner == MainWindow.localplayer)
				{
					if (this.drawUnitsSelf)
					{
						goto Label_005A;
					}
					continue;
				}
				if (!this.drawUnitsAllies)
				{
					continue;
				}

			Label_005A:
				float Radius = unit.minimapRadius;
				
				if (Radius < MinimumRadius)
					Radius = MinimumRadius;
				Radius += AddToRadius;

				float uX = unit.locationX;
				float uY = MainWindow.map_height - unit.locationY;
				bool uDetect = unit.detector;
				bool uCloak = unit.cloaked;
				Color uColor = GameData.player_colors[uOwner];

				if (uCloak)
				{
					if (uDetect)
					{
						this.DrawRectangle(uX, uY, (Radius + AddToRadius * 2) * 2, (Radius + AddToRadius * 2) * 2, Color.Black, true, true, 108.4f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
						this.DrawRectangle(uX, uY, (Radius + AddToRadius) * 2, (Radius + AddToRadius) * 2, Color.Gold, true, true, 108.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
						this.DrawRectangle(uX, uY, Radius * 2, Radius * 2, Color.Gray, true, true, 109.4f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
						this.DrawRectangle(uX, uY, (Radius - AddToRadius) * 2, (Radius - AddToRadius) * 2, uColor, true, true, 109.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
					}
					else
					{
						this.DrawRectangle(uX, uY, (Radius + AddToRadius) * 2, (Radius + AddToRadius) * 2, Color.Black, true, true, 98.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
						this.DrawRectangle(uX, uY, Radius * 2, Radius * 2, Color.Gray, true, true, 99.4f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
						this.DrawRectangle(uX, uY, (Radius - AddToRadius) * 2, (Radius - AddToRadius) * 2, uColor, true, true, 99.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
					}
				}
				else if (uDetect)
				{
					this.DrawRectangle(uX, uY, (Radius + AddToRadius) * 2, (Radius + AddToRadius) * 2, Color.Black, true, true, 48.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
					this.DrawRectangle(uX, uY, Radius * 2, Radius * 2, Color.Gold, true, true, 49.4f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
					this.DrawRectangle(uX, uY, (Radius - AddToRadius) * 2, (Radius - AddToRadius) * 2, uColor, true, true, 49.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
				}
				else
				{
					this.DrawRectangle(uX, uY, Radius * 2, Radius * 2, Color.Black, true, true, 0.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
					this.DrawRectangle(uX, uY, (Radius - AddToRadius) * 2, (Radius - AddToRadius) * 2, uColor, true, true, 1.9f + (uOwner == MainWindow.neutralplayer ? 0 : (16 - uOwner)));
				}
			}
		}

		private void DrawUnitsOnScreen()
		{
			SmartClicker instance = SmartClicker.GetInstance(GameData.SC2Handle);
			if (instance != null)
			{
				for (int i = 0; i < MainWindow.total_units; i++)
				{
					Point point = instance.Game2Screen((double)MainWindow.x_coords[i], (double)MainWindow.y_coords[i]);
					Point point2 = instance.Game2Screen((double)MainWindow.x_coordsDest[i], (double)MainWindow.y_coordsDest[i]);
					if ((((MainWindow.player_teams[MainWindow.p_owner[i]] != MainWindow.localteam) && (point.X > instance.Margins.left)) && ((point.X < instance.Margins.right) && (point.Y > instance.Margins.top))) && ((point.Y < instance.Margins.bot) && !this.IsUnitVisible(i)))
					{
						for (int j = 0; (j < this.unit_textures.Length) && this.drawUnitEnemiesScreen; j++)
						{
							if (((long)this.unit_textures[j].Value) == (long)MainWindow.units[i].unitType)
							{
								int num3 = 0x23;
								for (int k = 0; k < this.unit_textures_sizes.Length; k++)
								{
									if (((long)this.unit_textures_sizes[k].Key) == (long)MainWindow.units[i].unitType)
									{
										num3 = this.unit_textures_sizes[k].Value;
									}
								}
								this.DrawTexture((float)(point.X - (num3 / 2)), (float)(point.Y - (num3 / 2)), (float)num3, (float)num3, this.unit_textures[j].Key, true);
								break;
							}
						}
						if (((point2.X >= instance.Margins.left) || (point2.Y <= instance.Margins.bot)) && this.drawUnitEnemiesDestinationScreen)
						{
							this.DrawLine((float)point.X, (float)point.Y, (float)point2.X, (float)point2.Y, Color.Yellow, true);
						}
					}
				}
			}
		}

		[DllImport("dwmapi.dll")]
		private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(ref int enabled);
		/*private void GetAbilities()
		{
			this.unit_queue_counter = new int[0x10, 130];
			this.abilities = GameData.getAbilitiesData();
			for (int i = 0; i < this.abilities.Count; i++)
			{
				if (this.abilities[i].ClassName == Abil.AbilClass.CAbilBuildable)
				{
					AbilBuildable ability = (AbilBuildable)this.abilities[i].Ability;
					Unit unit = ability.Unit;
					if (MainWindow.unit_show[(int)unit.unitType])
					{
						this.unit_queue_counter[(int)((IntPtr)unit.playerNumber), (int)((IntPtr)MainWindow.unit_count_index[(int)unit.unitType])]++;
					}
				}
			}
		}*/

		private object getTallest(object[,] heights, int column, int row)
		{
			dynamic centerCopy = heights[column, row];
			List<object> source = new List<object>();
			source.Add(heights[column - 1, row + 1]);
			source.Add(heights[column, row + 1]);
			source.Add(heights[column + 1, row + 1]);
			source.Add(heights[column - 1, row]);
			source.Add(heights[column, row]);
			source.Add(heights[column + 1, row]);
			source.Add(heights[column - 1, row - 1]);
			source.Add(heights[column, row - 1]);
			source.Add(heights[column + 1, row - 1]);

			IEnumerable<object> enumerable = from height in source
											 orderby ((dynamic)height).h descending
											 select height;

			if (enumerable.First<dynamic>().h == enumerable.Last<dynamic>().h)
			{
				return centerCopy;
			}
			return source.First<object>();
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
		private void initDirectX()
		{
			PresentParameters parameters = new PresentParameters
			{
				EnableAutoDepthStencil = true,
				AutoDepthStencilFormat = DepthFormat.D16,
				Windowed = true,
				SwapEffect = SwapEffect.Discard,
				BackBufferFormat = Microsoft.DirectX.Direct3D.Format.A8R8G8B8
			};
			try
			{
				this.device = new Device(0, DeviceType.Hardware, base.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters[] { parameters });
			}
			catch(Exception ex)
			{
				Program.MessageOk("There was an error with Direct3D when creating the window. This can sometimes happen randomly, but if it always happens, there is a problem with your instalation of Direct3D, or your graphics card doesn't support something important.", MessageBoxIcon.Exclamation);
				Utilities.WebTools.WT.ReportCrash(ex, "oops", null, null);
				Process.GetCurrentProcess().Kill();
				return;
			}
			this.device.RenderState.AlphaBlendEnable = true;
			this.device.RenderState.SourceBlend = Blend.SourceAlpha;
			this.device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			this.textSprite = new Sprite(this.device);
			this.textureSprite = new Sprite(this.device);
		}

		private void initFrame()
		{
			this.frame = new HUDFrame(this);
			this.frame.Text = this._HUDType.ToString() + " HUD";
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectX_HUDs));
			this.tmrRefreshRate = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// tmrRefreshRate
			// 
			this.tmrRefreshRate.Interval = 500;
			this.tmrRefreshRate.Tick += new System.EventHandler(this.tmrRefreshRate_Tick);
			// 
			// DirectX_HUDs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(255, 1, 1, 1);
			this.ClientSize = new System.Drawing.Size(150, 150);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(150, 150);
			this.Name = "DirectX_HUDs";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "HUDs";
			this.TopMost = true;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DirectX_HUDs_FormClosed);
			this.Shown += new System.EventHandler(this.DirectX_HUDs_Shown);
			this.SizeChanged += new System.EventHandler(this.DirectX_HUDs_SizeChanged);
			this.VisibleChanged += new System.EventHandler(this.DirectX_HUDs_VisibleChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.DirectX_HUDs_Paint);
			this.ResumeLayout(false);

		}

		private void initVariables()
		{
			this.nextObserverPanelHotkey = new Utilities.KeyboardHook.KeyboardHook();
			this.previousObserverPanelHotkey = new Utilities.KeyboardHook.KeyboardHook();
		}

		private void initWindow()
		{
			SetWindowLong(base.Handle, -20, (IntPtr)(((GetWindowLong(base.Handle, -20) ^ 0x80000) ^ 0x20) | 8L));
			if (AeroEnabled)
			{
				SetLayeredWindowAttributes(base.Handle, 0x00010101, 0xff, LWA_ALPHA);
			}
			else
			{
				SetLayeredWindowAttributes(base.Handle, 0x00010101, 0xff, LWA_COLORKEY);
			}
		}

		private bool IsUnitVisible(int j)
		{
			for (int i = 0; i < MainWindow.total_units; i++)
			{
				if ((j != i) && (MainWindow.player_teams[MainWindow.p_owner[i]] == MainWindow.localteam))
				{
					int num2 = (int)Math.Pow((double)(MainWindow.x_coords[j] - MainWindow.x_coords[i]), 2.0);
					int num3 = (int)Math.Pow((double)(MainWindow.y_coords[j] - MainWindow.y_coords[i]), 2.0);
					int num4 = (int)Math.Sqrt((double)(num2 + num3));
					if (num4 < 15)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void LoadPositionAndSize()
		{	
			IniFile ini = new IniFile(MainWindow.settings_path);
			if (ini.Exists())
			{
				ini.Load();
			}
			else
				return;

			try
			{
				this.DesiredClientRect = this.DefaultClientRect;

				string SectionName = this._HUDType.ToString() + " HUD";

				if (ini.ContainsKey(SectionName))
				{
					if (ini[SectionName].ContainsKey("X"))
						this.DesiredClientRect.X = int.Parse(ini[SectionName]["X"]);
					if (ini[SectionName].ContainsKey("Y"))
						this.DesiredClientRect.Y = int.Parse(ini[SectionName]["Y"]);
					if (ini[SectionName].ContainsKey("SizeX"))
						this.DesiredClientRect.Width = int.Parse(ini[SectionName]["SizeX"]);
					if (ini[SectionName].ContainsKey("SizeY"))
						this.DesiredClientRect.Height = int.Parse(ini[SectionName]["SizeY"]);

					if (this._HUDType == HUDType.Observer)
					{
						CurrentObserverPanelTab = (ObserverPanelTabs)Enum.Parse(typeof(ObserverPanelTabs), ini[SectionName]["Mode"]);
						ObserverPanelDrawDirection = (ObserverPanelDirection)Enum.Parse(typeof(ObserverPanelDirection), ini[SectionName]["Direction"]);
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				Rectangle ScreenRect = Screen.GetBounds(new Point(0, 0));
				if (this.DesiredClientRect.X <= ScreenRect.Left)
					this.DesiredClientRect.X = ScreenRect.Left + 5;
				if (this.DesiredClientRect.Y <= ScreenRect.Top)
					this.DesiredClientRect.Y = ScreenRect.Top + 5;
				if (this.DesiredClientRect.Left >= ScreenRect.Right)
					this.DesiredClientRect.X = ScreenRect.Right - 5;
				if (this.DesiredClientRect.Top >= ScreenRect.Bottom)
					this.DesiredClientRect.Y = ScreenRect.Bottom - 5;	
			}
		}

		private void SavePositionAndSize()
		{
			this.DesiredClientRect = this.RectangleToScreen(this.ClientRectangle);	
			IniFile file = new IniFile(MainWindow.settings_path);
			if (file.Exists())
			{
				file.Load();
			}
			IniSection section = new IniSection();

			string SectionName = this.ContentHUDType.ToString() + " HUD";
			if (file.HasSection(SectionName))
			{
				section = file[SectionName];
				file.Remove(SectionName);
			}

			if (section.ContainsKey("X"))
				section["X"] = this.DesiredClientRect.X.ToString();
			else
				section.Add("X", this.DesiredClientRect.X.ToString());

			if (section.ContainsKey("Y"))
				section["Y"] = this.DesiredClientRect.Y.ToString();
			else
				section.Add("Y", this.DesiredClientRect.Y.ToString());

			if (section.ContainsKey("SizeX"))
				section["SizeX"] = this.DesiredClientRect.Width.ToString();
			else
				section.Add("SizeX", this.DesiredClientRect.Width.ToString());

			if (section.ContainsKey("SizeY"))
				section["SizeY"] = this.DesiredClientRect.Height.ToString();
			else
				section.Add("SizeY", this.DesiredClientRect.Height.ToString());

			if(this._HUDType == HUDType.Observer)
			{
				if (section.ContainsKey("Mode"))
					section["Mode"] = CurrentObserverPanelTab.ToString();
				else
					section.Add("Mode", CurrentObserverPanelTab.ToString());
				if (section.ContainsKey("Direction"))
					section["Direction"] = ObserverPanelDrawDirection.ToString();
				else
					section.Add("Direction", ObserverPanelDrawDirection.ToString());
			}

			file.Add(SectionName, section);
			file.Save();
		}

		private void loadMapSettings(IniFile ini)
		{
			try
			{
				MainWindow.minimap_location_x = int.Parse(ini["Map HUD"]["X"]);
				MainWindow.minimap_location_y = int.Parse(ini["Map HUD"]["Y"]);
				MainWindow.minimap_size_x = int.Parse(ini["Map HUD"]["SizeX"]);
				MainWindow.minimap_size_y = int.Parse(ini["Map HUD"]["SizeY"]);
			}
			catch (Exception)
			{
				Rectangle MinimapRect = GameData.GetMinimapCoords();
				MainWindow.minimap_location_x = MinimapRect.X;
				MainWindow.minimap_location_y = MinimapRect.Y;
				MainWindow.minimap_size_x = MinimapRect.Width;
				MainWindow.minimap_size_y = MinimapRect.Height;
			}
		}


		public void LoadOptionSettings()
		{
			IniFile file = new IniFile(MainWindow.settings_path);
			if (file.Exists())
			{
				file.Load();
				try
				{
					ObserverPanelDrawDirection = (ObserverPanelDirection)Enum.Parse(typeof(ObserverPanelDirection), file["OptionsDrawing"]["cbObserverPanelDrawDirection"]);
				}
				catch
				{
				}
				try
				{
					this.drawCameraSpectator = bool.Parse(file["OptionsDrawing"]["chkSpectator"]);
				}
				catch
				{
				}
				try
				{
					this.drawCameraEnemies = bool.Parse(file["OptionsDrawing"]["chkCameraEnemies"]);
				}
				catch
				{
				}
				try
				{
					this.drawCameraAllies = bool.Parse(file["OptionsDrawing"]["chkCameraAllies"]);
				}
				catch
				{
				}
				try
				{
					this.drawCameraSelf = bool.Parse(file["OptionsDrawing"]["chkCameraSelf"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitsEnemies = bool.Parse(file["OptionsDrawing"]["chkUnitsEnemies"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitsAllies = bool.Parse(file["OptionsDrawing"]["chkUnitsAllies"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitsSelf = bool.Parse(file["OptionsDrawing"]["chkUnitsSelf"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitDestinationEnemies = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationEnemies"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitDestinationAllies = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationAllies"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitDestinationSelf = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationSelf"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitEnemiesScreen = bool.Parse(file["OptionsDrawing"]["chkUnitEnemiesScreen"]);
				}
				catch
				{
				}
				try
				{
					this.drawUnitEnemiesDestinationScreen = bool.Parse(file["OptionsDrawing"]["chkUnitDestinationEnemiesScreen"]);
				}
				catch
				{
				}
				try
				{
					if (this._HUDType == HUDType.Observer)
					{
						Keys key = (Keys)Enum.Parse(typeof(Keys), file["OptionsHotkeys"]["cbPreviousPanelHotkey"]);
						if (this.previousObserverPanelHotkey.HotKey != key)
						{
							this.previousObserverPanelHotkey.RegisterHotKey(key);
							this.previousObserverPanelHotkey.KeyPressed += new EventHandler<KeyPressedEventArgs>(this.previousObserverPanelHotkey_KeyPressed);
						}
					}
				}
				catch
				{
				}
				try
				{
					if (this._HUDType == HUDType.Observer)
					{
						Keys keys2 = (Keys)Enum.Parse(typeof(Keys), file["OptionsHotkeys"]["cbNextPanelHotkey"]);
						if (this.nextObserverPanelHotkey.HotKey != keys2)
						{
							this.nextObserverPanelHotkey.RegisterHotKey(keys2);
							this.nextObserverPanelHotkey.KeyPressed += new EventHandler<KeyPressedEventArgs>(this.nextObserverPanelHotkey_KeyPressed);
						}
					}
				}
				catch
				{
				}
			}
		}

		private void LoadSettings()
		{
			this.LoadOptionSettings();
			IniFile ini = new IniFile(MainWindow.settings_path);
			if (ini.Exists())
			{
				ini.Load();
				this.LoadPositionAndSize();
				Rectangle WindowRect = this.frame.DesktopBounds;
				Rectangle ClientRect = this.frame.RectangleToScreen(this.frame.ClientRectangle);

				this.frame.Location = new Point(this.DesiredClientRect.X - (ClientRect.X - WindowRect.X), this.DesiredClientRect.Y - (ClientRect.Y - WindowRect.Y));
				this.frame.ClientSize = this.DesiredClientRect.Size;
				
				if (this._HUDType == HUDType.Map)
				{
					//this.loadMapSettings(ini); // This shouldn't be needed anymore, now that we have LoadPositionAndSize().
				}
			}
		}

		private void nextObserverPanelHotkey_KeyPressed(object sender, KeyPressedEventArgs e)
		{
			ComboBox box = (ComboBox)this.frame.Controls["cbObserverPanel"];
			if ((box.SelectedIndex - 1) > -1)
			{
				box.SelectedIndex--;
			}
		}

		private void previousObserverPanelHotkey_KeyPressed(object sender, KeyPressedEventArgs e)
		{
			ComboBox box = (ComboBox)this.frame.Controls["cbObserverPanel"];
			if ((box.SelectedIndex + 1) < box.Items.Count)
			{
				box.SelectedIndex++;
			}
		}

		public string removeBlanksFromByteString(byte[] byteString)
		{
			int count = 0;
			byte[] buffer = byteString;
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == 0)
				{
					return Encoding.ASCII.GetString(byteString, 0, count);
				}
				count++;
			}
			return "FAIL! 00248";
		}

		public static void SaveMapSettings(int MapX, int MapY, int MapWidth, int MapHeight)
		{
			IniFile file = new IniFile(MainWindow.settings_path);
			if (file.Exists())
			{
				file.Load();
			}
			IniSection section = new IniSection();
			if (file.HasSection("Map HUD"))
			{
				file.Remove("Map HUD");
			}
			section.Add("X", MapX.ToString());
			section.Add("Y", MapY.ToString());
			section.Add("SizeX", MapWidth.ToString());
			section.Add("SizeY", MapHeight.ToString());
			file.Add("Map HUD", section);
			file.Save();
		}

		[DllImport("user32.dll")]
		private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		private void tmrRefreshRate_Tick(object sender, EventArgs e)
		{
			if (this._HUDType != HUDType.Info && (!_2csAPI.InGame() || _2csAPI.Player.LocalPlayer.victoryStatus != VictoryStatus.Playing))
			{
				base.Hide();
			}
			else if (!base.Visible)
			{
				base.Show();
				this.frame.Show();
			}
			if (!this.pause)
			{
				this.device.Clear(ClearFlags.Target, Color.FromArgb(0, 1, 1, 1), 0f, 0);
				this.device.BeginScene();
				this.DrawStuff();
				this.device.EndScene();
				try
				{
					this.device.Present();
				}
				catch
				{
				}
			}
		}

		private void updateFrameSize()
		{
			switch (this._HUDType)
			{
				case HUDType.Map:
					this.frame.updateSize();
					break;

				case HUDType.Observer:
					break;

				default:
					return;
			}
		}

		public static bool AeroEnabled
		{
			get
			{
				try
				{
					if (System.Environment.OSVersion.Version.Major < 6)
					{
						return false;
					}
					int enabled = 0;
					DwmIsCompositionEnabled(ref enabled);
					if (enabled == 0)
					{
						return false;
					}
				}
				catch
				{
					return false;
				}
				return true;
			}
		}

		public HUDType ContentHUDType
		{
			get
			{
				return this._HUDType;
			}
		}

		public enum HUDType : byte
		{
			CellFlags = 3,
			CliffLevel = 4,
			Map = 0,
			Observer = 1,
			PerimeterWithRamp = 6,
			Ramps = 5,
			Resources = 2,
			Info = 11
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct Margins
		{
			public int Left;
			public int Right;
			public int Top;
			public int Bottom;
		}

		public enum ObserverPanelDirection : byte
		{
			Down = 0,
			Left = 1
		}

		public enum ObserverPanelTabs
		{
			Buildings_and_Units_Same_Line,
			Buildings_and_Units_Different_Lines,
			Buildings,
			Units
		}

		private enum UnitDrawStyle
		{
			ObserverPanel
		}

		public delegate void updateFrameSizeDelegate();

	}
}

