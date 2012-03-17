namespace Foole.Mpq
{
    using System;

    public class MpqParserException : Exception
    {
        public MpqParserException()
        {
        }

        public MpqParserException(string message) : base(message)
        {
        }

        public MpqParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

