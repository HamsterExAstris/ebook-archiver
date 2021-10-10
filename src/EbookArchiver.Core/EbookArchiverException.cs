using System;
using System.Runtime.Serialization;

namespace EbookArchiver
{
    public class EbookArchiverException : Exception
    {
        public EbookArchiverException()
        {
        }

        public EbookArchiverException(string? message) : base(message)
        {
        }

        public EbookArchiverException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EbookArchiverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
