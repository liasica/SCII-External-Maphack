namespace ICSharpCode.SharpZipLib.Core
{
    using System;
    using System.IO;

    public class NameAndSizeFilter : PathFilter
    {
        private long maxSize;
        private long minSize;

        public NameAndSizeFilter(string filter, long minSize, long maxSize) : base(filter)
        {
            this.minSize = 0L;
            this.maxSize = 0x7fffffffffffffffL;
            this.minSize = minSize;
            this.maxSize = maxSize;
        }

        public override bool IsMatch(string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            long length = info.Length;
            return ((base.IsMatch(fileName) && (this.MinSize <= length)) && (this.MaxSize >= length));
        }

        public long MaxSize
        {
            get
            {
                return this.maxSize;
            }
            set
            {
                this.maxSize = value;
            }
        }

        public long MinSize
        {
            get
            {
                return this.minSize;
            }
            set
            {
                this.minSize = value;
            }
        }
    }
}

