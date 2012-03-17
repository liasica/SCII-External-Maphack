namespace Utilities.Paloma
{
	using System;

	internal static class TargaConstants
	{
		internal const int ExtensionAreaAuthorCommentsByteLength = 0x144;
		internal const int ExtensionAreaAuthorNameByteLength = 0x29;
		internal const int ExtensionAreaColorCorrectionTableValueLength = 0x100;
		internal const int ExtensionAreaJobNameByteLength = 0x29;
		internal const int ExtensionAreaSoftwareIDByteLength = 0x29;
		internal const int ExtensionAreaSoftwareVersionLetterByteLength = 1;
		internal const int FooterByteLength = 0x1a;
		internal const int FooterReservedCharByteLength = 1;
		internal const int FooterSignatureByteLength = 0x10;
		internal const int FooterSignatureOffsetFromEnd = 0x12;
		internal const int HeaderByteLength = 0x12;
		internal const string TargaFooterASCIISignature = "TRUEVISION-XFILE";
	}
}

