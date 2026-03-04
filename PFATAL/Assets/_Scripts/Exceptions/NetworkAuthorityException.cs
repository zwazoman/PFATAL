using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace _Scripts.Exceptions
{
    /// <summary>
    /// thrown when a client tries to call a method that should only be called by the server or the host
    /// </summary>
    public class NetworkAuthorityException : Exception
    {
        public NetworkAuthorityException()
        {
        }

        protected NetworkAuthorityException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NetworkAuthorityException(string message) : base(message)
        {
        }

        public NetworkAuthorityException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
    }
}