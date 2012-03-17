namespace ICSharpCode.SharpZipLib.Zip
{
    using System;

    [Flags]
    internal enum GeneralBitFlags
    {
        Descriptor = 8,
        Encrypted = 1,
        EnhancedCompress = 0x1000,
        HeaderMasked = 0x2000,
        Method = 6,
        Patched = 0x20,
        Reserved = 0x10,
        StrongEncryption = 0x40
    }
}

