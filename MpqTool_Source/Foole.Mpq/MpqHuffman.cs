namespace Foole.Mpq
{
    using System;
    using System.IO;

    internal static class MpqHuffman
    {
        private static readonly byte[][] sPrime;

        static MpqHuffman()
        {
            byte[][] bufferArray = new byte[9][];
            byte[] buffer = new byte[0x100];
            buffer[0] = 10;
            buffer[0xff] = 2;
            bufferArray[0] = buffer;
            bufferArray[1] = new byte[] { 
                0x54, 0x16, 0x16, 13, 12, 8, 6, 5, 6, 5, 6, 3, 4, 4, 3, 5, 
                14, 11, 20, 0x13, 0x13, 9, 11, 6, 5, 4, 3, 2, 3, 2, 2, 2, 
                13, 7, 9, 6, 6, 4, 3, 2, 4, 3, 3, 3, 3, 3, 2, 2, 
                9, 6, 4, 4, 4, 4, 3, 2, 3, 2, 2, 2, 2, 3, 2, 4, 
                8, 3, 4, 7, 9, 5, 3, 3, 3, 3, 2, 2, 2, 3, 2, 2, 
                3, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 2, 1, 2, 2, 
                6, 10, 8, 8, 6, 7, 4, 3, 4, 4, 2, 2, 4, 2, 3, 3, 
                4, 3, 7, 7, 9, 6, 4, 3, 3, 2, 1, 2, 2, 2, 2, 2, 
                10, 2, 2, 3, 2, 2, 1, 1, 2, 2, 2, 6, 3, 5, 2, 3, 
                2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 1, 1, 1, 
                2, 1, 1, 1, 1, 1, 1, 2, 4, 4, 4, 7, 9, 8, 12, 2, 
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 3, 
                4, 1, 2, 4, 5, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 
                4, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                2, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 
                2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 2, 2, 2, 6, 0x4b
             };
            bufferArray[2] = new byte[] { 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0x27, 0, 0, 0x23, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0xff, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 6, 14, 0x10, 4, 
                6, 8, 5, 4, 4, 3, 3, 2, 2, 3, 3, 1, 1, 2, 1, 1, 
                1, 4, 2, 4, 2, 2, 2, 1, 1, 4, 1, 1, 2, 3, 3, 2, 
                3, 1, 3, 6, 4, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 
                1, 0x29, 7, 0x16, 0x12, 0x40, 10, 10, 0x11, 0x25, 1, 3, 0x17, 0x10, 0x26, 0x2a, 
                0x10, 1, 0x23, 0x23, 0x2f, 0x10, 6, 7, 2, 9, 1, 1, 1, 1, 1
             };
            bufferArray[3] = new byte[] { 
                0xff, 11, 7, 5, 11, 2, 2, 2, 6, 2, 2, 1, 4, 2, 1, 3, 
                9, 1, 1, 1, 3, 4, 1, 1, 2, 1, 1, 1, 2, 1, 1, 1, 
                5, 1, 1, 1, 13, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                2, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 
                10, 4, 2, 1, 6, 3, 2, 1, 1, 1, 1, 1, 3, 1, 1, 1, 
                5, 2, 3, 4, 3, 3, 3, 2, 1, 1, 1, 2, 1, 2, 3, 3, 
                1, 3, 1, 1, 2, 5, 1, 1, 4, 3, 5, 1, 3, 1, 3, 3, 
                2, 1, 4, 3, 10, 6, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                2, 2, 1, 10, 2, 5, 1, 1, 2, 7, 2, 0x17, 1, 5, 1, 1, 
                14, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                6, 2, 1, 4, 5, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
                1, 1, 1, 1, 1, 1, 1, 1, 7, 1, 1, 2, 1, 1, 1, 1, 
                2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 0x11
             };
            bufferArray[4] = new byte[] { 0xff, 0xfb, 0x98, 0x9a, 0x84, 0x85, 0x63, 100, 0x3e, 0x3e, 0x22, 0x22, 0x13, 0x13, 0x18, 0x17 };
            bufferArray[5] = new byte[] { 
                0xff, 0xf1, 0x9d, 0x9e, 0x9a, 0x9b, 0x9a, 0x97, 0x93, 0x93, 140, 0x8e, 0x86, 0x88, 0x80, 130, 
                0x7c, 0x7c, 0x72, 0x73, 0x69, 0x6b, 0x5f, 0x60, 0x55, 0x56, 0x4a, 0x4b, 0x40, 0x41, 0x37, 0x37, 
                0x2f, 0x2f, 0x27, 0x27, 0x21, 0x21, 0x1b, 0x1c, 0x17, 0x17, 0x13, 0x13, 0x10, 0x10, 13, 13, 
                11, 11, 9, 9, 8, 8, 7, 7, 6, 5, 5, 4, 4, 4, 0x19, 0x18
             };
            bufferArray[6] = new byte[] { 
                0xc3, 0xcb, 0xf5, 0x41, 0xff, 0x7b, 0xf7, 0x21, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0xbf, 0xcc, 0xf2, 0x40, 0xfd, 0x7c, 0xf7, 0x22, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0x7a, 70
             };
            bufferArray[7] = new byte[] { 
                0xc3, 0xd9, 0xef, 0x3d, 0xf9, 0x7c, 0xe9, 30, 0xfd, 0xab, 0xf1, 0x2c, 0xfc, 0x5b, 0xfe, 0x17, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0xbd, 0xd9, 0xec, 0x3d, 0xf5, 0x7d, 0xe8, 0x1d, 0xfb, 0xae, 240, 0x2c, 0xfb, 0x5c, 0xff, 0x18, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0x70, 0x6c
             };
            bufferArray[8] = new byte[] { 
                0xba, 0xc5, 0xda, 0x33, 0xe3, 0x6d, 0xd8, 0x18, 0xe5, 0x94, 0xda, 0x23, 0xdf, 0x4a, 0xd1, 0x10, 
                0xee, 0xaf, 0xe4, 0x2c, 0xea, 90, 0xde, 0x15, 0xf4, 0x87, 0xe9, 0x21, 0xf6, 0x43, 0xfc, 0x12, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0xb0, 0xc7, 0xd8, 0x33, 0xe3, 0x6b, 0xd6, 0x18, 0xe7, 0x95, 0xd8, 0x23, 0xdb, 0x49, 0xd0, 0x11, 
                0xe9, 0xb2, 0xe2, 0x2b, 0xe8, 0x5c, 0xdd, 0x15, 0xf1, 0x87, 0xe7, 0x20, 0xf7, 0x44, 0xff, 0x13, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0x5f, 0x9e
             };
            sPrime = bufferArray;
        }

        private static void AdjustTree(LinkedNode newNode)
        {
            LinkedNode parent = newNode;
            while (parent != null)
            {
                LinkedNode prev;
                parent.Weight++;
                LinkedNode node2 = parent;
                while (true)
                {
                    prev = node2.Prev;
                    if ((prev == null) || (prev.Weight >= parent.Weight))
                    {
                        break;
                    }
                    node2 = prev;
                }
                if (node2 == parent)
                {
                    parent = parent.Parent;
                }
                else
                {
                    if (node2.Prev != null)
                    {
                        node2.Prev.Next = node2.Next;
                    }
                    node2.Next.Prev = node2.Prev;
                    node2.Next = parent.Next;
                    node2.Prev = parent;
                    if (parent.Next != null)
                    {
                        parent.Next.Prev = node2;
                    }
                    parent.Next = node2;
                    parent.Prev.Next = parent.Next;
                    parent.Next.Prev = parent.Prev;
                    LinkedNode next = prev.Next;
                    parent.Next = next;
                    parent.Prev = prev;
                    next.Prev = parent;
                    prev.Next = parent;
                    LinkedNode node5 = parent.Parent;
                    LinkedNode node6 = node2.Parent;
                    if (node5.Child0 == parent)
                    {
                        node5.Child0 = node2;
                    }
                    if ((node5 != node6) && (node6.Child0 == node2))
                    {
                        node6.Child0 = parent;
                    }
                    parent.Parent = node6;
                    node2.Parent = node5;
                    parent = parent.Parent;
                }
            }
        }

        private static LinkedNode BuildList(byte[] primeData)
        {
            LinkedNode node = new LinkedNode(0x100, 1);
            node = node.Insert(new LinkedNode(0x101, 1));
            for (int i = 0; i < primeData.Length; i++)
            {
                if (primeData[i] != 0)
                {
                    node = node.Insert(new LinkedNode(i, primeData[i]));
                }
            }
            return node;
        }

        private static LinkedNode BuildTree(LinkedNode tail)
        {
            LinkedNode node = tail;
            while (node != null)
            {
                LinkedNode node2 = node;
                LinkedNode prev = node.Prev;
                if (prev == null)
                {
                    return node;
                }
                LinkedNode other = new LinkedNode(0, node2.Weight + prev.Weight) {
                    Child0 = node2
                };
                node2.Parent = other;
                prev.Parent = other;
                node.Insert(other);
                node = node.Prev.Prev;
            }
            return node;
        }

        private static LinkedNode Decode(BitStream input, LinkedNode head)
        {
            LinkedNode node = head;
            while (node.Child0 != null)
            {
                int num = input.ReadBits(1);
                if (num == -1)
                {
                    throw new Exception("Unexpected end of file");
                }
                node = (num == 0) ? node.Child0 : node.Child1;
            }
            return node;
        }

        public static MemoryStream Decompress(Stream data)
        {
            int decompressedValue;
            int index = data.ReadByte();
            if (index == 0)
            {
                throw new NotImplementedException("Compression type 0 is not currently supported");
            }
            LinkedNode tail = BuildList(sPrime[index]);
            LinkedNode head = BuildTree(tail);
            MemoryStream stream = new MemoryStream();
            BitStream input = new BitStream(data);
            do
            {
                decompressedValue = Decode(input, head).DecompressedValue;
                switch (decompressedValue)
                {
                    case 0x100:
                        break;

                    case 0x101:
                    {
                        int decomp = input.ReadBits(8);
                        stream.WriteByte((byte) decomp);
                        tail = InsertNode(tail, decomp);
                        break;
                    }
                    default:
                        stream.WriteByte((byte) decompressedValue);
                        break;
                }
            }
            while (decompressedValue != 0x100);
            stream.Seek(0L, SeekOrigin.Begin);
            return stream;
        }

        private static LinkedNode InsertNode(LinkedNode tail, int decomp)
        {
            LinkedNode node = tail;
            LinkedNode prev = tail.Prev;
            LinkedNode node3 = new LinkedNode(node.DecompressedValue, node.Weight) {
                Parent = node
            };
            LinkedNode newNode = new LinkedNode(decomp, 0) {
                Parent = node
            };
            node.Child0 = newNode;
            tail.Next = node3;
            node3.Prev = tail;
            newNode.Prev = node3;
            node3.Next = newNode;
            AdjustTree(newNode);
            AdjustTree(newNode);
            return prev;
        }
    }
}

