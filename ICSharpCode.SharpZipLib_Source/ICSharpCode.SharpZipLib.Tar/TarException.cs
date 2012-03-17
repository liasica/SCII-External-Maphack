namespace ICSharpCode.SharpZipLib.Tar
{
    using ICSharpCode.SharpZipLib;
    using System;

    public class TarException : SharpZipBaseException
    {
        public TarException()
        {
        }

        public TarException(string message) : base(message)
        {
        }
    }
}

