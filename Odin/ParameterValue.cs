using System;

namespace Odin
{
    public class ParameterValue
    {
        public ParameterMap ParameterMap { get; set; }

        private object _value = null;
        private bool _isSet = false;

        public object Value { get {return _value;
            ;
        }
            set
            {
                _value = value;
                _isSet = true;
            } }

        public string Name => ParameterMap.ParameterInfo.Name;

        public ParameterValue(ParameterMap parameterMap)
        {
            ParameterMap = parameterMap;

            if (parameterMap.IsBooleanSwitch())
                this.Value = false;

            if (parameterMap.IsOptional())
                this.Value = Type.Missing;
        }

        public bool IsValueSet()
        {
            return _isSet;
        }
    }
}