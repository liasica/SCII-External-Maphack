namespace ICSharpCode.SharpZipLib.GZip
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip.Compression;
	using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.IO;

    public class GZipInputStream : InflaterInputStream
    {
        protected Crc32 crc;
        protected bool eos;
        private bool readGZIPHeader;

        public GZipInputStream(Stream baseInputStream) : this(baseInputStream, 0x1000)
        {
        }

        public GZipInputStream(Stream baseInputStream, int size) : base(baseInputStream, new Inflater(true), size)
        {
            this.crc = new Crc32();
        }

        public override int Read(byte[] buf, int offset, int len)
        {
            if (!this.readGZIPHeader)
            {
                this.ReadHeader();
            }
            if (this.eos)
            {
                return 0;
            }
            int num = base.Read(buf, offset, len);
            if (num > 0)
            {
                this.crc.Update(buf, offset, num);
            }
            if (base.inf.IsFinished)
            {
                this.ReadFooter();
            }
            return num;
        }

        private void ReadFooter()
        {
            int num3;
            byte[] destinationArray = new byte[8];
            int remainingInput = base.inf.RemainingInput;
            if (remainingInput > 8)
            {
                remainingInput = 8;
            }
            Array.Copy(base.inputBuffer.RawData, base.inputBuffer.RawLength - base.inf.RemainingInput, destinationArray, 0, remainingInput);
            for (int i = 8 - remainingInput; i > 0; i -= num3)
            {
                num3 = base.baseInputStream.Read(destinationArray, 8 - i, i);
                if (num3 <= 0)
                {
                    throw new GZipException("Early EOF baseInputStream GZIP footer");
                }
            }
            int num4 = (((destinationArray[0] & 0xff) | ((destinationArray[1] & 0xff) << 8)) | ((destinationArray[2] & 0xff) << 0x10)) | (destinationArray[3] << 0x18);
            if (num4 != ((int) this.crc.Value))
            {
                throw new GZipException(string.Concat(new object[] { "GZIP crc sum mismatch, theirs \"", num4, "\" and ours \"", (int) this.crc.Value }));
            }
            int num5 = (((destinationArray[4] & 0xff) | ((destinationArray[5] & 0xff) << 8)) | ((destinationArray[6] & 0xff) << 0x10)) | (destinationArray[7] << 0x18);
            if (num5 != base.inf.TotalOut)
            {
                throw new GZipException("Number of bytes mismatch in footer");
            }
            this.eos = true;
        }

        private void ReadHeader()
        {
            Crc32 crc = new Crc32();
            int bval = base.baseInputStream.ReadByte();
            if (bval < 0)
            {
                this.eos = true;
            }
            else
            {
                crc.Update(bval);
                if (bval != (GZipConstants.GZIP_MAGIC >> 8))
                {
                    throw new GZipException("Error baseInputStream GZIP header, first byte doesn't match");
                }
                bval = base.baseInputStream.ReadByte();
                if (bval != (GZipConstants.GZIP_MAGIC & 0xff))
                {
                    throw new GZipException("Error baseInputStream GZIP header,  second byte doesn't match");
                }
                crc.Update(bval);
                int num2 = base.baseInputStream.ReadByte();
                if (num2 != 8)
                {
                    throw new GZipException("Error baseInputStream GZIP header, data not baseInputStream deflate format");
                }
                crc.Update(num2);
                int num3 = base.baseInputStream.ReadByte();
                if (num3 < 0)
                {
                    throw new GZipException("Early EOF baseInputStream GZIP header");
                }
                crc.Update(num3);
                if ((num3 & 0xd0) != 0)
                {
                    throw new GZipException("Reserved flag bits baseInputStream GZIP header != 0");
                }
                for (int i = 0; i < 6; i++)
                {
                    int num5 = base.baseInputStream.ReadByte();
                    if (num5 < 0)
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP header");
                    }
                    crc.Update(num5);
                }
                if ((num3 & 4) != 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int num7 = base.baseInputStream.ReadByte();
                        if (num7 < 0)
                        {
                            throw new GZipException("Early EOF baseInputStream GZIP header");
                        }
                        crc.Update(num7);
                    }
                    if ((base.baseInputStream.ReadByte() < 0) || (base.baseInputStream.ReadByte() < 0))
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP header");
                    }
                    int num8 = base.baseInputStream.ReadByte();
                    int num9 = base.baseInputStream.ReadByte();
                    if ((num8 < 0) || (num9 < 0))
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP header");
                    }
                    crc.Update(num8);
                    crc.Update(num9);
                    int num10 = (num8 << 8) | num9;
                    for (int k = 0; k < num10; k++)
                    {
                        int num12 = base.baseInputStream.ReadByte();
                        if (num12 < 0)
                        {
                            throw new GZipException("Early EOF baseInputStream GZIP header");
                        }
                        crc.Update(num12);
                    }
                }
                if ((num3 & 8) != 0)
                {
                    int num13;
                    while ((num13 = base.baseInputStream.ReadByte()) > 0)
                    {
                        crc.Update(num13);
                    }
                    if (num13 < 0)
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP file name");
                    }
                    crc.Update(num13);
                }
                if ((num3 & 0x10) != 0)
                {
                    int num14;
                    while ((num14 = base.baseInputStream.ReadByte()) > 0)
                    {
                        crc.Update(num14);
                    }
                    if (num14 < 0)
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP comment");
                    }
                    crc.Update(num14);
                }
                if ((num3 & 2) != 0)
                {
                    int num16 = base.baseInputStream.ReadByte();
                    if (num16 < 0)
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP header");
                    }
                    int num15 = base.baseInputStream.ReadByte();
                    if (num15 < 0)
                    {
                        throw new GZipException("Early EOF baseInputStream GZIP header");
                    }
                    num16 = (num16 << 8) | num15;
                    if (num16 != (((int) crc.Value) & 0xffff))
                    {
                        throw new GZipException("Header CRC value mismatch");
                    }
                }
                this.readGZIPHeader = true;
            }
        }
    }
}

