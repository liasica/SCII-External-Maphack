namespace Foole.Mpq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class MpqArchive : IDisposable, IEnumerable<MpqEntry>, IEnumerable
    {
        private MpqEntry[] _entries;
        private MpqHash[] _hashes;
        private long _headerOffset;
        private MpqHeader _mpqHeader;
        private static uint[] sStormBuffer = BuildStormBuffer();

        public MpqArchive(Stream sourceStream)
        {
            this.BaseStream = sourceStream;
            this.Init();
        }

        public MpqArchive(string filename)
        {
            //this.BaseStream = File.Open(filename, FileMode.Open, FileAccess.Read);
			this.BaseStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.Init();
        }

        public MpqArchive(Stream sourceStream, bool loadListfile)
        {
            this.BaseStream = sourceStream;
            this.Init();
            if (loadListfile)
            {
                this.AddListfileFilenames();
            }
        }

        public bool AddFilename(string filename)
        {
            MpqHash hash;
            if (!this.TryGetHashEntry(filename, out hash))
            {
                return false;
            }
            this._entries[hash.BlockIndex].Filename = filename;
            return true;
        }

        public void AddFilenames(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    this.AddFilename(reader.ReadLine());
                }
            }
        }

        public bool AddListfileFilenames()
        {
            if (!this.AddFilename("(listfile)"))
            {
                return false;
            }
            using (Stream stream = this.OpenFile("(listfile)"))
            {
                this.AddFilenames(stream);
            }
            return true;
        }

        private static uint[] BuildStormBuffer()
        {
            uint num = 0x100001;
            uint[] numArray = new uint[0x500];
            for (uint i = 0; i < 0x100; i++)
            {
                uint index = i;
                int num4 = 0;
                while (num4 < 5)
                {
                    num = ((num * 0x7d) + 3) % 0x2aaaab;
                    uint num5 = (uint) ((num & 0xffff) << 0x10);
                    num = ((num * 0x7d) + 3) % 0x2aaaab;
                    numArray[index] = num5 | (num & 0xffff);
                    num4++;
                    index += 0x100;
                }
            }
            return numArray;
        }

        internal static void DecryptBlock(byte[] data, uint seed1)
        {
            uint num = 0xeeeeeeee;
            for (int i = 0; i < (data.Length - 3); i += 4)
            {
                num += sStormBuffer[(int) ((IntPtr) (0x400 + (seed1 & 0xff)))];
                uint num3 = BitConverter.ToUInt32(data, i) ^ (seed1 + num);
                seed1 = ((~seed1 << 0x15) + 0x11111111) | (seed1 >> 11);
                num = ((num3 + num) + (num << 5)) + 3;
                data[i] = (byte) (num3 & 0xff);
                data[i + 1] = (byte) ((num3 >> 8) & 0xff);
                data[i + 2] = (byte) ((num3 >> 0x10) & 0xff);
                data[i + 3] = (byte) ((num3 >> 0x18) & 0xff);
            }
        }

        internal static void DecryptBlock(uint[] data, uint seed1)
        {
            uint num = 0xeeeeeeee;
            for (int i = 0; i < data.Length; i++)
            {
                num += sStormBuffer[(int) ((IntPtr) (0x400 + (seed1 & 0xff)))];
                uint num3 = data[i];
                num3 ^= seed1 + num;
                seed1 = ((~seed1 << 0x15) + 0x11111111) | (seed1 >> 11);
                num = ((num3 + num) + (num << 5)) + 3;
                data[i] = num3;
            }
        }

        internal static void DecryptTable(byte[] data, string key)
        {
            DecryptBlock(data, HashString(key, 0x300));
        }

        internal static uint DetectFileSeed(uint value0, uint value1, uint decrypted)
        {
            uint num = (value0 ^ decrypted) - 0xeeeeeeee;
            for (int i = 0; i < 0x100; i++)
            {
                uint num3 = num - sStormBuffer[0x400 + i];
                uint num4 = 0xeeeeeeee + sStormBuffer[(int) ((IntPtr) (0x400 + (num3 & 0xff)))];
                uint num5 = value0 ^ (num3 + num4);
                if (num5 == decrypted)
                {
                    uint num6 = num3;
                    num3 = ((~num3 << 0x15) + 0x11111111) | (num3 >> 11);
                    num4 = ((num5 + num4) + (num4 << 5)) + 3;
                    num4 += sStormBuffer[(int) ((IntPtr) (0x400 + (num3 & 0xff)))];
                    num5 = value1 ^ (num3 + num4);
                    if ((num5 & 0xfffc0000) == 0)
                    {
                        return num6;
                    }
                }
            }
            return 0;
        }

        public void Dispose()
        {
            if (this.BaseStream != null)
            {
                this.BaseStream.Close();
            }
        }

        public bool FileExists(string filename)
        {
            MpqHash hash;
            return this.TryGetHashEntry(filename, out hash);
        }

        internal static uint HashString(string input, int offset)
        {
            uint num = 0x7fed7fed;
            uint num2 = 0xeeeeeeee;
            foreach (char ch in input)
            {
                int num3 = char.ToUpper(ch);
				if (num3 >= 256)
					num3 = 255;
                num = sStormBuffer[offset + num3] ^ (num + num2);
                num2 = (uint) ((((num3 + num) + num2) + (num2 << 5)) + 3);
            }
            return num;
        }

        private void Init()
        {
            if (!this.LocateMpqHeader())
            {
                throw new MpqParserException("Unable to find MPQ header");
            }
            if (((this._mpqHeader.HashTableOffsetHigh != 0) || (this._mpqHeader.ExtendedBlockTableOffset != 0L)) || (this._mpqHeader.BlockTableOffsetHigh != 0))
            {
                throw new MpqParserException("MPQ format version 1 features are not supported");
            }
            BinaryReader reader = new BinaryReader(this.BaseStream);
            this.BlockSize = ((int) 0x200) << this._mpqHeader.BlockSize;
            this.BaseStream.Seek((long) this._mpqHeader.HashTablePos, SeekOrigin.Begin);
            byte[] data = reader.ReadBytes((int) (this._mpqHeader.HashTableSize * MpqHash.Size));
            DecryptTable(data, "(hash table)");
            BinaryReader br = new BinaryReader(new MemoryStream(data));
            this._hashes = new MpqHash[this._mpqHeader.HashTableSize];
            for (int i = 0; i < this._mpqHeader.HashTableSize; i++)
            {
                this._hashes[i] = new MpqHash(br);
            }
            this.BaseStream.Seek((long) this._mpqHeader.BlockTablePos, SeekOrigin.Begin);
            byte[] buffer2 = reader.ReadBytes((int) (this._mpqHeader.BlockTableSize * MpqEntry.Size));
            DecryptTable(buffer2, "(block table)");
            br = new BinaryReader(new MemoryStream(buffer2));
            this._entries = new MpqEntry[this._mpqHeader.BlockTableSize];
            for (int j = 0; j < this._mpqHeader.BlockTableSize; j++)
            {
                this._entries[j] = new MpqEntry(br, (uint) this._headerOffset);
            }
        }

        private bool LocateMpqHeader()
        {
            BinaryReader br = new BinaryReader(this.BaseStream);
            for (long i = 0L; i < (this.BaseStream.Length - MpqHeader.Size); i += 0x200L)
            {
                this.BaseStream.Seek(i, SeekOrigin.Begin);
                this._mpqHeader = MpqHeader.FromReader(br);
                if (this._mpqHeader != null)
                {
                    this._headerOffset = i;
                    this._mpqHeader.SetHeaderOffset(this._headerOffset);
                    return true;
                }
            }
            return false;
        }

        public MpqStream OpenFile(MpqEntry entry)
        {
            return new MpqStream(this, entry);
        }

        public MpqStream OpenFile(string filename)
        {
            MpqHash hash;
            if (!this.TryGetHashEntry(filename, out hash))
            {
                throw new FileNotFoundException("File not found: " + filename);
            }
            MpqEntry entry = this._entries[hash.BlockIndex];
            if (entry.Filename == null)
            {
                entry.Filename = filename;
            }
            return new MpqStream(this, entry);
        }

        IEnumerator<MpqEntry> IEnumerable<MpqEntry>.GetEnumerator()
        {
            foreach (MpqEntry iteratorVariable0 in this._entries)
            {
                yield return iteratorVariable0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._entries.GetEnumerator();
        }

        private bool TryGetHashEntry(string filename, out MpqHash hash)
        {
            uint num = HashString(filename, 0) & (this._mpqHeader.HashTableSize - 1);
            uint num2 = HashString(filename, 0x100);
            uint num3 = HashString(filename, 0x200);
            for (uint i = num; i < this._hashes.Length; i++)
            {
                hash = this._hashes[i];
                if ((hash.Name1 == num2) && (hash.Name2 == num3))
                {
                    return true;
                }
            }
            for (uint j = 0; j < num; j++)
            {
                hash = this._hashes[j];
                if ((hash.Name1 == num2) && (hash.Name2 == num3))
                {
                    return true;
                }
            }
            hash = new MpqHash();
            return false;
        }

        internal Stream BaseStream { get; private set; }

        internal int BlockSize { get; private set; }

        public int Count
        {
            get
            {
                return this._entries.Length;
            }
        }

        public MpqHeader Header
        {
            get
            {
                return this._mpqHeader;
            }
        }

        public MpqEntry this[int index]
        {
            get
            {
                return this._entries[index];
            }
        }

        public MpqEntry this[string filename]
        {
            get
            {
                MpqHash hash;
                if (!this.TryGetHashEntry(filename, out hash))
                {
                    return null;
                }
                return this._entries[hash.BlockIndex];
            }
        }

        /*[CompilerGenerated]
        private sealed class GetEnumerator>d__0 : IEnumerator<MpqEntry>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private MpqEntry <>2__current;
            public MpqArchive <>4__this;
            public MpqEntry[] <>7__wrap3;
            public int <>7__wrap4;
            public MpqEntry <entry>5__1;

            [DebuggerHidden]
            public GetEnumerator>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
            }

            private void <>m__Finally2()
            {
                this.<>1__state = -1;
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>1__state = 1;
                            this.<>7__wrap3 = this.<>4__this._entries;
                            this.<>7__wrap4 = 0;
                            goto Label_0082;

                        case 2:
                            this.<>1__state = 1;
                            this.<>7__wrap4++;
                            goto Label_0082;

                        default:
                            goto Label_0098;
                    }
                Label_0043:
                    this.<entry>5__1 = this.<>7__wrap3[this.<>7__wrap4];
                    this.<>2__current = this.<entry>5__1;
                    this.<>1__state = 2;
                    return true;
                Label_0082:
                    if (this.<>7__wrap4 < this.<>7__wrap3.Length)
                    {
                        goto Label_0043;
                    }
                    this.<>m__Finally2();
                Label_0098:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                        this.<>m__Finally2();
                        return;
                }
            }

            MpqEntry IEnumerator<MpqEntry>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }*/
    }
}

