namespace ICSharpCode.SharpZipLib.Zip.Compression
{
    using ICSharpCode.SharpZipLib;
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;

    public class Inflater
    {
        private Adler32 adler;
        private static int[] CPDEXT = new int[] { 
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 
            7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
         };
        private static int[] CPDIST = new int[] { 
            1, 2, 3, 4, 5, 7, 9, 13, 0x11, 0x19, 0x21, 0x31, 0x41, 0x61, 0x81, 0xc1, 
            0x101, 0x181, 0x201, 0x301, 0x401, 0x601, 0x801, 0xc01, 0x1001, 0x1801, 0x2001, 0x3001, 0x4001, 0x6001
         };
        private static int[] CPLENS = new int[] { 
            3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 0x11, 0x13, 0x17, 0x1b, 0x1f, 
            0x23, 0x2b, 0x33, 0x3b, 0x43, 0x53, 0x63, 0x73, 0x83, 0xa3, 0xc3, 0xe3, 0x102
         };
        private static int[] CPLEXT = new int[] { 
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
         };
        private const int DECODE_BLOCKS = 2;
        private const int DECODE_CHKSUM = 11;
        private const int DECODE_DICT = 1;
        private const int DECODE_DYN_HEADER = 6;
        private const int DECODE_HEADER = 0;
        private const int DECODE_HUFFMAN = 7;
        private const int DECODE_HUFFMAN_DIST = 9;
        private const int DECODE_HUFFMAN_DISTBITS = 10;
        private const int DECODE_HUFFMAN_LENBITS = 8;
        private const int DECODE_STORED = 5;
        private const int DECODE_STORED_LEN1 = 3;
        private const int DECODE_STORED_LEN2 = 4;
        private InflaterHuffmanTree distTree;
        private InflaterDynHeader dynHeader;
        private const int FINISHED = 12;
        private StreamManipulator input;
        private bool isLastBlock;
        private InflaterHuffmanTree litlenTree;
        private int mode;
        private int neededBits;
        private bool noHeader;
        private OutputWindow outputWindow;
        private int readAdler;
        private int repDist;
        private int repLength;
        private int totalIn;
        private int totalOut;
        private int uncomprLen;

        public Inflater() : this(false)
        {
        }

        public Inflater(bool noHeader)
        {
            this.noHeader = noHeader;
            this.adler = new Adler32();
            this.input = new StreamManipulator();
            this.outputWindow = new OutputWindow();
            this.mode = noHeader ? 2 : 0;
        }

        private bool Decode()
        {
            int num2;
            int num3;
            switch (this.mode)
            {
                case 0:
                    return this.DecodeHeader();

                case 1:
                    return this.DecodeDict();

                case 2:
                    if (!this.isLastBlock)
                    {
                        int num = this.input.PeekBits(3);
                        if (num < 0)
                        {
                            return false;
                        }
                        this.input.DropBits(3);
                        if ((num & 1) != 0)
                        {
                            this.isLastBlock = true;
                        }
                        switch ((num >> 1))
                        {
                            case 0:
                                this.input.SkipToByteBoundary();
                                this.mode = 3;
                                break;

                            case 1:
                                this.litlenTree = InflaterHuffmanTree.defLitLenTree;
                                this.distTree = InflaterHuffmanTree.defDistTree;
                                this.mode = 7;
                                break;

                            case 2:
                                this.dynHeader = new InflaterDynHeader();
                                this.mode = 6;
                                break;
                        }
                        throw new SharpZipBaseException("Unknown block type " + num);
                    }
                    if (!this.noHeader)
                    {
                        this.input.SkipToByteBoundary();
                        this.neededBits = 0x20;
                        this.mode = 11;
                        return true;
                    }
                    this.mode = 12;
                    return false;

                case 3:
                    this.uncomprLen = this.input.PeekBits(0x10);
                    if (this.uncomprLen >= 0)
                    {
                        this.input.DropBits(0x10);
                        this.mode = 4;
                        goto Label_0163;
                    }
                    return false;

                case 4:
                    goto Label_0163;

                case 5:
                    goto Label_01A5;

                case 6:
                    if (this.dynHeader.Decode(this.input))
                    {
                        this.litlenTree = this.dynHeader.BuildLitLenTree();
                        this.distTree = this.dynHeader.BuildDistTree();
                        this.mode = 7;
                        goto Label_0229;
                    }
                    return false;

                case 7:
                case 8:
                case 9:
                case 10:
                    goto Label_0229;

                case 11:
                    return this.DecodeChksum();

                case 12:
                    return false;

                default:
                    throw new SharpZipBaseException("Inflater.Decode unknown mode");
            }
            return true;
        Label_0163:
            num2 = this.input.PeekBits(0x10);
            if (num2 < 0)
            {
                return false;
            }
            this.input.DropBits(0x10);
            if (num2 != (this.uncomprLen ^ 0xffff))
            {
                throw new SharpZipBaseException("broken uncompressed block");
            }
            this.mode = 5;
        Label_01A5:
            num3 = this.outputWindow.CopyStored(this.input, this.uncomprLen);
            this.uncomprLen -= num3;
            if (this.uncomprLen == 0)
            {
                this.mode = 2;
                return true;
            }
            return !this.input.IsNeedingInput;
        Label_0229:
            return this.DecodeHuffman();
        }

        private bool DecodeChksum()
        {
            while (this.neededBits > 0)
            {
                int num = this.input.PeekBits(8);
                if (num < 0)
                {
                    return false;
                }
                this.input.DropBits(8);
                this.readAdler = (this.readAdler << 8) | num;
                this.neededBits -= 8;
            }
            if (((int) this.adler.Value) != this.readAdler)
            {
                throw new SharpZipBaseException(string.Concat(new object[] { "Adler chksum doesn't match: ", (int) this.adler.Value, " vs. ", this.readAdler }));
            }
            this.mode = 12;
            return false;
        }

        private bool DecodeDict()
        {
            while (this.neededBits > 0)
            {
                int num = this.input.PeekBits(8);
                if (num < 0)
                {
                    return false;
                }
                this.input.DropBits(8);
                this.readAdler = (this.readAdler << 8) | num;
                this.neededBits -= 8;
            }
            return false;
        }

        private bool DecodeHeader()
        {
            int num = this.input.PeekBits(0x10);
            if (num < 0)
            {
                return false;
            }
            this.input.DropBits(0x10);
            num = ((num << 8) | (num >> 8)) & 0xffff;
            if ((num % 0x1f) != 0)
            {
                throw new SharpZipBaseException("Header checksum illegal");
            }
            if ((num & 0xf00) != (Deflater.DEFLATED << 8))
            {
                throw new SharpZipBaseException("Compression Method unknown");
            }
            if ((num & 0x20) == 0)
            {
                this.mode = 2;
            }
            else
            {
                this.mode = 1;
                this.neededBits = 0x20;
            }
            return true;
        }

        private bool DecodeHuffman()
        {
            int freeSpace = this.outputWindow.GetFreeSpace();
            while (freeSpace >= 0x102)
            {
                int num2;
                switch (this.mode)
                {
                    case 7:
                        goto Label_0051;

                    case 8:
                        goto Label_00C5;

                    case 9:
                        goto Label_0114;

                    case 10:
                        goto Label_0154;

                    default:
                        throw new SharpZipBaseException("Inflater unknown mode");
                }
            Label_0037:
                this.outputWindow.Write(num2);
                if (--freeSpace < 0x102)
                {
                    return true;
                }
            Label_0051:
                if (((num2 = this.litlenTree.GetSymbol(this.input)) & -256) == 0)
                {
                    goto Label_0037;
                }
                if (num2 < 0x101)
                {
                    if (num2 < 0)
                    {
                        return false;
                    }
                    this.distTree = null;
                    this.litlenTree = null;
                    this.mode = 2;
                    return true;
                }
                try
                {
                    this.repLength = CPLENS[num2 - 0x101];
                    this.neededBits = CPLEXT[num2 - 0x101];
                }
                catch (Exception)
                {
                    throw new SharpZipBaseException("Illegal rep length code");
                }
            Label_00C5:
                if (this.neededBits > 0)
                {
                    this.mode = 8;
                    int num3 = this.input.PeekBits(this.neededBits);
                    if (num3 < 0)
                    {
                        return false;
                    }
                    this.input.DropBits(this.neededBits);
                    this.repLength += num3;
                }
                this.mode = 9;
            Label_0114:
                num2 = this.distTree.GetSymbol(this.input);
                if (num2 < 0)
                {
                    return false;
                }
                try
                {
                    this.repDist = CPDIST[num2];
                    this.neededBits = CPDEXT[num2];
                }
                catch (Exception)
                {
                    throw new SharpZipBaseException("Illegal rep dist code");
                }
            Label_0154:
                if (this.neededBits > 0)
                {
                    this.mode = 10;
                    int num4 = this.input.PeekBits(this.neededBits);
                    if (num4 < 0)
                    {
                        return false;
                    }
                    this.input.DropBits(this.neededBits);
                    this.repDist += num4;
                }
                this.outputWindow.Repeat(this.repLength, this.repDist);
                freeSpace -= this.repLength;
                this.mode = 7;
            }
            return true;
        }

        public int Inflate(byte[] buf)
        {
            return this.Inflate(buf, 0, buf.Length);
        }

        public int Inflate(byte[] buf, int offset, int len)
        {
            if (len < 0)
            {
                throw new ArgumentOutOfRangeException("len < 0");
            }
            if (len == 0)
            {
                if (!this.IsFinished)
                {
                    this.Decode();
                }
                return 0;
            }
            int num = 0;
            do
            {
                if (this.mode != 11)
                {
                    int num2 = this.outputWindow.CopyOutput(buf, offset, len);
                    this.adler.Update(buf, offset, num2);
                    offset += num2;
                    num += num2;
                    this.totalOut += num2;
                    len -= num2;
                    if (len == 0)
                    {
                        return num;
                    }
                }
            }
            while (this.Decode() || ((this.outputWindow.GetAvailable() > 0) && (this.mode != 11)));
            return num;
        }

        public void Reset()
        {
            this.mode = this.noHeader ? 2 : 0;
            this.totalIn = this.totalOut = 0;
            this.input.Reset();
            this.outputWindow.Reset();
            this.dynHeader = null;
            this.litlenTree = null;
            this.distTree = null;
            this.isLastBlock = false;
            this.adler.Reset();
        }

        public void SetDictionary(byte[] buffer)
        {
            this.SetDictionary(buffer, 0, buffer.Length);
        }

        public void SetDictionary(byte[] buffer, int offset, int len)
        {
            if (!this.IsNeedingDictionary)
            {
                throw new InvalidOperationException();
            }
            this.adler.Update(buffer, offset, len);
            if (((int) this.adler.Value) != this.readAdler)
            {
                throw new SharpZipBaseException("Wrong adler checksum");
            }
            this.adler.Reset();
            this.outputWindow.CopyDict(buffer, offset, len);
            this.mode = 2;
        }

        public void SetInput(byte[] buf)
        {
            this.SetInput(buf, 0, buf.Length);
        }

        public void SetInput(byte[] buffer, int offset, int length)
        {
            this.input.SetInput(buffer, offset, length);
            this.totalIn += length;
        }

        public int Adler
        {
            get
            {
                if (!this.IsNeedingDictionary)
                {
                    return (int) this.adler.Value;
                }
                return this.readAdler;
            }
        }

        public bool IsFinished
        {
            get
            {
                return ((this.mode == 12) && (this.outputWindow.GetAvailable() == 0));
            }
        }

        public bool IsNeedingDictionary
        {
            get
            {
                return ((this.mode == 1) && (this.neededBits == 0));
            }
        }

        public bool IsNeedingInput
        {
            get
            {
                return this.input.IsNeedingInput;
            }
        }

        public int RemainingInput
        {
            get
            {
                return this.input.AvailableBytes;
            }
        }

        public int TotalIn
        {
            get
            {
                return (this.totalIn - this.RemainingInput);
            }
        }

        public int TotalOut
        {
            get
            {
                return this.totalOut;
            }
        }
    }
}

