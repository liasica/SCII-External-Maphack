namespace ICSharpCode.SharpZipLib.Zip.Compression
{
    using System;

    public class Deflater
    {
        public static int BEST_COMPRESSION = 9;
        public static int BEST_SPEED = 1;
        private static int BUSY_STATE = 0x10;
        private static int CLOSED_STATE = 0x7f;
        public static int DEFAULT_COMPRESSION = -1;
        public static int DEFLATED = 8;
        private DeflaterEngine engine;
        private static int FINISHED_STATE = 30;
        private static int FINISHING_STATE = 0x1c;
        private static int FLUSHING_STATE = 20;
        private static int INIT_STATE = 0;
        private static int IS_FINISHING = 8;
        private static int IS_FLUSHING = 4;
        private static int IS_SETDICT = 1;
        private int level;
        public static int NO_COMPRESSION = 0;
        private bool noZlibHeaderOrFooter;
        private DeflaterPending pending;
        private static int SETDICT_STATE = 1;
        private int state;
        private long totalOut;

        public Deflater() : this(DEFAULT_COMPRESSION, false)
        {
        }

        public Deflater(int lvl) : this(lvl, false)
        {
        }

        public Deflater(int level, bool noZlibHeaderOrFooter)
        {
            if (level == DEFAULT_COMPRESSION)
            {
                level = 6;
            }
            else if ((level < NO_COMPRESSION) || (level > BEST_COMPRESSION))
            {
                throw new ArgumentOutOfRangeException("level");
            }
            this.pending = new DeflaterPending();
            this.engine = new DeflaterEngine(this.pending);
            this.noZlibHeaderOrFooter = noZlibHeaderOrFooter;
            this.SetStrategy(DeflateStrategy.Default);
            this.SetLevel(level);
            this.Reset();
        }

        public int Deflate(byte[] output)
        {
            return this.Deflate(output, 0, output.Length);
        }

        public int Deflate(byte[] output, int offset, int length)
        {
            int num = length;
            if (this.state == CLOSED_STATE)
            {
                throw new InvalidOperationException("Deflater closed");
            }
            if (this.state < BUSY_STATE)
            {
                int s = (DEFLATED + 0x70) << 8;
                int num3 = (this.level - 1) >> 1;
                if ((num3 < 0) || (num3 > 3))
                {
                    num3 = 3;
                }
                s |= num3 << 6;
                if ((this.state & IS_SETDICT) != 0)
                {
                    s |= 0x20;
                }
                s += 0x1f - (s % 0x1f);
                this.pending.WriteShortMSB(s);
                if ((this.state & IS_SETDICT) != 0)
                {
                    int adler = this.engine.Adler;
                    this.engine.ResetAdler();
                    this.pending.WriteShortMSB(adler >> 0x10);
                    this.pending.WriteShortMSB(adler & 0xffff);
                }
                this.state = BUSY_STATE | (this.state & (IS_FLUSHING | IS_FINISHING));
            }
            while (true)
            {
                int num5 = this.pending.Flush(output, offset, length);
                offset += num5;
                this.totalOut += num5;
                length -= num5;
                if ((length == 0) || (this.state == FINISHED_STATE))
                {
                    return (num - length);
                }
                if (!this.engine.Deflate((this.state & IS_FLUSHING) != 0, (this.state & IS_FINISHING) != 0))
                {
                    if (this.state == BUSY_STATE)
                    {
                        return (num - length);
                    }
                    if (this.state == FLUSHING_STATE)
                    {
                        if (this.level != NO_COMPRESSION)
                        {
                            for (int i = 8 + (-this.pending.BitCount & 7); i > 0; i -= 10)
                            {
                                this.pending.WriteBits(2, 10);
                            }
                        }
                        this.state = BUSY_STATE;
                    }
                    else if (this.state == FINISHING_STATE)
                    {
                        this.pending.AlignToByte();
                        if (!this.noZlibHeaderOrFooter)
                        {
                            int num7 = this.engine.Adler;
                            this.pending.WriteShortMSB(num7 >> 0x10);
                            this.pending.WriteShortMSB(num7 & 0xffff);
                        }
                        this.state = FINISHED_STATE;
                    }
                }
            }
        }

        public void Finish()
        {
            this.state |= IS_FLUSHING | IS_FINISHING;
        }

        public void Flush()
        {
            this.state |= IS_FLUSHING;
        }

        public int GetLevel()
        {
            return this.level;
        }

        public void Reset()
        {
            this.state = this.noZlibHeaderOrFooter ? BUSY_STATE : INIT_STATE;
            this.totalOut = 0L;
            this.pending.Reset();
            this.engine.Reset();
        }

        public void SetDictionary(byte[] dict)
        {
            this.SetDictionary(dict, 0, dict.Length);
        }

        public void SetDictionary(byte[] dict, int offset, int length)
        {
            if (this.state != INIT_STATE)
            {
                throw new InvalidOperationException();
            }
            this.state = SETDICT_STATE;
            this.engine.SetDictionary(dict, offset, length);
        }

        public void SetInput(byte[] input)
        {
            this.SetInput(input, 0, input.Length);
        }

        public void SetInput(byte[] input, int off, int len)
        {
            if ((this.state & IS_FINISHING) != 0)
            {
                throw new InvalidOperationException("finish()/end() already called");
            }
            this.engine.SetInput(input, off, len);
        }

        public void SetLevel(int lvl)
        {
            if (lvl == DEFAULT_COMPRESSION)
            {
                lvl = 6;
            }
            else if ((lvl < NO_COMPRESSION) || (lvl > BEST_COMPRESSION))
            {
                throw new ArgumentOutOfRangeException("lvl");
            }
            if (this.level != lvl)
            {
                this.level = lvl;
                this.engine.SetLevel(lvl);
            }
        }

        public void SetStrategy(DeflateStrategy strategy)
        {
            this.engine.Strategy = strategy;
        }

        public int Adler
        {
            get
            {
                return this.engine.Adler;
            }
        }

        public bool IsFinished
        {
            get
            {
                return ((this.state == FINISHED_STATE) && this.pending.IsFlushed);
            }
        }

        public bool IsNeedingInput
        {
            get
            {
                return this.engine.NeedsInput();
            }
        }

        public int TotalIn
        {
            get
            {
                return this.engine.TotalIn;
            }
        }

        public long TotalOut
        {
            get
            {
                return this.totalOut;
            }
        }
    }
}

