namespace ICSharpCode.SharpZipLib.Core
{
    using System;

    public class DirectoryEventArgs : ScanEventArgs
    {
        private bool hasMatchingFiles;

        public DirectoryEventArgs(string name, bool hasMatchingFiles) : base(name)
        {
            this.hasMatchingFiles = hasMatchingFiles;
        }

        public bool HasMatchingFiles
        {
            get
            {
                return this.hasMatchingFiles;
            }
        }
    }
}

