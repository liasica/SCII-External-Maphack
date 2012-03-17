namespace Foole.Mpq
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class MpqHeader
    {
        public static readonly uint MpqId = 0x1a51504d;
        public static readonly uint Size = 0x20;

        public static MpqHeader FromReader(BinaryReader br)
        {
            uint num = br.ReadUInt32();
            if (num != MpqId)
            {
                return null;
            }
            MpqHeader header2 = new MpqHeader {
                ID = num,
                DataOffset = br.ReadUInt32(),
                ArchiveSize = br.ReadUInt32(),
                MpqVersion = br.ReadUInt16(),
                BlockSize = br.ReadUInt16(),
                HashTablePos = br.ReadUInt32(),
                BlockTablePos = br.ReadUInt32(),
                HashTableSize = br.ReadUInt32(),
                BlockTableSize = br.ReadUInt32()
            };
            MpqHeader header = header2;
            if (header.MpqVersion == 1)
            {
                header.ExtendedBlockTableOffset = br.ReadInt64();
                header.HashTableOffsetHigh = br.ReadInt16();
                header.BlockTableOffsetHigh = br.ReadInt16();
            }
            return header;
        }

        public void SetHeaderOffset(long headerOffset)
        {
            this.HashTablePos += (uint) headerOffset;
            this.BlockTablePos += (uint) headerOffset;
            if (this.DataOffset == 0x6d9e4b86)
            {
                this.DataOffset = Size + ((uint) headerOffset);
            }
        }

        public uint ArchiveSize { get; private set; }

        public ushort BlockSize { get; private set; }

        public short BlockTableOffsetHigh { get; private set; }

        public uint BlockTablePos { get; private set; }

        public uint BlockTableSize { get; private set; }

        public uint DataOffset { get; private set; }

        public long ExtendedBlockTableOffset { get; private set; }

        public short HashTableOffsetHigh { get; private set; }

        public uint HashTablePos { get; private set; }

        public uint HashTableSize { get; private set; }

        public uint ID { get; private set; }

        public ushort MpqVersion { get; private set; }
    }
}

