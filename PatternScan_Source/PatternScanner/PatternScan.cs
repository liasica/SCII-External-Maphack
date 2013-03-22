namespace PatternScanner
{
	using Magic;
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Drawing;
	using Utilities.MemoryHandling;
	public class PatternScan
	{
		private uint _GalaxyDataTable;
		private uint _Timer;
		private uint _Timer2;
		private uint _LocalPlayerNumber;
		private uint _LocalSelection;
		private uint _MapBounds;
		private uint _MapInfoPtr;
		private uint _PlayerSelection;
		private uint _PlayerStruct;
		private uint _PlayerStructSize;
		private BlackMagic _process;
		private uint _UnitStruct;
		private uint _SelectionFunction;
		private uint _OrderFunction;
		private uint _MainLoopHook;

		private void BuildPattern(List<byte> pattern, ref string mask, byte value)
		{
			byte[] bytes = BitConverter.GetBytes((short) value);
			pattern.Add(bytes[0]);
			mask = mask + "x";
		}

		private void BuildPattern(List<byte> pattern, ref string mask, int value)
		{
			byte[] bytes = BitConverter.GetBytes(0x2211e);
			pattern.AddRange(bytes);
			for (int i = 0; i < bytes.Length; i++)
			{
				mask = mask + "x";
			}
		}

		private void BuildPatternWithNull(List<byte> pattern, ref string mask, int count)
		{
			for (int i = 1; i <= count; i++)
			{
				pattern.Add(0);
				mask = mask + "x";
			}
		}

		public List<uint> FindDataRegionPatterns(byte[] pattern, string mask)
		{
			uint startAddress = (uint)this.Process.MainModule.BaseAddress;
			return this.FindDataRegionPatterns(startAddress, pattern, mask);
		}

		public List<uint> FindDataRegionPatterns(uint startAddress, byte[] pattern, string mask)
		{
			uint num = 0x03630000;
			return this.FindPatterns(startAddress, (int) (num - startAddress), pattern, mask);
		}

		public uint FindPattern(byte[] pattern, string mask)
		{
			if (this.Process == null)
			{
				return 0;
			}
			return this.Process.FindPattern(pattern, mask);
		}

		public List<uint> FindPatterns(uint startAddress, int searchLength, byte[] pattern, string mask)
		{
			List<uint> list = new List<uint>();
			if (this.Process == null)
			{
				return list;
			}
			uint dwStart = startAddress;
			uint num2 = dwStart;
			int num3 = 0;
		Label_0016:
			try
			{
				dwStart = this.Process.FindPattern(dwStart, searchLength, pattern, mask);
			}
			catch
			{
				goto Label_0044;
			}
			if (dwStart == num2)
			{
				return list;
			}
			list.Add(dwStart++);
			searchLength -= (int) (dwStart - num2);
			num2 = dwStart;
		
			num3++;
			goto Label_0016;
		Label_0044:
			return list;
		}

		public uint GalaxyDataTable()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._GalaxyDataTable == 0)
			{
				byte[] bPattern = new byte[] { 0x8D, 0x04, 0x40, 0x03, 0xC0, 0x8B, 0xD1, 0x03, 0xC0, 0xC1, 0xEA, 0x10, 0x66, 0x3B, 0x90, 0, 0, 0, 0 };
				uint num = this.Process.FindPattern(bPattern, "xxxxxxxxxxxxxxx????");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._GalaxyDataTable = this.Process.ReadUInt(num + 15);
				}
			}
			return this._GalaxyDataTable;
		}

		public uint Timer() //the source for Galaxy_GameGetMissionTime
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._Timer == 0)
			{
				byte[] bPattern = new byte[] { 
					0x81, 0x05, 0, 0, 0, 0, 0x00, 0x01, 0x00, 0x00, 0xA3, 0, 0, 0, 0, 0xA1,
					0, 0, 0, 0, 0x03, 0x05, 0, 0, 0, 0, 0xF7, 0x05, 0, 0, 0, 0,
					0x00, 0x02, 0x00, 0x00, 0xA3, 0, 0, 0, 0, 0, 0, 0x81, 0x05, 0, 0, 0,
					0, 0x00, 0x01, 0x00, 0x00
				 };
				uint num = this.Process.FindPattern(bPattern, "xx????xxxxx????x????xx????xx????xxxxx??????xx????xxxx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._Timer = this.Process.ReadUInt(num + 45);
				}
			}
			return this._Timer;
		}

		public uint Timer2() //seems to be the amount of time the GUI is on the screen.
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._Timer2 == 0)
			{
				byte[] bPattern = new byte[] { 
					0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x2B, 0xC1,  0x8B, 0xD8, 0x03, 0xCB, 0x89, 0x0D
				 };
				uint num = this.Process.FindPattern(bPattern, "xx????xxxxxxxx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._Timer2 = this.Process.ReadUInt(num + 2);
				}
			}
			return this._Timer2;
		}

		public uint LocalPlayerNumber()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._LocalPlayerNumber == 0)
			{
				byte[] bPattern = new byte[] { 0xA0, 0, 0, 0, 0, 0xC3, 0xA0, 0, 0, 0, 0, 0xC3};
				uint num = this.Process.FindPattern(bPattern, "x????xx????x");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					if(this.Process.ReadUInt(num + 7) + 1 == this.Process.ReadUInt(num + 1))
						this._LocalPlayerNumber = this.Process.ReadUInt(num + 7);
				}
			}
			return this._LocalPlayerNumber;
		}

		public uint LocalSelection()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._LocalSelection == 0)
			{
				byte[] bPattern = new byte[] { 0x53, 0x68, 0, 0, 0, 0, 0x8D, 0x8D, 0x4C, 0xF0, 0xFF, 0xFF };
				uint num = this.Process.FindPattern(bPattern, "xx????xxxxxx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._LocalSelection = this.Process.ReadUInt(num + 2);
				}
			}
			return this._LocalSelection;
		}

		public uint MapBounds()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._MapBounds == 0)
			{
				byte[] bPattern = new byte[] { 0x8B, 0x16, 0x89, 0x15, 0, 0, 0, 0, 0x8B, 0x46, 0x04, 0xA3, 0, 0, 0, 0, 0x8B, 0x4E, 0x08, 0x89, 0x0D, 0, 0, 0, 0, 0x8B, 0x56, 0x0C, 0x89, 0x15, 0, 0, 0, 0 };
				uint num = this.Process.FindPattern(bPattern, "xxxx????xxxx????xxxxx????xxxxx????");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._MapBounds = this.Process.ReadUInt(num + 4);
				}
				
			}
			return this._MapBounds;
		}

		public uint MapInfoPtr()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._MapInfoPtr == 0)
			{
				byte[] bPattern = new byte[] { 0x8B, 0x0D, 0, 0, 0, 0, 0x56, 0xE8, 0, 0, 0, 0, 0x8B, 0x0D, 0, 0, 0, 0, 0x8B, 0xF1 };
				uint num = this.Process.FindPattern(bPattern, "xx????xx????xx????xx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					if(this.Process.ReadUInt(num + 2) == this.Process.ReadUInt(num + 14))
						this._MapInfoPtr = this.Process.ReadUInt(num + 2);
				}

			}
			return this._MapInfoPtr;
		}

		public uint PlayerSelection()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._PlayerSelection == 0)
			{
				byte[] bPattern = new byte[] { 0x0F, 0xB6, 0xC1, 0x69, 0xC0, 0x60, 0x1B, 0x00, 0x00, 0x05, 0, 0, 0, 0, 0xC3 };
				uint num = this.Process.FindPattern(bPattern, "xxxxxxxxxx????x");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._PlayerSelection = this.Process.ReadUInt(num + 10);
				}
			}
			return this._PlayerSelection;
		}

		public uint PlayerStruct()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._PlayerStruct == 0)
			{
				byte[] bPattern = new byte[] { 15, 0xb6, 0xc1, 0x69, 0xc0, 0, 0, 0, 0, 5, 0, 0, 0, 0 };
				uint num = this.Process.FindPattern(bPattern, "xxxxx??xxx????");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._PlayerStructSize = this.Process.ReadUShort(num + 5);
					this._PlayerStruct = this.Process.ReadUInt(num + 10);
				}
			}
			return this._PlayerStruct;
		}

		public uint PlayerStructSize()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._PlayerStructSize == 0)
			{
				byte[] bPattern = new byte[] { 15, 0xb6, 0xc1, 0x69, 0xc0, 0, 0, 0, 0, 5, 0, 0, 0, 0 };
				uint num = this.Process.FindPattern(bPattern, "xxxxx??xxx????");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._PlayerStructSize = this.Process.ReadUShort(num + 5);
					this._PlayerStruct = this.Process.ReadUInt(num + 10);
				}
			}
			return this._PlayerStructSize;
		}

		public uint SelectionFunction()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._SelectionFunction == 0)
			{
				/*byte[] bPattern = new byte[] { 0x8D, 0x45, 0xFC, 0x50, 0x8D, 0x4D, 0x08, 0xC7, 0x45, 0xFC, 0x1D, 0x00, 0x00, 0x00, 0xE8, 0, 0, 0, 0, 0x6A, 0x01, 0x8A, 0xD3, 0xB9, 0x01, 0x00, 0x00, 0x00 };
				uint num = this.Process.FindPattern(bPattern, "xxxxxxxxxxxxxxx????xxxxxxxxx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._SelectionFunction = (uint)(num + 19 + this.Process.ReadInt(num + 15));
				}*/

				byte[] bPattern = new byte[] { 0x55, 0x8B, 0xEC, 0xB8, 0, 0, 0, 0, 0xE8, 0, 0, 0, 0, 0x8A, 0x45, 0x1C, 0x56, 0x8B, 0x75, 0x18, 0x56, 0xE8, 0, 0, 0, 0, 0x85, 0xC0 };
				uint num = this.Process.FindPattern(bPattern, "xxxx????x????xxxxxxxxx????xx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._SelectionFunction = num;
				}
			}
			return this._SelectionFunction;
		}

		public uint OrderFunction()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._OrderFunction == 0)
			{
				byte[] bPattern = new byte[] { 0x83, 0x3D, 0, 0, 0, 0, 0x00, 0x57, 0x8B, 0xF9, 0x75, 0, 0x8A, 0x47, 0x2A, 0x3A, 0x05, 0, 0, 0, 0, 0x75, 0, 0x8B, 0x47, 0x24,
					0xA9, 0x00, 0x01, 0x00, 0x00, 0x74, 0, 0xA9, 0x00, 0x00, 0x04, 0x00, 0x75, 0, 0xE8, 0, 0, 0, 0, 0xE8, 0, 0, 0, 0 };
				uint num = this.Process.FindPattern(bPattern, "xx????xxxxx?xxxxx????x?xxxxxxxxx?xxxxxx?x????x????");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._OrderFunction = num;
				}
			}
			return this._OrderFunction;
		}

		public uint MainLoopHook()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._MainLoopHook == 0)
			{
				byte[] bPattern = new byte[] { 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x1C, 0x53, 0xE8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x6A, 0x00, 0x6A, 0x00, 0x6A, 0x00, 0x6A, 0x00, 0x8D, 0x45, 0xE4, 0x50 };
				uint num = this.Process.FindPattern(bPattern, "xxxxxxxx??????????xxxxxxxxxxxx");
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._MainLoopHook = num + 7;
				}
			}
			return this._MainLoopHook;
		}


		public void PrintSC2Patterns()
		{
			if (this.Process != null)
			{
				Console.Title = "QaZZy's SC2 Offset Generator";
				Console.WriteLine("\nAny offsets with a value of 0x0 failed.\n");
				Console.WriteLine("SC2 Version: " + this.Process.GetModule("SC2.exe").FileVersionInfo.FileVersion);
				Console.WriteLine("Local Player Number: 0x" + string.Format("{0:X}", this.LocalPlayerNumber()));
				Console.WriteLine("Player Struct: 0x" + string.Format("{0:X}", this.PlayerStruct()));
				Console.WriteLine("Player Struct Size: 0x" + string.Format("{0:X}", this.PlayerStructSize()));
				Console.WriteLine("Unit Struct: 0x" + string.Format("{0:X}", this.UnitStruct()));
				Console.WriteLine("Map Information Pointer: 0x" + string.Format("{0:X}", this.MapBounds()));
				Console.WriteLine("Player Selection: 0x" + string.Format("{0:X}", this.PlayerSelection()));
				Console.WriteLine("Local Selection: 0x" + string.Format("{0:X}", this.LocalSelection()));
			}
		}

		public void Setup(int pid)
		{
			if (pid == 0)
			{
				Console.WriteLine("StarCraft II isn't running.");
			}
			else
			{
				this.Process = new BlackMagic();
				this.Process.OpenProcessAndThread(pid);
			}
		}

		public void Setup(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
			{
				Console.WriteLine("StarCraft II isn't running.");
			}
			else
			{
				this.Process = new BlackMagic();
				this.Process.SetDebugPrivileges = false;
				this.Process.OpenProcessAndThread(hWnd);
			}
		}

		public uint UnitStruct()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._UnitStruct == 0)
			{
				byte[] bPattern = new byte[] { 0xc1, 0xe0, 6, 5, 0, 0, 0, 0, 0x33 };
				uint num = this.Process.FindPattern(bPattern, "xxxx????x");
				if (num == (uint)this.Process.MainModule.BaseAddress)
				{
					bPattern = new byte[] { 0xc1, 0xe0, 7, 5, 0, 0, 0, 0, 0x33 };
					num = this.Process.FindPattern(bPattern, "xxxx????x");
				}
				if (num != (uint)this.Process.MainModule.BaseAddress)
				{
					this._UnitStruct = this.Process.ReadUInt(num + 4);
				}
			}
			return this._UnitStruct;
		}


		public Rectangle MinimapCoords()
		{
			if (this.Process == null)
			{
				return Rectangle.Empty;
			}
			string sPattern = "00 6D 6C 64 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 E0 0F 4D 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 28 03 00 00 1C 00 00 00 2A 04 00 00 22 01 00 00 28 03 00 00 1C 00 00 00 2A 04 00 00 22 01 00 00";
			byte[] bPattern = {0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x28, 0x03, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x2A, 0x04, 0x00, 0x00, 0x22, 0x01, 0x00, 0x00, 0x28, 0x03, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x2A, 0x04, 0x00, 0x00, 0x22, 0x01, 0x00, 0x00};
			
			List<uint[]> RegionList = Utilities.MemoryHandling.ReadWriteMemory.getProcessDynamicMemoryRegions(this.Process.ProcessHandle);
			Comparison<uint[]> comp = delegate (uint[] val1, uint[] val2)
			{
				uint center = 0x30000000;
				uint location1 = val1[0] + val1[1] / 2;
				uint location2 = val2[0] + val2[1] / 2;

				if(location1 > center)
					location1 = location1 - center;
				else
					location1 = center - location1;

				if(location2 > center)
					location2 = location2 - center;
				else
					location2 = center - location2;

				if(location1 == location2)
					return 0;
				if(location1 > location2)
					return 1;

				return -1;
			};
			RegionList.Sort(comp);


			uint Address = 0;
			foreach (uint[] region in RegionList)
			{
				Address = region[0];
				uint OldAddress = Address;

				do
				{
					OldAddress = Address;
					try
					{
						BlackMagic.PatternDataEntry dataentry = new BlackMagic.PatternDataEntry(OldAddress, (int)(region[0] + region[1]) - (int)OldAddress - 1
							, this.Process.ReadBytes(OldAddress, (int)(region[0] + region[1]) - (int)OldAddress - 1));
			

						//Address = this.Process.FindPattern(OldAddress, (int)(region[0] + region[1]) - (int) OldAddress - 1, sPattern, "xxxx????????????????xxxx????????????????????xxxxxxxx??xx??xx??xx??xx??xx??xx??xx??xx");
						Address = SPattern.FindPattern(dataentry.bData, bPattern, "xxxxxxxx??xx??xx??xx??xx??xx??xx??xx??xx") + OldAddress;
					}
					catch
					{
						int oops = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
						continue;
					}
					if (Address != OldAddress)
					{
						byte[] temp = this.Process.ReadBytes(Address, 40);
						if (temp != null
						 && temp[0x08] == temp[0x18] && temp[0x09] == temp[0x19]
						 && temp[0x0c] == temp[0x1c] && temp[0x0d] == temp[0x1d]
						 && temp[0x10] == temp[0x20] && temp[0x11] == temp[0x21]
						 && temp[0x14] == temp[0x24] && temp[0x15] == temp[0x25])
						{
							int Y = (temp[0x0b] << 24) + (temp[0x0a] << 16) + (temp[0x09] << 8) + temp[0x08];
							int X = (temp[0x0f] << 24) + (temp[0x0e] << 16) + (temp[0x0d] << 8) + temp[0x0c];
							int Y2 = (temp[0x13] << 24) + (temp[0x12] << 16) + (temp[0x11] << 8) + temp[0x10];
							int X2 = (temp[0x17] << 24) + (temp[0x16] << 16) + (temp[0x15] << 8) + temp[0x14];
							if (Y > 1 && X > 1 && Y < Y2 && X < X2 && X < Y / 10 && X2 < Y2 / 2)
							{
								Rectangle ScreenRect = Screen.GetBounds(Point.Empty);
								if (X > ScreenRect.Left && Y > ScreenRect.Bottom / 3 && X2 < ScreenRect.Right && Y2 < ScreenRect.Bottom)
									return new Rectangle(X, Y, X2 - X, Y2 - Y);
							}
						}
					}
					Address += 40;
				} while (Address > OldAddress + 40 && Address < (region[0] + region[1]) - 40);
			}

			return Rectangle.Empty;
		}

		public BlackMagic Process
		{
			get
			{
				return this._process;
			}
			set
			{
				this._process = value;
			}
		}
	}
}

