namespace Utilities.ProcessTools
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.DirectoryServices;
	using System.IO;
	using System.Management;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Threading;

	public static class PT
	{
		public static bool isAdmin(string processName)
		{
			string str = "";
			string str2 = "";
			ObjectQuery query = new ObjectQuery("Select * From Win32_Process where Name='" + processName + ".exe'");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
			foreach (ManagementObject obj2 in searcher.Get())
			{
				string[] strArray = new string[2];
				obj2.InvokeMethod("GetOwner", (object[]) strArray);
				str = strArray[0].ToString();
				str2 = strArray[1].ToString();
				break;
			}
			string str3 = str2 + "/" + str;
			using (DirectoryEntry entry = new DirectoryEntry("WinNT://./Administrators,group"))
			{
				foreach (object obj3 in (System.Collections.IEnumerable) entry.Invoke("Members", new object[0]))
				{
					using (DirectoryEntry entry2 = new DirectoryEntry(obj3))
					{
						Console.WriteLine(entry2.Path);
						if (entry2.Path.Contains(str3))
						{
							return true;
						}
						continue;
					}
				}
			}
			return false;
		}

		public class CpuUsage
		{
			private short _cpuUsage = -1;
			private DateTime _lastRun = DateTime.MinValue;
			private TimeSpan _prevProcTotal;
			private System.Runtime.InteropServices.ComTypes.FILETIME _prevSysKernel;
			private System.Runtime.InteropServices.ComTypes.FILETIME _prevSysUser;
			private long _runCount;

			public CpuUsage()
			{
				this._prevSysUser.dwHighDateTime = this._prevSysUser.dwLowDateTime = 0;
				this._prevSysKernel.dwHighDateTime = this._prevSysKernel.dwLowDateTime = 0;
				this._prevProcTotal = TimeSpan.MinValue;
				this._runCount = 0L;
			}

			[DllImport("kernel32.dll", SetLastError=true)]
			private static extern bool GetSystemTimes(out System.Runtime.InteropServices.ComTypes.FILETIME lpIdleTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime);
			public short GetUsage()
			{
				short num = this._cpuUsage;
				if (Interlocked.Increment(ref this._runCount) == 1L)
				{
					System.Runtime.InteropServices.ComTypes.FILETIME filetime;
					System.Runtime.InteropServices.ComTypes.FILETIME filetime2;
					System.Runtime.InteropServices.ComTypes.FILETIME filetime3;
					if (!this.EnoughTimePassed)
					{
						Interlocked.Decrement(ref this._runCount);
						return num;
					}
					TimeSpan totalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
					if (!GetSystemTimes(out filetime, out filetime2, out filetime3))
					{
						Interlocked.Decrement(ref this._runCount);
						return num;
					}
					if (!this.IsFirstRun)
					{
						ulong num2 = this.SubtractTimes(filetime2, this._prevSysKernel);
						ulong num3 = this.SubtractTimes(filetime3, this._prevSysUser);
						ulong num4 = num2 + num3;
						long num5 = totalProcessorTime.Ticks - this._prevProcTotal.Ticks;
						if (num4 > 0L)
						{
							this._cpuUsage = (short) ((100.0 * num5) / ((double) num4));
						}
					}
					this._prevProcTotal = totalProcessorTime;
					this._prevSysKernel = filetime2;
					this._prevSysUser = filetime3;
					this._lastRun = DateTime.Now;
					num = this._cpuUsage;
				}
				Interlocked.Decrement(ref this._runCount);
				return num;
			}

			public bool IsFileOpenOrReadOnly(string file)
			{
				try
				{
					if ((File.GetAttributes(file) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
					{
						using (FileStream stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
						{
							try
							{
								stream.ReadByte();
								return false;
							}
							catch (IOException)
							{
								return true;
							}
							finally
							{
								stream.Close();
								stream.Dispose();
							}
						}
					}
					return true;
				}
				catch (IOException)
				{
					return true;
				}
			}

			private ulong SubtractTimes(System.Runtime.InteropServices.ComTypes.FILETIME a, System.Runtime.InteropServices.ComTypes.FILETIME b)
			{
				ulong num = (ulong) (a.dwHighDateTime | a.dwLowDateTime);
				ulong num2 = (ulong) (b.dwHighDateTime | b.dwLowDateTime);
				return (num - num2);
			}

			private bool EnoughTimePassed
			{
				get
				{
					TimeSpan span = (TimeSpan) (DateTime.Now - this._lastRun);
					return (span.TotalMilliseconds > 250.0);
				}
			}

			private bool IsFirstRun
			{
				get
				{
					return (this._lastRun == DateTime.MinValue);
				}
			}
		}
	}
}

