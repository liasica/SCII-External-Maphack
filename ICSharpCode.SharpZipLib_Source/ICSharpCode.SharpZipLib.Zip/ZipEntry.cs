namespace ICSharpCode.SharpZipLib.Zip
{
    using System;
    using System.IO;

    public class ZipEntry : ICloneable
    {
        private string comment;
        private ulong compressedSize;
        private uint crc;
        private uint dosTime;
        private int externalFileAttributes;
        private byte[] extra;
        private int flags;
        private ushort known;
        private static int KNOWN_CRC = 4;
        private static int KNOWN_CSIZE = 2;
        private static int KNOWN_EXTERN_ATTRIBUTES = 0x10;
        private static int KNOWN_SIZE = 1;
        private static int KNOWN_TIME = 8;
        private ICSharpCode.SharpZipLib.Zip.CompressionMethod method;
        private string name;
        private int offset;
        private ulong size;
        private ushort versionMadeBy;
        private ushort versionToExtract;
        private int zipFileIndex;

        public ZipEntry(ZipEntry e)
        {
            this.known = 0;
            this.externalFileAttributes = -1;
            this.method = ICSharpCode.SharpZipLib.Zip.CompressionMethod.Deflated;
            this.extra = null;
            this.comment = null;
            this.zipFileIndex = -1;
            this.known = e.known;
            this.name = e.name;
            this.size = e.size;
            this.compressedSize = e.compressedSize;
            this.crc = e.crc;
            this.dosTime = e.dosTime;
            this.method = e.method;
            this.ExtraData = e.ExtraData;
            this.comment = e.comment;
            this.versionToExtract = e.versionToExtract;
            this.versionMadeBy = e.versionMadeBy;
            this.externalFileAttributes = e.externalFileAttributes;
            this.flags = e.flags;
            this.zipFileIndex = -1;
            this.offset = 0;
        }

        public ZipEntry(string name) : this(name, 0, 20)
        {
        }

        internal ZipEntry(string name, int versionRequiredToExtract) : this(name, versionRequiredToExtract, 20)
        {
        }

        internal ZipEntry(string name, int versionRequiredToExtract, int madeByInfo)
        {
            this.known = 0;
            this.externalFileAttributes = -1;
            this.method = ICSharpCode.SharpZipLib.Zip.CompressionMethod.Deflated;
            this.extra = null;
            this.comment = null;
            this.zipFileIndex = -1;
            if (name == null)
            {
                throw new ArgumentNullException("ZipEntry name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("ZipEntry name is empty");
            }
            if ((versionRequiredToExtract != 0) && (versionRequiredToExtract < 10))
            {
                throw new ArgumentOutOfRangeException("versionRequiredToExtract");
            }
            this.DateTime = System.DateTime.Now;
            this.name = name;
            this.versionMadeBy = (ushort) madeByInfo;
            this.versionToExtract = (ushort) versionRequiredToExtract;
        }

        public static string CleanName(string name)
        {
            return CleanName(name, true);
        }

        public static string CleanName(string name, bool relativePath)
        {
            if (name == null)
            {
                return "";
            }
            if (Path.IsPathRooted(name))
            {
                name = name.Substring(Path.GetPathRoot(name).Length);
            }
            name = name.Replace(@"\", "/");
            if (relativePath)
            {
                if ((name.Length > 0) && ((name[0] == Path.AltDirectorySeparatorChar) || (name[0] == Path.DirectorySeparatorChar)))
                {
                    name = name.Remove(0, 1);
                }
                return name;
            }
            if (((name.Length > 0) && (name[0] != Path.AltDirectorySeparatorChar)) && (name[0] != Path.DirectorySeparatorChar))
            {
                name = name.Insert(0, "/");
            }
            return name;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public override string ToString()
        {
            return this.name;
        }

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                if ((value != null) && (value.Length > 0xffff))
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.comment = value;
            }
        }

        public long CompressedSize
        {
            get
            {
                if ((this.known & KNOWN_CSIZE) == 0)
                {
                    return -1L;
                }
                return (long) this.compressedSize;
            }
            set
            {
                if ((value & -4294967296L) != 0L)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.compressedSize = (ulong) value;
                this.known = (ushort) (this.known | ((ushort) KNOWN_CSIZE));
            }
        }

        public ICSharpCode.SharpZipLib.Zip.CompressionMethod CompressionMethod
        {
            get
            {
                return this.method;
            }
            set
            {
                this.method = value;
            }
        }

        public long Crc
        {
            get
            {
                if ((this.known & KNOWN_CRC) == 0)
                {
                    return -1L;
                }
                return (long) (this.crc & 0xffffffffL);
            }
            set
            {
                if ((this.crc & 18446744069414584320L) != 0L)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.crc = (uint) value;
                this.known = (ushort) (this.known | ((ushort) KNOWN_CRC));
            }
        }

        public System.DateTime DateTime
        {
            get
            {
                if (this.dosTime == 0)
                {
                    return System.DateTime.Now;
                }
                uint num = 2 * (this.dosTime & 0x1f);
                uint num2 = (this.dosTime >> 5) & 0x3f;
                uint num3 = (this.dosTime >> 11) & 0x1f;
                uint num4 = (this.dosTime >> 0x10) & 0x1f;
                uint num5 = (this.dosTime >> 0x15) & 15;
                uint num6 = ((this.dosTime >> 0x19) & 0x7f) + 0x7bc;
                return new System.DateTime((int) num6, (int) num5, (int) num4, (int) num3, (int) num2, (int) num);
            }
            set
            {
                this.DosTime = (long) ((ulong) ((((((((value.Year - 0x7bc) & 0x7f) << 0x19) | (value.Month << 0x15)) | (value.Day << 0x10)) | (value.Hour << 11)) | (value.Minute << 5)) | (value.Second >> 1)));
            }
        }

        public long DosTime
        {
            get
            {
                if ((this.known & KNOWN_TIME) == 0)
                {
                    return 0L;
                }
                return (long) this.dosTime;
            }
            set
            {
                this.dosTime = (uint) value;
                this.known = (ushort) (this.known | ((ushort) KNOWN_TIME));
            }
        }

        public int ExternalFileAttributes
        {
            get
            {
                if ((this.known & KNOWN_EXTERN_ATTRIBUTES) == 0)
                {
                    return -1;
                }
                return this.externalFileAttributes;
            }
            set
            {
                this.externalFileAttributes = value;
                this.known = (ushort) (this.known | ((ushort) KNOWN_EXTERN_ATTRIBUTES));
            }
        }

        public byte[] ExtraData
        {
            get
            {
                return this.extra;
            }
            set
            {
                if (value == null)
                {
                    this.extra = null;
                }
                else
                {
                    if (value.Length > 0xffff)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    this.extra = new byte[value.Length];
                    Array.Copy(value, 0, this.extra, 0, value.Length);
                    try
                    {
                        int num3;
                        for (int i = 0; i < this.extra.Length; i += num3)
                        {
                            int num2 = (this.extra[i++] & 0xff) | ((this.extra[i++] & 0xff) << 8);
                            num3 = (this.extra[i++] & 0xff) | ((this.extra[i++] & 0xff) << 8);
                            if ((num3 < 0) || ((i + num3) > this.extra.Length))
                            {
                                return;
                            }
                            if (num2 == 0x5455)
                            {
                                int num4 = this.extra[i];
                                if (((num4 & 1) != 0) && (num3 >= 5))
                                {
                                    int seconds = (((this.extra[i + 1] & 0xff) | ((this.extra[i + 2] & 0xff) << 8)) | ((this.extra[i + 3] & 0xff) << 0x10)) | ((this.extra[i + 4] & 0xff) << 0x18);
                                    this.DateTime = (new System.DateTime(0x7b2, 1, 1, 0, 0, 0) + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
                                    this.known = (ushort) (this.known | ((ushort) KNOWN_TIME));
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public int Flags
        {
            get
            {
                return this.flags;
            }
            set
            {
                this.flags = value;
            }
        }

        public int HostSystem
        {
            get
            {
                return ((this.versionMadeBy >> 8) & 0xff);
            }
        }

        public bool IsCrypted
        {
            get
            {
                return ((this.flags & 1) != 0);
            }
            set
            {
                if (value)
                {
                    this.flags |= 1;
                }
                else
                {
                    this.flags &= -2;
                }
            }
        }

        public bool IsDirectory
        {
            get
            {
                int length = this.name.Length;
                bool flag = (length > 0) && (this.name[length - 1] == '/');
                if ((!flag && ((this.known & KNOWN_EXTERN_ATTRIBUTES) != 0)) && ((this.HostSystem == 0) && ((this.ExternalFileAttributes & 0x10) != 0)))
                {
                    flag = true;
                }
                return flag;
            }
        }

        public bool IsFile
        {
            get
            {
                bool flag = !this.IsDirectory;
                if ((flag && ((this.known & KNOWN_EXTERN_ATTRIBUTES) != 0)) && ((this.HostSystem == 0) && ((this.ExternalFileAttributes & 8) != 0)))
                {
                    flag = false;
                }
                return flag;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                if ((value & -4294967296L) != 0L)
                {
                    throw new ArgumentOutOfRangeException("Offset");
                }
                this.offset = value;
            }
        }

        public bool RequiresZip64
        {
            get
            {
                if (this.size <= 0xffffffffL)
                {
                    return (this.compressedSize > 0xffffffffL);
                }
                return true;
            }
        }

        public long Size
        {
            get
            {
                if ((this.known & KNOWN_SIZE) == 0)
                {
                    return -1L;
                }
                return (long) this.size;
            }
            set
            {
                if ((value & -4294967296L) != 0L)
                {
                    throw new ArgumentOutOfRangeException("size");
                }
                this.size = (ulong) value;
                this.known = (ushort) (this.known | ((ushort) KNOWN_SIZE));
            }
        }

        public int Version
        {
            get
            {
                if (this.versionToExtract != 0)
                {
                    return this.versionToExtract;
                }
                int num = 10;
                if (ICSharpCode.SharpZipLib.Zip.CompressionMethod.Deflated == this.method)
                {
                    return 20;
                }
                if (this.IsDirectory)
                {
                    return 20;
                }
                if (this.IsCrypted)
                {
                    return 20;
                }
                if (((this.known & KNOWN_EXTERN_ATTRIBUTES) != 0) && ((this.externalFileAttributes & 8) != 0))
                {
                    num = 11;
                }
                return num;
            }
        }

        public int VersionMadeBy
        {
            get
            {
                return (this.versionMadeBy & 0xff);
            }
        }

        public int ZipFileIndex
        {
            get
            {
                return this.zipFileIndex;
            }
            set
            {
                this.zipFileIndex = value;
            }
        }
    }
}

