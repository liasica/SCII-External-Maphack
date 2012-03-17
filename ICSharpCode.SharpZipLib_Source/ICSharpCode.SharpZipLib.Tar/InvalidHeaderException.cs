namespace ICSharpCode.SharpZipLib.Tar
{
    using System;

    public class InvalidHeaderException : TarException
    {
        public InvalidHeaderException()
        {
        }

        public InvalidHeaderException(string msg) : base(msg)
        {
        }
    }
}

