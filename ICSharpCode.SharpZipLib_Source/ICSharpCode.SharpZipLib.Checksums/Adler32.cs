namespace ICSharpCode.SharpZipLib.Checksums
{
    using System;

    public sealed class Adler32 : IChecksum
    {
        private static readonly uint BASE = 0xfff1;
        private uint checksum;

        public Adler32()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.checksum = 1;
        }

        public void Update(int bval)
        {
            uint num = this.checksum & 0xffff;
            uint num2 = this.checksum >> 0x10;
            num = (uint) ((num + (bval & 0xff)) % BASE);
            num2 = (num + num2) % BASE;
            this.checksum = (num2 << 0x10) + num;
        }

        public void Update(byte[] buffer)
        {
            this.Update(buffer, 0, buffer.Length);
        }

        public void Update(byte[] buf, int off, int len)
        {
            if (buf == null)
            {
                throw new ArgumentNullException("buf");
            }
            if (((off < 0) || (len < 0)) || ((off + len) > buf.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            uint num = this.checksum & 0xffff;
            uint num2 = this.checksum >> 0x10;
            while (len > 0)
            {
                int num3 = 0xed8;
                if (num3 > len)
                {
                    num3 = len;
                }
                len -= num3;
                while (--num3 >= 0)
                {
                    num += (uint) (buf[off++] & 0xff);
                    num2 += num;
                }
                num = num % BASE;
                num2 = num2 % BASE;
            }
            this.checksum = (num2 << 0x10) | num;
        }

        public long Value
        {
            get
            {
                return (long) this.checksum;
            }
        }
    }
}

