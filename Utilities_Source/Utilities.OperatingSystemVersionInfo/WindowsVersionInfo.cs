namespace Utilities.OperatingSystemVersionInfo
{
	using System;
	using System.Runtime.InteropServices;

	public class WindowsVersionInfo
	{
		private string GetOSVersionInfo()
		{
			string str = "Unsupported Version";
			Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx osvi = new Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx {
				VersionInfoSize = Marshal.SizeOf(typeof(Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx))
			};
			Utilities.OperatingSystemVersionInfo.NativeMethods.GetVersionEx(ref osvi);
			if ((2 != osvi.PlatformId) || (osvi.MajorVersion <= 4))
			{
				return str;
			}
			if ((osvi.MajorVersion == 5) && (osvi.MinorVersion == 0))
			{
				str = "Windows 2000";
			}
			if ((osvi.MajorVersion == 5) && (osvi.MinorVersion == 1))
			{
				str = "Windows XP";
			}
			if ((osvi.MajorVersion == 5) && (osvi.MinorVersion == 2))
			{
				if (osvi.ProductType == 1)
				{
					str = "Windows XP Professional x64";
				}
				else
				{
					str = "Windows Server 2003";
					if (Utilities.OperatingSystemVersionInfo.NativeMethods.GetSystemMetrics(0x59) != 0)
					{
						str = str + " R2";
					}
				}
			}
			if ((osvi.MajorVersion == 6) && (osvi.MinorVersion == 0))
			{
				if (osvi.ProductType == 1)
				{
					str = "Windows Vista";
				}
				else
				{
					str = "Windows Server 2008";
				}
			}
			if ((osvi.MajorVersion != 6) || (osvi.MinorVersion != 1))
			{
				return str;
			}
			if (osvi.ProductType == 1)
			{
				return "Windows 7";
			}
			return "Windows Server 2008 R2";
		}

		private string GetProductTypeInfo()
		{
			string str = string.Empty;
			Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx osvi = new Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx {
				VersionInfoSize = Marshal.SizeOf(typeof(Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx))
			};
			Utilities.OperatingSystemVersionInfo.NativeMethods.GetVersionEx(ref osvi);
			if (osvi.MajorVersion > 5)
			{
				uint type = 0;
				Utilities.OperatingSystemVersionInfo.NativeMethods.GetProductInfo(osvi.MajorVersion, osvi.MinorVersion, osvi.ServicePackMajor, osvi.ServicePackMinor, ref type);
				switch (type)
				{
					case 0:
						return "Unknown Product";

					case 1:
						return "Ultimate Edition";

					case 2:
						return "Home Basic Edition";

					case 3:
						return "Home Premium Edition";

					case 4:
						return "Enterprise Edition";

					case 5:
						return "Home Basic N Edition";

					case 6:
						return "Business Edition";

					case 7:
						return "Server Standard (Full)";

					case 8:
						return "Server Datacenter (Full)";

					case 9:
						return "Windows Small Business Server";

					case 10:
						return "Server Enterprise (Full)";

					case 11:
						return "Starter Edition";

					case 12:
						return "Server Datacenter (Core)";

					case 13:
						return "Server Standard (Core)";

					case 14:
						return "Server Enterprise (Core)";

					case 15:
						return "Server Enterprise for Itanium-based Systems";

					case 0x10:
						return "Business N Edition";

					case 0x11:
						return "Web Server (Full)";

					case 0x12:
						return "HPC Edition";

					case 0x13:
						return "Home Server Edition";

					case 20:
						return "Storage Server Express";

					case 0x15:
						return "Storage Server Standard";

					case 0x16:
						return "Storage Server Workgroup";

					case 0x17:
						return "Storage Server Enterprise";

					case 0x18:
						return "Windows Server 2008 for Windows Essential Server Solutions";

					case 0x19:
						return "Windows Small Busines Server Premium";

					case 0x1a:
						return "Home Premium N Edition";

					case 0x1b:
						return "Enterprise N Edition";

					case 0x1c:
						return "Ulitmate N Edition";

					case 0x1d:
						return "Web Server (Core)";

					case 30:
						return "Windows Essential Business Server Management Server";

					case 0x1f:
						return "Windows Essential Business Server Security Server";

					case 0x20:
						return "Windows Essential Business Server Messaging Server";

					case 0x21:
						return "Server Foundation";

					case 0x22:
					case 0x2b:
					case 0x2c:
					case 0x2d:
					case 0x2e:
					case 50:
					case 0x33:
					case 0x34:
					case 0x35:
					case 0x36:
					case 0x37:
					case 0x38:
					case 0x39:
					case 0x3a:
					case 0x3b:
					case 60:
					case 0x3d:
					case 0x3e:
					case 0x3f:
					case 0x40:
					case 0x41:
						return str;

					case 0x23:
						return "Windows Server 2008 without Hyper-V for Windows Essential Server Solutions";

					case 0x24:
						return "Server Standard without Hyper-V (Full)";

					case 0x25:
						return "Server Datacenter without Hyper-V (Full)";

					case 0x26:
						return "Server Enterprise without Hyper-V (Full)";

					case 0x27:
						return "Server Datacenter without Hyper-V (Core)";

					case 40:
						return "Server Standard without Hyper-V (Core)";

					case 0x29:
						return "Server Enterprise without Hyper-V (Core)";

					case 0x2a:
						return "Microsoft Hyper-V Server";

					case 0x2f:
						return "Starter N Edition";

					case 0x30:
						return "Professional Edition";

					case 0x31:
						return "Professional N Edition";

					case 0x42:
						return "Starter E Edition";

					case 0x43:
						return "Home Basic E Edition";

					case 0x44:
						return "Home Premium E Edition";

					case 0x45:
						return "Professional E Edition";

					case 70:
						return "Enterprise E Edition";

					case 0x47:
						return "Ultimate E Edition";

					case 0xabcdabcd:
						return "Unlicensed or Expired";
				}
			}
			return str;
		}

		private string GetServicePackInfo()
		{
			Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx osvi = new Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx {
				VersionInfoSize = Marshal.SizeOf(typeof(Utilities.OperatingSystemVersionInfo.NativeMethods.OSVersionInfoEx))
			};
			Utilities.OperatingSystemVersionInfo.NativeMethods.GetVersionEx(ref osvi);
			return osvi.CSDVersion;
		}

		public string GetOSVersion
		{
			get
			{
				return this.GetOSVersionInfo();
			}
		}

		public string GetProductType
		{
			get
			{
				return this.GetProductTypeInfo();
			}
		}

		public string GetServicePack
		{
			get
			{
				return this.GetServicePackInfo();
			}
		}
	}
}

