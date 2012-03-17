namespace ICSharpCode.SharpZipLib.Zip.Compression
{
    using System;

    public class PendingBuffer
    {
        private int bitCount;
        private uint bits;
        protected byte[] buf;
        private int end;
        private int start;

        public PendingBuffer() : this(0x1000)
        {
        }

        public PendingBuffer(int bufsize)
        {
            this.buf = new byte[bufsize];
        }

        public void AlignToByte()
        {
            if (this.bitCount > 0)
            {
                this.buf[this.end++] = (byte) this.bits;
                if (this.bitCount > 8)
                {
                    this.buf[this.end++] = (byte) (this.bits >> 8);
                }
            }
            this.bits = 0;
            this.bitCount = 0;
        }

        public int Flush(byte[] output, int offset, int length)
        {
            if (this.bitCount >= 8)
            {
                this.buf[this.end++] = (byte) this.bits;
                this.bits = this.bits >> 8;
                this.bitCount -= 8;
            }
            if (length > (this.end - this.start))
            {
                length = this.end - this.start;
                Array.Copy(this.buf, this.start, output, offset, length);
                this.start = 0;
                this.end = 0;
                return length;
            }
            Array.Copy(this.buf, this.start, output, offset, length);
            this.start += length;
            return length;
        }

        public void Reset()
        {
            this.start = this.end = this.bitCount = 0;
        }

        public byte[] ToByteArray()
        {
            byte[] destinationArray = new byte[this.end - this.start];
            Array.Copy(this.buf, this.start, destinationArray, 0, destinationArray.Length);
            this.start = 0;
            this.end = 0;
            return destinationArray;
        }

        public void WriteBits(int b, int count)
        {
            this.bits |= (uint) (b << this.bitCount);
            this.bitCount += count;
            if (this.bitCount >= 0x10)
            {
                this.buf[this.end++] = (byte) this.bits;
                this.buf[this.end++] = (byte) (this.bits >> 8);
                this.bits = this.bits >> 0x10;
                this.bitCount -= 0x10;
            }
        }

        public void WriteBlock(byte[] block, int offset, int len)
        {
            Array.Copy(block, offset, this.buf, this.end, len);
            this.end += len;
        }

        public void WriteByte(int b)
        {
            this.buf[this.end++] = (byte) b;
        }

        public void WriteInt(int s)
        {
            this.buf[this.end++] = (byte) s;
            this.buf[this.end++] = (byte) (s >> 8);
            this.buf[this.end++] = (byte) (s >> 0x10);
            this.buf[this.end++] = (byte) (s >> 0x18);
        }

        public void WriteShort(int s)
        {
            this.buf[this.end++] = (byte) s;
            this.buf[this.end++] = (byte) (s >> 8);
        }

        public void WriteShortMSB(int s)
        {
            this.buf[this.end++] = (byte) (s >> 8);
            this.buf[this.end++] = (byte) s;
        }

        public int BitCount
        {
            get
            {
                return this.bitCount;
            }
        }

        public bool IsFlushed
        {
            get
            {
                return (this.end == 0);
            }
        }
    }
}

