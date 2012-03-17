namespace ICSharpCode.SharpZipLib.BZip2
{
    using ICSharpCode.SharpZipLib;
    using System;

    public class BZip2Exception : SharpZipBaseException
    {
        public BZip2Exception()
        {
        }

        public BZip2Exception(string message) : base(message)
        {
        }
    }
}

