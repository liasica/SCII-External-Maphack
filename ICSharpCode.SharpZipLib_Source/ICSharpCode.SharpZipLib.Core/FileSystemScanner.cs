namespace ICSharpCode.SharpZipLib.Core
{
    using System;
    using System.IO;

    public class FileSystemScanner
    {
        private bool alive;
        public DirectoryFailureDelegate DirectoryFailure;
        private IScanFilter directoryFilter;
        public FileFailureDelegate FileFailure;
        private IScanFilter fileFilter;
        public ProcessDirectoryDelegate ProcessDirectory;
        public ProcessFileDelegate ProcessFile;

        public FileSystemScanner(IScanFilter fileFilter)
        {
            this.fileFilter = fileFilter;
        }

        public FileSystemScanner(string filter)
        {
            this.fileFilter = new PathFilter(filter);
        }

        public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
        {
            this.fileFilter = fileFilter;
            this.directoryFilter = directoryFilter;
        }

        public FileSystemScanner(string fileFilter, string directoryFilter)
        {
            this.fileFilter = new PathFilter(fileFilter);
            this.directoryFilter = new PathFilter(directoryFilter);
        }

        public void OnDirectoryFailure(string directory, Exception e)
        {
            if (this.DirectoryFailure == null)
            {
                this.alive = false;
            }
            else
            {
                ScanFailureEventArgs args = new ScanFailureEventArgs(directory, e);
                this.DirectoryFailure(this, args);
                this.alive = args.ContinueRunning;
            }
        }

        public void OnFileFailure(string file, Exception e)
        {
            if (this.FileFailure == null)
            {
                this.alive = false;
            }
            else
            {
                ScanFailureEventArgs args = new ScanFailureEventArgs(file, e);
                this.FileFailure(this, args);
                this.alive = args.ContinueRunning;
            }
        }

        public void OnProcessDirectory(string directory, bool hasMatchingFiles)
        {
            if (this.ProcessDirectory != null)
            {
                DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
                this.ProcessDirectory(this, e);
                this.alive = e.ContinueRunning;
            }
        }

        public void OnProcessFile(string file)
        {
            if (this.ProcessFile != null)
            {
                ScanEventArgs e = new ScanEventArgs(file);
                this.ProcessFile(this, e);
                this.alive = e.ContinueRunning;
            }
        }

        public void Scan(string directory, bool recurse)
        {
            this.alive = true;
            this.ScanDir(directory, recurse);
        }

        private void ScanDir(string directory, bool recurse)
        {
            try
            {
                string[] files = Directory.GetFiles(directory);
                bool hasMatchingFiles = false;
                for (int i = 0; i < files.Length; i++)
                {
                    if (!this.fileFilter.IsMatch(files[i]))
                    {
                        files[i] = null;
                    }
                    else
                    {
                        hasMatchingFiles = true;
                    }
                }
                this.OnProcessDirectory(directory, hasMatchingFiles);
                if (this.alive && hasMatchingFiles)
                {
                    foreach (string str in files)
                    {
                        try
                        {
                            if (str != null)
                            {
                                this.OnProcessFile(str);
                                if (!this.alive)
                                {
                                    goto Label_0090;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            this.OnFileFailure(str, exception);
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                this.OnDirectoryFailure(directory, exception2);
            }
        Label_0090:
            if (this.alive && recurse)
            {
                try
                {
                    foreach (string str2 in Directory.GetDirectories(directory))
                    {
                        if ((this.directoryFilter == null) || this.directoryFilter.IsMatch(str2))
                        {
                            this.ScanDir(str2, true);
                            if (!this.alive)
                            {
                                return;
                            }
                        }
                    }
                }
                catch (Exception exception3)
                {
                    this.OnDirectoryFailure(directory, exception3);
                }
            }
        }
    }
}

