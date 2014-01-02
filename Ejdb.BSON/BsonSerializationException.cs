using System;
using System.Runtime.Serialization;

namespace Ejdb.BSON
{
    [Serializable]
    public class BsonSerializationException : Exception
    {
        public BsonSerializationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        public BsonSerializationException(string message) : base(message)
        {
        }

        public BsonSerializationException()
        {
        }

        protected BsonSerializationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}