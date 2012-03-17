namespace ICSharpCode.SharpZipLib.Tar
{
    using System;
    using System.IO;

    public class TarOutputStream : Stream
    {
        protected byte[] assemBuf;
        protected int assemLen;
        protected byte[] blockBuf;
        protected TarBuffer buffer;
        protected long currBytes;
        protected long currSize;
        protected bool debug;
        protected Stream outputStream;

        public TarOutputStream(Stream outputStream) : this(outputStream, 20)
        {
        }

        public TarOutputStream(Stream outputStream, int blockFactor)
        {
            this.outputStream = outputStream;
            this.buffer = TarBuffer.CreateOutputTarBuffer(outputStream, blockFactor);
            this.debug = false;
            this.assemLen = 0;
            this.assemBuf = new byte[0x200];
            this.blockBuf = new byte[0x200];
        }

        public override void Close()
        {
            this.Finish();
            this.buffer.Close();
        }

        public void CloseEntry()
        {
            if (this.assemLen > 0)
            {
                for (int i = this.assemLen; i < this.assemBuf.Length; i++)
                {
                    this.assemBuf[i] = 0;
                }
                this.buffer.WriteBlock(this.assemBuf);
                this.currBytes += this.assemLen;
                this.assemLen = 0;
            }
            if (this.currBytes < this.currSize)
            {
                throw new TarException(string.Concat(new object[] { "entry closed at '", this.currBytes, "' before the '", this.currSize, "' bytes specified in the header were written" }));
            }
        }

        public void Finish()
        {
            this.WriteEOFRecord();
        }

        public override void Flush()
        {
            this.outputStream.Flush();
        }

        public int GetRecordSize()
        {
            return this.buffer.GetRecordSize();
        }

        public void PutNextEntry(TarEntry entry)
        {
            if (entry.TarHeader.Name.Length >= TarHeader.NAMELEN)
            {
                TarHeader header = new TarHeader();
                new TarHeader { TypeFlag = 0x4c, Name = header.Name + "././@LongLink", UserId = 0, GroupId = 0, GroupName = "", UserName = "", LinkName = "", Size = entry.TarHeader.Name.Length }.WriteHeader(this.blockBuf);
                this.buffer.WriteBlock(this.blockBuf);
                int nameOffset = 0;
                while (nameOffset < entry.TarHeader.Name.Length)
                {
                    Array.Clear(this.blockBuf, 0, this.blockBuf.Length);
                    TarHeader.GetAsciiBytes(entry.TarHeader.Name, nameOffset, this.blockBuf, 0, 0x200);
                    nameOffset += 0x200;
                    this.buffer.WriteBlock(this.blockBuf);
                }
            }
            entry.WriteEntryHeader(this.blockBuf);
            this.buffer.WriteBlock(this.blockBuf);
            this.currBytes = 0L;
            this.currSize = entry.IsDirectory ? 0L : entry.Size;
        }

        public override int Read(byte[] b, int off, int len)
        {
            return this.outputStream.Read(b, off, len);
        }

        public override int ReadByte()
        {
            return this.outputStream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.outputStream.Seek(offset, origin);
        }

        public override void SetLength(long val)
        {
            this.outputStream.SetLength(val);
        }

        public override void Write(byte[] wBuf, int wOffset, int numToWrite)
        {
            if (wBuf == null)
            {
                throw new ArgumentNullException("TarOutputStream.Write buffer null");
            }
            if ((this.currBytes + numToWrite) > this.currSize)
            {
                throw new ArgumentOutOfRangeException(string.Concat(new object[] { "request to write '", numToWrite, "' bytes exceeds size in header of '", this.currSize, "' bytes" }));
            }
            if (this.assemLen > 0)
            {
                if ((this.assemLen + numToWrite) >= this.blockBuf.Length)
                {
                    int length = this.blockBuf.Length - this.assemLen;
                    Array.Copy(this.assemBuf, 0, this.blockBuf, 0, this.assemLen);
                    Array.Copy(wBuf, wOffset, this.blockBuf, this.assemLen, length);
                    this.buffer.WriteBlock(this.blockBuf);
                    this.currBytes += this.blockBuf.Length;
                    wOffset += length;
                    numToWrite -= length;
                    this.assemLen = 0;
                }
                else
                {
                    Array.Copy(wBuf, wOffset, this.assemBuf, this.assemLen, numToWrite);
                    wOffset += numToWrite;
                    this.assemLen += numToWrite;
                    numToWrite -= numToWrite;
                }
            }
            while (numToWrite > 0)
            {
                if (numToWrite < this.blockBuf.Length)
                {
                    Array.Copy(wBuf, wOffset, this.assemBuf, this.assemLen, numToWrite);
                    this.assemLen += numToWrite;
                    return;
                }
                this.buffer.WriteBlock(wBuf, wOffset);
                int num2 = this.blockBuf.Length;
                this.currBytes += num2;
                numToWrite -= num2;
                wOffset += num2;
            }
        }

        public override void WriteByte(byte b)
        {
            this.Write(new byte[] { b }, 0, 1);
        }

        private void WriteEOFRecord()
        {
            Array.Clear(this.blockBuf, 0, this.blockBuf.Length);
            this.buffer.WriteBlock(this.blockBuf);
        }

        public override bool CanRead
        {
            get
            {
                return this.outputStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.outputStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.outputStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.outputStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.outputStream.Position;
            }
            set
            {
                this.outputStream.Position = value;
            }
        }
    }
}

