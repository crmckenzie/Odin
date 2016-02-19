using System;

namespace Odin.Exceptions
{
    /// <summary>
    /// Thrown when a token specifies a non-existent parameter to an action.
    /// </summary>
    public class UnmappedParameterException : Exception
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public UnmappedParameterException(string message) : base(message)
        {
            
        }
    }
}