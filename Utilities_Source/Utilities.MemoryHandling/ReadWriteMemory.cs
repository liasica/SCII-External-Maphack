namespace Utilities.MemoryHandling
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Utilities.WinControl;
	using System.Collections;
	using System.Collections.Generic;

	public class ReadWriteMemory
	{
		private int m_lpBytesRead;
		private int m_lpBytesWrote;
		public IntPtr m_lpHandle;

		public ReadWriteMemory(int pid)
		{
			this.m_lpHandle = Imports.OpenProcess((Imports.ProcessAccessFlags)0x1f0fff, false, pid);
			if (this.m_lpHandle.ToInt32() == 0)
			{
				Console.WriteLine(string.Concat(new object[] { "Failed To Open Process: ", pid, ". Error: ", Imports.GetLastError() }), "Error");
			}
		}

		public ReadWriteMemory(IntPtr hwnd)
		{
			this.m_lpHandle = Imports.OpenProcess((Imports.ProcessAccessFlags)0x1f0fff, false, WC.Hwnd2ID(hwnd));
			if (this.m_lpHandle.ToInt32() == 0)
			{
				Console.WriteLine(string.Concat(new object[] { "Failed To Open Process: ", hwnd, ". Error: ", Imports.GetLastError() }), "Error");
			}
		}

		public void CreateRemoteThread(uint address, IntPtr parameter)
		{
			Imports.CreateRemoteThread(this.m_lpHandle, IntPtr.Zero, 0, address, parameter, 0, IntPtr.Zero);
		}

		~ReadWriteMemory()
		{
			Imports.CloseHandle(this.m_lpHandle);
		}

		public static uint FindPattern(byte[] bData, byte[] bPattern, string szMask)
		{
			if ((bData == null) || (bData.Length == 0))
			{
				throw new ArgumentNullException("bData");
			}
			if ((bPattern == null) || (bPattern.Length == 0))
			{
				throw new ArgumentNullException("bPattern");
			}
			if (szMask == string.Empty)
			{
				throw new ArgumentNullException("szMask");
			}
			if (bPattern.Length != szMask.Length)
			{
				throw new ArgumentException("Pattern and Mask lengths must be the same.");
			}
			bool flag = false;
			int length = bPattern.Length;
			int num4 = bData.Length - length;
			for (int i = 0; i < num4; i++)
			{
				flag = true;
				for (int j = 0; j < length; j++)
				{
					if (((szMask[j] == 'x') && (bPattern[j] != bData[i + j])) || ((szMask[j] == '!') && (bPattern[j] == bData[i + j])))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return (uint) i;
				}
			}
			return 0;
		}

		public uint FindPattern(uint dwStart, uint dwEnd, byte[] bPattern, string szMask)
		{
			byte[] buffer;
			if (dwStart > dwEnd)
			{
				throw new ArgumentException("Start Address cannot be bigger than the End Address");
			}
			int bufferLength = (int) (dwEnd - dwStart);
			if ((bPattern == null) || (bPattern.Length == 0))
			{
				throw new ArgumentNullException("bData");
			}
			if (bPattern.Length != szMask.Length)
			{
				throw new ArgumentException("bData and szMask must be of the same size");
			}
			this.ReadMemory(dwStart, bufferLength, out buffer);
			if (buffer == null)
			{
				throw new Exception("Could not read memory in FindPattern.");
			}
			uint num2 = FindPattern(buffer, bPattern, szMask);
			if (num2 == 0)
			{
				return 0;
			}
			return (dwStart + num2);
		}

		public void getProcessMemoryRegions()
		{
			long num = 0x7fffffffL;
			long num2 = 0L;
			do
			{
				Imports.MEMORY_BASIC_INFORMATION lpBuffer = new Imports.MEMORY_BASIC_INFORMATION();
				bool flag = Imports.VirtualQueryEx(Process.GetProcessesByName("SC2")[0].Handle, (IntPtr) num2, out lpBuffer, (uint) Marshal.SizeOf(lpBuffer));
				Console.WriteLine("{0}-{1} : {2} bytes result={3}", new object[] { lpBuffer.BaseAddress, (uint) (((int) lpBuffer.BaseAddress) + ((int) lpBuffer.RegionSize)), lpBuffer.RegionSize, flag });
				num2 = lpBuffer.BaseAddress.ToInt64() + lpBuffer.RegionSize.ToInt64();
			}
			while (num2 <= num);
		}

		public static List<uint[]> getProcessDynamicMemoryRegions(IntPtr ProcessHandle)
		{
			long MaxAddress = 0x7fffffffL;
			long CurrentAddress = 0L;

			List<uint[]> ReturnVal = new List<uint[]>();

			do
			{
				Imports.MEMORY_BASIC_INFORMATION lpBuffer = new Imports.MEMORY_BASIC_INFORMATION();
				bool flag = Imports.VirtualQueryEx(ProcessHandle, (IntPtr)CurrentAddress, out lpBuffer, (uint)Marshal.SizeOf(lpBuffer));

				if (flag && lpBuffer.State == 0x1000 && (uint)lpBuffer.RegionSize > 0 
					&& lpBuffer.Protect == (uint) Imports.MemoryProtection.ReadWrite && lpBuffer.Type == 0x20000)
				{
					uint[] temp = new uint[2];
					temp[0] = (uint)lpBuffer.BaseAddress;
					temp[1] = (uint)lpBuffer.RegionSize;
					ReturnVal.Add(temp);
				}
				CurrentAddress = lpBuffer.BaseAddress.ToInt64() + lpBuffer.RegionSize.ToInt64();
			}
			while (CurrentAddress <= MaxAddress);

			return ReturnVal;
		}

		/*public static object RawDeserialize(byte[] rawData, int position, Type anyType)
		{
			int cb = Marshal.SizeOf(anyType);
			if (cb > rawData.Length)
			{
				return null;
			}
			IntPtr destination = Marshal.AllocHGlobal(cb);
			Marshal.Copy(rawData, position, destination, cb);
			object obj2 = Marshal.PtrToStructure(destination, anyType);
			Marshal.FreeHGlobal(destination);
			return obj2;
		}*/

		public static unsafe object RawDeserialize(byte[] rawData, int position, Type anyType)
		{
			int cb = Marshal.SizeOf(anyType);
			if (cb > rawData.Length)
			{
				return null;
			}
			object obj2;
			fixed(void* ptr = &rawData[0])
			{
				IntPtr destination = new IntPtr(ptr);
				obj2 = Marshal.PtrToStructure(destination, anyType);
			}
			return obj2;
		}

		/*public static byte[] RawSerialize(object anything)
		{
			int cb = Marshal.SizeOf(anything);
			IntPtr ptr = Marshal.AllocHGlobal(cb);
			Marshal.StructureToPtr(anything, ptr, false);
			byte[] destination = new byte[cb];
			Marshal.Copy(ptr, destination, 0, cb);
			Marshal.FreeHGlobal(ptr);
			return destination;
		}*/

		public static unsafe byte[] RawSerialize(object anything)
		{
			int cb = Marshal.SizeOf(anything);
			byte[] destination = new byte[cb];
			fixed (void* ptr = &destination[0])
			{
				IntPtr iptr = new IntPtr(ptr);
				Marshal.StructureToPtr(anything, iptr, false);
			}
			return destination;
		}

		public object ReadMemory(IntPtr address, Type type)
		{
			byte[] buffer;
			this.ReadMemory(address, Marshal.SizeOf(type), out buffer);
			return RawDeserialize(buffer, 0, type);
		}

		public object ReadMemory(uint address, Type type)
		{
			byte[] buffer;
			this.ReadMemory(address, Marshal.SizeOf(type), out buffer);
			return RawDeserialize(buffer, 0, type);
		}

		public bool ReadMemory(IntPtr memoryLocation, int bufferLength, out byte[] lpBuffer)
		{
			int lpBytesRead;
			lpBuffer = new byte[bufferLength];
			if (this.m_lpHandle.ToInt32() == 0)
			{
				return false;
			}
			if (!Imports.ReadProcessMemory(this.m_lpHandle, memoryLocation, lpBuffer, bufferLength, out lpBytesRead))
			{
				return false;
			}
			if (lpBytesRead != bufferLength)
			{
				return false;
			}
			return true;
		}

		public bool ReadMemory(uint memoryLocation, int bufferLength, out byte[] lpBuffer)
		{
			int lpBytesRead;
			lpBuffer = new byte[bufferLength];
			if (this.m_lpHandle.ToInt32() == 0)
			{
				return false;
			}
			try
			{
				UIntPtr lpAddress = (UIntPtr) memoryLocation;
				Imports.MEMORY_BASIC_INFORMATION memory_basic_information = new Imports.MEMORY_BASIC_INFORMATION();
				Imports.VirtualQueryEx(this.m_lpHandle, lpAddress, out memory_basic_information, (uint) bufferLength);
				if (!Imports.ReadProcessMemory(this.m_lpHandle, lpAddress, lpBuffer, bufferLength, out lpBytesRead))
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

		public bool ReadMemory(uint memoryLocation, int bufferLength, out int uintOut)
		{
			int lpBytesRead;
			uintOut = 0;
			if (this.m_lpHandle.ToInt32() == 0)
			{
				return false;
			}
			byte[] lpBuffer = new byte[4];
			if (!Imports.ReadProcessMemory(this.m_lpHandle, (IntPtr) memoryLocation, lpBuffer, bufferLength, out lpBytesRead))
			{
				return false;
			}
			uintOut = BitConverter.ToInt32(lpBuffer, 0);
			return true;
		}

		public bool ReadMemory(uint memoryLocation, int bufferLength, out uint uintOut)
		{
			int lpBytesRead;
			uintOut = 0;
			if (this.m_lpHandle.ToInt32() == 0)
			{
				return false;
			}
			byte[] lpBuffer = new byte[4];
			if (!Imports.ReadProcessMemory(this.m_lpHandle, (IntPtr) memoryLocation, lpBuffer, bufferLength, out lpBytesRead))
			{
				return false;
			}
			uintOut = BitConverter.ToUInt32(lpBuffer, 0);
			return true;
		}

		public bool WriteMemory(IntPtr memoryLocation, int bufferLength, ref byte[] lpBuffer)
		{
			int lpBytesWrote;
			if (this.m_lpHandle.ToInt32() == 0)
			{
				return false;
			}
			if (!Imports.WriteProcessMemory(this.m_lpHandle, memoryLocation, lpBuffer, bufferLength, out lpBytesWrote))
			{
				return false;
			}
			return true;
		}

		public bool WriteMemory(uint memoryLocation, int bufferLength, ref byte[] lpBuffer)
		{
			return this.WriteMemory((IntPtr) memoryLocation, bufferLength, ref lpBuffer);
		}
	}
}

