namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib.Core;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class FastZip
    {
        private byte[] buffer;
        private ConfirmOverwriteDelegate confirmDelegate;
        private bool createEmptyDirectories;
        private NameFilter directoryFilter;
        private FastZipEvents events;
        private NameFilter fileFilter;
        private ZipInputStream inputStream;
        private ZipNameTransform nameTransform;
        private ZipOutputStream outputStream;
        private Overwrite overwrite;
        private string password;
        private bool restoreDateTime;
        private string sourceDirectory;
        private string targetDirectory;

        public FastZip()
        {
            this.password = null;
            this.restoreDateTime = false;
            this.createEmptyDirectories = false;
            this.events = null;
        }

        public FastZip(FastZipEvents events)
        {
            this.password = null;
            this.restoreDateTime = false;
            this.createEmptyDirectories = false;
            this.events = events;
        }

        private void AddFileContents(string name)
        {
            if (this.buffer == null)
            {
                this.buffer = new byte[0x1000];
            }
            FileStream stream = File.OpenRead(name);
            try
            {
                int num;
                do
                {
                    num = stream.Read(this.buffer, 0, this.buffer.Length);
                    this.outputStream.Write(this.buffer, 0, num);
                }
                while (num > 0);
            }
            finally
            {
                stream.Close();
            }
        }

        public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter)
        {
            this.CreateZip(zipFileName, sourceDirectory, recurse, fileFilter, null);
        }

        public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter)
        {
            this.NameTransform = new ZipNameTransform(true, sourceDirectory);
            this.sourceDirectory = sourceDirectory;
            this.outputStream = new ZipOutputStream(File.Create(zipFileName));
            try
            {
                FileSystemScanner scanner = new FileSystemScanner(fileFilter, directoryFilter);
                scanner.ProcessFile = (ProcessFileDelegate) Delegate.Combine(scanner.ProcessFile, new ProcessFileDelegate(this.ProcessFile));

                if (this.CreateEmptyDirectories)
                {
                    scanner.ProcessDirectory = (ProcessDirectoryDelegate) Delegate.Combine(scanner.ProcessDirectory, new ProcessDirectoryDelegate(this.ProcessDirectory));
                }
                scanner.Scan(sourceDirectory, recurse);
            }
            finally
            {
                this.outputStream.Close();
            }
        }

        private void ExtractEntry(ZipEntry entry)
        {
            bool flag = this.NameIsValid(entry.Name);
            string path = null;
            string str2 = null;
            if (flag)
            {
                string name;
                if (Path.IsPathRooted(entry.Name))
                {
                    string pathRoot = Path.GetPathRoot(entry.Name);
                    name = Path.Combine(Path.GetDirectoryName(entry.Name.Substring(pathRoot.Length)), Path.GetFileName(entry.Name));
                }
                else
                {
                    name = entry.Name;
                }
                str2 = Path.Combine(this.targetDirectory, name);
                path = Path.GetDirectoryName(Path.GetFullPath(str2));
                flag = flag && (name.Length > 0);
            }
            if ((flag && !Directory.Exists(path)) && (!entry.IsDirectory || this.CreateEmptyDirectories))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    flag = false;
                }
            }
            if (flag && entry.IsFile)
            {
                this.ExtractFileEntry(entry, str2);
            }
        }

        private void ExtractFileEntry(ZipEntry entry, string targetName)
        {
            bool flag = true;
            if (((this.overwrite == Overwrite.Prompt) && (this.confirmDelegate != null)) && File.Exists(targetName))
            {
                flag = this.confirmDelegate(targetName);
            }
            if (flag)
            {
                if (this.events != null)
                {
                    this.events.OnProcessFile(entry.Name);
                }
                FileStream stream = File.Create(targetName);
                try
                {
                    int num;
                    if (this.buffer == null)
                    {
                        this.buffer = new byte[0x1000];
                    }
                    do
                    {
                        num = this.inputStream.Read(this.buffer, 0, this.buffer.Length);
                        stream.Write(this.buffer, 0, num);
                    }
                    while (num > 0);
                }
                finally
                {
                    stream.Close();
                }
                if (this.restoreDateTime)
                {
                    File.SetLastWriteTime(targetName, entry.DateTime);
                }
            }
        }

        public void ExtractZip(string zipFileName, string targetDirectory, string fileFilter)
        {
            this.ExtractZip(zipFileName, targetDirectory, Overwrite.Always, null, fileFilter, null);
        }

        public void ExtractZip(string zipFileName, string targetDirectory, Overwrite overwrite, ConfirmOverwriteDelegate confirmDelegate, string fileFilter, string directoryFilter)
        {
            if ((overwrite == Overwrite.Prompt) && (confirmDelegate == null))
            {
                throw new ArgumentNullException("confirmDelegate");
            }
            this.overwrite = overwrite;
            this.confirmDelegate = confirmDelegate;
            this.targetDirectory = targetDirectory;
            this.fileFilter = new NameFilter(fileFilter);
            this.directoryFilter = new NameFilter(directoryFilter);
            this.inputStream = new ZipInputStream(File.OpenRead(zipFileName));
            try
            {
                ZipEntry entry;
                if (this.password != null)
                {
                    this.inputStream.Password = this.password;
                }
                while ((entry = this.inputStream.GetNextEntry()) != null)
                {
                    if (this.directoryFilter.IsMatch(Path.GetDirectoryName(entry.Name)) && this.fileFilter.IsMatch(entry.Name))
                    {
                        this.ExtractEntry(entry);
                    }
                }
            }
            finally
            {
                this.inputStream.Close();
            }
        }

        private bool NameIsValid(string name)
        {
            return (((name != null) && (name.Length > 0)) && (name.IndexOfAny(Path.InvalidPathChars) < 0));
        }

        private void ProcessDirectory(object sender, DirectoryEventArgs e)
        {
            if (!e.HasMatchingFiles && this.createEmptyDirectories)
            {
                if (this.events != null)
                {
                    this.events.OnProcessDirectory(e.Name, e.HasMatchingFiles);
                }
                if (e.Name != this.sourceDirectory)
                {
                    ZipEntry entry = new ZipEntry(this.nameTransform.TransformDirectory(e.Name));
                    this.outputStream.PutNextEntry(entry);
                }
            }
        }

        private void ProcessFile(object sender, ScanEventArgs e)
        {
            if (this.events != null)
            {
                this.events.OnProcessFile(e.Name);
            }
            ZipEntry entry = new ZipEntry(this.nameTransform.TransformFile(e.Name));
            this.outputStream.PutNextEntry(entry);
            this.AddFileContents(e.Name);
        }

        public bool CreateEmptyDirectories
        {
            get
            {
                return this.createEmptyDirectories;
            }
            set
            {
                this.createEmptyDirectories = value;
            }
        }

        public ZipNameTransform NameTransform
        {
            get
            {
                return this.nameTransform;
            }
            set
            {
                if (value == null)
                {
                    this.nameTransform = new ZipNameTransform();
                }
                else
                {
                    this.nameTransform = value;
                }
            }
        }

        public delegate bool ConfirmOverwriteDelegate(string fileName);

        public enum Overwrite
        {
            Prompt,
            Never,
            Always
        }
    }
}

