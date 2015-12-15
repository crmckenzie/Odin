using System;
using System.Collections.Generic;
using System.Reflection;
using Odin.Configuration;
using Odin.Exceptions;

namespace Odin
{
    public class ParameterValue
    {
        public static Dictionary<Type, Func<object, object>> Coercion { get; set; }
        static ParameterValue()
        {
            Coercion = new Dictionary<Type, Func<object, object>>
            {
                [typeof(bool)] = o => bool.Parse(o.ToString()),
                [typeof(int)] = o => int.Parse(o.ToString())
            };
        }


        private bool _isSet;

        private object _value;

        public ParameterValue(ActionInvocation actionInvocation, ParameterMap parameterMap)
        {
            ActionInvocation = actionInvocation;
            ParameterMap = parameterMap;

            if (parameterMap.IsBooleanSwitch())
                Value = false;

            if (parameterMap.IsOptional)
                Value = Type.Missing;
        }

        public ActionInvocation ActionInvocation { get; }

        public ParameterMap ParameterMap { get; }

        public Conventions Conventions => ParameterMap.Conventions;

        public ParameterInfo ParameterInfo => ParameterMap.ParameterInfo;
        public Type ParameterType => ParameterInfo.ParameterType;

        public int Position => ParameterInfo.Position;

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

        public string[] Tokens => ActionInvocation.Tokens;
        
        public bool IsValueSet()
        {
            return _isSet;
        }
        public bool IsBooleanSwitch()
        {
            return ParameterInfo.ParameterType == typeof(bool);
        }

        public bool IsIdentifiedBy(string arg)
        {
            if (Conventions.IsIdentifiedBy(this.ParameterMap, arg))
                return true;
            return HasAlias(arg);
        }

        public bool HasAlias(string arg)
        {
            return ParameterMap.HasAlias(arg);
        }

        public object Coerce(object value)
        {
            try
            {
                var key = this.ParameterInfo.ParameterType;
                if (Coercion.ContainsKey(key))
                    return Coercion[key].Invoke(value);
                return value;
            }
            catch (Exception e)
            {
                throw new ParameterConversionException(this, value, e);
            }
        }

        public int SetValue(string arg, int i)
        {
            if (IsBooleanSwitch())
            {
                Value = true;
            }
            else if (NextArgIsIdentifier(i))
            {
                return 1;
            }
            else if (IsIdentifiedBy(arg) && HasNextValue(i))
            {
                var value = Tokens[i + 1];
                Value = Coerce(value);
                return 2;
            }
            else
            {
                Value = Coerce(arg);
            }

            return 1;
        }

        public bool HasNextValue(int indexOfCurrentArg)
        {
            return Tokens.Length > (indexOfCurrentArg + 1);
        }

        public bool NextArgIsIdentifier(int indexOfCurrentArg)
        {
            var j = indexOfCurrentArg + 1;
            if (j < Tokens.Length)
            {
                return Conventions.IsArgumentIdentifier(Tokens[j]);
            }
            return false;
        }

    }
}