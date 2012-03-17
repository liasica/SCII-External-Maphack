namespace ICSharpCode.SharpZipLib.Tar
{
    using System;
    using System.IO;

    public class TarBuffer
    {
        private int blockFactor = 20;
        public const int BlockSize = 0x200;
        private int currentBlockIndex;
        private int currentRecordIndex;
        public const int DefaultBlockFactor = 20;
        public const int DefaultRecordSize = 0x2800;
        private Stream inputStream;
        private Stream outputStream;
        private byte[] recordBuffer;
        private int recordSize = 0x2800;

        protected TarBuffer()
        {
        }

        public void Close()
        {
            if (this.outputStream != null)
            {
                this.Flush();
                this.outputStream.Close();
                this.outputStream = null;
            }
            else if (this.inputStream != null)
            {
                this.inputStream.Close();
                this.inputStream = null;
            }
        }

        public static TarBuffer CreateInputTarBuffer(Stream inputStream)
        {
            return CreateInputTarBuffer(inputStream, 20);
        }

        public static TarBuffer CreateInputTarBuffer(Stream inputStream, int blockFactor)
        {
            TarBuffer buffer = new TarBuffer {
                inputStream = inputStream,
                outputStream = null
            };
            buffer.Initialize(blockFactor);
            return buffer;
        }

        public static TarBuffer CreateOutputTarBuffer(Stream outputStream)
        {
            return CreateOutputTarBuffer(outputStream, 20);
        }

        public static TarBuffer CreateOutputTarBuffer(Stream outputStream, int blockFactor)
        {
            TarBuffer buffer = new TarBuffer {
                inputStream = null,
                outputStream = outputStream
            };
            buffer.Initialize(blockFactor);
            return buffer;
        }

        private void Flush()
        {
            if (this.outputStream == null)
            {
                throw new TarException("TarBuffer.Flush no output stream defined");
            }
            if (this.currentBlockIndex > 0)
            {
                this.WriteRecord();
            }
            this.outputStream.Flush();
        }

        public int GetBlockFactor()
        {
            return this.blockFactor;
        }

        public int GetCurrentBlockNum()
        {
            return this.currentBlockIndex;
        }

        public int GetCurrentRecordNum()
        {
            return this.currentRecordIndex;
        }

        public int GetRecordSize()
        {
            return this.recordSize;
        }

        private void Initialize(int blockFactor)
        {
            this.blockFactor = blockFactor;
            this.recordSize = blockFactor * 0x200;
            this.recordBuffer = new byte[this.RecordSize];
            if (this.inputStream != null)
            {
                this.currentRecordIndex = -1;
                this.currentBlockIndex = this.BlockFactor;
            }
            else
            {
                this.currentRecordIndex = 0;
                this.currentBlockIndex = 0;
            }
        }

        public bool IsEOFBlock(byte[] block)
        {
            int index = 0;
            int num2 = 0x200;
            while (index < num2)
            {
                if (block[index] != 0)
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        public byte[] ReadBlock()
        {
            if (this.inputStream == null)
            {
                throw new TarException("TarBuffer.ReadBlock - no input stream defined");
            }
            if ((this.currentBlockIndex >= this.BlockFactor) && !this.ReadRecord())
            {
                return null;
            }
            byte[] destinationArray = new byte[0x200];
            Array.Copy(this.recordBuffer, this.currentBlockIndex * 0x200, destinationArray, 0, 0x200);
            this.currentBlockIndex++;
            return destinationArray;
        }

        private bool ReadRecord()
        {
            long num3;
            if (this.inputStream == null)
            {
                throw new TarException("no input stream stream defined");
            }
            this.currentBlockIndex = 0;
            int offset = 0;
            for (int i = this.RecordSize; i > 0; i -= (int) num3)
            {
                num3 = this.inputStream.Read(this.recordBuffer, offset, i);
                if (num3 <= 0L)
                {
                    break;
                }
                offset += (int) num3;
            }
            this.currentRecordIndex++;
            return true;
        }

        public void SkipBlock()
        {
            if (this.inputStream == null)
            {
                throw new TarException("no input stream defined");
            }
            if ((this.currentBlockIndex < this.BlockFactor) || this.ReadRecord())
            {
                this.currentBlockIndex++;
            }
        }

        public void WriteBlock(byte[] block)
        {
            if (this.outputStream == null)
            {
                throw new TarException("TarBuffer.WriteBlock - no output stream defined");
            }
            if (block.Length != 0x200)
            {
                throw new TarException(string.Concat(new object[] { "TarBuffer.WriteBlock - block to write has length '", block.Length, "' which is not the block size of '", 0x200, "'" }));
            }
            if (this.currentBlockIndex >= this.BlockFactor)
            {
                this.WriteRecord();
            }
            Array.Copy(block, 0, this.recordBuffer, this.currentBlockIndex * 0x200, 0x200);
            this.currentBlockIndex++;
        }

        public void WriteBlock(byte[] buf, int offset)
        {
            if (this.outputStream == null)
            {
                throw new TarException("TarBuffer.WriteBlock - no output stream stream defined");
            }
            if ((offset + 0x200) > buf.Length)
            {
                throw new TarException(string.Concat(new object[] { "TarBuffer.WriteBlock - record has length '", buf.Length, "' with offset '", offset, "' which is less than the record size of '", this.recordSize, "'" }));
            }
            if (this.currentBlockIndex >= this.BlockFactor)
            {
                this.WriteRecord();
            }
            Array.Copy(buf, offset, this.recordBuffer, this.currentBlockIndex * 0x200, 0x200);
            this.currentBlockIndex++;
        }

        private void WriteRecord()
        {
            if (this.outputStream == null)
            {
                throw new TarException("TarBuffer.WriteRecord no output stream defined");
            }
            this.outputStream.Write(this.recordBuffer, 0, this.RecordSize);
            this.outputStream.Flush();
            this.currentBlockIndex = 0;
            this.currentRecordIndex++;
        }

        public int BlockFactor
        {
            get
            {
                return this.blockFactor;
            }
        }

        public int RecordSize
        {
            get
            {
                return this.recordSize;
            }
        }
    }
}

