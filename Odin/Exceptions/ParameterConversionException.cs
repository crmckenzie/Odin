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
        public Parameter Parameter { get; }

        /// <summary>
        /// Gets the token that could not be parsed.
        /// </summary>
        public string Token { get; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="token"></param>
        /// <param name="exception"></param>
        public ParameterConversionException(Parameter parameter, string token, Exception exception) : 
            base($"Argument conversion failed for parameter {parameter.Name}.\nCould not convert '{token}' to type {parameter.ParameterType.FullName}.\n", exception)
        {
            Parameter = parameter;
            Token = token;
        }
    }
}