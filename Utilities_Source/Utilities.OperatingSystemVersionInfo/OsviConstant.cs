namespace Utilities.OperatingSystemVersionInfo
{
	using System;

	internal class OsviConstant
	{
		internal const int ServerR2 = 0x59;
		internal const int SupportedPlatform = 2;
		internal const int WorkStation = 1;

		private OsviConstant()
		{
		}

		internal enum MajorVersion
		{
			NT5 = 5,
			NT6 = 6
		}

		internal enum MinorVersion
		{
			Windows2000 = 0,
			Windows7 = 1,
			WindowsServer2003 = 2,
			WindowsVista = 0,
			WindowsXP = 1
		}

		internal enum ProductInfo : uint
		{
			Business = 6,
			BusinessN = 0x10,
			ClusterServer = 0x12,
			DatacenterServer = 8,
			DatacenterServerCore = 12,
			DataCenterServerCoreV = 0x27,
			DataCenterServerV = 0x25,
			Enterprise = 4,
			EnterpriseE = 70,
			EnterpriseN = 0x1b,
			EnterpriseServer = 10,
			EnterpriseServerCore = 14,
			EnterpriseServerCoreV = 0x29,
			EnterpriseServerIA64 = 15,
			EnterpriseServerV = 0x26,
			HomeBasic = 2,
			HomeBasicE = 0x43,
			HomeBasicN = 5,
			HomePremium = 3,
			HomePremiumE = 0x44,
			HomePremiumN = 0x1a,
			HomeServer = 0x13,
			HyperV = 0x2a,
			MediumBusinessServerManagement = 30,
			MediumBusinessServerMessaging = 0x20,
			MediumBusinessServerSecurity = 0x1f,
			Professional = 0x30,
			ProfessionalE = 0x45,
			ProfessionalN = 0x31,
			ServerForSmallBusiness = 0x18,
			ServerForSmallBusinessV = 0x23,
			ServerFoundation = 0x21,
			SmallBusinessServer = 9,
			SmallBusinessServerPremium = 0x19,
			StandardServer = 7,
			StandardServerCore = 13,
			StandardServerCoreV = 40,
			StandardServerV = 0x24,
			Starter = 11,
			StarterE = 0x42,
			StarterN = 0x2f,
			StorageEnterpriseServer = 0x17,
			StorageExpressServer = 20,
			StorageStandardServer = 0x15,
			StorageWorkgroupServer = 0x16,
			Ultimate = 1,
			UltimateE = 0x47,
			UltimateN = 0x1c,
			Undefined = 0,
			Unlicensed = 0xabcdabcd,
			WebServer = 0x11,
			WebServerCore = 0x1d
		}
	}
}

