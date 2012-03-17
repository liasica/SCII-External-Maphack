namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
    using System;

    public class StreamManipulator
    {
        private int bits_in_buffer = 0;
        private uint buffer = 0;
        private byte[] window;
        private int window_end = 0;
        private int window_start = 0;

        public int CopyBytes(byte[] output, int offset, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            if ((this.bits_in_buffer & 7) != 0)
            {
                throw new InvalidOperationException("Bit buffer is not byte aligned!");
            }
            int num = 0;
            while ((this.bits_in_buffer > 0) && (length > 0))
            {
                output[offset++] = (byte) this.buffer;
                this.buffer = this.buffer >> 8;
                this.bits_in_buffer -= 8;
                length--;
                num++;
            }
            if (length == 0)
            {
                return num;
            }
            int num2 = this.window_end - this.window_start;
            if (length > num2)
            {
                length = num2;
            }
            Array.Copy(this.window, this.window_start, output, offset, length);
            this.window_start += length;
            if (((this.window_start - this.window_end) & 1) != 0)
            {
                this.buffer = (uint) (this.window[this.window_start++] & 0xff);
                this.bits_in_buffer = 8;
            }
            return (num + length);
        }

        public void DropBits(int n)
        {
            this.buffer = this.buffer >> n;
            this.bits_in_buffer -= n;
        }

        public int GetBits(int n)
        {
            int num = this.PeekBits(n);
            if (num >= 0)
            {
                this.DropBits(n);
            }
            return num;
        }

        public int PeekBits(int n)
        {
            if (this.bits_in_buffer < n)
            {
                if (this.window_start == this.window_end)
                {
                    return -1;
                }
                this.buffer |= (uint) (((this.window[this.window_start++] & 0xff) | ((this.window[this.window_start++] & 0xff) << 8)) << this.bits_in_buffer);
                this.bits_in_buffer += 0x10;
            }
            return (((int) this.buffer) & ((((int) 1) << n) - 1));
        }

        public void Reset()
        {
            this.buffer = (uint) (this.window_start = this.window_end = this.bits_in_buffer = 0);
        }

        public void SetInput(byte[] buf, int off, int len)
        {
            if (this.window_start < this.window_end)
            {
                throw new InvalidOperationException("Old input was not completely processed");
            }
            int num = off + len;
            if (((0 > off) || (off > num)) || (num > buf.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((len & 1) != 0)
            {
                this.buffer |= (uint) ((buf[off++] & 0xff) << this.bits_in_buffer);
                this.bits_in_buffer += 8;
            }
            this.window = buf;
            this.window_start = off;
            this.window_end = num;
        }

        public void SkipToByteBoundary()
        {
            this.buffer = this.buffer >> (this.bits_in_buffer & 7);
            this.bits_in_buffer &= -8;
        }

        public int AvailableBits
        {
            get
            {
                return this.bits_in_buffer;
            }
        }

        public int AvailableBytes
        {
            get
            {
                return ((this.window_end - this.window_start) + (this.bits_in_buffer >> 3));
            }
        }

        public bool IsNeedingInput
        {
            get
            {
                return (this.window_start == this.window_end);
            }
        }
    }
}

