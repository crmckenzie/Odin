using System;

namespace Odin.Exceptions
{
    public class ParameterConversionException : Exception
    {
        public ParameterValue ParameterValue { get; }
        public object Value { get; }

        public ParameterConversionException(ParameterValue parameterValue, object value, Exception exception) : 
            base($"Argument conversion failed for parameter {parameterValue.Name}.\nCould not convert '{value}' to type {parameterValue.ParameterType.FullName}.\n", exception)
        {
            ParameterValue = parameterValue;
            Value = value;
        }
    }
}