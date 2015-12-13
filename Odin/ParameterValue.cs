using System;
using System.Reflection;
using Odin.Configuration;

namespace Odin
{
    public class ParameterValue
    {
        private bool _isSet;

        private object _value;

        public ParameterValue(ParameterMap parameterMap)
        {
            ParameterMap = parameterMap;

            if (parameterMap.IsBooleanSwitch())
                Value = false;

            if (parameterMap.IsOptional)
                Value = Type.Missing;
        }

        public ParameterMap ParameterMap { get; }

        public Conventions Conventions => ParameterMap.Conventions;

        public ParameterInfo ParameterInfo => ParameterMap.ParameterInfo;

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _isSet = true;
            }
        }

        public string Name => ParameterMap.ParameterInfo.Name;

        public bool IsValueSet()
        {
            return _isSet;
        }

        public bool IsIdentifiedBy(string arg)
        {
            if (arg == Conventions.GetArgumentName(this.ParameterInfo))
                return true;
            return HasAlias(arg);
        }

        public bool HasAlias(string arg)
        {
            return ParameterMap.HasAlias(arg);
        }
    }
}