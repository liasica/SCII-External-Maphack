namespace ICSharpCode.SharpZipLib.GZip
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip.Compression;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.IO;

    public class GZipOutputStream : DeflaterOutputStream
    {
        protected Crc32 crc;

        public GZipOutputStream(Stream baseOutputStream) : this(baseOutputStream, 0x1000)
        {
        }

        public GZipOutputStream(Stream baseOutputStream, int size) : base(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, true), size)
        {
            this.crc = new Crc32();
            this.WriteHeader();
        }

        public override void Close()
        {
            this.Finish();
            if (base.IsStreamOwner)
            {
                base.baseOutputStream.Close();
            }
        }

        public override void Finish()
        {
            base.Finish();
            int totalIn = base.def.TotalIn;
            int num2 = (int) (((ulong) this.crc.Value) & 0xffffffffL);
            byte[] buffer = new byte[] { (byte) num2, (byte) (num2 >> 8), (byte) (num2 >> 0x10), (byte) (num2 >> 0x18), (byte) totalIn, (byte) (totalIn >> 8), (byte) (totalIn >> 0x10), (byte) (totalIn >> 0x18) };
            base.baseOutputStream.Write(buffer, 0, buffer.Length);
        }

        public int GetLevel()
        {
            return base.def.GetLevel();
        }

        public void SetLevel(int level)
        {
            if (level < Deflater.BEST_SPEED)
            {
                throw new ArgumentOutOfRangeException("level");
            }
            base.def.SetLevel(level);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.crc.Update(buf, off, len);
            base.Write(buf, off, len);
        }

        private void WriteHeader()
        {
            int num = (int) (DateTime.Now.Ticks / 0x2710L);
            byte[] buffer2 = new byte[10];
            buffer2[0] = (byte) (GZipConstants.GZIP_MAGIC >> 8);
            buffer2[1] = (byte) GZipConstants.GZIP_MAGIC;
            buffer2[2] = (byte) Deflater.DEFLATED;
            buffer2[4] = (byte) num;
            buffer2[5] = (byte) (num >> 8);
            buffer2[6] = (byte) (num >> 0x10);
            buffer2[7] = (byte) (num >> 0x18);
            buffer2[9] = 0xff;
            byte[] buffer = buffer2;
            base.baseOutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}

