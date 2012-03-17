namespace ICSharpCode.SharpZipLib.Tar
{
    using System;
    using System.IO;

    public class TarEntry : ICloneable
    {
        private string file;
        private ICSharpCode.SharpZipLib.Tar.TarHeader header;

        private TarEntry()
        {
        }

        public TarEntry(byte[] headerBuf)
        {
            this.Initialize();
            this.header.ParseBuffer(headerBuf);
        }

        public TarEntry(ICSharpCode.SharpZipLib.Tar.TarHeader header)
        {
            this.file = null;
            this.header = header;
        }

        public void AdjustEntryName(byte[] outbuf, string newName)
        {
            int offset = 0;
            ICSharpCode.SharpZipLib.Tar.TarHeader.GetNameBytes(newName, outbuf, offset, ICSharpCode.SharpZipLib.Tar.TarHeader.NAMELEN);
        }

        public object Clone()
        {
            return new TarEntry { file = this.file, header = (ICSharpCode.SharpZipLib.Tar.TarHeader) this.header.Clone(), Name = this.Name };
        }

        public static TarEntry CreateEntryFromFile(string fileName)
        {
            TarEntry entry = new TarEntry();
            entry.Initialize();
            entry.GetFileTarHeader(entry.header, fileName);
            return entry;
        }

        public static TarEntry CreateTarEntry(string name)
        {
            TarEntry entry = new TarEntry();
            entry.Initialize();
            entry.NameTarHeader(entry.header, name);
            return entry;
        }

        public override bool Equals(object it)
        {
            return ((it is TarEntry) && this.Name.Equals(((TarEntry) it).Name));
        }

        public TarEntry[] GetDirectoryEntries()
        {
            if ((this.file == null) || !Directory.Exists(this.file))
            {
                return new TarEntry[0];
            }
            string[] fileSystemEntries = Directory.GetFileSystemEntries(this.file);
            TarEntry[] entryArray = new TarEntry[fileSystemEntries.Length];
            for (int i = 0; i < fileSystemEntries.Length; i++)
            {
                entryArray[i] = CreateEntryFromFile(fileSystemEntries[i]);
            }
            return entryArray;
        }

        public void GetFileTarHeader(ICSharpCode.SharpZipLib.Tar.TarHeader hdr, string file)
        {
            this.file = file;
            string str = file;
            if (str.IndexOf(Environment.CurrentDirectory) == 0)
            {
                str = str.Substring(Environment.CurrentDirectory.Length);
            }
            str = str.Replace(Path.DirectorySeparatorChar, '/');
            while (str.StartsWith("/"))
            {
                str = str.Substring(1);
            }
            hdr.LinkName = string.Empty;
            hdr.Name = str;
            if (Directory.Exists(file))
            {
                hdr.Mode = 0x3eb;
                hdr.TypeFlag = 0x35;
                if ((hdr.Name.Length == 0) || (hdr.Name[hdr.Name.Length - 1] != '/'))
                {
                    hdr.Name = hdr.Name + "/";
                }
                hdr.Size = 0L;
            }
            else
            {
                hdr.Mode = 0x81c0;
                hdr.TypeFlag = 0x30;
                hdr.Size = new FileInfo(file.Replace('/', Path.DirectorySeparatorChar)).Length;
            }
            hdr.ModTime = System.IO.File.GetLastWriteTime(file.Replace('/', Path.DirectorySeparatorChar)).ToUniversalTime();
            hdr.DevMajor = 0;
            hdr.DevMinor = 0;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        private void Initialize()
        {
            this.file = null;
            this.header = new ICSharpCode.SharpZipLib.Tar.TarHeader();
        }

        public bool IsDescendent(TarEntry desc)
        {
            return desc.Name.StartsWith(this.Name);
        }

        public void NameTarHeader(ICSharpCode.SharpZipLib.Tar.TarHeader hdr, string name)
        {
            bool flag = name.EndsWith("/");
            hdr.Name = name;
            hdr.Mode = flag ? 0x3eb : 0x81c0;
            hdr.UserId = 0;
            hdr.GroupId = 0;
            hdr.Size = 0L;
            hdr.ModTime = DateTime.UtcNow;
            hdr.TypeFlag = flag ? ((byte) 0x35) : ((byte) 0x30);
            hdr.LinkName = string.Empty;
            hdr.UserName = string.Empty;
            hdr.GroupName = string.Empty;
            hdr.DevMajor = 0;
            hdr.DevMinor = 0;
        }

        public void SetIds(int userId, int groupId)
        {
            this.UserId = userId;
            this.GroupId = groupId;
        }

        public void SetNames(string userName, string groupName)
        {
            this.UserName = userName;
            this.GroupName = groupName;
        }

        public void WriteEntryHeader(byte[] outbuf)
        {
            this.header.WriteHeader(outbuf);
        }

        public string File
        {
            get
            {
                return this.file;
            }
        }

        public int GroupId
        {
            get
            {
                return this.header.GroupId;
            }
            set
            {
                this.header.GroupId = value;
            }
        }

        public string GroupName
        {
            get
            {
                return this.header.GroupName;
            }
            set
            {
                this.header.GroupName = value;
            }
        }

        public bool IsDirectory
        {
            get
            {
                if (this.file != null)
                {
                    return Directory.Exists(this.file);
                }
                if ((this.header == null) || ((this.header.TypeFlag != 0x35) && !this.Name.EndsWith("/")))
                {
                    return false;
                }
                return true;
            }
        }

        public DateTime ModTime
        {
            get
            {
                return this.header.ModTime;
            }
            set
            {
                this.header.ModTime = value;
            }
        }

        public string Name
        {
            get
            {
                return this.header.Name;
            }
            set
            {
                this.header.Name = value;
            }
        }

        public long Size
        {
            get
            {
                return this.header.Size;
            }
            set
            {
                this.header.Size = value;
            }
        }

        public ICSharpCode.SharpZipLib.Tar.TarHeader TarHeader
        {
            get
            {
                return this.header;
            }
        }

        public int UserId
        {
            get
            {
                return this.header.UserId;
            }
            set
            {
                this.header.UserId = value;
            }
        }

        public string UserName
        {
            get
            {
                return this.header.UserName;
            }
            set
            {
                this.header.UserName = value;
            }
        }
    }
}

