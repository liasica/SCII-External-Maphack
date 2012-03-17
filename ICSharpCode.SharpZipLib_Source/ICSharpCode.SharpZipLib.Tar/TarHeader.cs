namespace ICSharpCode.SharpZipLib.Tar
{
    using System;
    using System.Text;

    public class TarHeader : ICloneable
    {
        private int checksum;
        public static readonly int CHKSUMLEN = 8;
        public const int CHKSUMOFS = 0x94;
        private static readonly DateTime dateTime1970 = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
        internal static int defaultGroupId = 0;
        internal static string defaultGroupName = "None";
        internal static string defaultUser = null;
        internal static int defaultUserId = 0;
        public static readonly int DEVLEN = 8;
        private int devMajor;
        private int devMinor;
        public static readonly int GIDLEN = 8;
        public static readonly int GNAMELEN = 0x20;
        public static readonly string GNU_TMAGIC = "ustar  ";
        private int groupId;
        internal static int groupIdAsSet = 0;
        private string groupName;
        internal static string groupNameAsSet = "None";
        private bool isChecksumValid;
        public const byte LF_ACL = 0x41;
        public const byte LF_BLK = 0x34;
        public const byte LF_CHR = 0x33;
        public const byte LF_CONTIG = 0x37;
        public const byte LF_DIR = 0x35;
        public const byte LF_EXTATTR = 0x45;
        public const byte LF_FIFO = 0x36;
        public const byte LF_GHDR = 0x67;
        public const byte LF_GNU_DUMPDIR = 0x44;
        public const byte LF_GNU_LONGLINK = 0x4b;
        public const byte LF_GNU_LONGNAME = 0x4c;
        public const byte LF_GNU_MULTIVOL = 0x4d;
        public const byte LF_GNU_NAMES = 0x4e;
        public const byte LF_GNU_SPARSE = 0x53;
        public const byte LF_GNU_VOLHDR = 0x56;
        public const byte LF_LINK = 0x31;
        public const byte LF_META = 0x49;
        public const byte LF_NORMAL = 0x30;
        public const byte LF_OLDNORM = 0;
        public const byte LF_SYMLINK = 50;
        public static readonly byte LF_XHDR = 120;
        private string linkName;
        private string magic;
        public static readonly int MAGICLEN = 6;
        private int mode;
        public static readonly int MODELEN = 8;
        private DateTime modTime;
        public static readonly int MODTIMELEN = 12;
        private string name;
        public static readonly int NAMELEN = 100;
        private long size;
        public static readonly int SIZELEN = 12;
        private static readonly long timeConversionFactor = 0x989680L;
        public static readonly string TMAGIC = "ustar ";
        private byte typeFlag;
        public static readonly int UIDLEN = 8;
        public static readonly int UNAMELEN = 0x20;
        private int userId;
        internal static int userIdAsSet = 0;
        private string userName;
        internal static string userNameAsSet = null;
        private string version;
        public static readonly int VERSIONLEN = 2;

        public TarHeader()
        {
            this.Magic = TMAGIC;
            this.Version = " ";
            this.Name = "";
            this.LinkName = "";
            this.UserId = defaultUserId;
            this.GroupId = defaultGroupId;
            this.UserName = defaultUser;
            this.GroupName = defaultGroupName;
            this.Size = 0L;
        }

        public object Clone()
        {
            return new TarHeader { Name = this.Name, Mode = this.Mode, UserId = this.UserId, GroupId = this.GroupId, Size = this.Size, ModTime = this.ModTime, TypeFlag = this.TypeFlag, LinkName = this.LinkName, Magic = this.Magic, Version = this.Version, UserName = this.UserName, GroupName = this.GroupName, DevMajor = this.DevMajor, DevMinor = this.DevMinor };
        }

        private static int ComputeCheckSum(byte[] buf)
        {
            int num = 0;
            for (int i = 0; i < buf.Length; i++)
            {
                num += buf[i];
            }
            return num;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TarHeader))
            {
                return false;
            }
            TarHeader header = obj as TarHeader;
            return ((((((this.name == header.name) && (this.mode == header.mode)) && ((this.UserId == header.UserId) && (this.GroupId == header.GroupId))) && (((this.Size == header.Size) && (this.ModTime == header.ModTime)) && ((this.Checksum == header.Checksum) && (this.TypeFlag == header.TypeFlag)))) && ((((this.LinkName == header.LinkName) && (this.Magic == header.Magic)) && ((this.Version == header.Version) && (this.UserName == header.UserName))) && ((this.GroupName == header.GroupName) && (this.DevMajor == header.DevMajor)))) && (this.DevMinor == header.DevMinor));
        }

        public static int GetAsciiBytes(string toAdd, int nameOffset, byte[] buffer, int bufferOffset, int length)
        {
            for (int i = 0; (i < length) && ((nameOffset + i) < toAdd.Length); i++)
            {
                buffer[bufferOffset + i] = (byte) toAdd[nameOffset + i];
            }
            return (bufferOffset + length);
        }

        private static int GetCheckSumOctalBytes(long val, byte[] buf, int offset, int length)
        {
            GetOctalBytes(val, buf, offset, length - 1);
            return (offset + length);
        }

        private static int GetCTime(DateTime dateTime)
        {
            return (int) ((dateTime.Ticks - dateTime1970.Ticks) / timeConversionFactor);
        }

        private static DateTime GetDateTimeFromCTime(long ticks)
        {
            try
            {
                return new DateTime(dateTime1970.Ticks + (ticks * timeConversionFactor));
            }
            catch
            {
                return dateTime1970;
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public static int GetLongOctalBytes(long val, byte[] buf, int offset, int length)
        {
            return GetOctalBytes(val, buf, offset, length);
        }

        [Obsolete]
        public string GetName()
        {
            return this.name.ToString();
        }

        public static int GetNameBytes(string name, byte[] buf, int offset, int length)
        {
            return GetNameBytes(name, 0, buf, offset, length);
        }

        public static int GetNameBytes(StringBuilder name, byte[] buf, int offset, int length)
        {
            return GetNameBytes(name.ToString(), 0, buf, offset, length);
        }

        public static int GetNameBytes(string name, int nameOffset, byte[] buf, int bufferOffset, int length)
        {
            int num = 0;
            while ((num < (length - 1)) && ((nameOffset + num) < name.Length))
            {
                buf[bufferOffset + num] = (byte) name[nameOffset + num];
                num++;
            }
            while (num < length)
            {
                buf[bufferOffset + num] = 0;
                num++;
            }
            return (bufferOffset + length);
        }

        public static int GetNameBytes(StringBuilder name, int nameOffset, byte[] buf, int bufferOffset, int length)
        {
            return GetNameBytes(name.ToString(), nameOffset, buf, bufferOffset, length);
        }

        public static int GetOctalBytes(long val, byte[] buf, int offset, int length)
        {
            int num = length - 1;
            buf[offset + num] = 0;
            num--;
            if (val > 0L)
            {
                long num2 = val;
                while ((num >= 0) && (num2 > 0L))
                {
                    buf[offset + num] = (byte) (0x30 + ((byte) (num2 & 7L)));
                    num2 = num2 >> 3;
                    num--;
                }
            }
            while (num >= 0)
            {
                buf[offset + num] = 0x30;
                num--;
            }
            return (offset + length);
        }

        private static int MakeCheckSum(byte[] buf)
        {
            int num = 0;
            for (int i = 0; i < 0x94; i++)
            {
                num += buf[i];
            }
            for (int j = 0; j < CHKSUMLEN; j++)
            {
                num += 0x20;
            }
            for (int k = 0x94 + CHKSUMLEN; k < buf.Length; k++)
            {
                num += buf[k];
            }
            return num;
        }

        public void ParseBuffer(byte[] header)
        {
            int offset = 0;
            this.name = ParseName(header, offset, NAMELEN).ToString();
            offset += NAMELEN;
            this.mode = (int) ParseOctal(header, offset, MODELEN);
            offset += MODELEN;
            this.UserId = (int) ParseOctal(header, offset, UIDLEN);
            offset += UIDLEN;
            this.GroupId = (int) ParseOctal(header, offset, GIDLEN);
            offset += GIDLEN;
            this.Size = ParseOctal(header, offset, SIZELEN);
            offset += SIZELEN;
            this.ModTime = GetDateTimeFromCTime(ParseOctal(header, offset, MODTIMELEN));
            offset += MODTIMELEN;
            this.checksum = (int) ParseOctal(header, offset, CHKSUMLEN);
            offset += CHKSUMLEN;
            this.TypeFlag = header[offset++];
            this.LinkName = ParseName(header, offset, NAMELEN).ToString();
            offset += NAMELEN;
            this.Magic = ParseName(header, offset, MAGICLEN).ToString();
            offset += MAGICLEN;
            this.Version = ParseName(header, offset, VERSIONLEN).ToString();
            offset += VERSIONLEN;
            this.UserName = ParseName(header, offset, UNAMELEN).ToString();
            offset += UNAMELEN;
            this.GroupName = ParseName(header, offset, GNAMELEN).ToString();
            offset += GNAMELEN;
            this.DevMajor = (int) ParseOctal(header, offset, DEVLEN);
            offset += DEVLEN;
            this.DevMinor = (int) ParseOctal(header, offset, DEVLEN);
            this.isChecksumValid = this.Checksum == MakeCheckSum(header);
        }

        public static StringBuilder ParseName(byte[] header, int offset, int length)
        {
            StringBuilder builder = new StringBuilder(length);
            for (int i = offset; i < (offset + length); i++)
            {
                if (header[i] == 0)
                {
                    return builder;
                }
                builder.Append((char) header[i]);
            }
            return builder;
        }

        public static long ParseOctal(byte[] header, int offset, int length)
        {
            long num = 0L;
            bool flag = true;
            int num2 = offset + length;
            for (int i = offset; i < num2; i++)
            {
                if (header[i] == 0)
                {
                    return num;
                }
                if ((header[i] == 0x20) || (header[i] == 0x30))
                {
                    if (flag)
                    {
                        continue;
                    }
                    if (header[i] == 0x20)
                    {
                        return num;
                    }
                }
                flag = false;
                num = (num << 3) + (header[i] - 0x30);
            }
            return num;
        }

        public static void ResetValueDefaults()
        {
            defaultUserId = 0;
            defaultGroupId = 0;
            defaultGroupName = "None";
            defaultUser = null;
        }

        internal static void RestoreSetValues()
        {
            defaultUserId = userIdAsSet;
            defaultUser = userNameAsSet;
            defaultGroupId = groupIdAsSet;
            defaultGroupName = groupNameAsSet;
        }

        internal static void SetActiveDefaults(int userId, string userName, int groupId, string groupName)
        {
            defaultUserId = userId;
            defaultUser = userName;
            defaultGroupId = groupId;
            defaultGroupName = groupName;
        }

        public static void SetValueDefaults(int userId, string userName, int groupId, string groupName)
        {
            defaultUserId = userIdAsSet = userId;
            defaultUser = userNameAsSet = userName;
            defaultGroupId = groupIdAsSet = groupId;
            defaultGroupName = groupNameAsSet = groupName;
        }

        public void WriteHeader(byte[] outbuf)
        {
            int offset = 0;
            offset = GetNameBytes(this.Name, outbuf, offset, NAMELEN);
            offset = GetOctalBytes((long) this.mode, outbuf, offset, MODELEN);
            offset = GetOctalBytes((long) this.UserId, outbuf, offset, UIDLEN);
            offset = GetOctalBytes((long) this.GroupId, outbuf, offset, GIDLEN);
            offset = GetLongOctalBytes(this.Size, outbuf, offset, SIZELEN);
            offset = GetLongOctalBytes((long) GetCTime(this.ModTime), outbuf, offset, MODTIMELEN);
            int num3 = offset;
            for (int i = 0; i < CHKSUMLEN; i++)
            {
                outbuf[offset++] = 0x20;
            }
            outbuf[offset++] = this.TypeFlag;
            offset = GetNameBytes(this.LinkName, outbuf, offset, NAMELEN);
            offset = GetAsciiBytes(this.Magic, 0, outbuf, offset, MAGICLEN);
            offset = GetNameBytes(this.Version, outbuf, offset, VERSIONLEN);
            offset = GetNameBytes(this.UserName, outbuf, offset, UNAMELEN);
            offset = GetNameBytes(this.GroupName, outbuf, offset, GNAMELEN);
            if ((this.TypeFlag == 0x33) || (this.TypeFlag == 0x34))
            {
                offset = GetOctalBytes((long) this.DevMajor, outbuf, offset, DEVLEN);
                offset = GetOctalBytes((long) this.DevMinor, outbuf, offset, DEVLEN);
            }
            while (offset < outbuf.Length)
            {
                outbuf[offset++] = 0;
            }
            this.checksum = ComputeCheckSum(outbuf);
            GetCheckSumOctalBytes((long) this.checksum, outbuf, num3, CHKSUMLEN);
            this.isChecksumValid = true;
        }

        public int Checksum
        {
            get
            {
                return this.checksum;
            }
        }

        public int DevMajor
        {
            get
            {
                return this.devMajor;
            }
            set
            {
                this.devMajor = value;
            }
        }

        public int DevMinor
        {
            get
            {
                return this.devMinor;
            }
            set
            {
                this.devMinor = value;
            }
        }

        public int GroupId
        {
            get
            {
                return this.groupId;
            }
            set
            {
                this.groupId = value;
            }
        }

        public string GroupName
        {
            get
            {
                return this.groupName;
            }
            set
            {
                if (value == null)
                {
                    this.groupName = "None";
                }
                else
                {
                    this.groupName = value;
                }
            }
        }

        public bool IsChecksumValid
        {
            get
            {
                return this.isChecksumValid;
            }
        }

        public string LinkName
        {
            get
            {
                return this.linkName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this.linkName = value;
            }
        }

        public string Magic
        {
            get
            {
                return this.magic;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this.magic = value;
            }
        }

        public int Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
            }
        }

        public DateTime ModTime
        {
            get
            {
                return this.modTime;
            }
            set
            {
                if (value < dateTime1970)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.modTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this.name = value;
            }
        }

        public long Size
        {
            get
            {
                return this.size;
            }
            set
            {
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.size = value;
            }
        }

        public byte TypeFlag
        {
            get
            {
                return this.typeFlag;
            }
            set
            {
                this.typeFlag = value;
            }
        }

        public int UserId
        {
            get
            {
                return this.userId;
            }
            set
            {
                this.userId = value;
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                if (value != null)
                {
                    this.userName = value.Substring(0, Math.Min(UNAMELEN, value.Length));
                }
                else
                {
                    string userName = Environment.UserName;
                    if (userName.Length > UNAMELEN)
                    {
                        userName = userName.Substring(0, UNAMELEN);
                    }
                    this.userName = userName;
                }
            }
        }

        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this.version = value;
            }
        }
    }
}

