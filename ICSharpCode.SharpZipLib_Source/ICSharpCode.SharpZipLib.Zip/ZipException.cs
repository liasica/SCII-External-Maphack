namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib;
    using System;

    public class ZipException : SharpZipBaseException
    {
        public ZipException()
        {
        }

        public ZipException(string msg) : base(msg)
        {
        }
    }
}

