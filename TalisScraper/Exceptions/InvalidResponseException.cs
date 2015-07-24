using System;
using System.Runtime.Serialization;

namespace TalisScraper.Exceptions
{
    /// <summary>
    /// Custom exception used for JsonScraper. Thrown the response from a valid endpoint is not of the expected format.
    /// </summary>
    [Serializable]
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException() : base()
        {
        }

        public InvalidResponseException(string message)
            : base(message)
        { }

        // Ensure Exception is Serializable
        protected InvalidResponseException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }
}
