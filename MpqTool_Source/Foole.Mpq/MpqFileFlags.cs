namespace Foole.Mpq
{
    using System;

    [Flags]
    public enum MpqFileFlags : uint
    {
        BlockOffsetAdjustedKey = 0x20000,
        Compressed = 0xff00,
        CompressedMulti = 0x200,
        CompressedPK = 0x100,
        Encrypted = 0x10000,
        Exists = 0x80000000,
        FileHasMetadata = 0x4000000,
        SingleUnit = 0x1000000
    }
}

