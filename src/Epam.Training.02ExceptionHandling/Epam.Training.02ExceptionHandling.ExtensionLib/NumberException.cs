using System;

namespace Epam.Training._02ExceptionHandling.ExtensionLib
{
    [Serializable]
    public class NumberException : Exception
    {
        public NumberException()
        {
        }

        public NumberException(string message, int charPosition) : base(message)
        {
            CharPosition = charPosition;
        }

        public NumberException(string message, Exception inner, int charPosition) : base(message, inner)
        {
            CharPosition = charPosition;
        }

        protected NumberException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public int? CharPosition { get; private set;}
    }
}
