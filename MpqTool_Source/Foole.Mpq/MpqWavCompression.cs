namespace Foole.Mpq
{
    using System;
    using System.IO;

    internal static class MpqWavCompression
    {
        private static readonly int[] sLookup = new int[] { 
            7, 8, 9, 10, 11, 12, 13, 14, 0x10, 0x11, 0x13, 0x15, 0x17, 0x19, 0x1c, 0x1f, 
            0x22, 0x25, 0x29, 0x2d, 50, 0x37, 60, 0x42, 0x49, 80, 0x58, 0x61, 0x6b, 0x76, 130, 0x8f, 
            0x9d, 0xad, 190, 0xd1, 230, 0xfd, 0x117, 0x133, 0x151, 0x173, 0x198, 0x1c1, 0x1ee, 0x220, 0x256, 0x292, 
            0x2d4, 0x31c, 0x36c, 0x3c3, 0x424, 0x48e, 0x502, 0x583, 0x610, 0x6ab, 0x756, 0x812, 0x8e0, 0x9c3, 0xabd, 0xbd0, 
            0xcff, 0xe4c, 0xfba, 0x114c, 0x1307, 0x14ee, 0x1706, 0x1954, 0x1bdc, 0x1ea5, 0x21b6, 0x2515, 0x28ca, 0x2cdf, 0x315b, 0x364b, 
            0x3bb9, 0x41b2, 0x4844, 0x4f7e, 0x5771, 0x602f, 0x69ce, 0x7462, 0x7fff
         };
        private static readonly int[] sLookup2 = new int[] { 
            -1, 0, -1, 4, -1, 2, -1, 6, -1, 1, -1, 5, -1, 3, -1, 7, 
            -1, 1, -1, 5, -1, 3, -1, 7, -1, 2, -1, 4, -1, 6, -1, 8
         };

        public static byte[] Decompress(Stream data, int channelCount)
        {
            int[] numArray = new int[] { 0x2c, 0x2c };
            int[] numArray2 = new int[channelCount];
            BinaryReader reader = new BinaryReader(data);
            MemoryStream output = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(output);
            reader.ReadByte();
            byte num = reader.ReadByte();
            for (int i = 0; i < channelCount; i++)
            {
                short num3 = reader.ReadInt16();
                numArray2[i] = num3;
                writer.Write(num3);
            }
            int index = channelCount - 1;
            while (data.Position < data.Length)
            {
                byte num5 = reader.ReadByte();
                if (channelCount == 2)
                {
                    index = 1 - index;
                }
                if ((num5 & 0x80) != 0)
                {
                    switch ((num5 & 0x7f))
                    {
                        case 0:
                        {
                            if (numArray[index] != 0)
                            {
                                numArray[index]--;
                            }
                            writer.Write((short) numArray2[index]);
                            continue;
                        }
                        case 1:
                        {
                            numArray[index] += 8;
                            if (numArray[index] > 0x58)
                            {
                                numArray[index] = 0x58;
                            }
                            if (channelCount == 2)
                            {
                                index = 1 - index;
                            }
                            continue;
                        }
                        case 2:
                        {
                            continue;
                        }
                    }
                    numArray[index] -= 8;
                    if (numArray[index] < 0)
                    {
                        numArray[index] = 0;
                    }
                    if (channelCount == 2)
                    {
                        index = 1 - index;
                    }
                }
                else
                {
                    int num6 = sLookup[numArray[index]];
                    int num7 = num6 >> num;
                    if ((num5 & 1) != 0)
                    {
                        num7 += num6;
                    }
                    if ((num5 & 2) != 0)
                    {
                        num7 += num6 >> 1;
                    }
                    if ((num5 & 4) != 0)
                    {
                        num7 += num6 >> 2;
                    }
                    if ((num5 & 8) != 0)
                    {
                        num7 += num6 >> 3;
                    }
                    if ((num5 & 0x10) != 0)
                    {
                        num7 += num6 >> 4;
                    }
                    if ((num5 & 0x20) != 0)
                    {
                        num7 += num6 >> 5;
                    }
                    int num8 = numArray2[index];
                    if ((num5 & 0x40) != 0)
                    {
                        num8 -= num7;
                        if (num8 <= -32768)
                        {
                            num8 = -32768;
                        }
                    }
                    else
                    {
                        num8 += num7;
                        if (num8 >= 0x7fff)
                        {
                            num8 = 0x7fff;
                        }
                    }
                    numArray2[index] = num8;
                    writer.Write((short) num8);
                    numArray[index] += sLookup2[num5 & 0x1f];
                    if (numArray[index] < 0)
                    {
                        numArray[index] = 0;
                    }
                    else if (numArray[index] > 0x58)
                    {
                        numArray[index] = 0x58;
                    }
                }
            }
            return output.ToArray();
        }
    }
}

