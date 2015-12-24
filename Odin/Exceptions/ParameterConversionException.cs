using System;
using System.Runtime.Serialization;

namespace Odin.Exceptions
{
    [Serializable]
    public class ParameterConversionException : Exception
    {
        public ParameterValue ParameterValue { get; }

        public string Token { get; }
        
        public ParameterConversionException(ParameterValue parameterValue, string token, Exception exception) : 
            base($"Argument conversion failed for parameter {parameterValue.Name}.\nCould not convert '{token}' to type {parameterValue.ParameterType.FullName}.\n", exception)
        {
            ParameterValue = parameterValue;
            Token = token;
        }
    }
}