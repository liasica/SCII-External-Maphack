namespace Foole.Mpq
{
    using System;
    using System.IO;

    internal class BitStream
    {
        private Stream _baseStream;
        private int _bitCount;
        private int _current;

        public BitStream(Stream sourceStream)
        {
            this._baseStream = sourceStream;
        }

        public bool EnsureBits(int bitCount)
        {
            if (bitCount > this._bitCount)
            {
                if (this._baseStream.Position >= this._baseStream.Length)
                {
                    return false;
                }
                int num = this._baseStream.ReadByte();
                this._current |= num << this._bitCount;
                this._bitCount += 8;
            }
            return true;
        }

        public int PeekByte()
        {
            if (!this.EnsureBits(8))
            {
                return -1;
            }
            return (this._current & 0xff);
        }

        public int ReadBits(int bitCount)
        {
            if (bitCount > 0x10)
            {
                throw new ArgumentOutOfRangeException("BitCount", "Maximum BitCount is 16");
            }
            if (!this.EnsureBits(bitCount))
            {
                return -1;
            }
            int num = this._current & (((int) 0xffff) >> (0x10 - bitCount));
            this.WasteBits(bitCount);
            return num;
        }

        private bool WasteBits(int bitCount)
        {
            this._current = this._current >> bitCount;
            this._bitCount -= bitCount;
            return true;
        }
    }
}

