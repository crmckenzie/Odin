using System;
using System.Runtime.Serialization;

namespace Odin.Exceptions
{
    /// <summary>
    /// Thrown when Odin is unable to parse a parameter value.
    /// </summary>
    public class ParameterConversionException : Exception
    {
        /// <summary>
        /// Gets the ParameterValue that could not be parse the token.
        /// </summary>
        public ParameterValue ParameterValue { get; }

        /// <summary>
        /// Gets the token that could not be parsed.
        /// </summary>
        public string Token { get; }
        
        public ParameterConversionException(ParameterValue parameterValue, string token, Exception exception) : 
            base($"Argument conversion failed for parameter {parameterValue.Name}.\nCould not convert '{token}' to type {parameterValue.ParameterType.FullName}.\n", exception)
        {
            ParameterValue = parameterValue;
            Token = token;
        }
    }
}