namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib.Core;
    using System;

    public class FastZipEvents
    {
        public DirectoryFailureDelegate DirectoryFailure;
        public FileFailureDelegate FileFailure;
        public ProcessDirectoryDelegate ProcessDirectory;
        public ProcessFileDelegate ProcessFile;

        public void OnDirectoryFailure(string directory, Exception e)
        {
            if (this.DirectoryFailure != null)
            {
                ScanFailureEventArgs args = new ScanFailureEventArgs(directory, e);
                this.DirectoryFailure(this, args);
            }
        }

        public void OnFileFailure(string file, Exception e)
        {
            if (this.FileFailure != null)
            {
                ScanFailureEventArgs args = new ScanFailureEventArgs(file, e);
                this.FileFailure(this, args);
            }
        }

        public void OnProcessDirectory(string directory, bool hasMatchingFiles)
        {
            if (this.ProcessDirectory != null)
            {
                DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
                this.ProcessDirectory(this, e);
            }
        }

        public void OnProcessFile(string file)
        {
            if (this.ProcessFile != null)
            {
                ScanEventArgs e = new ScanEventArgs(file);
                this.ProcessFile(this, e);
            }
        }
    }
}

