using System;

namespace Odin
{
    public class ParameterConversionException : Exception
    {
        public ParameterMap ParameterMap { get; }
        public object Value { get; }

        public ParameterConversionException(ParameterMap parameterMap, object value, Exception exception) : 
            base($"Argument conversion failed for parameter {parameterMap.Name}.\nCould not convert '{value}' to type {parameterMap.ParameterType.FullName}.\n", exception)
        {
            ParameterMap = parameterMap;
            Value = value;
        }
    }
}