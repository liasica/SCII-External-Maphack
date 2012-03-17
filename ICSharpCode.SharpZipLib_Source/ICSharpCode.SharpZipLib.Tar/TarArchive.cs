namespace ICSharpCode.SharpZipLib.Tar
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class TarArchive
    {
        private bool applyUserInfoOverrides = false;
        private bool asciiTranslate;
        private int groupId;
        private string groupName;
        private bool keepOldFiles;
        private string pathPrefix;
        private byte[] recordBuf;
        private int recordSize;
        private string rootPath;
        private TarInputStream tarIn;
        private TarOutputStream tarOut;
        private int userId;
        private string userName;

        public event ProgressMessageHandler ProgressMessageEvent;

        protected TarArchive()
        {
        }

        public void CloseArchive()
        {
            if (this.tarIn != null)
            {
                this.tarIn.Close();
            }
            else if (this.tarOut != null)
            {
                this.tarOut.Flush();
                this.tarOut.Close();
            }
        }

        public static TarArchive CreateInputTarArchive(Stream inputStream)
        {
            return CreateInputTarArchive(inputStream, 20);
        }

        public static TarArchive CreateInputTarArchive(Stream inputStream, int blockFactor)
        {
            TarArchive archive = new TarArchive {
                tarIn = new TarInputStream(inputStream, blockFactor)
            };
            archive.Initialize(blockFactor * 0x200);
            return archive;
        }

        public static TarArchive CreateOutputTarArchive(Stream outputStream)
        {
            return CreateOutputTarArchive(outputStream, 20);
        }

        public static TarArchive CreateOutputTarArchive(Stream outputStream, int blockFactor)
        {
            TarArchive archive = new TarArchive {
                tarOut = new TarOutputStream(outputStream, blockFactor)
            };
            archive.Initialize(blockFactor * 0x200);
            return archive;
        }

        private void EnsureDirectoryExists(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch (Exception exception)
                {
                    throw new TarException("Exception creating directory '" + directoryName + "', " + exception.Message);
                }
            }
        }

        public void ExtractContents(string destDir)
        {
            while (true)
            {
                TarEntry nextEntry = this.tarIn.GetNextEntry();
                if (nextEntry == null)
                {
                    return;
                }
                this.ExtractEntry(destDir, nextEntry);
            }
        }

        private void ExtractEntry(string destDir, TarEntry entry)
        {
            int num;
            this.OnProgressMessageEvent(entry, null);
            string name = entry.Name;
            if (Path.IsPathRooted(name))
            {
                name = name.Substring(Path.GetPathRoot(name).Length);
            }
            name = name.Replace('/', Path.DirectorySeparatorChar);
            string directoryName = Path.Combine(destDir, name);
            if (entry.IsDirectory)
            {
                this.EnsureDirectoryExists(directoryName);
                return;
            }
            string str3 = Path.GetDirectoryName(directoryName);
            this.EnsureDirectoryExists(str3);
            bool flag = true;
            FileInfo info = new FileInfo(directoryName);
            if (info.Exists)
            {
                if (this.keepOldFiles)
                {
                    this.OnProgressMessageEvent(entry, "Destination file already exists");
                    flag = false;
                }
                else if ((info.Attributes & FileAttributes.ReadOnly) != 0)
                {
                    this.OnProgressMessageEvent(entry, "Destination file already exists, and is read-only");
                    flag = false;
                }
            }
            if (!flag)
            {
                return;
            }
            bool flag2 = false;
            Stream stream = File.Create(directoryName);
            if (this.asciiTranslate)
            {
                flag2 = !this.IsBinary(directoryName);
            }
            StreamWriter writer = null;
            if (flag2)
            {
                writer = new StreamWriter(stream);
            }
            byte[] buffer = new byte[0x8000];
        Label_00E2:
            num = this.tarIn.Read(buffer, 0, buffer.Length);
            if (num > 0)
            {
                if (flag2)
                {
                    int index = 0;
                    for (int i = 0; i < num; i++)
                    {
                        if (buffer[i] == 10)
                        {
                            string str4 = Encoding.ASCII.GetString(buffer, index, i - index);
                            writer.WriteLine(str4);
                            index = i + 1;
                        }
                    }
                }
                else
                {
                    stream.Write(buffer, 0, num);
                }
                goto Label_00E2;
            }
            if (flag2)
            {
                writer.Close();
            }
            else
            {
                stream.Close();
            }
        }

        private void Initialize(int recordSize)
        {
            this.recordSize = recordSize;
            this.rootPath = null;
            this.pathPrefix = null;
            this.userId = 0;
            this.userName = string.Empty;
            this.groupId = 0;
            this.groupName = string.Empty;
            this.keepOldFiles = false;
            this.recordBuf = new byte[this.RecordSize];
        }

        private void InternalWriteEntry(TarEntry sourceEntry, bool recurse)
        {
            string path = null;
            string file = sourceEntry.File;
            TarEntry entry = (TarEntry) sourceEntry.Clone();
            if (this.applyUserInfoOverrides)
            {
                entry.GroupId = this.groupId;
                entry.GroupName = this.groupName;
                entry.UserId = this.userId;
                entry.UserName = this.userName;
            }
            this.OnProgressMessageEvent(entry, null);
            if ((this.asciiTranslate && !entry.IsDirectory) && !this.IsBinary(file))
            {
                path = Path.GetTempFileName();
                StreamReader reader = File.OpenText(file);
                Stream stream = File.Create(path);
                while (true)
                {
                    string s = reader.ReadLine();
                    if (s == null)
                    {
                        break;
                    }
                    byte[] bytes = Encoding.ASCII.GetBytes(s);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.WriteByte(10);
                }
                reader.Close();
                stream.Flush();
                stream.Close();
                entry.Size = new FileInfo(path).Length;
                file = path;
            }
            string str4 = null;
            if ((this.rootPath != null) && entry.Name.StartsWith(this.rootPath))
            {
                str4 = entry.Name.Substring(this.rootPath.Length + 1);
            }
            if (this.pathPrefix != null)
            {
                str4 = (str4 == null) ? (this.pathPrefix + "/" + entry.Name) : (this.pathPrefix + "/" + str4);
            }
            if (str4 != null)
            {
                entry.Name = str4;
            }
            this.tarOut.PutNextEntry(entry);
            if (entry.IsDirectory)
            {
                if (recurse)
                {
                    TarEntry[] directoryEntries = entry.GetDirectoryEntries();
                    for (int i = 0; i < directoryEntries.Length; i++)
                    {
                        this.InternalWriteEntry(directoryEntries[i], recurse);
                    }
                }
            }
            else
            {
                Stream stream2 = File.OpenRead(file);
                int num2 = 0;
                byte[] buffer = new byte[0x8000];
                while (true)
                {
                    int count = stream2.Read(buffer, 0, buffer.Length);
                    if (count <= 0)
                    {
                        break;
                    }
                    this.tarOut.Write(buffer, 0, count);
                    num2 += count;
                }
                stream2.Close();
                if ((path != null) && (path.Length > 0))
                {
                    File.Delete(path);
                }
                this.tarOut.CloseEntry();
            }
        }

        private bool IsBinary(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                int count = Math.Min(0x1000, (int) stream.Length);
                byte[] buffer = new byte[count];
                int num2 = stream.Read(buffer, 0, count);
                for (int i = 0; i < num2; i++)
                {
                    byte num4 = buffer[i];
                    if (((num4 < 8) || ((num4 > 13) && (num4 < 0x20))) || (num4 == 0xff))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ListContents()
        {
            while (true)
            {
                TarEntry nextEntry = this.tarIn.GetNextEntry();
                if (nextEntry == null)
                {
                    return;
                }
                this.OnProgressMessageEvent(nextEntry, null);
            }
        }

        protected virtual void OnProgressMessageEvent(TarEntry entry, string message)
        {
            if (this.ProgressMessageEvent != null)
            {
                this.ProgressMessageEvent(this, entry, message);
            }
        }

        public void SetAsciiTranslation(bool asciiTranslate)
        {
            this.asciiTranslate = asciiTranslate;
        }

        public void SetKeepOldFiles(bool keepOldFiles)
        {
            this.keepOldFiles = keepOldFiles;
        }

        public void SetUserInfo(int userId, string userName, int groupId, string groupName)
        {
            this.userId = userId;
            this.userName = userName;
            this.groupId = groupId;
            this.groupName = groupName;
            this.applyUserInfoOverrides = true;
        }

        public void WriteEntry(TarEntry sourceEntry, bool recurse)
        {
            try
            {
                if (recurse)
                {
                    TarHeader.SetValueDefaults(sourceEntry.UserId, sourceEntry.UserName, sourceEntry.GroupId, sourceEntry.GroupName);
                }
                this.InternalWriteEntry(sourceEntry, recurse);
            }
            finally
            {
                if (recurse)
                {
                    TarHeader.RestoreSetValues();
                }
            }
        }

        public bool ApplyUserInfoOverrides
        {
            get
            {
                return this.applyUserInfoOverrides;
            }
            set
            {
                this.applyUserInfoOverrides = value;
            }
        }

        public int GroupId
        {
            get
            {
                return this.groupId;
            }
        }

        public string GroupName
        {
            get
            {
                return this.groupName;
            }
        }

        public string PathPrefix
        {
            get
            {
                return this.pathPrefix;
            }
            set
            {
                this.pathPrefix = value;
            }
        }

        public int RecordSize
        {
            get
            {
                if (this.tarIn != null)
                {
                    return this.tarIn.GetRecordSize();
                }
                if (this.tarOut != null)
                {
                    return this.tarOut.GetRecordSize();
                }
                return 0x2800;
            }
        }

        public string RootPath
        {
            get
            {
                return this.rootPath;
            }
            set
            {
                this.rootPath = value;
            }
        }

        public int UserId
        {
            get
            {
                return this.userId;
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
        }
    }
}

