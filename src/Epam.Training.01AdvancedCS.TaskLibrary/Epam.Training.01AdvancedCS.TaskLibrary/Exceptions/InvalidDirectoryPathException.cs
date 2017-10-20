using System;

namespace Epam.Training._01AdvancedCS.TaskLibrary.Exceptions
{
    [Serializable]
    public class InvalidDirectoryNameException : Exception
    {
        public InvalidDirectoryNameException() { }
        public InvalidDirectoryNameException(string message) : base(message) { }
        public InvalidDirectoryNameException(string message, char[] chars) : base(message) { }
        public InvalidDirectoryNameException(string message, Exception inner) : base(message, inner) { }
        protected InvalidDirectoryNameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
