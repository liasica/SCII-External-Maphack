namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Encryption;
	using ICSharpCode.SharpZipLib.Zip.Compression;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class ZipInputStream : InflaterInputStream
    {
        private Crc32 crc;
        private ZipEntry entry;
        private int flags;
        private ReaderDelegate internalReader;
        private int method;
        private string password;
        private long size;

        public ZipInputStream(Stream baseInputStream) : base(baseInputStream, new Inflater(true))
        {
            this.crc = new Crc32();
            this.entry = null;
            this.password = null;
            this.internalReader = new ReaderDelegate(this.InitialRead);
        }

        public int BodyRead(byte[] b, int off, int len)
        {
            if (this.crc == null)
            {
                throw new InvalidOperationException("Closed.");
            }
            if ((this.entry == null) || (len <= 0))
            {
                return 0;
            }
            bool flag = false;
            switch (this.method)
            {
                case 0:
                    if ((len > base.csize) && (base.csize >= 0L))
                    {
                        len = (int) base.csize;
                    }
                    len = base.inputBuffer.ReadClearTextBuffer(b, off, len);
                    if (len > 0)
                    {
                        base.csize -= len;
                        this.size -= len;
                    }
                    if (base.csize == 0L)
                    {
                        flag = true;
                    }
                    else if (len < 0)
                    {
                        throw new ZipException("EOF in stored block");
                    }
                    break;

                case 8:
                    len = base.Read(b, off, len);
                    if (len <= 0)
                    {
                        if (!base.inf.IsFinished)
                        {
                            throw new ZipException("Inflater not finished!?");
                        }
                        base.inputBuffer.Available = base.inf.RemainingInput;
                        if (((this.flags & 8) == 0) && ((base.inf.TotalIn != base.csize) || (base.inf.TotalOut != this.size)))
                        {
                            throw new ZipException(string.Concat(new object[] { "size mismatch: ", base.csize, ";", this.size, " <-> ", base.inf.TotalIn, ";", base.inf.TotalOut }));
                        }
                        base.inf.Reset();
                        flag = true;
                    }
                    break;
            }
            if (len > 0)
            {
                this.crc.Update(b, off, len);
            }
            if (flag)
            {
                base.StopDecrypting();
                if ((this.flags & 8) != 0)
                {
                    this.ReadDataDescriptor();
                }
                if (((this.crc.Value & 0xffffffffL) != this.entry.Crc) && (this.entry.Crc != -1L))
                {
                    throw new ZipException("CRC mismatch");
                }
                this.crc.Reset();
                this.entry = null;
            }
            return len;
        }

        public override void Close()
        {
            base.Close();
            this.crc = null;
            this.entry = null;
        }

        public void CloseEntry()
        {
            if (this.crc == null)
            {
                throw new InvalidOperationException("Closed.");
            }
            if (this.entry != null)
            {
                if (this.method == 8)
                {
                    if ((this.flags & 8) != 0)
                    {
                        byte[] buffer = new byte[0x800];
                        while (this.Read(buffer, 0, buffer.Length) > 0)
                        {
                        }
                        return;
                    }
                    base.csize -= base.inf.TotalIn;
                    base.inputBuffer.Available -= base.inf.RemainingInput;
                }
                if ((base.inputBuffer.Available > base.csize) && (base.csize >= 0L))
                {
                    base.inputBuffer.Available -= (int) base.csize;
                }
                else
                {
                    base.csize -= base.inputBuffer.Available;
                    base.inputBuffer.Available = 0;
                    while (base.csize != 0L)
                    {
                        int num = (int) base.Skip(base.csize & ((long) 0xffffffffL));
                        if (num <= 0)
                        {
                            throw new ZipException("Zip archive ends early.");
                        }
                        base.csize -= num;
                    }
                }
                this.size = 0L;
                this.crc.Reset();
                if (this.method == 8)
                {
                    base.inf.Reset();
                }
                this.entry = null;
            }
        }

        public ZipEntry GetNextEntry()
        {
            if (this.crc == null)
            {
                throw new InvalidOperationException("Closed.");
            }
            if (this.entry != null)
            {
                this.CloseEntry();
            }
            int num = base.inputBuffer.ReadLeInt();
            switch (num)
            {
                case 0x2014b50:
                case 0x6054b50:
                case 0x5054b50:
                case 0x6064b50:
                    this.Close();
                    return null;

                case 0x30304b50:
                case 0x8074b50:
                    num = base.inputBuffer.ReadLeInt();
                    break;
            }
            if (num != 0x4034b50)
            {
                throw new ZipException("Wrong Local header signature: 0x" + string.Format("{0:X}", num));
            }
            short versionRequiredToExtract = (short) base.inputBuffer.ReadLeShort();
            this.flags = base.inputBuffer.ReadLeShort();
            this.method = base.inputBuffer.ReadLeShort();
            uint num3 = (uint) base.inputBuffer.ReadLeInt();
            int num4 = base.inputBuffer.ReadLeInt();
            base.csize = base.inputBuffer.ReadLeInt();
            this.size = base.inputBuffer.ReadLeInt();
            int num5 = base.inputBuffer.ReadLeShort();
            int num6 = base.inputBuffer.ReadLeShort();
            bool flag = (this.flags & 1) == 1;
            byte[] buffer = new byte[num5];
            base.inputBuffer.ReadRawBuffer(buffer);
            string name = ZipConstants.ConvertToString(buffer);
            this.entry = new ZipEntry(name, versionRequiredToExtract);
            this.entry.Flags = this.flags;
            if ((this.method == 0) && ((!flag && (base.csize != this.size)) || (flag && ((base.csize - 12L) != this.size))))
            {
                throw new ZipException("Stored, but compressed != uncompressed");
            }
            if ((this.method != 0) && (this.method != 8))
            {
                throw new ZipException("Unknown compression method " + this.method);
            }
            this.entry.CompressionMethod = (CompressionMethod) this.method;
            if ((this.flags & 8) == 0)
            {
                this.entry.Crc = num4 & ((long) 0xffffffffL);
                this.entry.Size = this.size & ((long) 0xffffffffL);
                this.entry.CompressedSize = base.csize & ((long) 0xffffffffL);
            }
            else
            {
                if (num4 != 0)
                {
                    this.entry.Crc = num4 & ((long) 0xffffffffL);
                }
                if (this.size != 0L)
                {
                    this.entry.Size = this.size & ((long) 0xffffffffL);
                }
                if (base.csize != 0L)
                {
                    this.entry.CompressedSize = base.csize & ((long) 0xffffffffL);
                }
            }
            this.entry.DosTime = num3;
            if (num6 > 0)
            {
                byte[] buffer2 = new byte[num6];
                base.inputBuffer.ReadRawBuffer(buffer2);
                this.entry.ExtraData = buffer2;
            }
            this.internalReader = new ReaderDelegate(this.InitialRead);
            return this.entry;
        }

        private int InitialRead(byte[] destination, int offset, int count)
        {
            if (this.entry.Version > 20)
            {
                throw new ZipException("Libray cannot extract this entry version required (" + this.entry.Version.ToString() + ")");
            }
            if (this.entry.IsCrypted)
            {
                if (this.password == null)
                {
                    throw new ZipException("No password set.");
                }
                PkzipClassicManaged managed = new PkzipClassicManaged();
                byte[] rgbKey = PkzipClassic.GenerateKeys(Encoding.ASCII.GetBytes(this.password));
                base.inputBuffer.CryptoTransform = managed.CreateDecryptor(rgbKey, null);
                byte[] outBuffer = new byte[12];
                base.inputBuffer.ReadClearTextBuffer(outBuffer, 0, 12);
                if ((this.flags & 8) == 0)
                {
                    if (outBuffer[11] != ((byte) (this.entry.Crc >> 0x18)))
                    {
                        throw new ZipException("Invalid password");
                    }
                }
                else if (outBuffer[11] != ((byte) ((this.entry.DosTime >> 8) & 0xffL)))
                {
                    throw new ZipException("Invalid password");
                }
                if (base.csize >= 12L)
                {
                    base.csize -= 12L;
                }
            }
            else
            {
                base.inputBuffer.CryptoTransform = null;
            }
            if ((this.method == 8) && (base.inputBuffer.Available > 0))
            {
                base.inputBuffer.SetInflaterInput(base.inf);
            }
            this.internalReader = new ReaderDelegate(this.BodyRead);
            return this.BodyRead(destination, offset, count);
        }

        public override int Read(byte[] destination, int index, int count)
        {
            return this.internalReader(destination, index, count);
        }

        public override int ReadByte()
        {
            byte[] buffer = new byte[1];
            if (this.Read(buffer, 0, 1) <= 0)
            {
                return -1;
            }
            return (buffer[0] & 0xff);
        }

        private void ReadDataDescriptor()
        {
            if (base.inputBuffer.ReadLeInt() != 0x8074b50)
            {
                throw new ZipException("Data descriptor signature not found");
            }
            this.entry.Crc = base.inputBuffer.ReadLeInt() & ((long) 0xffffffffL);
            base.csize = base.inputBuffer.ReadLeInt();
            this.size = base.inputBuffer.ReadLeInt();
            this.entry.Size = this.size & ((long) 0xffffffffL);
            this.entry.CompressedSize = base.csize & ((long) 0xffffffffL);
        }

        public override int Available
        {
            get
            {
                if (this.entry == null)
                {
                    return 0;
                }
                return 1;
            }
        }

        public bool CanDecompressEntry
        {
            get
            {
                return ((this.entry != null) && (this.entry.Version <= 20));
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        private delegate int ReaderDelegate(byte[] b, int offset, int length);
    }
}

