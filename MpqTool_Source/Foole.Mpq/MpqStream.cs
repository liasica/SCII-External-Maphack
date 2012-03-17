namespace Foole.Mpq
{
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.IO;

    public class MpqStream : Stream
    {
        private uint[] _blockPositions;
        private int _blockSize;
        private int _currentBlockIndex = -1;
        private byte[] _currentData;
        private MpqEntry _entry;
        private long _position;
        private Stream _stream;

        internal MpqStream(MpqArchive archive, MpqEntry entry)
        {
            this._entry = entry;
            this._stream = archive.BaseStream;
            this._blockSize = archive.BlockSize;
            if (this._entry.IsCompressed && !this._entry.IsSingleUnit)
            {
                this.LoadBlockPositions();
            }
        }

        private void BufferData()
        {
            int blockIndex = (int) (this._position / ((long) this._blockSize));
            if (blockIndex != this._currentBlockIndex)
            {
                int expectedLength = (int) Math.Min(this.Length - (blockIndex * this._blockSize), (long) this._blockSize);
                this._currentData = this.LoadBlock(blockIndex, expectedLength);
                this._currentBlockIndex = blockIndex;
            }
        }

        private static byte[] BZip2Decompress(Stream data, int expectedLength)
        {
            MemoryStream outstream = new MemoryStream(expectedLength);
            ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(data, outstream);
            return outstream.ToArray();
        }

        private static byte[] DecompressMulti(byte[] input, int outputLength)
        {
            Stream data = new MemoryStream(input);
            byte num = (byte) data.ReadByte();
            switch (num)
            {
                case 0x10:
                    return BZip2Decompress(data, outputLength);

                case 0x12:
                    throw new MpqParserException("LZMA compression is not yet supported");

                case 0x22:
                    throw new MpqParserException("Sparse compression + Deflate compression is not yet supported");

                case 1:
                    return MpqHuffman.Decompress(data).ToArray();

                case 2:
                    return ZlibDecompress(data, outputLength);

                case 8:
                    return PKDecompress(data, outputLength);

                case 0x40:
                    return MpqWavCompression.Decompress(data, 1);

                case 0x41:
                    return MpqWavCompression.Decompress(MpqHuffman.Decompress(data), 1);

                case 0x30:
                    throw new MpqParserException("Sparse compression + BZip2 compression is not yet supported");

                case 0x80:
                    return MpqWavCompression.Decompress(data, 2);

                case 0x81:
                    return MpqWavCompression.Decompress(MpqHuffman.Decompress(data), 2);

                case 0x88:
                    return MpqWavCompression.Decompress(new MemoryStream(PKDecompress(data, outputLength)), 2);

                case 0x48:
                    return MpqWavCompression.Decompress(new MemoryStream(PKDecompress(data, outputLength)), 1);
            }
            throw new MpqParserException("Compression is not yet supported: 0x" + num.ToString("X"));
        }

        public override void Flush()
        {
        }

        private byte[] LoadBlock(int blockIndex, int expectedLength)
        {
            uint num;
            int num2;
            if (this._entry.IsCompressed)
            {
                num = this._blockPositions[blockIndex];
                num2 = (int) (this._blockPositions[blockIndex + 1] - num);
            }
            else
            {
                num = (uint) (blockIndex * this._blockSize);
                num2 = expectedLength;
            }
            num += this._entry.FilePos;
            byte[] buffer = new byte[num2];
            lock (this._stream)
            {
                this._stream.Seek((long) num, SeekOrigin.Begin);
                if (this._stream.Read(buffer, 0, num2) != num2)
                {
                    throw new MpqParserException("Insufficient data or invalid data length");
                }
            }
            if (this._entry.IsEncrypted && (this._entry.FileSize > 3))
            {
                if (this._entry.EncryptionSeed == 0)
                {
                    throw new MpqParserException("Unable to determine encryption key");
                }
                uint num3 = ((uint) blockIndex) + this._entry.EncryptionSeed;
                MpqArchive.DecryptBlock(buffer, num3);
            }
            if (!this._entry.IsCompressed || (num2 == expectedLength))
            {
                return buffer;
            }
            if ((this._entry.Flags & MpqFileFlags.CompressedMulti) != ((MpqFileFlags) 0))
            {
                return DecompressMulti(buffer, expectedLength);
            }
            return PKDecompress(new MemoryStream(buffer), expectedLength);
        }

        private void LoadBlockPositions()
        {
            int num = ((int) (((this._entry.FileSize + this._blockSize) - 1) / ((long) this._blockSize))) + 1;
            if ((this._entry.Flags & MpqFileFlags.FileHasMetadata) != ((MpqFileFlags) 0))
            {
                num++;
            }
            this._blockPositions = new uint[num];
            lock (this._stream)
            {
                this._stream.Seek((long) this._entry.FilePos, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(this._stream);
                for (int i = 0; i < num; i++)
                {
                    this._blockPositions[i] = reader.ReadUInt32();
                }
            }
            uint decrypted = (uint) (num * 4);
            if (this._entry.IsEncrypted)
            {
                if (this._entry.EncryptionSeed == 0)
                {
                    this._entry.EncryptionSeed = MpqArchive.DetectFileSeed(this._blockPositions[0], this._blockPositions[1], decrypted) + 1;
                    if (this._entry.EncryptionSeed == 1)
                    {
                        throw new MpqParserException("Unable to determine encyption seed");
                    }
                }
                MpqArchive.DecryptBlock(this._blockPositions, this._entry.EncryptionSeed - 1);
                if (this._blockPositions[0] != decrypted)
                {
                    throw new MpqParserException("Decryption failed");
                }
                if (this._blockPositions[1] > (this._blockSize + decrypted))
                {
                    throw new MpqParserException("Decryption failed");
                }
            }
        }

        private void LoadSingleUnit()
        {
            byte[] buffer = new byte[this._entry.CompressedSize];
            lock (this._stream)
            {
                this._stream.Seek((long) this._entry.FilePos, SeekOrigin.Begin);
                if (this._stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    throw new MpqParserException("Insufficient data or invalid data length");
                }
            }
            if (this._entry.CompressedSize == this._entry.FileSize)
            {
                this._currentData = buffer;
            }
            else
            {
                this._currentData = DecompressMulti(buffer, (int) this._entry.FileSize);
            }
        }

        private static byte[] PKDecompress(Stream data, int expectedLength)
        {
            PKLibDecompress decompress = new PKLibDecompress(data);
            return decompress.Explode(expectedLength);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this._entry.IsSingleUnit)
            {
                return this.ReadInternalSingleUnit(buffer, offset, count);
            }
            int num = count;
            int num2 = 0;
            while (num > 0)
            {
                int num3 = this.ReadInternal(buffer, offset, num);
                if (num3 == 0)
                {
                    return num2;
                }
                num2 += num3;
                offset += num3;
                num -= num3;
            }
            return num2;
        }

        public override int ReadByte()
        {
            if (this._position >= this.Length)
            {
                return -1;
            }
            if (this._entry.IsSingleUnit)
            {
                return this.ReadByteSingleUnit();
            }
            this.BufferData();
            int index = (int) (this._position % ((long) this._blockSize));
            this._position += 1L;
            return this._currentData[index];
        }

        private int ReadByteSingleUnit()
        {
            long num;
            if (this._currentData == null)
            {
                this.LoadSingleUnit();
            }
            this._position = (num = this._position) + 1L;
            return this._currentData[(int) ((IntPtr) num)];
        }

        private int ReadInternal(byte[] buffer, int offset, int count)
        {
            if (this._position >= this.Length)
            {
                return 0;
            }
            this.BufferData();
            int sourceIndex = (int) (this._position % ((long) this._blockSize));
            int length = Math.Min(this._currentData.Length - sourceIndex, count);
            if (length <= 0)
            {
                return 0;
            }
            Array.Copy(this._currentData, sourceIndex, buffer, offset, length);
            this._position += length;
            return length;
        }

        private int ReadInternalSingleUnit(byte[] buffer, int offset, int count)
        {
            if (this._position >= this.Length)
            {
                return 0;
            }
            if (this._currentData == null)
            {
                this.LoadSingleUnit();
            }
            int num = Math.Min(this._currentData.Length - ((int) this._position), count);
            Array.Copy(this._currentData, this._position, buffer, (long) offset, (long) num);
            this._position += num;
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long num;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    num = offset;
                    break;

                case SeekOrigin.Current:
                    num = this.Position + offset;
                    break;

                case SeekOrigin.End:
                    num = this.Length + offset;
                    break;

                default:
                    throw new ArgumentException("Origin", "Invalid SeekOrigin");
            }
            if (num < 0L)
            {
                throw new ArgumentOutOfRangeException("Attmpted to Seek before the beginning of the stream");
            }
            if (num >= this.Length)
            {
                throw new ArgumentOutOfRangeException("Attmpted to Seek beyond the end of the stream");
            }
            this._position = num;
            return this._position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Writing is not supported");
        }

        private static byte[] ZlibDecompress(Stream data, int expectedLength)
        {
            byte[] buffer = new byte[expectedLength];
            Stream stream = new InflaterInputStream(data);
            int offset = 0;
            while (expectedLength > 0)
            {
                int num2 = stream.Read(buffer, offset, expectedLength);
                if (num2 == 0)
                {
                    return buffer;
                }
                offset += num2;
                expectedLength -= num2;
            }
            return buffer;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return (long) this._entry.FileSize;
            }
        }

        public override long Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }
    }
}

