using System;
using System.Collections.Generic;

namespace Odin.Parsing
{
    public static class ReflectionExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsArray(this Type type)
        {
            return type.IsSubclassOf(typeof(Array));
        }
        public static bool IsBoolean(this Type type)
        {
            return type == typeof(bool)
                   || type == typeof(bool?)
                ;
        }

        public static bool Implements<T>(this Type type)
        {
            var interfaceType = typeof (T);
            return type.GetInterface(interfaceType.FullName) != null;
        }

        private static Dictionary<Type, Func<object, object>> Coercion { get; } 
            = new Dictionary<Type, Func<object, object>>()
        {
            [typeof(bool)] = o => bool.Parse(o.ToString()),
            [typeof(int)] = o => int.Parse(o.ToString()),
            [typeof(long)] = o => long.Parse(o.ToString()),
            [typeof(double)] = o => double.Parse(o.ToString()),
            [typeof(decimal)] = o => decimal.Parse(o.ToString()),
            [typeof(DateTime)] = o => DateTime.Parse(o.ToString()),
            [typeof(bool?)] = o => bool.Parse(o.ToString()),
            [typeof(int?)] = o => int.Parse(o.ToString()),
            [typeof(long?)] = o => long.Parse(o.ToString()),
            [typeof(double?)] = o => double.Parse(o.ToString()),
            [typeof(decimal?)] = o => decimal.Parse(o.ToString()),
            [typeof(DateTime?)] = o => DateTime.Parse(o.ToString())

        };

        public static object Coerce(this Type type, string token)
        {
            if (Coercion.ContainsKey(type))
                return Coercion[type].Invoke(token);

            if (type.IsEnum)
                return Enum.Parse(type, token);

            if (!type.IsNullableType()) return token;

            var genericType = type.GetGenericArguments()[0];
            if (genericType.IsEnum)
                return Enum.Parse(genericType, token);

            return token;
        }

    }
}