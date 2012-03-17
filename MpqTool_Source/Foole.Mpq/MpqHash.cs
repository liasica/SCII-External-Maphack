namespace Foole.Mpq
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MpqHash
    {
        public static readonly uint Size;
        public uint Name1 { get; private set; }
        public uint Name2 { get; private set; }
        public uint Locale { get; private set; }
        public uint BlockIndex { get; private set; }
        public MpqHash(BinaryReader br)
        {
            this = new MpqHash();
            this.Name1 = br.ReadUInt32();
            this.Name2 = br.ReadUInt32();
            this.Locale = br.ReadUInt32();
            this.BlockIndex = br.ReadUInt32();
        }

        static MpqHash()
        {
            Size = 0x10;
        }
    }
}

