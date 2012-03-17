namespace ICSharpCode.SharpZipLib.GZip
{
    using ICSharpCode.SharpZipLib;
    using System;

    public class GZipException : SharpZipBaseException
    {
        public GZipException()
        {
        }

        public GZipException(string message) : base(message)
        {
        }
    }
}

