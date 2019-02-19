using System;

namespace Odin
{
    internal class ParameterMismatchException : Exception
    {
        public ParameterMismatchException(string message) : base(message)
        {
        }
    }
}