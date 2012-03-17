namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Encryption;
    using ICSharpCode.SharpZipLib.Zip.Compression;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    public class ZipFile : IEnumerable
    {
        private Stream baseStream;
        private string comment;
        private ZipEntry[] entries;
        private bool isStreamOwner;
        private byte[] iv;
        private byte[] key;
        public KeysRequiredEventHandler KeysRequired;
        private string name;
        private long offsetOfFirstEntry;

        public ZipFile(FileStream file)
        {
            this.isStreamOwner = true;
            this.offsetOfFirstEntry = 0L;
            this.key = null;
            this.iv = null;
            this.baseStream = file;
            this.name = file.Name;
            try
            {
                this.ReadEntries();
            }
            catch
            {
                this.Close();
                throw;
            }
        }

        public ZipFile(Stream baseStream)
        {
            this.isStreamOwner = true;
            this.offsetOfFirstEntry = 0L;
            this.key = null;
            this.iv = null;
            this.baseStream = baseStream;
            this.name = null;
            try
            {
                this.ReadEntries();
            }
            catch
            {
                this.Close();
                throw;
            }
        }

        public ZipFile(string name)
        {
            this.isStreamOwner = true;
            this.offsetOfFirstEntry = 0L;
            this.key = null;
            this.iv = null;
            this.name = name;
            this.baseStream = File.OpenRead(name);
            try
            {
                this.ReadEntries();
            }
            catch
            {
                this.Close();
                throw;
            }
        }

        private void CheckClassicPassword(CryptoStream classicCryptoStream, ZipEntry entry)
        {
            byte[] outBuf = new byte[12];
            this.ReadFully(classicCryptoStream, outBuf);
            if ((entry.Flags & 8) == 0)
            {
                if (outBuf[11] != ((byte) (entry.Crc >> 0x18)))
                {
                    throw new ZipException("Invalid password");
                }
            }
            else if (outBuf[11] != ((byte) ((entry.DosTime >> 8) & 0xffL)))
            {
                throw new ZipException("Invalid password");
            }
        }

        private long CheckLocalHeader(ZipEntry entry)
        {
            return this.TestLocalHeader(entry, false, true);
        }

        public void Close()
        {
            this.entries = null;
            if (this.isStreamOwner)
            {
                lock (this.baseStream)
                {
                    this.baseStream.Close();
                }
            }
        }

        private Stream CreateAndInitDecryptionStream(Stream baseStream, ZipEntry entry)
        {
            CryptoStream classicCryptoStream = null;
            if ((entry.Version >= 50) && ((entry.Flags & 0x40) != 0))
            {
                throw new ZipException("Decryption method not supported");
            }
            PkzipClassicManaged managed = new PkzipClassicManaged();
            this.OnKeysRequired(entry.Name);
            if (!this.HaveKeys)
            {
                throw new ZipException("No password available for encrypted stream");
            }
            classicCryptoStream = new CryptoStream(baseStream, managed.CreateDecryptor(this.key, this.iv), CryptoStreamMode.Read);
            this.CheckClassicPassword(classicCryptoStream, entry);
            return classicCryptoStream;
        }

        private Stream CreateAndInitEncryptionStream(Stream baseStream, ZipEntry entry)
        {
            CryptoStream stream = null;
            if ((entry.Version < 50) || ((entry.Flags & 0x40) == 0))
            {
                PkzipClassicManaged managed = new PkzipClassicManaged();
                this.OnKeysRequired(entry.Name);
                if (!this.HaveKeys)
                {
                    throw new ZipException("No password available for encrypted stream");
                }
                stream = new CryptoStream(baseStream, managed.CreateEncryptor(this.key, this.iv), CryptoStreamMode.Write);
                if ((entry.Crc < 0L) || ((entry.Flags & 8) != 0))
                {
                    this.WriteEncryptionHeader(stream, entry.DosTime << 0x10);
                    return stream;
                }
                this.WriteEncryptionHeader(stream, entry.Crc);
            }
            return stream;
        }

        public int FindEntry(string name, bool ignoreCase)
        {
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipFile has been closed");
            }
            for (int i = 0; i < this.entries.Length; i++)
            {
                if (string.Compare(name, this.entries[i].Name, ignoreCase) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public ZipEntry GetEntry(string name)
        {
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipFile has been closed");
            }
            int index = this.FindEntry(name, true);
            if (index < 0)
            {
                return null;
            }
            return (ZipEntry) this.entries[index].Clone();
        }

        public IEnumerator GetEnumerator()
        {
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipFile has closed");
            }
            return new ZipEntryEnumeration(this.entries);
        }

        public Stream GetInputStream(ZipEntry entry)
        {
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipFile has closed");
            }
            int zipFileIndex = entry.ZipFileIndex;
            if (((zipFileIndex < 0) || (zipFileIndex >= this.entries.Length)) || (this.entries[zipFileIndex].Name != entry.Name))
            {
                zipFileIndex = this.FindEntry(entry.Name, true);
                if (zipFileIndex < 0)
                {
                    throw new IndexOutOfRangeException();
                }
            }
            return this.GetInputStream(zipFileIndex);
        }

        public Stream GetInputStream(int entryIndex)
        {
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipFile has closed");
            }
            long start = this.CheckLocalHeader(this.entries[entryIndex]);
            CompressionMethod compressionMethod = this.entries[entryIndex].CompressionMethod;
            Stream baseStream = new PartialInputStream(this.baseStream, start, this.entries[entryIndex].CompressedSize);
            if (this.entries[entryIndex].IsCrypted)
            {
                baseStream = this.CreateAndInitDecryptionStream(baseStream, this.entries[entryIndex]);
                if (baseStream == null)
                {
                    throw new ZipException("Unable to decrypt this entry");
                }
            }
            CompressionMethod method2 = compressionMethod;
            if (method2 != CompressionMethod.Stored)
            {
                if (method2 != CompressionMethod.Deflated)
                {
                    throw new ZipException("Unsupported compression method " + compressionMethod);
                }
                return new InflaterInputStream(baseStream, new Inflater(true));
            }
            return baseStream;
        }

        private Stream GetOutputStream(ZipEntry entry, string fileName)
        {
            this.baseStream.Seek(0L, SeekOrigin.End);
            Stream baseStream = File.OpenWrite(fileName);
            if (entry.IsCrypted)
            {
                baseStream = this.CreateAndInitEncryptionStream(baseStream, entry);
            }
            switch (entry.CompressionMethod)
            {
                case CompressionMethod.Stored:
                    return baseStream;

                case CompressionMethod.Deflated:
                    return new DeflaterOutputStream(baseStream);
            }
            throw new ZipException("Unknown compression method " + entry.CompressionMethod);
        }

        private long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
        {
            long offset = endLocation - minimumBlockSize;
            if (offset < 0L)
            {
                return -1L;
            }
            long num2 = Math.Max((long) (offset - maximumVariableData), (long) 0L);
            do
            {
                if (offset < num2)
                {
                    return -1L;
                }
                offset -= 1L;
                this.baseStream.Seek(offset, SeekOrigin.Begin);
            }
            while (this.ReadLeInt() != signature);
            return this.baseStream.Position;
        }

        private void OnKeysRequired(string fileName)
        {
            if (this.KeysRequired != null)
            {
                KeysRequiredEventArgs e = new KeysRequiredEventArgs(fileName, this.key);
                this.KeysRequired(this, e);
                this.key = e.Key;
            }
        }

        private void ReadEntries()
        {
            if (!this.baseStream.CanSeek)
            {
                throw new ZipException("ZipFile stream must be seekable");
            }
            long num = this.LocateBlockWithSignature(0x6054b50, this.baseStream.Length, 0x16, 0xffff);
            if (num < 0L)
            {
                throw new ZipException("Cannot find central directory");
            }
            this.ReadLeShort();
            this.ReadLeShort();
            int num2 = this.ReadLeShort();
            int num3 = this.ReadLeShort();
            int num4 = this.ReadLeInt();
            int num5 = this.ReadLeInt();
            byte[] buffer = new byte[this.ReadLeShort()];
            this.baseStream.Read(buffer, 0, buffer.Length);
            this.comment = ZipConstants.ConvertToString(buffer);
            this.entries = new ZipEntry[num3];
            if (num5 < (num - (4 + num4)))
            {
                this.offsetOfFirstEntry = num - ((4 + num4) + num5);
                if (this.offsetOfFirstEntry <= 0L)
                {
                    throw new ZipException("Invalid SFX file");
                }
            }
            this.baseStream.Seek(this.offsetOfFirstEntry + num5, SeekOrigin.Begin);
            for (int i = 0; i < num2; i++)
            {
                if (this.ReadLeInt() != 0x2014b50)
                {
                    throw new ZipException("Wrong Central Directory signature");
                }
                int madeByInfo = this.ReadLeShort();
                int versionRequiredToExtract = this.ReadLeShort();
                int num10 = this.ReadLeShort();
                int num11 = this.ReadLeShort();
                int num12 = this.ReadLeInt();
                int num13 = this.ReadLeInt();
                int num14 = this.ReadLeInt();
                int num15 = this.ReadLeInt();
                int num16 = this.ReadLeShort();
                int count = this.ReadLeShort();
                int num18 = this.ReadLeShort();
                this.ReadLeShort();
                this.ReadLeShort();
                int num19 = this.ReadLeInt();
                int num20 = this.ReadLeInt();
                byte[] buffer2 = new byte[Math.Max(num16, num18)];
                this.baseStream.Read(buffer2, 0, num16);
                ZipEntry entry = new ZipEntry(ZipConstants.ConvertToString(buffer2, num16), versionRequiredToExtract, madeByInfo) {
                    CompressionMethod = (CompressionMethod) num11,
                    Crc = num13 & ((long) 0xffffffffL),
                    Size = num15 & ((long) 0xffffffffL),
                    CompressedSize = num14 & ((long) 0xffffffffL),
                    Flags = num10,
                    DosTime = (long) ((ulong) num12)
                };
                if (count > 0)
                {
                    byte[] buffer3 = new byte[count];
                    this.baseStream.Read(buffer3, 0, count);
                    entry.ExtraData = buffer3;
                }
                if (num18 > 0)
                {
                    this.baseStream.Read(buffer2, 0, num18);
                    entry.Comment = ZipConstants.ConvertToString(buffer2, num18);
                }
                entry.ZipFileIndex = i;
                entry.Offset = num20;
                entry.ExternalFileAttributes = num19;
                this.entries[i] = entry;
            }
        }

        private void ReadFully(Stream s, byte[] outBuf)
        {
            int num3;
            int offset = 0;
            for (int i = outBuf.Length; i > 0; i -= num3)
            {
                num3 = s.Read(outBuf, offset, i);
                if (num3 <= 0)
                {
                    throw new ZipException("Unexpected EOF");
                }
                offset += num3;
            }
        }

        private int ReadLeInt()
        {
            return (this.ReadLeShort() | (this.ReadLeShort() << 0x10));
        }

        private int ReadLeShort()
        {
            return (this.baseStream.ReadByte() | (this.baseStream.ReadByte() << 8));
        }

        public bool TestArchive(bool testData)
        {
            bool flag = true;
            try
            {
                for (int i = 0; i < this.Size; i++)
                {
                    this.TestLocalHeader(this[i], true, true);
                    if (testData)
                    {
                        int num2;
                        Stream inputStream = this.GetInputStream(this[i]);
                        Crc32 crc = new Crc32();
                        byte[] buffer = new byte[0x1000];
                        while ((num2 = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            crc.Update(buffer, 0, num2);
                        }
                        if (this[i].Crc != crc.Value)
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        private long TestLocalHeader(ZipEntry entry, bool fullTest, bool extractTest)
        {
            lock (this.baseStream)
            {
                this.baseStream.Seek(this.offsetOfFirstEntry + entry.Offset, SeekOrigin.Begin);
                if (this.ReadLeInt() != 0x4034b50)
                {
                    throw new ZipException("Wrong local header signature");
                }
                short num = (short) this.ReadLeShort();
                if (extractTest && (num > 20))
                {
                    throw new ZipException(string.Format("Version required to extract this entry not supported ({0})", num));
                }
                short num2 = (short) this.ReadLeShort();
                if (extractTest && ((num2 & 0x3060) != 0))
                {
                    throw new ZipException("The library doesnt support the zip version required to extract this entry");
                }
                if (num2 != entry.Flags)
                {
                    throw new ZipException("Central header/local header flags mismatch");
                }
                if (entry.CompressionMethod != (CompressionMethod)this.ReadLeShort())
                {
                    throw new ZipException("Central header/local header compression method mismatch");
                }
                num = (short) this.ReadLeShort();
                num = (short) this.ReadLeShort();
                int num3 = this.ReadLeInt();
                if ((fullTest && ((num2 & 8) == 0)) && (num3 != ((int) entry.Crc)))
                {
                    throw new ZipException("Central header/local header crc mismatch");
                }
                num3 = this.ReadLeInt();
                num3 = this.ReadLeInt();
                int num4 = this.ReadLeShort();
                if (entry.Name.Length > num4)
                {
                    throw new ZipException("file name length mismatch");
                }
                int num5 = num4 + this.ReadLeShort();
                return (((this.offsetOfFirstEntry + entry.Offset) + 30L) + num5);
            }
        }

        private void WriteEncryptionHeader(Stream stream, long crcValue)
        {
            byte[] buffer = new byte[12];
            new Random().NextBytes(buffer);
            buffer[11] = (byte) (crcValue >> 0x18);
            stream.Write(buffer, 0, buffer.Length);
        }

        public ZipEntry this[int index]
        {
            get
            {
                return (ZipEntry) this.entries[index].Clone();
            }
        }

        private bool HaveKeys
        {
            get
            {
                return (this.key != null);
            }
        }

        private bool IsStreamOwner
        {
            get
            {
                return this.isStreamOwner;
            }
            set
            {
                this.isStreamOwner = value;
            }
        }

        private byte[] Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Password
        {
            set
            {
                if ((value == null) || (value.Length == 0))
                {
                    this.key = null;
                }
                else
                {
                    this.key = PkzipClassic.GenerateKeys(Encoding.ASCII.GetBytes(value));
                }
            }
        }

        public int Size
        {
            get
            {
                if (this.entries == null)
                {
                    throw new InvalidOperationException("ZipFile is closed");
                }
                return this.entries.Length;
            }
        }

        public string ZipFileComment
        {
            get
            {
                return this.comment;
            }
        }

        public delegate void KeysRequiredEventHandler(object sender, KeysRequiredEventArgs e);

        private class PartialInputStream : InflaterInputStream
        {
            private Stream baseStream;
            private long end;
            private long filepos;

            public PartialInputStream(Stream baseStream, long start, long len) : base(baseStream)
            {
                this.baseStream = baseStream;
                this.filepos = start;
                this.end = start + len;
            }

            public override void Close()
            {
            }

            public override int Read(byte[] b, int off, int len)
            {
                if (len > (this.end - this.filepos))
                {
                    len = (int) (this.end - this.filepos);
                    if (len == 0)
                    {
                        return 0;
                    }
                }
                lock (this.baseStream)
                {
                    this.baseStream.Seek(this.filepos, SeekOrigin.Begin);
                    int num = this.baseStream.Read(b, off, len);
                    if (num > 0)
                    {
                        this.filepos += len;
                    }
                    return num;
                }
            }

            public override int ReadByte()
            {
                if (this.filepos == this.end)
                {
                    return -1;
                }
                lock (this.baseStream)
                {
                    long num2;
                    this.filepos = (num2 = this.filepos) + 1L;
                    this.baseStream.Seek(num2, SeekOrigin.Begin);
                    return this.baseStream.ReadByte();
                }
            }

            public long SkipBytes(long amount)
            {
                if (amount < 0L)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (amount > (this.end - this.filepos))
                {
                    amount = this.end - this.filepos;
                }
                this.filepos += amount;
                return amount;
            }

            public override int Available
            {
                get
                {
                    long num = this.end - this.filepos;
                    if (num > 0x7fffffffL)
                    {
                        return 0x7fffffff;
                    }
                    return (int) num;
                }
            }
        }

        private class ZipEntryEnumeration : IEnumerator
        {
            private ZipEntry[] array;
            private int ptr = -1;

            public ZipEntryEnumeration(ZipEntry[] arr)
            {
                this.array = arr;
            }

            public bool MoveNext()
            {
                return (++this.ptr < this.array.Length);
            }

            public void Reset()
            {
                this.ptr = -1;
            }

            public object Current
            {
                get
                {
                    return this.array[this.ptr];
                }
            }
        }
    }
}

