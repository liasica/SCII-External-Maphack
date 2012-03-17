namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip.Compression;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.Collections;
    using System.IO;

    public class ZipOutputStream : DeflaterOutputStream
    {
        private Crc32 crc;
        private ZipEntry curEntry;
        private CompressionMethod curMethod;
        private int defaultCompressionLevel;
        private ArrayList entries;
        private long headerPatchPos;
        private long offset;
        private bool patchEntryHeader;
        private long size;
        private byte[] zipComment;

        public ZipOutputStream(Stream baseOutputStream) : base(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true))
        {
            this.entries = new ArrayList();
            this.crc = new Crc32();
            this.curEntry = null;
            this.defaultCompressionLevel = Deflater.DEFAULT_COMPRESSION;
            this.curMethod = CompressionMethod.Deflated;
            this.offset = 0L;
            this.zipComment = new byte[0];
            this.patchEntryHeader = false;
            this.headerPatchPos = -1L;
        }

        public void CloseEntry()
        {
            if (this.curEntry == null)
            {
                throw new InvalidOperationException("No open entry");
            }
            if (this.curMethod == CompressionMethod.Deflated)
            {
                base.Finish();
            }
            long num = (this.curMethod == CompressionMethod.Deflated) ? base.def.TotalOut : this.size;
            if (this.curEntry.Size < 0L)
            {
                this.curEntry.Size = this.size;
            }
            else if (this.curEntry.Size != this.size)
            {
                throw new ZipException(string.Concat(new object[] { "size was ", this.size, ", but I expected ", this.curEntry.Size }));
            }
            if (this.curEntry.CompressedSize < 0L)
            {
                this.curEntry.CompressedSize = num;
            }
            else if (this.curEntry.CompressedSize != num)
            {
                throw new ZipException(string.Concat(new object[] { "compressed size was ", num, ", but I expected ", this.curEntry.CompressedSize }));
            }
            if (this.curEntry.Crc < 0L)
            {
                this.curEntry.Crc = this.crc.Value;
            }
            else if (this.curEntry.Crc != this.crc.Value)
            {
                throw new ZipException(string.Concat(new object[] { "crc was ", this.crc.Value, ", but I expected ", this.curEntry.Crc }));
            }
            this.offset += num;
            if (this.offset > 0xffffffffL)
            {
                throw new ZipException("Maximum Zip file size exceeded");
            }
            if (this.curEntry.IsCrypted)
            {
                this.curEntry.CompressedSize += 12L;
            }
            if (this.patchEntryHeader)
            {
                long position = base.baseOutputStream.Position;
                base.baseOutputStream.Seek(this.headerPatchPos, SeekOrigin.Begin);
                this.WriteLeInt((int) this.curEntry.Crc);
                this.WriteLeInt((int) this.curEntry.CompressedSize);
                this.WriteLeInt((int) this.curEntry.Size);
                base.baseOutputStream.Seek(position, SeekOrigin.Begin);
                this.patchEntryHeader = false;
            }
            if ((this.curEntry.Flags & 8) != 0)
            {
                this.WriteLeInt(0x8074b50);
                this.WriteLeInt((int) this.curEntry.Crc);
                this.WriteLeInt((int) this.curEntry.CompressedSize);
                this.WriteLeInt((int) this.curEntry.Size);
                this.offset += 0x10L;
            }
            this.entries.Add(this.curEntry);
            this.curEntry = null;
        }

        public override void Finish()
        {
            if (this.entries != null)
            {
                if (this.curEntry != null)
                {
                    this.CloseEntry();
                }
                int num = 0;
                int num2 = 0;
                foreach (ZipEntry entry in this.entries)
                {
                    CompressionMethod compressionMethod = entry.CompressionMethod;
                    this.WriteLeInt(0x2014b50);
                    this.WriteLeShort(20);
                    this.WriteLeShort(entry.Version);
                    this.WriteLeShort(entry.Flags);
                    this.WriteLeShort((short) compressionMethod);
                    this.WriteLeInt((int) entry.DosTime);
                    this.WriteLeInt((int) entry.Crc);
                    this.WriteLeInt((int) entry.CompressedSize);
                    this.WriteLeInt((int) entry.Size);
                    byte[] buffer = ZipConstants.ConvertToArray(entry.Name);
                    if (buffer.Length > 0xffff)
                    {
                        throw new ZipException("Name too long.");
                    }
                    byte[] extraData = entry.ExtraData;
                    if (extraData == null)
                    {
                        extraData = new byte[0];
                    }
                    byte[] buffer3 = (entry.Comment != null) ? ZipConstants.ConvertToArray(entry.Comment) : new byte[0];
                    if (buffer3.Length > 0xffff)
                    {
                        throw new ZipException("Comment too long.");
                    }
                    this.WriteLeShort(buffer.Length);
                    this.WriteLeShort(extraData.Length);
                    this.WriteLeShort(buffer3.Length);
                    this.WriteLeShort(0);
                    this.WriteLeShort(0);
                    if (entry.ExternalFileAttributes != -1)
                    {
                        this.WriteLeInt(entry.ExternalFileAttributes);
                    }
                    else if (entry.IsDirectory)
                    {
                        this.WriteLeInt(0x10);
                    }
                    else
                    {
                        this.WriteLeInt(0);
                    }
                    this.WriteLeInt(entry.Offset);
                    base.baseOutputStream.Write(buffer, 0, buffer.Length);
                    base.baseOutputStream.Write(extraData, 0, extraData.Length);
                    base.baseOutputStream.Write(buffer3, 0, buffer3.Length);
                    num++;
                    num2 += ((0x2e + buffer.Length) + extraData.Length) + buffer3.Length;
                }
                this.WriteLeInt(0x6054b50);
                this.WriteLeShort(0);
                this.WriteLeShort(0);
                this.WriteLeShort(num);
                this.WriteLeShort(num);
                this.WriteLeInt(num2);
                this.WriteLeInt((int) this.offset);
                this.WriteLeShort(this.zipComment.Length);
                base.baseOutputStream.Write(this.zipComment, 0, this.zipComment.Length);
                base.baseOutputStream.Flush();
                this.entries = null;
            }
        }

        public int GetLevel()
        {
            return base.def.GetLevel();
        }

        public void PutNextEntry(ZipEntry entry)
        {
            if (this.entries == null)
            {
                throw new InvalidOperationException("ZipOutputStream was finished");
            }
            if (this.curEntry != null)
            {
                this.CloseEntry();
            }
            if (this.entries.Count >= 0xffff)
            {
                throw new ZipException("Too many entries for Zip file");
            }
            CompressionMethod compressionMethod = entry.CompressionMethod;
            int defaultCompressionLevel = this.defaultCompressionLevel;
            entry.Flags = 0;
            this.patchEntryHeader = false;
            bool flag = true;
            switch (compressionMethod)
            {
                case CompressionMethod.Stored:
                    if (entry.CompressedSize >= 0L)
                    {
                        if (entry.Size >= 0L)
                        {
                            if (entry.Size != entry.CompressedSize)
                            {
                                throw new ZipException("Method STORED, but compressed size != size");
                            }
                        }
                        else
                        {
                            entry.Size = entry.CompressedSize;
                        }
                    }
                    else if (entry.Size >= 0L)
                    {
                        entry.CompressedSize = entry.Size;
                    }
                    if ((entry.Size < 0L) || (entry.Crc < 0L))
                    {
                        if (base.CanPatchEntries)
                        {
                            flag = false;
                        }
                        else
                        {
                            compressionMethod = CompressionMethod.Deflated;
                            defaultCompressionLevel = 0;
                        }
                    }
                    break;

                case CompressionMethod.Deflated:
                    if (entry.Size == 0L)
                    {
                        entry.CompressedSize = entry.Size;
                        entry.Crc = 0L;
                        compressionMethod = CompressionMethod.Stored;
                    }
                    else if (((entry.CompressedSize < 0L) || (entry.Size < 0L)) || (entry.Crc < 0L))
                    {
                        flag = false;
                    }
                    break;
            }
            if (!flag)
            {
                if (!base.CanPatchEntries)
                {
                    entry.Flags |= 8;
                }
                else
                {
                    this.patchEntryHeader = true;
                }
            }
            if (base.Password != null)
            {
                entry.IsCrypted = true;
                if (entry.Crc < 0L)
                {
                    entry.Flags |= 8;
                }
            }
            entry.Offset = (int) this.offset;
            entry.CompressionMethod = compressionMethod;
            this.curMethod = compressionMethod;
            this.WriteLeInt(0x4034b50);
            this.WriteLeShort(entry.Version);
            this.WriteLeShort(entry.Flags);
            this.WriteLeShort((byte) compressionMethod);
            this.WriteLeInt((int) entry.DosTime);
            if (flag)
            {
                this.WriteLeInt((int) entry.Crc);
                this.WriteLeInt(entry.IsCrypted ? (((int) entry.CompressedSize) + 12) : ((int) entry.CompressedSize));
                this.WriteLeInt((int) entry.Size);
            }
            else
            {
                if (this.patchEntryHeader)
                {
                    this.headerPatchPos = base.baseOutputStream.Position;
                }
                this.WriteLeInt(0);
                this.WriteLeInt(0);
                this.WriteLeInt(0);
            }
            byte[] buffer = ZipConstants.ConvertToArray(entry.Name);
            if (buffer.Length > 0xffff)
            {
                throw new ZipException("Entry name too long.");
            }
            byte[] extraData = entry.ExtraData;
            if (extraData == null)
            {
                extraData = new byte[0];
            }
            if (extraData.Length > 0xffff)
            {
                throw new ZipException("Extra data too long.");
            }
            this.WriteLeShort(buffer.Length);
            this.WriteLeShort(extraData.Length);
            base.baseOutputStream.Write(buffer, 0, buffer.Length);
            base.baseOutputStream.Write(extraData, 0, extraData.Length);
            this.offset += (30 + buffer.Length) + extraData.Length;
            this.curEntry = entry;
            this.crc.Reset();
            if (compressionMethod == CompressionMethod.Deflated)
            {
                base.def.Reset();
                base.def.SetLevel(defaultCompressionLevel);
            }
            this.size = 0L;
            if (entry.IsCrypted)
            {
                if (entry.Crc < 0L)
                {
                    this.WriteEncryptionHeader(entry.DosTime << 0x10);
                }
                else
                {
                    this.WriteEncryptionHeader(entry.Crc);
                }
            }
        }

        public void SetComment(string comment)
        {
            byte[] buffer = ZipConstants.ConvertToArray(comment);
            if (buffer.Length > 0xffff)
            {
                throw new ArgumentOutOfRangeException("comment");
            }
            this.zipComment = buffer;
        }

        public void SetLevel(int level)
        {
            this.defaultCompressionLevel = level;
            base.def.SetLevel(level);
        }

        public override void Write(byte[] b, int off, int len)
        {
            if (this.curEntry == null)
            {
                throw new InvalidOperationException("No open entry.");
            }
            if (len > 0)
            {
                this.crc.Update(b, off, len);
                this.size += len;
                if ((this.size > 0xffffffffL) || (this.size < 0L))
                {
                    throw new ZipException("Maximum entry size exceeded");
                }
                switch (this.curMethod)
                {
                    case CompressionMethod.Stored:
                        if (base.Password != null)
                        {
                            byte[] destinationArray = new byte[len];
                            Array.Copy(b, off, destinationArray, 0, len);
                            base.EncryptBlock(destinationArray, 0, len);
                            base.baseOutputStream.Write(destinationArray, off, len);
                        }
                        else
                        {
                            base.baseOutputStream.Write(b, off, len);
                        }
                        break;

                    case CompressionMethod.Deflated:
                        base.Write(b, off, len);
                        break;
                }
            }
        }

        private void WriteEncryptionHeader(long crcValue)
        {
            this.offset += 12L;
            base.InitializePassword(base.Password);
            byte[] buffer = new byte[12];
            new Random().NextBytes(buffer);
            buffer[11] = (byte) (crcValue >> 0x18);
            base.EncryptBlock(buffer, 0, buffer.Length);
            base.baseOutputStream.Write(buffer, 0, buffer.Length);
        }

        private void WriteLeInt(int value)
        {
            this.WriteLeShort(value);
            this.WriteLeShort(value >> 0x10);
        }

        private void WriteLeLong(long value)
        {
            this.WriteLeInt((int) value);
            this.WriteLeInt((int) (value >> 0x20));
        }

        private void WriteLeShort(int value)
        {
            base.baseOutputStream.WriteByte((byte) (value & 0xff));
            base.baseOutputStream.WriteByte((byte) ((value >> 8) & 0xff));
        }

        public bool IsFinished
        {
            get
            {
                return (this.entries == null);
            }
        }
    }
}

