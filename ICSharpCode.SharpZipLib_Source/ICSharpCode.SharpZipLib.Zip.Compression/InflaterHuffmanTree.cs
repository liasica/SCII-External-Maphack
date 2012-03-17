namespace ICSharpCode.SharpZipLib.Zip.Compression
{
    using ICSharpCode.SharpZipLib;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;

    public class InflaterHuffmanTree
    {
        public static InflaterHuffmanTree defDistTree;
        public static InflaterHuffmanTree defLitLenTree;
        private static int MAX_BITLEN = 15;
        private short[] tree;

        static InflaterHuffmanTree()
        {
            try
            {
                byte[] codeLengths = new byte[0x120];
                int num = 0;
                while (num < 0x90)
                {
                    codeLengths[num++] = 8;
                }
                while (num < 0x100)
                {
                    codeLengths[num++] = 9;
                }
                while (num < 280)
                {
                    codeLengths[num++] = 7;
                }
                while (num < 0x120)
                {
                    codeLengths[num++] = 8;
                }
                defLitLenTree = new InflaterHuffmanTree(codeLengths);
                codeLengths = new byte[0x20];
                num = 0;
                while (num < 0x20)
                {
                    codeLengths[num++] = 5;
                }
                defDistTree = new InflaterHuffmanTree(codeLengths);
            }
            catch (Exception)
            {
                throw new SharpZipBaseException("InflaterHuffmanTree: static tree length illegal");
            }
        }

        public InflaterHuffmanTree(byte[] codeLengths)
        {
            this.BuildTree(codeLengths);
        }

        private void BuildTree(byte[] codeLengths)
        {
            int[] numArray = new int[MAX_BITLEN + 1];
            int[] numArray2 = new int[MAX_BITLEN + 1];
            for (int i = 0; i < codeLengths.Length; i++)
            {
                int index = codeLengths[i];
                if (index > 0)
                {
                    numArray[index]++;
                }
            }
            int toReverse = 0;
            int num4 = 0x200;
            for (int j = 1; j <= MAX_BITLEN; j++)
            {
                numArray2[j] = toReverse;
                toReverse += numArray[j] << (0x10 - j);
                if (j >= 10)
                {
                    int num6 = numArray2[j] & 0x1ff80;
                    int num7 = toReverse & 0x1ff80;
                    num4 += (num7 - num6) >> (0x10 - j);
                }
            }
            this.tree = new short[num4];
            int num8 = 0x200;
            for (int k = MAX_BITLEN; k >= 10; k--)
            {
                int num10 = toReverse & 0x1ff80;
                toReverse -= numArray[k] << (0x10 - k);
                int num11 = toReverse & 0x1ff80;
                for (int n = num11; n < num10; n += 0x80)
                {
                    this.tree[DeflaterHuffman.BitReverse(n)] = (short) ((-num8 << 4) | k);
                    num8 += ((int) 1) << (k - 9);
                }
            }
            for (int m = 0; m < codeLengths.Length; m++)
            {
                int num14 = codeLengths[m];
                if (num14 != 0)
                {
                    toReverse = numArray2[num14];
                    int num15 = DeflaterHuffman.BitReverse(toReverse);
                    if (num14 <= 9)
                    {
                        do
                        {
                            this.tree[num15] = (short) ((m << 4) | num14);
                            num15 += ((int) 1) << num14;
                        }
                        while (num15 < 0x200);
                    }
                    else
                    {
                        int num16 = this.tree[num15 & 0x1ff];
                        int num17 = ((int) 1) << (num16 & 15);
                        num16 = -(num16 >> 4);
                        do
                        {
                            this.tree[num16 | (num15 >> 9)] = (short) ((m << 4) | num14);
                            num15 += ((int) 1) << num14;
                        }
                        while (num15 < num17);
                    }
                    numArray2[num14] = toReverse + (((int) 1) << (0x10 - num14));
                }
            }
        }

        public int GetSymbol(StreamManipulator input)
        {
            int num2;
            int index = input.PeekBits(9);
            if (index >= 0)
            {
                num2 = this.tree[index];
                if (num2 >= 0)
                {
                    input.DropBits(num2 & 15);
                    return (num2 >> 4);
                }
                int num3 = -(num2 >> 4);
                int n = num2 & 15;
                index = input.PeekBits(n);
                if (index >= 0)
                {
                    num2 = this.tree[num3 | (index >> 9)];
                    input.DropBits(num2 & 15);
                    return (num2 >> 4);
                }
                int num5 = input.AvailableBits;
                index = input.PeekBits(num5);
                num2 = this.tree[num3 | (index >> 9)];
                if ((num2 & 15) <= num5)
                {
                    input.DropBits(num2 & 15);
                    return (num2 >> 4);
                }
                return -1;
            }
            int availableBits = input.AvailableBits;
            index = input.PeekBits(availableBits);
            num2 = this.tree[index];
            if ((num2 >= 0) && ((num2 & 15) <= availableBits))
            {
                input.DropBits(num2 & 15);
                return (num2 >> 4);
            }
            return -1;
        }
    }
}

