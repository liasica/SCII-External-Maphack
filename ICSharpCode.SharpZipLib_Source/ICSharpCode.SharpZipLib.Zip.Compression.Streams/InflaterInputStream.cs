namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
    using ICSharpCode.SharpZipLib;
    using ICSharpCode.SharpZipLib.Zip.Compression;
    using System;
    using System.IO;

    public class InflaterInputStream : Stream
    {
        protected Stream baseInputStream;
        protected long csize;
        protected Inflater inf;
        protected InflaterInputBuffer inputBuffer;
        private bool isClosed;
        private bool isStreamOwner;

        public InflaterInputStream(Stream baseInputStream) : this(baseInputStream, new Inflater(), 0x1000)
        {
        }

        public InflaterInputStream(Stream baseInputStream, Inflater inf) : this(baseInputStream, inf, 0x1000)
        {
        }

        public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
        {
            this.isClosed = false;
            this.isStreamOwner = true;
            if (baseInputStream == null)
            {
                throw new ArgumentNullException("InflaterInputStream baseInputStream is null");
            }
            if (inflater == null)
            {
                throw new ArgumentNullException("InflaterInputStream Inflater is null");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }
            this.baseInputStream = baseInputStream;
            this.inf = inflater;
            this.inputBuffer = new InflaterInputBuffer(baseInputStream);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
        }

        public override void Close()
        {
            if (!this.isClosed)
            {
                this.isClosed = true;
                if (this.isStreamOwner)
                {
                    this.baseInputStream.Close();
                }
            }
        }

        protected void Fill()
        {
            this.inputBuffer.Fill();
            this.inputBuffer.SetInflaterInput(this.inf);
        }

        public override void Flush()
        {
            this.baseInputStream.Flush();
        }

        public override int Read(byte[] b, int off, int len)
        {
            while (true)
            {
                int num;
                try
                {
                    num = this.inf.Inflate(b, off, len);
                }
                catch (Exception exception)
                {
                    throw new SharpZipBaseException(exception.ToString());
                }
                if (num > 0)
                {
                    return num;
                }
                if (this.inf.IsNeedingDictionary)
                {
                    throw new SharpZipBaseException("Need a dictionary");
                }
                if (this.inf.IsFinished)
                {
                    return 0;
                }
                if (!this.inf.IsNeedingInput)
                {
                    throw new InvalidOperationException("Don't know what to do");
                }
                this.Fill();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek not supported");
        }

        public override void SetLength(long val)
        {
            throw new NotSupportedException("InflaterInputStream SetLength not supported");
        }

        public long Skip(long n)
        {
            if (n <= 0L)
            {
                throw new ArgumentOutOfRangeException("n");
            }
            if (this.baseInputStream.CanSeek)
            {
                this.baseInputStream.Seek(n, SeekOrigin.Current);
                return n;
            }
            int num = 0x800;
            if (n < num)
            {
                num = (int) n;
            }
            byte[] buffer = new byte[num];
            return (long) this.baseInputStream.Read(buffer, 0, buffer.Length);
        }

        protected void StopDecrypting()
        {
            this.inputBuffer.CryptoTransform = null;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            throw new NotSupportedException("InflaterInputStream Write not supported");
        }

        public override void WriteByte(byte val)
        {
            throw new NotSupportedException("InflaterInputStream WriteByte not supported");
        }

        public virtual int Available
        {
            get
            {
                if (!this.inf.IsFinished)
                {
                    return 1;
                }
                return 0;
            }
        }

        public override bool CanRead
        {
            get
            {
                return this.baseInputStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public bool IsStreamOwner
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

        public override long Length
        {
            get
            {
                return (long) this.inputBuffer.RawLength;
            }
        }

        public override long Position
        {
            get
            {
                return this.baseInputStream.Position;
            }
            set
            {
                throw new NotSupportedException("InflaterInputStream Position not supported");
            }
        }
    }
}

