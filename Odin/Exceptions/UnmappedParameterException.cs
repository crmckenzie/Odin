using System;

namespace Odin.Exceptions
{
    [Serializable]
    public class UnmappedParameterException : Exception
    {
        public UnmappedParameterException(string message) : base(message)
        {
            
        }
    }
}