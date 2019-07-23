using System;
using System.Runtime.Serialization;

namespace LexoAlgorithm
{
    public class LexoException : Exception
    {
        public LexoException()
        {
        }

        protected LexoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public LexoException(string message) : base(message)
        {
        }

        public LexoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}