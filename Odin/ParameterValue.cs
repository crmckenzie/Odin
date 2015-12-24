using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Exceptions;

namespace Odin
{
    public class ParameterValue
    {
        private CustomParser _customParser;


        private bool _isSet;

        private object _value;

        static ParameterValue()
        {
            Coercion = new Dictionary<Type, Func<object, object>>
            {
                [typeof (bool)] = o => bool.Parse(o.ToString()),
                [typeof (int)] = o => int.Parse(o.ToString()),
                [typeof (long)] = o => long.Parse(o.ToString()),
                [typeof (double)] = o => double.Parse(o.ToString()),
                [typeof (decimal)] = o => decimal.Parse(o.ToString()),
                [typeof (DateTime)] = o => DateTime.Parse(o.ToString()),
                [typeof (bool?)] = o => bool.Parse(o.ToString()),
                [typeof (int?)] = o => int.Parse(o.ToString()),
                [typeof (long?)] = o => long.Parse(o.ToString()),
                [typeof (double?)] = o => double.Parse(o.ToString()),
                [typeof (decimal?)] = o => decimal.Parse(o.ToString()),
                [typeof (DateTime?)] = o => DateTime.Parse(o.ToString())
            };
        }

        public ParameterValue(MethodInvocation methodInvocation, ParameterInfo parameterInfo)
        {
            MethodInvocation = methodInvocation;
            ParameterInfo = parameterInfo;

            if (ParameterType == typeof (bool))
                Value = false;

            if (IsNullableType())
                Value = null;

            if (ParameterInfo.IsOptional)
                Value = Type.Missing;
        }

        private static Dictionary<Type, Func<object, object>> Coercion { get; }


        private MethodInvocation MethodInvocation { get; }

        public Conventions Conventions => MethodInvocation.Conventions;

        public ParameterInfo ParameterInfo { get; }
        public Type ParameterType => ParameterInfo.ParameterType;

        public int Position => ParameterInfo.Position;

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isSet = true;
            }
        }

        public string Name => ParameterInfo.Name;

        public string LongOptionName => Conventions.GetLongOptionName(ParameterInfo);

        public CustomParser CustomParser
        {
            get
            {
                if (!HasCustomParser())
                    return null;

                if (_customParser == null)
                {
                    _customParser = CreateCustomParser();
                }
                return _customParser;
            }
        }

        private bool IsNullableType()
        {
            return ParameterType.IsGenericType && ParameterType.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        private CustomParser CreateCustomParser()
        {
            var parserAttribute = ParameterInfo.GetCustomAttribute<ParserAttribute>();
            var types = new[] {typeof (ParameterValue)};
            var constructor = parserAttribute.ParserType.GetConstructor(types);
            if (constructor == null)
            {
                throw new TypeInitializationException(
                    "Could not find a constructor with the signature (ParameterValue).", null);
            }

            var parameters = new[] {this};
            var instance = constructor.Invoke(parameters);
            var typedInstance = (CustomParser) instance;
            return typedInstance;
        }

        public string GetDescription()
        {
            var attr = ParameterInfo.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
                return attr.Description;
            return "";
        }

        public bool IsValueSet()
        {
            return _isSet;
        }

        internal bool IsBoolean()
        {
            return ParameterType == typeof (bool)
                   || ParameterType == typeof (bool?)
                ;
        }

        public bool IsIdentifiedBy(string token)
        {
            if (Conventions.IsMatchingParameter(this, token))
                return true;
            return HasAlias(token);
        }

        private bool MatchesAlias(AliasAttribute aliasAttribute, string arg)
        {
            if (aliasAttribute == null)
                return false;

            var aliases = aliasAttribute.Aliases.Select(Conventions.GetShortOptionName);
            return aliases.Contains(arg);
        }

        private bool HasAlias(string arg)
        {
            var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
            return MatchesAlias(attr, arg);
        }

        public object Coerce(object value)
        {
            try
            {
                var key = ParameterType;
                if (Coercion.ContainsKey(key))
                    return Coercion[key].Invoke(value);

                if (key.IsEnum)
                    return Enum.Parse(key, value.ToString());

                if (!IsNullableType()) return value;

                var genericType = key.GetGenericArguments()[0];
                if (genericType.IsEnum)
                    return Enum.Parse(genericType, value.ToString());

                return value;
            }
            catch (Exception e)
            {
                throw new ParameterConversionException(this, value, e);
            }
        }


        public bool HasAliases()
        {
            var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
            if (attr == null)
                return false;

            return attr.Aliases.Any();
        }

        public string[] GetAliases()
        {
            var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
            if (attr == null)
                return new string[] {};

            return attr.Aliases.Select(a => Conventions.GetShortOptionName(a)).ToArray();
        }


        public bool HasCustomParser()
        {
            return ParameterInfo.GetCustomAttribute<ParserAttribute>() != null;
        }

        public bool IsArray()
        {
            return ParameterType.IsSubclassOf(typeof (Array));
        }
    }
}