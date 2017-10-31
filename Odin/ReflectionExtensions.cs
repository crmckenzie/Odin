using System;
using System.Collections.Generic;
using System.Text;

namespace Odin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static ConstructorInfo GetConstructor(this Type self, params Type[] types)
        {
            return self.GetTypeInfo().GetConstructor(types);
        }

        public static MethodInfo[] GetMethods(this Type self)
        {
            return self.GetTypeInfo().GetMethods();
        }

        public static ConstructorInfo[] GetConstructors(this Type self)
        {
            return self.GetTypeInfo().GetConstructors();
        }

        public static ConstructorInfo[] GetConstructors(this Type self, BindingFlags flags)
        {
            return self.GetTypeInfo().GetConstructors(flags);
        }

        public static T GetCustomAttribute<T>(this Type self) where T : Attribute
        {
            return self.GetTypeInfo().GetCustomAttribute<T>();
        }

        public static T GetCustomAttribute<T>(this Type self, bool inherit) where T : Attribute
        {
            return self.GetTypeInfo().GetCustomAttribute<T>(inherit);
        }

        public static FieldInfo[] GetFields(this Type self)
        {
            return self.GetTypeInfo().GetFields();
        }

        public static FieldInfo[] GetFields(this Type self, BindingFlags flags)
        {
            return self.GetTypeInfo().GetFields(flags);
        }

        public static Type GetFieldOrPropertyType(this MemberInfo m)
        {
            if (m is FieldInfo info)
            {
                return info.FieldType;
            }

            return ((PropertyInfo)m).PropertyType;
        }

        public static object GetFieldOrPropertyValue<T>(this MemberInfo memberInfo, T instance)
        {
            object currentValue = null;

            switch (memberInfo)
            {
                case FieldInfo info:
                    currentValue = info.GetValue(instance);
                    break;
                case PropertyInfo propertyInfo:
                    try
                    {
                        if (propertyInfo.GetGetMethod() != null)
                        {
                            currentValue = propertyInfo.GetValue(instance, null);
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine(String.Format("NBuilder warning: {0} threw an exception when attempting to read its current value", memberInfo.Name));
                    }
                    break;
            }

            return currentValue;
        }

        public static MethodInfo GetMethod(this Type self, string name)
        {
            return self.GetTypeInfo().GetMethod(name);
        } 

        public static PropertyInfo[] GetProperties(this Type self)
        {
            return self.GetTypeInfo().GetProperties();
        }

        public static PropertyInfo[] GetProperties(this Type self, BindingFlags flags)
        {
            return self.GetTypeInfo().GetProperties(flags);
        }

        public static Type[] GetGenericArguments(this Type self)
        {
            return self.GetTypeInfo().GetGenericArguments();
        }

        public static Type GetTypeWithoutNullability(this Type t)
        {
            return t.GetTypeInfo().IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(Nullable<>)
                       ? t.GetTypeInfo().GetGenericArguments().Single()
                       : t;
        }

        public static IList<MemberInfo> GetPublicInstancePropertiesAndFields(this Type t)
        {
            var memberInfos = new List<MemberInfo>();
            memberInfos.AddRange(t.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance));
            memberInfos.AddRange(t.GetTypeInfo().GetFields());
            return memberInfos;
        }

        public static bool Implements<T>(this Type type)
        {
            var interfaceType = typeof(T);
            return type.GetTypeInfo().GetInterface(interfaceType.FullName) != null;
        }

        public static bool IsAbstract(this Type self)
        {
            return self.GetTypeInfo().IsAbstract;
        }

        public static bool IsArray(this Type type)
        {
            return type.GetTypeInfo().IsSubclassOf(typeof(Array));
        }

        public static bool IsBoolean(this Type type)
        {
            return type == typeof(bool)
                   || type == typeof(bool?)
                ;
        }

        public static bool IsEnum(this Type self)
        {
            return self.GetTypeInfo().IsEnum;
        }

        public static bool IsGenericType(this Type self)
        {
            return self.GetTypeInfo().IsGenericType;
        }

        public static bool IsInterface(this Type self)
        {
            return self.GetTypeInfo().IsInterface;
        }

        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsValueType(this Type self)
        {
            return self.GetTypeInfo().IsValueType;
        }


        public static Type BaseType(this Type self)
        {
            return self.GetTypeInfo().BaseType;
        }

        public static void SetFieldOrPropertyValue<T>(this MemberInfo m, T instance, object value)
        {
            if (m is FieldInfo info)
            {
                info.SetValue(instance, value);
            }
            else
            {
                if (!(m is PropertyInfo propertyInfo)) return;
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(instance, value, null);
                }
            }
        }

        private static Dictionary<Type, Func<string, object>> Coercion { get; }
            = new Dictionary<Type, Func<string, object>>()
            {
                [typeof(bool)] = o => bool.Parse(o.ToString()),
                [typeof(int)] = o => int.Parse(o.ToString()),
                [typeof(long)] = o => long.Parse(o.ToString()),
                [typeof(double)] = o => double.Parse(o.ToString()),
                [typeof(decimal)] = o => decimal.Parse(o.ToString()),
                [typeof(DateTime)] = o => DateTime.Parse(o.ToString()),

                [typeof(bool?)] = ParseNullableBoolean,
                [typeof(int?)] = ParseNullableInt32,
                [typeof(long?)] = ParseNullableInt64,
                [typeof(double?)] = ParseNullableDouble,
                [typeof(decimal?)] = ParseNullableDecimal,
                [typeof(DateTime?)] = ParseNullableDateTime
            };

        private static object ParseNullableInt32(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return int.Parse(token);
        }

        private static object ParseNullableInt64(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return long.Parse(token);
        }
        private static object ParseNullableDecimal(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return decimal.Parse(token);
        }
        private static object ParseNullableDouble(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return double.Parse(token);
        }

        private static object ParseNullableDateTime(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return DateTime.Parse(token);
        }

        private static object ParseNullableBoolean(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? (bool?)null : bool.Parse(input);
        }

        public static object Coerce(this Parameter parameter, string token)
        {
            return parameter.ParameterType.Coerce(token);
        }

        public static object Coerce(this Type type, string token)
        {
            if (Coercion.ContainsKey(type))
                return Coercion[type].Invoke(token);

            if (type.IsEnum())
                return Enum.Parse(type, token);

            if (!type.IsNullableType()) return token;

            var genericType = type.GetGenericArguments()[0];
            return genericType.IsEnum() ? Enum.Parse(genericType, token) : token;
        }

    }

}
