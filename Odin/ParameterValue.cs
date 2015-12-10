using System;

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

            if (parameterMap.IsOptional())
                Value = Type.Missing;
        }

        public ParameterMap ParameterMap { get; set; }

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
    }
}