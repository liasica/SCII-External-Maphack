namespace Foole.Mpq
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class MpqEntry
    {
        private string _filename;
        private uint _fileOffset;
        public static readonly uint Size = 0x10;

        public MpqEntry(BinaryReader br, uint headerOffset)
        {
            this._fileOffset = br.ReadUInt32();
            this.FilePos = headerOffset + this._fileOffset;
            this.CompressedSize = br.ReadUInt32();
            this.FileSize = br.ReadUInt32();
            this.Flags = (MpqFileFlags) br.ReadUInt32();
            this.EncryptionSeed = 0;
        }

        private uint CalculateEncryptionSeed()
        {
            if (this.Filename == null)
            {
                return 0;
            }
            uint num = MpqArchive.HashString(Path.GetFileName(this.Filename), 0x300);
            if ((this.Flags & MpqFileFlags.BlockOffsetAdjustedKey) == MpqFileFlags.BlockOffsetAdjustedKey)
            {
                num = (num + this._fileOffset) ^ this.FileSize;
            }
            return num;
        }

        public override string ToString()
        {
            if (this.Filename != null)
            {
                return this.Filename;
            }
            if (!this.Exists)
            {
                return "(Deleted file)";
            }
            return string.Format("Unknown file @ {0}", this.FilePos);
        }

        public uint CompressedSize { get; private set; }

        public uint EncryptionSeed { get; internal set; }

        public bool Exists
        {
            get
            {
                return (this.Flags != ((MpqFileFlags) 0));
            }
        }

        public string Filename
        {
            get
            {
                return this._filename;
            }
            set
            {
                this._filename = value;
                this.EncryptionSeed = this.CalculateEncryptionSeed();
            }
        }

        internal uint FilePos { get; private set; }

        public uint FileSize { get; private set; }

        public MpqFileFlags Flags { get; internal set; }

        public int FlagsAsInt
        {
            get
            {
                return (int) this.Flags;
            }
        }

        public bool IsCompressed
        {
            get
            {
                return ((this.Flags & ((MpqFileFlags) 0xff00)) != ((MpqFileFlags) 0));
            }
        }

        public bool IsEncrypted
        {
            get
            {
                return ((this.Flags & MpqFileFlags.Encrypted) != ((MpqFileFlags) 0));
            }
        }

        public bool IsSingleUnit
        {
            get
            {
                return ((this.Flags & MpqFileFlags.SingleUnit) != ((MpqFileFlags) 0));
            }
        }
    }
}

