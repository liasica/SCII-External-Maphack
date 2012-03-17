namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
    using ICSharpCode.SharpZipLib;
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip.Compression;
    using System;
    using System.IO;

    public class DeflaterOutputStream : Stream
    {
        protected Stream baseOutputStream;
        protected byte[] buf;
        protected Deflater def;
        private bool isClosed;
        private bool isStreamOwner;
        private uint[] keys;
        private string password;

        public DeflaterOutputStream(Stream baseOutputStream) : this(baseOutputStream, new Deflater(), 0x200)
        {
        }

        public DeflaterOutputStream(Stream baseOutputStream, Deflater defl) : this(baseOutputStream, defl, 0x200)
        {
        }

        public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater, int bufsize)
        {
            this.isClosed = false;
            this.isStreamOwner = true;
            this.password = null;
            this.keys = null;
            if (!baseOutputStream.CanWrite)
            {
                throw new ArgumentException("baseOutputStream", "must support writing");
            }
            if (deflater == null)
            {
                throw new ArgumentNullException("deflater");
            }
            if (bufsize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufsize");
            }
            this.baseOutputStream = baseOutputStream;
            this.buf = new byte[bufsize];
            this.def = deflater;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("DeflaterOutputStream BeginWrite not currently supported");
        }

        public override void Close()
        {
            if (!this.isClosed)
            {
                this.isClosed = true;
                this.Finish();
                if (this.isStreamOwner)
                {
                    this.baseOutputStream.Close();
                }
            }
        }

        protected void Deflate()
        {
            while (!this.def.IsNeedingInput)
            {
                int length = this.def.Deflate(this.buf, 0, this.buf.Length);
                if (length <= 0)
                {
                    break;
                }
                if (this.keys != null)
                {
                    this.EncryptBlock(this.buf, 0, length);
                }
                this.baseOutputStream.Write(this.buf, 0, length);
            }
            if (!this.def.IsNeedingInput)
            {
                throw new SharpZipBaseException("DeflaterOutputStream can't deflate all input?");
            }
        }

        protected void EncryptBlock(byte[] buffer, int offset, int length)
        {
            for (int i = offset; i < (offset + length); i++)
            {
                byte[] buffer2;
                IntPtr ptr;
                byte ch = buffer[i];
                (buffer2 = buffer)[(int) (ptr = (IntPtr) i)] = (byte) (buffer2[(int) ptr] ^ this.EncryptByte());
                this.UpdateKeys(ch);
            }
        }

        protected byte EncryptByte()
        {
            uint num = (this.keys[2] & 0xffff) | 2;
            return (byte) ((num * (num ^ 1)) >> 8);
        }

        public virtual void Finish()
        {
            this.def.Finish();
            while (!this.def.IsFinished)
            {
                int length = this.def.Deflate(this.buf, 0, this.buf.Length);
                if (length <= 0)
                {
                    break;
                }
                if (this.keys != null)
                {
                    this.EncryptBlock(this.buf, 0, length);
                }
                this.baseOutputStream.Write(this.buf, 0, length);
            }
            if (!this.def.IsFinished)
            {
                throw new SharpZipBaseException("Can't deflate all input?");
            }
            this.baseOutputStream.Flush();
            this.keys = null;
        }

        public override void Flush()
        {
            this.def.Flush();
            this.Deflate();
            this.baseOutputStream.Flush();
        }

        protected void InitializePassword(string password)
        {
            this.keys = new uint[] { 0x12345678, 0x23456789, 0x34567890 };
            for (int i = 0; i < password.Length; i++)
            {
                this.UpdateKeys((byte) password[i]);
            }
        }

        public override int Read(byte[] b, int off, int len)
        {
            throw new NotSupportedException("DeflaterOutputStream Read not supported");
        }

        public override int ReadByte()
        {
            throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("DeflaterOutputStream Seek not supported");
        }

        public override void SetLength(long val)
        {
            throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
        }

        protected void UpdateKeys(byte ch)
        {
            this.keys[0] = Crc32.ComputeCrc32(this.keys[0], ch);
            this.keys[1] += (byte) this.keys[0];
            this.keys[1] = (this.keys[1] * 0x8088405) + 1;
            this.keys[2] = Crc32.ComputeCrc32(this.keys[2], (byte) (this.keys[1] >> 0x18));
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.def.SetInput(buf, off, len);
            this.Deflate();
        }

        public override void WriteByte(byte bval)
        {
            byte[] buffer = new byte[] { bval };
            this.Write(buffer, 0, 1);
        }

        public bool CanPatchEntries
        {
            get
            {
                return this.baseOutputStream.CanSeek;
            }
        }

        public override bool CanRead
        {
            get
            {
                return this.baseOutputStream.CanRead;
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
                return this.baseOutputStream.CanWrite;
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
                return this.baseOutputStream.Length;
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
                if ((value != null) && (value.Length == 0))
                {
                    this.password = null;
                }
                else
                {
                    this.password = value;
                }
            }
        }

        public override long Position
        {
            get
            {
                return this.baseOutputStream.Position;
            }
            set
            {
                throw new NotSupportedException("DefalterOutputStream Position not supported");
            }
        }
    }
}

