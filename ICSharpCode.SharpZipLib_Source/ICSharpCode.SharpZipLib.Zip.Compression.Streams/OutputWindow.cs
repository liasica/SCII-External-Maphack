namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
    using System;

    public class OutputWindow
    {
        private byte[] window = new byte[WINDOW_SIZE];
        private static int WINDOW_MASK = (WINDOW_SIZE - 1);
        private static int WINDOW_SIZE = 0x8000;
        private int windowEnd = 0;
        private int windowFilled = 0;

        public void CopyDict(byte[] dict, int offset, int len)
        {
            if (this.windowFilled > 0)
            {
                throw new InvalidOperationException();
            }
            if (len > WINDOW_SIZE)
            {
                offset += len - WINDOW_SIZE;
                len = WINDOW_SIZE;
            }
            Array.Copy(dict, offset, this.window, 0, len);
            this.windowEnd = len & WINDOW_MASK;
        }

        public int CopyOutput(byte[] output, int offset, int len)
        {
            int windowEnd = this.windowEnd;
            if (len > this.windowFilled)
            {
                len = this.windowFilled;
            }
            else
            {
                windowEnd = ((this.windowEnd - this.windowFilled) + len) & WINDOW_MASK;
            }
            int num2 = len;
            int length = len - windowEnd;
            if (length > 0)
            {
                Array.Copy(this.window, WINDOW_SIZE - length, output, offset, length);
                offset += length;
                len = windowEnd;
            }
            Array.Copy(this.window, windowEnd - len, output, offset, len);
            this.windowFilled -= num2;
            if (this.windowFilled < 0)
            {
                throw new InvalidOperationException();
            }
            return num2;
        }

        public int CopyStored(StreamManipulator input, int len)
        {
            int num;
            len = Math.Min(Math.Min(len, WINDOW_SIZE - this.windowFilled), input.AvailableBytes);
            int length = WINDOW_SIZE - this.windowEnd;
            if (len > length)
            {
                num = input.CopyBytes(this.window, this.windowEnd, length);
                if (num == length)
                {
                    num += input.CopyBytes(this.window, 0, len - length);
                }
            }
            else
            {
                num = input.CopyBytes(this.window, this.windowEnd, len);
            }
            this.windowEnd = (this.windowEnd + num) & WINDOW_MASK;
            this.windowFilled += num;
            return num;
        }

        public int GetAvailable()
        {
            return this.windowFilled;
        }

        public int GetFreeSpace()
        {
            return (WINDOW_SIZE - this.windowFilled);
        }

        public void Repeat(int len, int dist)
        {
            if ((this.windowFilled += len) > WINDOW_SIZE)
            {
                throw new InvalidOperationException("Window full");
            }
            int repStart = (this.windowEnd - dist) & WINDOW_MASK;
            int num2 = WINDOW_SIZE - len;
            if ((repStart > num2) || (this.windowEnd >= num2))
            {
                this.SlowRepeat(repStart, len, dist);
            }
            else if (len > dist)
            {
                while (len-- > 0)
                {
                    this.window[this.windowEnd++] = this.window[repStart++];
                }
            }
            else
            {
                Array.Copy(this.window, repStart, this.window, this.windowEnd, len);
                this.windowEnd += len;
            }
        }

        public void Reset()
        {
            this.windowFilled = this.windowEnd = 0;
        }

        private void SlowRepeat(int repStart, int len, int dist)
        {
            while (len-- > 0)
            {
                this.window[this.windowEnd++] = this.window[repStart++];
                this.windowEnd &= WINDOW_MASK;
                repStart &= WINDOW_MASK;
            }
        }

        public void Write(int abyte)
        {
            if (this.windowFilled++ == WINDOW_SIZE)
            {
                throw new InvalidOperationException("Window full");
            }
            this.window[this.windowEnd++] = (byte) abyte;
            this.windowEnd &= WINDOW_MASK;
        }
    }
}

