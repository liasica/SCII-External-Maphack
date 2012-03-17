namespace ICSharpCode.SharpZipLib.Zip
{
    using System;
    using System.Text;

    public sealed class ZipConstants
    {
        public const int CENATT = 0x24;
        public const int CENATX = 0x26;
        public const int CENCOM = 0x20;
        public const int CENCRC = 0x10;
        public const int CENDIGITALSIG = 0x5054b50;
        public const int CENDSK = 0x22;
        public const int CENEXT = 30;
        public const int CENFLG = 8;
        public const int CENHDR = 0x2e;
        public const int CENHOW = 10;
        public const int CENLEN = 0x18;
        public const int CENNAM = 0x1c;
        public const int CENOFF = 0x2a;
        public const int CENSIG = 0x2014b50;
        public const int CENSIG64 = 0x6064b50;
        public const int CENSIZ = 20;
        public const int CENTIM = 12;
        public const int CENVEM = 4;
        public const int CENVER = 6;
        public const int CRYPTO_HEADER_SIZE = 12;
        private static int defaultCodePage = 0;
        public const int ENDCOM = 20;
        public const int ENDDCD = 6;
        public const int ENDHDR = 0x16;
        public const int ENDNRD = 4;
        public const int ENDOFF = 0x10;
        public const int ENDSIG = 0x6054b50;
        public const int ENDSIZ = 12;
        public const int ENDSUB = 8;
        public const int ENDTOT = 10;
        public const int EXTCRC = 4;
        public const int EXTHDR = 0x10;
        public const int EXTLEN = 12;
        public const int EXTSIG = 0x8074b50;
        public const int EXTSIZ = 8;
        public const int LOCCRC = 14;
        public const int LOCEXT = 0x1c;
        public const int LOCFLG = 6;
        public const int LOCHDR = 30;
        public const int LOCHOW = 8;
        public const int LOCLEN = 0x16;
        public const int LOCNAM = 0x1a;
        public const int LOCSIG = 0x4034b50;
        public const int LOCSIZ = 0x12;
        public const int LOCTIM = 10;
        public const int LOCVER = 4;
        public const int SPANNINGSIG = 0x8074b50;
        public const int SPANTEMPSIG = 0x30304b50;
        public const int VERSION_MADE_BY = 20;
        public const int VERSION_STRONG_ENCRYPTION = 50;

        public static byte[] ConvertToArray(string str)
        {
            return Encoding.GetEncoding(DefaultCodePage).GetBytes(str);
        }

        public static string ConvertToString(byte[] data)
        {
            return ConvertToString(data, data.Length);
        }

        public static string ConvertToString(byte[] data, int length)
        {
            return Encoding.GetEncoding(DefaultCodePage).GetString(data, 0, length);
        }

        public static int DefaultCodePage
        {
            get
            {
                return defaultCodePage;
            }
            set
            {
                defaultCodePage = value;
            }
        }
    }
}

