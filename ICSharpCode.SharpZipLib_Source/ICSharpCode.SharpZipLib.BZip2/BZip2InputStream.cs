namespace ICSharpCode.SharpZipLib.BZip2
{
    using ICSharpCode.SharpZipLib.Checksums;
    using System;
    using System.IO;

    public class BZip2InputStream : Stream
    {
        private int[][] baseArray = new int[BZip2Constants.N_GROUPS][];
        private Stream baseStream;
        private bool blockRandomised;
        private int blockSize100k;
        private int bsBuff;
        private int bsLive;
        private int ch2;
        private int chPrev;
        private int computedBlockCRC;
        private uint computedCombinedCRC;
        private int count;
        private int currentChar = -1;
        private int currentState = 1;
        private int i2;
        private bool[] inUse = new bool[0x100];
        private int j2;
        private int last;
        private int[][] limit = new int[BZip2Constants.N_GROUPS][];
        private byte[] ll8;
        private IChecksum mCrc = new StrangeCRC();
        private int[] minLens = new int[BZip2Constants.N_GROUPS];
        private int nInUse;
        private const int NO_RAND_PART_A_STATE = 5;
        private const int NO_RAND_PART_B_STATE = 6;
        private const int NO_RAND_PART_C_STATE = 7;
        private int origPtr;
        private int[][] perm = new int[BZip2Constants.N_GROUPS][];
        private const int RAND_PART_A_STATE = 2;
        private const int RAND_PART_B_STATE = 3;
        private const int RAND_PART_C_STATE = 4;
        private int rNToGo = 0;
        private int rTPos = 0;
        private byte[] selector = new byte[BZip2Constants.MAX_SELECTORS];
        private byte[] selectorMtf = new byte[BZip2Constants.MAX_SELECTORS];
        private byte[] seqToUnseq = new byte[0x100];
        private const int START_BLOCK_STATE = 1;
        private int storedBlockCRC;
        private int storedCombinedCRC;
        private bool streamEnd = false;
        private int tPos;
        private int[] tt;
        private byte[] unseqToSeq = new byte[0x100];
        private int[] unzftab = new int[0x100];
        private byte z;

        public BZip2InputStream(Stream stream)
        {
            for (int i = 0; i < BZip2Constants.N_GROUPS; i++)
            {
                this.limit[i] = new int[BZip2Constants.MAX_ALPHA_SIZE];
                this.baseArray[i] = new int[BZip2Constants.MAX_ALPHA_SIZE];
                this.perm[i] = new int[BZip2Constants.MAX_ALPHA_SIZE];
            }
            this.ll8 = null;
            this.tt = null;
            this.BsSetStream(stream);
            this.Initialize();
            this.InitBlock();
            this.SetupBlock();
        }

        private static void BadBlockHeader()
        {
            throw new BZip2Exception("BZip2 input stream bad block header");
        }

        private static void BlockOverrun()
        {
            throw new BZip2Exception("BZip2 input stream block overrun");
        }

        private int BsGetint()
        {
            int num = 0;
            num = (num << 8) | this.BsR(8);
            num = (num << 8) | this.BsR(8);
            num = (num << 8) | this.BsR(8);
            return ((num << 8) | this.BsR(8));
        }

        private int BsGetInt32()
        {
            return this.BsGetint();
        }

        private int BsGetIntVS(int numBits)
        {
            return this.BsR(numBits);
        }

        private char BsGetUChar()
        {
            return (char) this.BsR(8);
        }

        private int BsR(int n)
        {
            while (this.bsLive < n)
            {
                this.FillBuffer();
            }
            int num = (this.bsBuff >> (this.bsLive - n)) & ((((int) 1) << n) - 1);
            this.bsLive -= n;
            return num;
        }

        private void BsSetStream(Stream f)
        {
            this.baseStream = f;
            this.bsLive = 0;
            this.bsBuff = 0;
        }

        public override void Close()
        {
            if (this.baseStream != null)
            {
                this.baseStream.Close();
            }
        }

        private void Complete()
        {
            this.storedCombinedCRC = this.BsGetInt32();
            if (this.storedCombinedCRC != this.computedCombinedCRC)
            {
                CrcError();
            }
            this.streamEnd = true;
        }

        private static void CompressedStreamEOF()
        {
            throw new BZip2Exception("BZip2 input stream end of compressed stream");
        }

        private static void CrcError()
        {
            throw new BZip2Exception("BZip2 input stream crc error");
        }

        private void EndBlock()
        {
            this.computedBlockCRC = (int) this.mCrc.Value;
            if (this.storedBlockCRC != this.computedBlockCRC)
            {
                CrcError();
            }
            this.computedCombinedCRC = ((this.computedCombinedCRC << 1) & uint.MaxValue) | (this.computedCombinedCRC >> 0x1f);
            this.computedCombinedCRC ^= (uint) this.computedBlockCRC;
        }

        private void FillBuffer()
        {
            int num = 0;
            try
            {
                num = this.baseStream.ReadByte();
            }
            catch (Exception)
            {
                CompressedStreamEOF();
            }
            if (num == -1)
            {
                CompressedStreamEOF();
            }
            this.bsBuff = (this.bsBuff << 8) | (num & 0xff);
            this.bsLive += 8;
        }

        public override void Flush()
        {
            if (this.baseStream != null)
            {
                this.baseStream.Flush();
            }
        }

        private void GetAndMoveToFrontDecode()
        {
            int num11;
            byte[] buffer = new byte[0x100];
            int num2 = BZip2Constants.baseBlockSize * this.blockSize100k;
            this.origPtr = this.BsGetIntVS(0x18);
            this.RecvDecodingTables();
            int num3 = this.nInUse + 1;
            int index = -1;
            int num5 = 0;
            for (int i = 0; i <= 0xff; i++)
            {
                this.unzftab[i] = 0;
            }
            for (int j = 0; j <= 0xff; j++)
            {
                buffer[j] = (byte) j;
            }
            this.last = -1;
            if (num5 == 0)
            {
                index++;
                num5 = BZip2Constants.G_SIZE;
            }
            num5--;
            int num8 = this.selector[index];
            int n = this.minLens[num8];
            int num10 = this.BsR(n);
            while (num10 > this.limit[num8][n])
            {
                if (n > 20)
                {
                    throw new BZip2Exception("Bzip data error");
                }
                n++;
                while (this.bsLive < 1)
                {
                    this.FillBuffer();
                }
                num11 = (this.bsBuff >> (this.bsLive - 1)) & 1;
                this.bsLive--;
                num10 = (num10 << 1) | num11;
            }
            if (((num10 - this.baseArray[num8][n]) < 0) || ((num10 - this.baseArray[num8][n]) >= BZip2Constants.MAX_ALPHA_SIZE))
            {
                throw new BZip2Exception("Bzip data error");
            }
            int num = this.perm[num8][num10 - this.baseArray[num8][n]];
            while (num != num3)
            {
                IntPtr ptr;
                if ((num == BZip2Constants.RUNA) || (num == BZip2Constants.RUNB))
                {
                    int num12 = -1;
                    int num13 = 1;
                    do
                    {
                        if (num == BZip2Constants.RUNA)
                        {
                            num12 += num13;
                        }
                        else if (num == BZip2Constants.RUNB)
                        {
                            num12 += 2 * num13;
                        }
                        num13 = num13 << 1;
                        if (num5 == 0)
                        {
                            index++;
                            num5 = BZip2Constants.G_SIZE;
                        }
                        num5--;
                        num8 = this.selector[index];
                        n = this.minLens[num8];
                        num10 = this.BsR(n);
                        while (num10 > this.limit[num8][n])
                        {
                            n++;
                            while (this.bsLive < 1)
                            {
                                this.FillBuffer();
                            }
                            num11 = (this.bsBuff >> (this.bsLive - 1)) & 1;
                            this.bsLive--;
                            num10 = (num10 << 1) | num11;
                        }
                        num = this.perm[num8][num10 - this.baseArray[num8][n]];
                    }
                    while ((num == BZip2Constants.RUNA) || (num == BZip2Constants.RUNB));
                    num12++;
                    byte num14 = this.seqToUnseq[buffer[0]];
                    this.unzftab[(int) (ptr = (IntPtr) num14)] = this.unzftab[(int) ptr] + num12;
                    while (num12 > 0)
                    {
                        this.last++;
                        this.ll8[this.last] = num14;
                        num12--;
                    }
                    if (this.last >= num2)
                    {
                        BlockOverrun();
                    }
                }
                else
                {
                    this.last++;
                    if (this.last >= num2)
                    {
                        BlockOverrun();
                    }
                    byte num15 = buffer[num - 1];
                    this.unzftab[(int) (ptr = (IntPtr) this.seqToUnseq[num15])] = this.unzftab[(int) ptr] + 1;
                    this.ll8[this.last] = this.seqToUnseq[num15];
                    for (int k = num - 1; k > 0; k--)
                    {
                        buffer[k] = buffer[k - 1];
                    }
                    buffer[0] = num15;
                    if (num5 == 0)
                    {
                        index++;
                        num5 = BZip2Constants.G_SIZE;
                    }
                    num5--;
                    num8 = this.selector[index];
                    n = this.minLens[num8];
                    num10 = this.BsR(n);
                    while (num10 > this.limit[num8][n])
                    {
                        n++;
                        while (this.bsLive < 1)
                        {
                            this.FillBuffer();
                        }
                        num11 = (this.bsBuff >> (this.bsLive - 1)) & 1;
                        this.bsLive--;
                        num10 = (num10 << 1) | num11;
                    }
                    num = this.perm[num8][num10 - this.baseArray[num8][n]];
                }
            }
        }

        private void HbCreateDecodeTables(int[] limit, int[] baseArray, int[] perm, char[] length, int minLen, int maxLen, int alphaSize)
        {
            int index = 0;
            for (int i = minLen; i <= maxLen; i++)
            {
                for (int num3 = 0; num3 < alphaSize; num3++)
                {
                    if (length[num3] == i)
                    {
                        perm[index] = num3;
                        index++;
                    }
                }
            }
            for (int j = 0; j < BZip2Constants.MAX_CODE_LEN; j++)
            {
                baseArray[j] = 0;
            }
            for (int k = 0; k < alphaSize; k++)
            {
                IntPtr ptr;
                baseArray[(int) (ptr = (IntPtr) (length[k] + '\x0001'))] = baseArray[(int) ptr] + 1;
            }
            for (int m = 1; m < BZip2Constants.MAX_CODE_LEN; m++)
            {
                baseArray[m] += baseArray[m - 1];
            }
            for (int n = 0; n < BZip2Constants.MAX_CODE_LEN; n++)
            {
                limit[n] = 0;
            }
            int num8 = 0;
            for (int num9 = minLen; num9 <= maxLen; num9++)
            {
                num8 += baseArray[num9 + 1] - baseArray[num9];
                limit[num9] = num8 - 1;
                num8 = num8 << 1;
            }
            for (int num10 = minLen + 1; num10 <= maxLen; num10++)
            {
                baseArray[num10] = ((limit[num10 - 1] + 1) << 1) - baseArray[num10];
            }
        }

        private void InitBlock()
        {
            char ch = this.BsGetUChar();
            char ch2 = this.BsGetUChar();
            char ch3 = this.BsGetUChar();
            char ch4 = this.BsGetUChar();
            char ch5 = this.BsGetUChar();
            char ch6 = this.BsGetUChar();
            if ((((ch == '\x0017') && (ch2 == 'r')) && ((ch3 == 'E') && (ch4 == '8'))) && ((ch5 == 'P') && (ch6 == '\x0090')))
            {
                this.Complete();
            }
            else if ((((ch != '1') || (ch2 != 'A')) || ((ch3 != 'Y') || (ch4 != '&'))) || ((ch5 != 'S') || (ch6 != 'Y')))
            {
                BadBlockHeader();
                this.streamEnd = true;
            }
            else
            {
                this.storedBlockCRC = this.BsGetInt32();
                this.blockRandomised = this.BsR(1) == 1;
                this.GetAndMoveToFrontDecode();
                this.mCrc.Reset();
                this.currentState = 1;
            }
        }

        private void Initialize()
        {
            char ch = this.BsGetUChar();
            char ch2 = this.BsGetUChar();
            char ch3 = this.BsGetUChar();
            char ch4 = this.BsGetUChar();
            if (((ch != 'B') || (ch2 != 'Z')) || (((ch3 != 'h') || (ch4 < '1')) || (ch4 > '9')))
            {
                this.streamEnd = true;
            }
            else
            {
                this.SetDecompressStructureSizes(ch4 - '0');
                this.computedCombinedCRC = 0;
            }
        }

        private void MakeMaps()
        {
            this.nInUse = 0;
            for (int i = 0; i < 0x100; i++)
            {
                if (this.inUse[i])
                {
                    this.seqToUnseq[this.nInUse] = (byte) i;
                    this.unseqToSeq[i] = (byte) this.nInUse;
                    this.nInUse++;
                }
            }
        }

        public override int Read(byte[] b, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int num2 = this.ReadByte();
                if (num2 == -1)
                {
                    return i;
                }
                b[offset + i] = (byte) num2;
            }
            return count;
        }

        public override int ReadByte()
        {
            if (this.streamEnd)
            {
                return -1;
            }
            int currentChar = this.currentChar;
            switch (this.currentState)
            {
                case 1:
                case 2:
                case 5:
                    return currentChar;

                case 3:
                    this.SetupRandPartB();
                    return currentChar;

                case 4:
                    this.SetupRandPartC();
                    return currentChar;

                case 6:
                    this.SetupNoRandPartB();
                    return currentChar;

                case 7:
                    this.SetupNoRandPartC();
                    return currentChar;
            }
            return currentChar;
        }

        private void RecvDecodingTables()
        {
            char[][] chArray = new char[BZip2Constants.N_GROUPS][];
            for (int i = 0; i < BZip2Constants.N_GROUPS; i++)
            {
                chArray[i] = new char[BZip2Constants.MAX_ALPHA_SIZE];
            }
            bool[] flagArray = new bool[0x10];
            for (int j = 0; j < 0x10; j++)
            {
                flagArray[j] = this.BsR(1) == 1;
            }
            for (int k = 0; k < 0x10; k++)
            {
                if (flagArray[k])
                {
                    for (int num4 = 0; num4 < 0x10; num4++)
                    {
                        this.inUse[(k * 0x10) + num4] = this.BsR(1) == 1;
                    }
                }
                else
                {
                    for (int num5 = 0; num5 < 0x10; num5++)
                    {
                        this.inUse[(k * 0x10) + num5] = false;
                    }
                }
            }
            this.MakeMaps();
            int alphaSize = this.nInUse + 2;
            int num7 = this.BsR(3);
            int num8 = this.BsR(15);
            for (int m = 0; m < num8; m++)
            {
                int num10 = 0;
                while (this.BsR(1) == 1)
                {
                    num10++;
                }
                this.selectorMtf[m] = (byte) num10;
            }
            byte[] buffer = new byte[BZip2Constants.N_GROUPS];
            for (int n = 0; n < num7; n++)
            {
                buffer[n] = (byte) n;
            }
            for (int num12 = 0; num12 < num8; num12++)
            {
                int index = this.selectorMtf[num12];
                byte num14 = buffer[index];
                while (index > 0)
                {
                    buffer[index] = buffer[index - 1];
                    index--;
                }
                buffer[0] = num14;
                this.selector[num12] = num14;
            }
            for (int num15 = 0; num15 < num7; num15++)
            {
                int num16 = this.BsR(5);
                int num17 = 0;
                goto Label_01B9;
            Label_0188:
                if (this.BsR(1) == 0)
                {
                    num16++;
                }
                else
                {
                    num16--;
                }
            Label_019F:
                if (this.BsR(1) == 1)
                {
                    goto Label_0188;
                }
                chArray[num15][num17] = (char) num16;
                num17++;
            Label_01B9:
                if (num17 < alphaSize)
                {
                    goto Label_019F;
                }
            }
            for (int num18 = 0; num18 < num7; num18++)
            {
                int num19 = 0x20;
                int num20 = 0;
                for (int num21 = 0; num21 < alphaSize; num21++)
                {
                    num20 = Math.Max(num20, chArray[num18][num21]);
                    num19 = Math.Min(num19, chArray[num18][num21]);
                }
                this.HbCreateDecodeTables(this.limit[num18], this.baseArray[num18], this.perm[num18], chArray[num18], num19, num20, alphaSize);
                this.minLens[num18] = num19;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("BZip2InputStream Seek not supported");
        }

        private void SetDecompressStructureSizes(int newSize100k)
        {
            if (((0 > newSize100k) || (newSize100k > 9)) || ((0 > this.blockSize100k) || (this.blockSize100k > 9)))
            {
                throw new BZip2Exception("Invalid block size");
            }
            this.blockSize100k = newSize100k;
            if (newSize100k != 0)
            {
                int num = BZip2Constants.baseBlockSize * newSize100k;
                this.ll8 = new byte[num];
                this.tt = new int[num];
            }
        }

        public override void SetLength(long val)
        {
            throw new NotSupportedException("BZip2InputStream SetLength not supported");
        }

        private void SetupBlock()
        {
            IntPtr ptr;
            int[] destinationArray = new int[0x101];
            destinationArray[0] = 0;
            Array.Copy(this.unzftab, 0, destinationArray, 1, 0x100);
            for (int i = 1; i <= 0x100; i++)
            {
                destinationArray[(int) (ptr = (IntPtr) i)] = destinationArray[(int) ptr] + destinationArray[i - 1];
            }
            for (int j = 0; j <= this.last; j++)
            {
                byte index = this.ll8[j];
                this.tt[destinationArray[index]] = j;
                destinationArray[(int) (ptr = (IntPtr) index)] = destinationArray[(int) ptr] + 1;
            }
            destinationArray = null;
            this.tPos = this.tt[this.origPtr];
            this.count = 0;
            this.i2 = 0;
            this.ch2 = 0x100;
            if (this.blockRandomised)
            {
                this.rNToGo = 0;
                this.rTPos = 0;
                this.SetupRandPartA();
            }
            else
            {
                this.SetupNoRandPartA();
            }
        }

        private void SetupNoRandPartA()
        {
            if (this.i2 <= this.last)
            {
                this.chPrev = this.ch2;
                this.ch2 = this.ll8[this.tPos];
                this.tPos = this.tt[this.tPos];
                this.i2++;
                this.currentChar = this.ch2;
                this.currentState = 6;
                this.mCrc.Update(this.ch2);
            }
            else
            {
                this.EndBlock();
                this.InitBlock();
                this.SetupBlock();
            }
        }

        private void SetupNoRandPartB()
        {
            if (this.ch2 != this.chPrev)
            {
                this.currentState = 5;
                this.count = 1;
                this.SetupNoRandPartA();
            }
            else
            {
                this.count++;
                if (this.count >= 4)
                {
                    this.z = this.ll8[this.tPos];
                    this.tPos = this.tt[this.tPos];
                    this.currentState = 7;
                    this.j2 = 0;
                    this.SetupNoRandPartC();
                }
                else
                {
                    this.currentState = 5;
                    this.SetupNoRandPartA();
                }
            }
        }

        private void SetupNoRandPartC()
        {
            if (this.j2 < this.z)
            {
                this.currentChar = this.ch2;
                this.mCrc.Update(this.ch2);
                this.j2++;
            }
            else
            {
                this.currentState = 5;
                this.i2++;
                this.count = 0;
                this.SetupNoRandPartA();
            }
        }

        private void SetupRandPartA()
        {
            if (this.i2 <= this.last)
            {
                this.chPrev = this.ch2;
                this.ch2 = this.ll8[this.tPos];
                this.tPos = this.tt[this.tPos];
                if (this.rNToGo == 0)
                {
                    this.rNToGo = BZip2Constants.rNums[this.rTPos];
                    this.rTPos++;
                    if (this.rTPos == 0x200)
                    {
                        this.rTPos = 0;
                    }
                }
                this.rNToGo--;
                this.ch2 ^= (this.rNToGo == 1) ? 1 : 0;
                this.i2++;
                this.currentChar = this.ch2;
                this.currentState = 3;
                this.mCrc.Update(this.ch2);
            }
            else
            {
                this.EndBlock();
                this.InitBlock();
                this.SetupBlock();
            }
        }

        private void SetupRandPartB()
        {
            if (this.ch2 != this.chPrev)
            {
                this.currentState = 2;
                this.count = 1;
                this.SetupRandPartA();
            }
            else
            {
                this.count++;
                if (this.count >= 4)
                {
                    this.z = this.ll8[this.tPos];
                    this.tPos = this.tt[this.tPos];
                    if (this.rNToGo == 0)
                    {
                        this.rNToGo = BZip2Constants.rNums[this.rTPos];
                        this.rTPos++;
                        if (this.rTPos == 0x200)
                        {
                            this.rTPos = 0;
                        }
                    }
                    this.rNToGo--;
                    this.z = (byte) (this.z ^ ((this.rNToGo == 1) ? ((byte) 1) : ((byte) 0)));
                    this.j2 = 0;
                    this.currentState = 4;
                    this.SetupRandPartC();
                }
                else
                {
                    this.currentState = 2;
                    this.SetupRandPartA();
                }
            }
        }

        private void SetupRandPartC()
        {
            if (this.j2 < this.z)
            {
                this.currentChar = this.ch2;
                this.mCrc.Update(this.ch2);
                this.j2++;
            }
            else
            {
                this.currentState = 2;
                this.i2++;
                this.count = 0;
                this.SetupRandPartA();
            }
        }

        public override void Write(byte[] array, int offset, int count)
        {
            throw new NotSupportedException("BZip2InputStream Write not supported");
        }

        public override void WriteByte(byte val)
        {
            throw new NotSupportedException("BZip2InputStream WriteByte not supported");
        }

        public override bool CanRead
        {
            get
            {
                return this.baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.baseStream.CanSeek;
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
                return this.baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.baseStream.Position;
            }
            set
            {
                throw new NotSupportedException("BZip2InputStream position cannot be set");
            }
        }
    }
}

