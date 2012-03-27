namespace maphack_external_directx
{
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

	public class MainWindow : Form
	{
		public static string settings_folder = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf('\\'));
		public static string settings_path = settings_folder + "\\settings.ini";

		private static DirectX_HUDs[] _HUDs;
		public static int active_players = 0;
		public static byte[] actual_player = new byte[0x10];
		public static byte actual_players = 0;

		public static List<Unit> units = new List<Unit>();
		public static List<Player> players = new List<Player>();
		private ToolStripButton btnOptions;
		private IContainer components;
		private ContextMenuStrip contextMenuStrip;
		private DataGridView dataGridViewPlayerData;
		public static int dgvHeight = 0x16;
		public static bool draw = false;
		public static bool got_local = false;
		public static int HUDRefreshRate = 500;
		public static bool ingame = false;
		public static uint localplayer = 0;
		public static int localteam = -1;
		public static DirectX_HUDs MapHUD;
		private string message = "SC2 is not opened";

		public static int minimap_location_x;
		public static int minimap_location_y;
		public static int minimap_size_x;
		public static int minimap_size_y;

		public static float minimap_scale;
		public static float minimap_offset_x;
		public static float minimap_offset_y;

		public static float map_height;
		public static float map_width;
		public static float playable_map_height;
		public static float playable_map_width;
		public static float playable_map_left;
		public static float playable_map_right;
		public static float playable_map_top;
		public static float playable_map_bottom;
		public static int neutralplayer = -1;
		public static int no_showing = 0;
		public static DirectX_HUDs ObserverHUD;
		public static uint[] p_owner = new uint[0x4000];
		public static bool pending = false;
		public static byte[] player_active = new byte[0x10];
		public static uint[] player_cameraX = new uint[0x10];
		public static uint[] player_cameraY = new uint[0x10];

		public static uint[] player_minerals = new uint[0x10];
		public static uint[] player_vespene = new uint[0x10];
		public static uint[] player_terrazine = new uint[0x10];
		public static uint[] player_custom_resource = new uint[0x10];

		public static string[] player_name = new string[0x10];
		public static bool[] player_playing = new bool[0x10];
		public static Data.Race[] player_race = new Data.Race[0x10];
		public static uint[] player_slots = new uint[0x10];
		public static VictoryStatus[] player_status = new VictoryStatus[0x10];
		public static string[] player_supply = new string[0x10];
		public static uint[] player_teams = new uint[0x10];
		public static PlayerType[] player_types = new PlayerType[0x10];
		public static string[] rank_textures = new string[0x10];
		public static string[] rank_tooltips = new string[0x10];
		private Thread readMemory;
		private ToolStripMenuItem resetToolStripMenuItem;
		public static DirectX_HUDs ResourcesHUD;
		public static bool[] show = new bool[0x10];
		public static bool[] show_window = new bool[0x10];
		private ToolStripMenuItem stopToolStripMenuItem;
		private System.Windows.Forms.Timer tmrMain;
		private ToolStrip toolStrip;
		public ToolStripButton toolStripButtonMap;
		public ToolStripButton toolStripButtonObserver;
		public ToolStripButton toolStripButtonResources;
		private ToolStripLabel toolStripLabelStatus;
		public static int total_units;

		public static Dictionary<string, int>[] unit_counts = new Dictionary<string, int>[16];
		public static Dictionary<string, string> unit_pictures = new Dictionary<string, string>();
		public static Dictionary<string, string> unit_names = new Dictionary<string, string>();

		public static int[] unit_count_index = new int[] { 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 
			-1, -1, 3, 4, 5, -1, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 
			0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16, 0x17, -1, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 30, 
			0x1f, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 40, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 
			0x2f, 0x30, 0x31, 50, 0x33, 0x34, -1, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60, 0x3d, 
			0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 70, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 
			0x4e, -1, 0x4f, 80, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 90, 0x5b, 0x5c, 
			0x5d, 0x5e, 0x5f, -1, 0x60, 0x61, 0x62, 0x63, 100, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 
			0x6c, 0x6d, 110, 0x6f, -1, -1, -1, -1, -1, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 
			0x77, 120, 0x79, -1, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f, -1, -1, -1, -1, -1, -1, 
			-1, 0x80, -1, -1, -1, -1, -1, -1, -1, 0x81, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
		 };
		public static int[,] unit_counter = new int[0x10, 130];
		public static bool[] unit_show = new bool[] { 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, 
			false, false, true, true, true, false, true, true, true, true, true, true, true, true, true, true, 
			true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, 
			true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, 
			true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, 
			true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, 
			true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, 
			true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, 
			true, true, true, true, false, false, false, false, false, true, true, true, true, true, true, true, 
			true, true, true, false, true, false, true, true, true, true, false, true, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, true, true, false, true, false, false, false, true, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 
			false, false, false, false, false, false, false, false, false, false, false
		 };
		public static int UpdateInfoDelay = 500;
		public static float[] x_coords = new float[0x4000];
		public static float[] x_coordsDest = new float[0x4000];
		public static float[] y_coords = new float[0x4000];
		private DataGridViewTextBoxColumn PlayerNumber;
		private DataGridViewImageColumn p_color;
		private DataGridViewImageColumn rank;
		private DataGridViewTextBoxColumn TeamNumber;
		private DataGridViewTextBoxColumn PlayerName;
		private DataGridViewTextBoxColumn PlayerAccountNumber;
		private DataGridViewTextBoxColumn Status;
		private DataGridViewTextBoxColumn Race;
		private DataGridViewCheckBoxColumn Toggle;
		private ToolTip toolTip1;
		public static float[] y_coordsDest = new float[0x4000];

		public MainWindow()
		{
			this.InitializeComponent();

			if (!Directory.Exists(settings_folder))
				Directory.CreateDirectory(settings_folder);

			if (!File.Exists(settings_path))
				File.Create(settings_path);

			draw = true;
			this.initThreads();
			this.updateTimers();
			if (/*!Debugger.IsAttached*/false)
			{
				new CheckingUpdates().Show();
			}
		}

		private void btnOptions_Click(object sender, EventArgs e)
		{
			new Options().ShowDialog();
			foreach (DirectX_HUDs ds in HUDs)
			{
				if (ds != null)
				{
					ds.LoadOptionSettings();
				}
			}
			this.updateTimers();
		}

		public void ClearRows()
		{
			this.dataGridViewPlayerData.Rows.Clear();
			this.dataGridViewPlayerData.Height = dgvHeight;
			base.ClientSize = new Size(this.dataGridViewPlayerData.Width, this.dataGridViewPlayerData.Height + this.toolStrip.Height);
		}

		public void closeHUD(ref DirectX_HUDs hud)
		{
			if (hud != null)
			{
				toggleHUD(ref hud, hud.ContentHUDType, this.toolStripButtonForHUD(hud.ContentHUDType));
			}
		}

		private void dataGridViewPlayerData_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if ((e.ColumnIndex == this.dataGridViewPlayerData.Columns["Toggle"].Index) && (e.RowIndex != -1))
			{
				uint index = uint.Parse(this.dataGridViewPlayerData.Rows[e.RowIndex].Cells[0].Value.ToString());
				this.togglePlayerInfo(index, e.RowIndex, e.ColumnIndex);
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

		public void GameClosed()
		{
			pending = false;
			active_players = 0;
			actual_player = new byte[0x10];
			player_playing = new bool[0x10];
			actual_players = 0;
			got_local = false;
			localplayer = 0;
			localteam = -1;
			neutralplayer = -1;
			active_players = 0;
			no_showing = 0;
			for (int i = 0; i < 0x10; i++)
			{
				show_window[i] = false;
				show[i] = false;
				player_types[i] = PlayerType.None;
				player_slots[i] = 0;
				player_name[i] = "";
				player_active[i] = 0;
				player_status[i] = VictoryStatus.Playing;
				player_race[i] = Data.Race.Neutral;
				actual_player[i] = 0;
			}
			ingame = false;
		}

		public void GameStart()
		{
			GameData.mapDat = new MapData(GameData.getMapData().mapInfo.filePath);
			unit_pictures = new Dictionary<string, string>();
			unit_names = new Dictionary<string, string>();
			active_players = 0;
			actual_players = 0;
			this.UpdateMapSize();
			this.GetPlayers(true);
			Database.Reset();
		}

		private void GetLocalPlayer()
		{
			if (active_players == 0)
			{
				got_local = false;
			}
			else
			{
				localplayer = _2csAPI.Player.LocalPlayerNumber;
				localteam = (localplayer == 0x10) ? 0 : ((int) player_teams[localplayer]);
				got_local = true;
				for (int i = 0; i < active_players; i++)
				{
					int index = player_active[i];
					if (player_teams[index] != localteam)
					{
						show[index] = true;
					}
				}
			}
		}

		private void GetPlayers(bool atGameStart = false)
		{
			actual_player = new byte[0x10];

			List<Data.Player> list = GameData.getPlayersData();
			int num = 0;
			for (byte i = 0; i < list.Count; i = (byte) (i + 1))
			{
				Data.Player player = list[i];
				if ((player.playerType != PlayerType.Neutral) && (player.playerType != PlayerType.Hostile))
				{
					if (atGameStart)
					{

						actual_player[actual_players++] = i;
						player_teams[i] = (uint) player.team;
						player_types[i] = player.playerType;
						player_name[i] = player.name;
						player_slots[i] = (uint) player.slotNumber;
						player_race[i] = player.race;
						if (player.race == Data.Race.Neutral)
						{
							neutralplayer = i;
						}
					}
					player_playing[i] = player.victoryStatus == VictoryStatus.Playing;
					player_active[num++] = i;
					player_cameraX[i] = (uint) player.cameraX;
					player_cameraY[i] = (uint) player.cameraY;
					player_status[i] = player.victoryStatus;
					player_minerals[i] = (uint) player.minerals;
					player_vespene[i] = (uint) player.gas;
					player_terrazine[i] = (uint) player.terrazine;
					player_custom_resource[i] = (uint) player.custom;
					player_supply[i] = player.supply + "/" + (player.supplyLimit < player.supplyCap? player.supplyLimit : player.supplyCap);
				}
			}
			active_players = num;
			lock (players)
			{
				players = list;
			}
		}

		private void GetUnits()
		{
			List<Unit> list = GameData.getUnitData();

			int[,] numArray = new int[0x10, 130];

			List<Data.Player> source = GameData.getPlayersData();
			if ((list.Count != 0) && (source.Count != 0))
			{
				Dictionary<string, int>[] newUnitCounts = new Dictionary<string, int>[16];
				for (int i = 0; i < newUnitCounts.Length; i++)
				{
					newUnitCounts[i] = new Dictionary<string, int>();
				}

				total_units = list.Count;
				foreach(Unit unit in list)
				{
					if ((unit.targetFilterFlags & (TargetFilter.Missile | TargetFilter.Dead)) != 0 || unit.textID.StartsWith("Beacon"))
						continue;
					
					lock (unit_pictures)
					{
						if (!unit_pictures.ContainsKey(unit.textID))
						{
							unit_pictures.Add(unit.textID, GameData.mapDat.GetUnitPictureFilename(unit.textID));
						}
					}

					lock (unit_names)
					{
						if (!unit_names.ContainsKey(unit.textID))
						{
							unit_names.Add(unit.textID, unit.name);
						}
					}

					if (newUnitCounts[unit.playerNumber].ContainsKey(unit.textID))
					{
						newUnitCounts[unit.playerNumber][unit.textID]++;
					}
					else
					{
						newUnitCounts[unit.playerNumber].Add(unit.textID, 1);
					}

					if ((int)unit.unitType < unit_count_index.Length && unit_count_index[(int)unit.unitType] != -1)
                    {
						try
						{
							numArray[(int) unit.playerNumber,  unit_count_index[(int) unit.unitType]]++;
						}
						catch
						{
						}
					}
				}
				lock(unit_counter)
				{
					unit_counter = numArray;
				}
				lock (unit_counts)
				{
					unit_counts = newUnitCounts;
				}
				lock (units)
				{
					units = list;
				}
			}
		}

		private void HUDOff(uint p_no)
		{
			if (show_window[p_no])
			{
				show_window[p_no] = false;
				no_showing--;
			}
		}

		private void HUDOn(uint p_no)
		{
			if (!show_window[p_no])
			{
				show_window[p_no] = true;
				no_showing++;
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.dataGridViewPlayerData = new System.Windows.Forms.DataGridView();
			this.PlayerNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.p_color = new System.Windows.Forms.DataGridViewImageColumn();
			this.rank = new System.Windows.Forms.DataGridViewImageColumn();
			this.TeamNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PlayerAccountNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Race = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Toggle = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripButtonResources = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonObserver = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonMap = new System.Windows.Forms.ToolStripButton();
			this.btnOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripLabelStatus = new System.Windows.Forms.ToolStripLabel();
			this.tmrMain = new System.Windows.Forms.Timer(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewPlayerData)).BeginInit();
			this.toolStrip.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridViewPlayerData
			// 
			this.dataGridViewPlayerData.AllowUserToAddRows = false;
			this.dataGridViewPlayerData.AllowUserToDeleteRows = false;
			this.dataGridViewPlayerData.AllowUserToOrderColumns = true;
			this.dataGridViewPlayerData.AllowUserToResizeColumns = false;
			this.dataGridViewPlayerData.AllowUserToResizeRows = false;
			this.dataGridViewPlayerData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewPlayerData.BackgroundColor = System.Drawing.Color.Black;
			this.dataGridViewPlayerData.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.GreenYellow;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Gray;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.DodgerBlue;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewPlayerData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridViewPlayerData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGridViewPlayerData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PlayerNumber,
            this.p_color,
            this.rank,
            this.TeamNumber,
            this.PlayerName,
            this.PlayerAccountNumber,
            this.Status,
            this.Race,
            this.Toggle});
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.Color.GreenYellow;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Black;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.DodgerBlue;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewPlayerData.DefaultCellStyle = dataGridViewCellStyle5;
			this.dataGridViewPlayerData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dataGridViewPlayerData.GridColor = System.Drawing.Color.Black;
			this.dataGridViewPlayerData.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewPlayerData.Name = "dataGridViewPlayerData";
			this.dataGridViewPlayerData.ReadOnly = true;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewPlayerData.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
			this.dataGridViewPlayerData.RowHeadersWidth = 24;
			this.dataGridViewPlayerData.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridViewPlayerData.Size = new System.Drawing.Size(535, 24);
			this.dataGridViewPlayerData.TabIndex = 2;
			this.dataGridViewPlayerData.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPlayerData_CellContentClick);
			// 
			// PlayerNumber
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.PlayerNumber.DefaultCellStyle = dataGridViewCellStyle2;
			this.PlayerNumber.FillWeight = 30.83903F;
			this.PlayerNumber.HeaderText = "#";
			this.PlayerNumber.MinimumWidth = 35;
			this.PlayerNumber.Name = "PlayerNumber";
			this.PlayerNumber.ReadOnly = true;
			// 
			// p_color
			// 
			this.p_color.FillWeight = 40.68842F;
			this.p_color.HeaderText = "C";
			this.p_color.Name = "p_color";
			this.p_color.ReadOnly = true;
			// 
			// rank
			// 
			this.rank.FillWeight = 53.61267F;
			this.rank.HeaderText = "R";
			this.rank.MinimumWidth = 25;
			this.rank.Name = "rank";
			this.rank.ReadOnly = true;
			// 
			// TeamNumber
			// 
			this.TeamNumber.HeaderText = "Team #";
			this.TeamNumber.Name = "TeamNumber";
			this.TeamNumber.ReadOnly = true;
			// 
			// PlayerName
			// 
			this.PlayerName.FillWeight = 133.6321F;
			this.PlayerName.HeaderText = "Name";
			this.PlayerName.Name = "PlayerName";
			this.PlayerName.ReadOnly = true;
			// 
			// PlayerAccountNumber
			// 
			this.PlayerAccountNumber.FillWeight = 155.6321F;
			this.PlayerAccountNumber.HeaderText = "Account Number";
			this.PlayerAccountNumber.Name = "PlayerAccountNumber";
			this.PlayerAccountNumber.ReadOnly = true;
			// 
			// Status
			// 
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.Status.DefaultCellStyle = dataGridViewCellStyle3;
			this.Status.FillWeight = 61.06282F;
			this.Status.HeaderText = "Status";
			this.Status.MinimumWidth = 55;
			this.Status.Name = "Status";
			this.Status.ReadOnly = true;
			// 
			// Race
			// 
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.Race.DefaultCellStyle = dataGridViewCellStyle4;
			this.Race.FillWeight = 56.07122F;
			this.Race.HeaderText = "Race";
			this.Race.MinimumWidth = 51;
			this.Race.Name = "Race";
			this.Race.ReadOnly = true;
			// 
			// Toggle
			// 
			this.Toggle.FillWeight = 58.0429F;
			this.Toggle.HeaderText = "Display";
			this.Toggle.MinimumWidth = 53;
			this.Toggle.Name = "Toggle";
			this.Toggle.ReadOnly = true;
			this.Toggle.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.Toggle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// toolStrip
			// 
			this.toolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.toolStrip.ContextMenuStrip = this.contextMenuStrip;
			this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonResources,
            this.toolStripButtonObserver,
            this.toolStripButtonMap,
            this.btnOptions,
            this.toolStripLabelStatus});
			this.toolStrip.Location = new System.Drawing.Point(0, 23);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.toolStrip.Size = new System.Drawing.Size(535, 25);
			this.toolStrip.TabIndex = 3;
			this.toolStrip.Text = "toolStrip";
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.stopToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.Size = new System.Drawing.Size(153, 70);
			// 
			// resetToolStripMenuItem
			// 
			this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
			this.resetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.resetToolStripMenuItem.Text = "Reset Settings";
			this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.stopToolStripMenuItem.Text = "Stop";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
			// 
			// toolStripButtonResources
			// 
			this.toolStripButtonResources.BackColor = System.Drawing.Color.Gray;
			this.toolStripButtonResources.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonResources.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonResources.Name = "toolStripButtonResources";
			this.toolStripButtonResources.Size = new System.Drawing.Size(84, 22);
			this.toolStripButtonResources.Text = "Resources Off";
			this.toolStripButtonResources.Click += new System.EventHandler(this.toolStripButtonResources_Click);
			// 
			// toolStripButtonObserver
			// 
			this.toolStripButtonObserver.BackColor = System.Drawing.Color.Gray;
			this.toolStripButtonObserver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonObserver.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonObserver.Name = "toolStripButtonObserver";
			this.toolStripButtonObserver.Size = new System.Drawing.Size(78, 22);
			this.toolStripButtonObserver.Text = "Observer Off";
			this.toolStripButtonObserver.Click += new System.EventHandler(this.toolStripButtonObserver_Click);
			// 
			// toolStripButtonMap
			// 
			this.toolStripButtonMap.BackColor = System.Drawing.Color.Gray;
			this.toolStripButtonMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonMap.Name = "toolStripButtonMap";
			this.toolStripButtonMap.Size = new System.Drawing.Size(55, 22);
			this.toolStripButtonMap.Text = "Map Off";
			this.toolStripButtonMap.Click += new System.EventHandler(this.toolStripButtonMap_Click);
			// 
			// btnOptions
			// 
			this.btnOptions.BackColor = System.Drawing.Color.LightGray;
			this.btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnOptions.Name = "btnOptions";
			this.btnOptions.Size = new System.Drawing.Size(53, 22);
			this.btnOptions.Text = "Options";
			this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
			// 
			// toolStripLabelStatus
			// 
			this.toolStripLabelStatus.BackColor = System.Drawing.Color.DarkRed;
			this.toolStripLabelStatus.ForeColor = System.Drawing.Color.DodgerBlue;
			this.toolStripLabelStatus.Name = "toolStripLabelStatus";
			this.toolStripLabelStatus.Size = new System.Drawing.Size(0, 22);
			// 
			// tmrMain
			// 
			this.tmrMain.Enabled = true;
			this.tmrMain.Interval = 1000;
			this.tmrMain.Tick += new System.EventHandler(this.tmrMain_Tick);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(535, 48);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.dataGridViewPlayerData);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainWindow";
			this.Text = "SCIIEMH v0.12";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewPlayerData)).EndInit();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void initThreads()
		{
			this.readMemory = new Thread(new ThreadStart(this.ReadMemory));
			this.readMemory.IsBackground = true;
			this.readMemory.Start();
		}

		private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}

		private void PopulatePlayerRows()
		{
			using (List<Data.Player>.Enumerator enumerator = (from x in _2csAPI.Player.ActualPlayers
				orderby x.team
				select x).ToList<Data.Player>().GetEnumerator())
			{
				ThreadStart start = null;
				Data.Player player;
				while (enumerator.MoveNext())
				{
					player = enumerator.Current;
					if (start == null)
					{
						start = delegate {
							this.updateRankTexture(player);
						};
					}
					new Thread(start).Start();
					uint number = player.number;
					DataGridViewRow dataGridViewRow = new DataGridViewRow();
					DataGridViewTextBoxCell dataGridViewCell = new DataGridViewTextBoxCell();
					DataGridViewImageCell cell2 = new DataGridViewImageCell();
					DataGridViewImageCell cell3 = new DataGridViewImageCell();
					DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
					DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
					DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
					DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
					DataGridViewCheckBoxCell cell8 = new DataGridViewCheckBoxCell();

					DataGridViewTextBoxCell cellAN = new DataGridViewTextBoxCell();

					dataGridViewCell.Value = number;
					dataGridViewRow.Cells.Add(dataGridViewCell);
					Bitmap image = new Bitmap(10, 10);
					Graphics graphics = Graphics.FromImage(image);
					Brush brush = new SolidBrush(player.drawingColor);
					graphics.FillRectangle(brush, 0, 0, 10, 10);
					cell2.Value = image;
					dataGridViewRow.Cells.Add(cell2);
					graphics.Dispose();
					brush.Dispose();
					cell3.Value = Image.FromFile(@"Leagues\none.png");
					dataGridViewRow.Cells.Add(cell3);
					cell4.Value = "Team " + (player.team + 1);
					dataGridViewRow.Cells.Add(cell4);
					cell5.Value = player.name;
					dataGridViewRow.Cells.Add(cell5);

					cellAN.Value = player.accountNumber;
					dataGridViewRow.Cells.Add(cellAN);

					cell6.Value = player.victoryStatus;
					dataGridViewRow.Cells.Add(cell6);
					cell7.Value = player.race;
					dataGridViewRow.Cells.Add(cell7);
					if (player.team != _2csAPI.Player.LocalPlayerTeam)
					{
						cell8.Value = true;
						this.HUDOn(number);
					}
					else
					{
						cell8.Value = false;
					}
					dataGridViewRow.Cells.Add(cell8);
					this.dataGridViewPlayerData.Rows.Add(dataGridViewRow);
				}
			}
		}

		private void ReadMemory()
		{
		Label_0000:
			while (Debugger.IsAttached)
			{
				this.UpdateStuff();
				Thread.Sleep(UpdateInfoDelay);
			}
			try
			{
				this.UpdateStuff();
				Thread.Sleep(UpdateInfoDelay);
				goto Label_0000;
			}
			catch (ThreadAbortException)
			{
				goto Label_0000;
			}
			catch (Exception exception)
			{
				IntPtr? handleToScreenShot = null;
				Size? screenShotSize = null;
				WT.ReportCrash(exception, Program.ApplicationTitle + " " + Program.ApplicationVersion, handleToScreenShot, screenShotSize, true);
				goto Label_0000;
			}
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (DirectX_HUDs ds in HUDs)
			{
				if (ds != null)
				{
					ds.pause = true;
				}
			}
			this.GameClosed();
			if (this.dataGridViewPlayerData.Height != dgvHeight)
			{
				this.dataGridViewPlayerData.Rows.Clear();
				this.dataGridViewPlayerData.Height = dgvHeight;
				base.ClientSize = new Size(this.dataGridViewPlayerData.Width, this.dataGridViewPlayerData.Height + this.toolStrip.Height);
			}
			foreach (DirectX_HUDs ds2 in HUDs)
			{
				if (ds2 != null)
				{
					ds2.pause = false;
					ds2.Visible = true;
				}
			}
			rank_textures = new string[0x10];
			rank_tooltips = new string[0x10];
			if (File.Exists(settings_path))
			{
				File.Delete(settings_path);
			}
		}

		public void SetStatus(string status)
		{
			this.message = status;
		}

		private void stopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (DirectX_HUDs ds in HUDs)
			{
				if (ds != null)
				{
					ds.pause = true;
					ds.Visible = false;
				}
			}
			this.GameClosed();
			this.message = "Stopped";
		}

		private void tmrMain_Tick(object sender, EventArgs e)
		{
			if (!GameData.SC2Opened)
			{
				this.message = "SC2 is not opened";
			}
			else
			{
				this.message = _2csAPI.InGame() ? "In Game" : ("SC2 Version " + GameData.SC2Version);
			}
			this.toolStripLabelStatus.Text = this.message;
			if (_2csAPI.InGame() && draw)
			{
				if (this.dataGridViewPlayerData.RowCount == 0)
				{
					this.PopulatePlayerRows();
					this.dataGridViewPlayerData.Height = (this.dataGridViewPlayerData.Rows.Count + 1) * dgvHeight;
					base.ClientSize = new Size(this.dataGridViewPlayerData.Width, this.dataGridViewPlayerData.Height + this.toolStrip.Height);
				}
				else if (base.ClientSize.Height != (this.dataGridViewPlayerData.Height + this.toolStrip.Height))
				{
					base.ClientSize = new Size(this.dataGridViewPlayerData.Width, this.dataGridViewPlayerData.Height + this.toolStrip.Height);
				}
				else
				{
					List<Data.Player> actualPlayers = _2csAPI.Player.ActualPlayers;
					actualPlayers.Sort(delegate (Player a1, Player a2)
						{
							if (a1.team != a2.team)
								return a1.team - a2.team;
							return (int)a1.number - (int)a2.number;
						});


					for (int i = 0; i < actualPlayers.Count; i++)
					{
						Data.Player player = actualPlayers[i];
						uint number = player.number;
						Image NewRankTexture = null;
						if (rank_textures[number] != null)
						{
							NewRankTexture = Image.FromFile(rank_textures[number]);
							NewRankTexture.Tag = rank_textures[number];
						}
						else
						{
							NewRankTexture = Image.FromFile(@"Leagues\none.png");
							NewRankTexture.Tag = @"Leagues\none.png";
						}

						if (((Bitmap)this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["rank"].Index].Value).Tag != NewRankTexture.Tag)
							this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["rank"].Index].Value = NewRankTexture;

						this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["rank"].Index].ToolTipText = rank_tooltips[number];

						this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["PlayerAccountNumber"].Index].Value = player.accountNumber;
						this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["Status"].Index].Value = player.victoryStatus;
						this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["TeamNumber"].Index].Value = "Team " + (player.team + 1);
						Color oldColor = ((Bitmap)this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["p_color"].Index].Value).GetPixel(0, 0);
						Color newColor = player.drawingColor;

						if (((Bitmap)this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["p_color"].Index].Value).GetPixel(0, 0) != player.drawingColor)
						{
							Bitmap image = new Bitmap(10, 10);
							Graphics graphics = Graphics.FromImage(image);
							Brush brush = new SolidBrush(player.drawingColor);
							graphics.FillRectangle(brush, 0, 0, 10, 10);
							
							this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["p_color"].Index].Value = image;
						}
						this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["PlayerName"].Index].Value = player.name;
						this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["PlayerNumber"].Index].Value = player.number;
					}
				}
			}
			else if (this.dataGridViewPlayerData.Height != dgvHeight)
			{
				this.dataGridViewPlayerData.Rows.Clear();
				this.dataGridViewPlayerData.Height = dgvHeight;
				base.ClientSize = new Size(this.dataGridViewPlayerData.Width, this.dataGridViewPlayerData.Height + this.toolStrip.Height);
			}
		}

		private static void toggleHUD(ref DirectX_HUDs hud, DirectX_HUDs.HUDType hudType, ToolStripButton button)
		{
			if (hud == null)
			{
				button.Text = hudType.ToString() + " Off";
				button.BackColor = Color.FromArgb(0x80, 0xff, 0x80);
				hud = new DirectX_HUDs(hudType);
				hud.Show();
			}
			else
			{
				button.Text = hudType.ToString() + " On";
				button.BackColor = Color.Gray;
				hud.Close();
				hud = null;
			}
		}

		private void togglePlayerInfo(uint index, int rowIndex, int columnIndex)
		{
			bool flag = show_window[index];
			this.dataGridViewPlayerData.Rows[rowIndex].Cells[columnIndex].Value = !flag;
			if (flag)
			{
				this.HUDOff(index);
			}
			else
			{
				this.HUDOn(index);
			}
			if ((ResourcesHUD != null) && (ResourcesHUD.frame != null))
			{
				ResourcesHUD.frame.updateSize();
			}
		}

		public ToolStripButton toolStripButtonForHUD(DirectX_HUDs.HUDType hudType)
		{
			switch (hudType)
			{
				case DirectX_HUDs.HUDType.Map:
					return this.toolStripButtonMap;

				case DirectX_HUDs.HUDType.Observer:
					return this.toolStripButtonObserver;

				case DirectX_HUDs.HUDType.Resources:
					return this.toolStripButtonResources;
			}
			return null;
		}

		public void toolStripButtonMap_Click(object sender, EventArgs e)
		{
			toggleHUD(ref MapHUD, DirectX_HUDs.HUDType.Map, this.toolStripButtonMap);
		}

		public void toolStripButtonObserver_Click(object sender, EventArgs e)
		{
			toggleHUD(ref ObserverHUD, DirectX_HUDs.HUDType.Observer, this.toolStripButtonObserver);
		}

		public void toolStripButtonResources_Click(object sender, EventArgs e)
		{
			toggleHUD(ref ResourcesHUD, DirectX_HUDs.HUDType.Resources, this.toolStripButtonResources);
		}

		private void UpdateMapSize()
		{
			int left1 = GameData.MapEdgeLeft;
			int right1 = GameData.MapEdgeRight;
			int top1 = GameData.MapEdgeTop;
			int bottom1 = GameData.MapEdgeBottom;
			int left2 = GameData.MapEdgeLeft2;
			int right2 = GameData.MapEdgeRight2;
			int top2 = GameData.MapEdgeTop2;
			int bottom2 = GameData.MapEdgeBottom2;

			if (left1 <= left2 + 3 && left1 >= left2 - 3)
				playable_map_left = left1;
			else
				playable_map_left = left2;

			if (right1 <= right2 + 3 && right1 >= right2 - 3)
				playable_map_right = right1;
			else
				playable_map_right = right2;

			if (top1 <= top2 + 3 && top1 >= top2 - 3)
				playable_map_top = top1;
			else
				playable_map_top = top2;

			if (bottom1 <= bottom2 + 3 && bottom1 >= bottom2 - 3)
				playable_map_bottom = bottom1;
			else
				playable_map_bottom = bottom2;

			playable_map_width = playable_map_right - playable_map_left;
			playable_map_height = playable_map_top - playable_map_bottom;


			map_width = GameData.MapFullWidth;
			map_height = GameData.MapFullHeight;
			playable_map_width = GameData.MapPlayableWidth2;
			playable_map_height = GameData.MapPlayableHeight2;
			playable_map_left = GameData.MapEdgeLeft2;
			playable_map_right = GameData.MapEdgeRight2;
			playable_map_top = GameData.MapEdgeTop2;
			playable_map_bottom = GameData.MapEdgeBottom2;
			int lol = 0;

			/*playable_map_width += 5;
			playable_map_height += 5;
			playable_map_left -= 2;
			playable_map_right += 3;
			playable_map_top += 3;
			playable_map_bottom -= 2;*/
		}

		private void updateRankTexture(Data.Player p)
		{
			if (/*Debugger.IsAttached*/false)
			{
				p.UpdateRankTexture();
				rank_textures[p.number] = p.rank_texture;
			}
			else
			{
				try
				{
					p.UpdateRankTexture();
					rank_textures[p.number] = p.rank_texture;
					rank_tooltips[p.number] = p.rankIconTooltip;
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception exception)
				{
					WT.ReportCrash(exception, Program.ApplicationTitle + " " + Program.ApplicationVersion, null, null, false);
				}
			}
		}

		private void UpdateStuff()
		{
			if (!GameData.SC2Opened)
			{
				this.GameClosed();
			}
			else
			{
				ingame = _2csAPI.InGame();
				if (!ingame)
				{
					this.GameClosed();
				}
				else
				{
					if (actual_players == 0)
					{
						this.GameStart();
					}
					if (!got_local)
					{
						this.GetLocalPlayer();
					}
					else
					{
						this.UpdateMapSize();
						this.GetPlayers(false);
						this.GetUnits();
					}
				}
			}
		}

		private void updateTimers()
		{
			IniFile file = new IniFile(settings_path);
			if (file.Exists())
			{
				file.Load();
				try
				{
					this.tmrMain.Interval = int.Parse(file["OptionsTimers"]["udGUI"]);
				}
				catch
				{
				}
				this.tmrMain.Enabled = true;
				try
				{
					HUDRefreshRate = int.Parse(file["OptionsTimers"]["udDrawing"]);
				}
				catch
				{
				}
				try
				{
					UpdateInfoDelay = int.Parse(file["OptionsTimers"]["udData"]);
				}
				catch
				{
				}
			}
		}

		public static DirectX_HUDs[] HUDs
		{
			get
			{
				if (_HUDs == null)
				{
					_HUDs = new DirectX_HUDs[] { ResourcesHUD, MapHUD, ObserverHUD };
					foreach (DirectX_HUDs ds in _HUDs)
					{
						if (ds != null)
						{
							ds.Show();
						}
					}
				}
				return _HUDs;
			}
		}

		public static Size minimap_size
		{
			get
			{
				return new Size((int) map_width, (int) map_height);
			}
		}
	}
}

