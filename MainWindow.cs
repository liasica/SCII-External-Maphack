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
        private static DirectX_HUDs[] _HUDs;
        public static int active_players = 0;
        public static byte[] actual_player = new byte[0x10];
        public static byte actual_players = 0;
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
        public static float minimap_height;
        public static float minimap_width;
        public static int neutralplayer = -1;
        public static int no_showing = 0;
        public static DirectX_HUDs ObserverHUD;
        private DataGridViewImageColumn p_color;
        public static uint[] p_owner = new uint[0xfa0];
        public static bool pending = false;
        public static byte[] player_active = new byte[0x10];
        public static uint[] player_cameraX = new uint[0x10];
        public static uint[] player_cameraY = new uint[0x10];
        public static uint[] player_minerals = new uint[0x10];
        public static string[] player_name = new string[0x10];
        public static bool[] player_playing = new bool[0x10];
        public static Data.Race[] player_race = new Data.Race[0x10];
        public static uint[] player_slots = new uint[0x10];
        public static VictoryStatus[] player_status = new VictoryStatus[0x10];
        public static string[] player_supply = new string[0x10];
        public static uint[] player_teams = new uint[0x10];
        public static PlayerType[] player_types = new PlayerType[0x10];
        public static uint[] player_vespene = new uint[0x10];
        private DataGridViewTextBoxColumn PlayerName;
        private DataGridViewTextBoxColumn PlayerNumber;
        private DataGridViewTextBoxColumn Race;
        private DataGridViewImageColumn rank;
        public static string[] rank_textures = new string[0x10];
        private Thread readMemory;
        private ToolStripMenuItem resetToolStripMenuItem;
        public static DirectX_HUDs ResourcesHUD;
        public static bool[] show = new bool[0x10];
        public static bool[] show_window = new bool[0x10];
        private DataGridViewTextBoxColumn Status;
        private ToolStripMenuItem stopToolStripMenuItem;
        private DataGridViewTextBoxColumn TeamNumber;
        private System.Windows.Forms.Timer tmrMain;
        private DataGridViewCheckBoxColumn Toggle;
        private ToolStrip toolStrip;
        public ToolStripButton toolStripButtonMap;
        public ToolStripButton toolStripButtonObserver;
        public ToolStripButton toolStripButtonResources;
        private ToolStripLabel toolStripLabelStatus;
        public static int total_units;
        public static uint[] u_type = new uint[0xfa0];
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
        public static float[] x_coords = new float[0xfa0];
        public static float[] x_coordsDest = new float[0xfa0];
        public static float[] y_coords = new float[0xfa0];
        public static float[] y_coordsDest = new float[0xfa0];

        public MainWindow()
        {
            this.InitializeComponent();
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "SC2 External Maphack");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                path = Path.Combine(path, "settings.ini");
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
            draw = true;
            this.initThreads();
            this.updateTimers();
            if (!Debugger.IsAttached)
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
            active_players = 0;
            actual_players = 0;
            this.UpdateMapSize();
            this.GetPlayers(true);
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
            List<Data.Player> list = GameData.getPlayersData();
            int num = 0;
            for (byte i = 0; i < list.Count; i = (byte) (i + 1))
            {
                Data.Player player = list[i];
                if ((player_types[i] != PlayerType.Neutral) && (player_types[i] != PlayerType.Hostile))
                {
                    if (atGameStart)
                    {
                        actual_players = (byte) (actual_players + 1);
                        actual_player[actual_players] = i;
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
                    player_supply[i] = player.supply + "/" + player.supplyCap;
                }
            }
            active_players = num;
        }

        private void GetUnits()
        {
            Func<Data.Player, bool> predicate = null;
            uint owner;
            if ((total_units + 1) < 0xfa0)
            {
                int[,] numArray = new int[0x10, 130];
                List<Unit> list = GameData.getUnitData();
                List<Data.Player> source = GameData.getPlayersData();
                if ((list.Count != 0) && (source.Count != 0))
                {
                    total_units = list.Count;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list.Count >= x_coords.Length)
                        {
                            IntPtr? handleToScreenShot = null;
                            Size? screenShotSize = null;
                            WT.ReportCrash(new Exception("The current unit count (" + list.Count + ") exceed the array size."), Program.ApplicationTitle + " " + Program.ApplicationVersion, handleToScreenShot, screenShotSize, false);
                        }
                        else
                        {
                            Unit unit = list[i];
                            uint unitType = (uint) unit.unitType;
                            if ((Array.IndexOf(Enum.GetValues(typeof(UnitType)), (UnitType) unitType) != -1) && unit_show[unitType])
                            {
                                owner = unit.playerNumber;
                                if (predicate == null)
                                {
                                    predicate = x => x.number == owner;
                                }
                                Data.Player? nullable = new Data.Player?(source.Where<Data.Player>(predicate).FirstOrDefault<Data.Player>());
                                if (!nullable.HasValue)
                                {
                                    IntPtr? nullable4 = null;
                                    Size? nullable5 = null;
                                    WT.ReportCrash(new Exception("The current owner (" + owner + ") was not found in the players."), Program.ApplicationTitle + " " + Program.ApplicationVersion, nullable4, nullable5, false);
                                }
                                else if (nullable.Value.victoryStatus != VictoryStatus.Playing)
                                {
                                    x_coords[i] = 0f;
                                    y_coords[i] = 0f;
                                    x_coordsDest[i] = 0f;
                                    y_coordsDest[i] = 0f;
                                }
                                else
                                {
                                    if (unit_count_index[unitType] != -1)
                                    {
                                        try
                                        {
                                            numArray[(int) ((IntPtr) owner), (int) ((IntPtr) unit_count_index[unitType])]++;
                                        }
                                        catch (Exception exception)
                                        {
                                            string message = "There was an error in using the unit counter.";
                                            IntPtr? nullable6 = null;
                                            Size? nullable7 = null;
                                            WT.ReportCrash(new Exception(message, exception), Program.ApplicationTitle + " " + Program.ApplicationVersion, nullable6, nullable7, false);
                                        }
                                    }
                                    x_coords[i] = unit.locationX;
                                    y_coords[i] = unit.locationY;
                                    x_coordsDest[i] = unit.destinationX;
                                    y_coordsDest[i] = unit.destinationY;
                                    p_owner[i] = owner;
                                    u_type[i] = unitType;
                                }
                            }
                        }
                    }
                    unit_counter = numArray;
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
            this.components = new Container();
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            DataGridViewCellStyle style2 = new DataGridViewCellStyle();
            DataGridViewCellStyle style3 = new DataGridViewCellStyle();
            DataGridViewCellStyle style4 = new DataGridViewCellStyle();
            DataGridViewCellStyle style5 = new DataGridViewCellStyle();
            DataGridViewCellStyle style6 = new DataGridViewCellStyle();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(MainWindow));
            this.dataGridViewPlayerData = new DataGridView();
            this.PlayerNumber = new DataGridViewTextBoxColumn();
            this.p_color = new DataGridViewImageColumn();
            this.rank = new DataGridViewImageColumn();
            this.TeamNumber = new DataGridViewTextBoxColumn();
            this.PlayerName = new DataGridViewTextBoxColumn();
            this.Status = new DataGridViewTextBoxColumn();
            this.Race = new DataGridViewTextBoxColumn();
            this.Toggle = new DataGridViewCheckBoxColumn();
            this.toolStrip = new ToolStrip();
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.resetToolStripMenuItem = new ToolStripMenuItem();
            this.stopToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripButtonResources = new ToolStripButton();
            this.toolStripButtonObserver = new ToolStripButton();
            this.toolStripButtonMap = new ToolStripButton();
            this.btnOptions = new ToolStripButton();
            this.toolStripLabelStatus = new ToolStripLabel();
            this.tmrMain = new System.Windows.Forms.Timer(this.components);
            ((ISupportInitialize) this.dataGridViewPlayerData).BeginInit();
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            base.SuspendLayout();
            this.dataGridViewPlayerData.AllowUserToAddRows = false;
            this.dataGridViewPlayerData.AllowUserToDeleteRows = false;
            this.dataGridViewPlayerData.AllowUserToOrderColumns = true;
            this.dataGridViewPlayerData.AllowUserToResizeColumns = false;
            this.dataGridViewPlayerData.AllowUserToResizeRows = false;
            this.dataGridViewPlayerData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPlayerData.BackgroundColor = Color.Black;
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = Color.FromArgb(0x40, 0x40, 0x40);
            style.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = Color.GreenYellow;
            style.SelectionBackColor = Color.Gray;
            style.SelectionForeColor = Color.DodgerBlue;
            style.WrapMode = DataGridViewTriState.True;
            this.dataGridViewPlayerData.ColumnHeadersDefaultCellStyle = style;
            this.dataGridViewPlayerData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewPlayerData.Columns.AddRange(new DataGridViewColumn[] { this.PlayerNumber, this.p_color, this.rank, this.TeamNumber, this.PlayerName, this.Status, this.Race, this.Toggle });
            style2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style2.BackColor = Color.FromArgb(0x40, 0x40, 0x40);
            style2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style2.ForeColor = Color.GreenYellow;
            style2.SelectionBackColor = Color.Black;
            style2.SelectionForeColor = Color.DodgerBlue;
            style2.WrapMode = DataGridViewTriState.False;
            this.dataGridViewPlayerData.DefaultCellStyle = style2;
            this.dataGridViewPlayerData.GridColor = Color.Black;
            this.dataGridViewPlayerData.Location = new Point(0, 0);
            this.dataGridViewPlayerData.Name = "dataGridViewPlayerData";
            this.dataGridViewPlayerData.ReadOnly = true;
            style3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style3.BackColor = Color.FromArgb(0x40, 0x40, 0x40);
            style3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style3.ForeColor = SystemColors.WindowText;
            style3.SelectionBackColor = SystemColors.Highlight;
            style3.SelectionForeColor = SystemColors.HighlightText;
            style3.WrapMode = DataGridViewTriState.True;
            this.dataGridViewPlayerData.RowHeadersDefaultCellStyle = style3;
            this.dataGridViewPlayerData.RowHeadersWidth = 0x18;
            this.dataGridViewPlayerData.ScrollBars = ScrollBars.None;
            this.dataGridViewPlayerData.Size = new Size(0x199, 0x18);
            this.dataGridViewPlayerData.TabIndex = 2;
            this.dataGridViewPlayerData.CellContentClick += new DataGridViewCellEventHandler(this.dataGridViewPlayerData_CellContentClick);
            style4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.PlayerNumber.DefaultCellStyle = style4;
            this.PlayerNumber.FillWeight = 30.83903f;
            this.PlayerNumber.HeaderText = "#";
            this.PlayerNumber.MinimumWidth = 0x23;
            this.PlayerNumber.Name = "PlayerNumber";
            this.PlayerNumber.ReadOnly = true;
            this.p_color.FillWeight = 40.68842f;
            this.p_color.HeaderText = "C";
            this.p_color.Name = "p_color";
            this.p_color.ReadOnly = true;
            this.rank.FillWeight = 53.61267f;
            this.rank.HeaderText = "R";
            this.rank.MinimumWidth = 0x19;
            this.rank.Name = "rank";
            this.rank.ReadOnly = true;
            this.TeamNumber.HeaderText = "Team #";
            this.TeamNumber.Name = "TeamNumber";
            this.TeamNumber.ReadOnly = true;
            this.PlayerName.FillWeight = 133.6321f;
            this.PlayerName.HeaderText = "Name";
            this.PlayerName.Name = "PlayerName";
            this.PlayerName.ReadOnly = true;
            style5.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Status.DefaultCellStyle = style5;
            this.Status.FillWeight = 61.06282f;
            this.Status.HeaderText = "Status";
            this.Status.MinimumWidth = 0x37;
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            style6.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Race.DefaultCellStyle = style6;
            this.Race.FillWeight = 56.07122f;
            this.Race.HeaderText = "Race";
            this.Race.MinimumWidth = 0x33;
            this.Race.Name = "Race";
            this.Race.ReadOnly = true;
            this.Toggle.FillWeight = 58.0429f;
            this.Toggle.HeaderText = "Display";
            this.Toggle.MinimumWidth = 0x35;
            this.Toggle.Name = "Toggle";
            this.Toggle.ReadOnly = true;
            this.Toggle.Resizable = DataGridViewTriState.True;
            this.Toggle.SortMode = DataGridViewColumnSortMode.Automatic;
            this.toolStrip.BackColor = Color.FromArgb(0x40, 0x40, 0x40);
            this.toolStrip.ContextMenuStrip = this.contextMenuStrip;
            this.toolStrip.Dock = DockStyle.Bottom;
            this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new ToolStripItem[] { this.toolStripButtonResources, this.toolStripButtonObserver, this.toolStripButtonMap, this.btnOptions, this.toolStripLabelStatus });
            this.toolStrip.Location = new Point(0, 0x17);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RightToLeft = RightToLeft.Yes;
            this.toolStrip.Size = new Size(0x199, 0x19);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip";
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] { this.resetToolStripMenuItem, this.stopToolStripMenuItem });
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new Size(0x99, 70);
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new EventHandler(this.resetToolStripMenuItem_Click);
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new EventHandler(this.stopToolStripMenuItem_Click);
            this.toolStripButtonResources.BackColor = Color.Gray;
            this.toolStripButtonResources.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.toolStripButtonResources.Image = (Image) manager.GetObject("toolStripButtonResources.Image");
            this.toolStripButtonResources.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonResources.Name = "toolStripButtonResources";
            this.toolStripButtonResources.Size = new Size(0x54, 0x16);
            this.toolStripButtonResources.Text = "Resources Off";
            this.toolStripButtonResources.Click += new EventHandler(this.toolStripButtonResources_Click);
            this.toolStripButtonObserver.BackColor = Color.Gray;
            this.toolStripButtonObserver.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.toolStripButtonObserver.Image = (Image) manager.GetObject("toolStripButtonObserver.Image");
            this.toolStripButtonObserver.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonObserver.Name = "toolStripButtonObserver";
            this.toolStripButtonObserver.Size = new Size(0x4e, 0x16);
            this.toolStripButtonObserver.Text = "Observer Off";
            this.toolStripButtonObserver.Click += new EventHandler(this.toolStripButtonObserver_Click);
            this.toolStripButtonMap.BackColor = Color.Gray;
            this.toolStripButtonMap.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.toolStripButtonMap.Image = (Image) manager.GetObject("toolStripButtonMap.Image");
            this.toolStripButtonMap.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonMap.Name = "toolStripButtonMap";
            this.toolStripButtonMap.Size = new Size(0x37, 0x16);
            this.toolStripButtonMap.Text = "Map Off";
            this.toolStripButtonMap.Click += new EventHandler(this.toolStripButtonMap_Click);
            this.btnOptions.BackColor = Color.LightGray;
            this.btnOptions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.btnOptions.ImageTransparentColor = Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new Size(0x35, 0x16);
            this.btnOptions.Text = "Options";
            this.btnOptions.Click += new EventHandler(this.btnOptions_Click);
            this.toolStripLabelStatus.BackColor = Color.DarkRed;
            this.toolStripLabelStatus.ForeColor = Color.DodgerBlue;
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            this.toolStripLabelStatus.Size = new Size(0, 0x16);
            this.tmrMain.Interval = 0x3e8;
            this.tmrMain.Tick += new EventHandler(this.tmrMain_Tick);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Black;
            base.ClientSize = new Size(0x199, 0x30);
            base.Controls.Add(this.toolStrip);
            base.Controls.Add(this.dataGridViewPlayerData);
            base.FormBorderStyle = FormBorderStyle.Fixed3D;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.Name = "MainWindow";
            this.Text = "StarCraft II";
            base.FormClosed += new FormClosedEventHandler(this.MainWindow_FormClosed);
            ((ISupportInitialize) this.dataGridViewPlayerData).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
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
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), @"SC2 External Maphack\settings.ini");
            if (File.Exists(path))
            {
                File.Delete(path);
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
                    for (int i = 0; i < actualPlayers.Count; i++)
                    {
                        Data.Player player = actualPlayers[i];
                        uint number = player.number;
                        if (rank_textures[number] != null)
                        {
                            this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["rank"].Index].Value = Image.FromFile(rank_textures[number]);
                        }
                        else
                        {
                            this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["rank"].Index].Value = Image.FromFile(@"Leagues\none.png");
                        }
                        this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["Status"].Index].Value = player.victoryStatus;
                        this.dataGridViewPlayerData.Rows[i].Cells[this.dataGridViewPlayerData.Columns["Race"].Index].Value = player.race;
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
            minimap_width = GameData.MapFullWidth;
            minimap_height = GameData.MapFullHeight;
        }

        private void updateRankTexture(Data.Player p)
        {
            if (Debugger.IsAttached)
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
                        this.GetPlayers(false);
                        this.GetUnits();
                    }
                }
            }
        }

        private void updateTimers()
        {
            IniFile file = new IniFile(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), @"SC2 External Maphack\settings.ini"));
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
                return new Size((int) minimap_width, (int) minimap_height);
            }
        }
    }
}

