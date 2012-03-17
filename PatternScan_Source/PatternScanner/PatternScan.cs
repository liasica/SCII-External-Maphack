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
		private uint _LocalPlayerNumber;
		private uint _LocalSelection;
		private uint _MapInfoPtr;
		private uint _PlayerSelection;
		private uint _PlayerStruct;
		private uint _PlayerStructSize;
		private BlackMagic _process;
		private uint _UnitStruct;

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
			uint startAddress = 0x800000;
			return this.FindDataRegionPatterns(startAddress, pattern, mask);
		}

		public List<uint> FindDataRegionPatterns(uint startAddress, byte[] pattern, string mask)
		{
			uint num = 0x3630000;
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
		Label_0044:
			num3++;
			goto Label_0016;
		}

		public uint LocalPlayerNumber()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._LocalPlayerNumber == 0)
			{
				byte[] bPattern = new byte[] { 
					0x75, 0x11, 0x8a, 13, 0, 0, 0, 0, 0xba, 1, 0, 0, 0, 0xd3, 0xe2, 0xf7, 
					210, 0x21, 0x10, 0xc3
				 };
				uint num = this.Process.FindPattern(bPattern, "xxxx????xxxxxxxxxxxx");
				if (num != 0x800000)
				{
					this._LocalPlayerNumber = this.Process.ReadUInt(num + 4);
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
				byte[] bPattern = new byte[] { 0x88, 0x45, 0xf4, 0x8d, 0x45, 0xff, 80, 0x56, 0xb9, 0, 0, 0, 0 };
				uint num = this.Process.FindPattern(bPattern, "xxxxxxxxx????");
				if (num != 0x800000)
				{
					this._LocalSelection = this.Process.ReadUInt(num + 9);
				}
			}
			return this._LocalSelection;
		}

		public uint MapInfoPtr()
		{
			if (this.Process == null)
			{
				return 0;
			}
			if (this._MapInfoPtr == 0)
			{
				byte[] bPattern = new byte[] { 0x8b, 13, 0, 0, 0, 0, 0x8b, 0x81, 0, 9, 0, 0, 0xa3 };
				uint num = this.Process.FindPattern(bPattern, "xx????xx?xxxx");
				if (num != 0x800000)
				{
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
				byte[] bPattern = new byte[] { 
					15, 0xb6, 0xc9, 0x8b, 0xc1, 0x69, 0xc9, 0xb0, 0x81, 0, 0, 0x69, 0xc0, 0xf8, 12, 0, 
					0, 5, 0, 0, 0, 0
				 };
				uint num = this.Process.FindPattern(bPattern, "xxxxxxxxxxxxxxxxxx????");
				if (num != 0x800000)
				{
					this._PlayerSelection = this.Process.ReadUInt(num + 0x12);
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
				if (num != 0x800000)
				{
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
				if (num != 0x800000)
				{
					this._PlayerStructSize = this.Process.ReadUShort(num + 5);
				}
			}
			return this._PlayerStructSize;
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
				Console.WriteLine("Map Information Pointer: 0x" + string.Format("{0:X}", this.MapInfoPtr()));
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
				if (num == 0x800000)
				{
					bPattern = new byte[] { 0xc1, 0xe0, 7, 5, 0, 0, 0, 0, 0x33 };
					num = this.Process.FindPattern(bPattern, "xxxx????x");
				}
				if (num != 0x800000)
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
			byte[] bPattern = {0xE0, 0x0F, 0x4D, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x28, 0x03, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x2A, 0x04, 0x00, 0x00, 0x22, 0x01, 0x00, 0x00, 0x28, 0x03, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x2A, 0x04, 0x00, 0x00, 0x22, 0x01, 0x00, 0x00};
			
			List<uint[]> RegionList = Utilities.MemoryHandling.ReadWriteMemory.getProcessDynamicMemoryRegions();
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
						Address = SPattern.FindPattern(dataentry.bData, bPattern, "xxxx????????????????xxxxxxxx??xx??xx??xx??xx??xx??xx??xx??xx") + OldAddress;
					}
					catch
					{
						int oops = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
						continue;
					}
					if (Address != OldAddress)
					{
						byte[] temp = this.Process.ReadBytes(Address, 60);
						if (temp[0x1c] == temp[0x2c] && temp[0x1d] == temp[0x2d]
						 && temp[0x20] == temp[0x30] && temp[0x21] == temp[0x31]
						 && temp[0x24] == temp[0x34] && temp[0x25] == temp[0x35]
						 && temp[0x28] == temp[0x38] && temp[0x29] == temp[0x39])
						{
							int Y = (temp[0x1f] << 24) + (temp[0x1e] << 16) + (temp[0x1d] << 8) + temp[0x1c];
							int X = (temp[0x23] << 24) + (temp[0x22] << 16) + (temp[0x21] << 8) + temp[0x20];
							int Y2 = (temp[0x27] << 24) + (temp[0x26] << 16) + (temp[0x25] << 8) + temp[0x24];
							int X2 = (temp[0x2b] << 24) + (temp[0x2a] << 16) + (temp[0x29] << 8) + temp[0x28];
							if (Y > 1 && X > 1 && Y < Y2 && X < X2 && X < Y / 10 && X2 < Y2 / 2)
							{
								Rectangle ScreenRect = Screen.GetBounds(Point.Empty);
								if (X > ScreenRect.Left && Y > ScreenRect.Top && X2 < ScreenRect.Right && Y2 < ScreenRect.Bottom)
									return new Rectangle(X, Y, X2 - X, Y2 - Y);
							}
						}
					}
					Address += 60;
				} while (Address > OldAddress + 60 && Address < (region[0] + region[1]) - 60);
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

