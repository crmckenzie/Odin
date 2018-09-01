using System;

namespace Odin
{
    internal class ParameterMisMatchException : Exception
    {
        public ParameterMisMatchException(string message) : base(message)
        {
        }
    }
}