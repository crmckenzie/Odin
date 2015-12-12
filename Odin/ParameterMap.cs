using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Odin
{
    public class ParameterMap
    {

        static ParameterMap()
        {
            Coercion = new Dictionary<Type, Func<object, object>>
            {
                [typeof (bool)] = o => bool.Parse(o.ToString()),
                [typeof(int)] = o => int.Parse(o.ToString())
            };
        }

        public static Dictionary<Type, Func<object, object>> Coercion { get; set; }

        public ActionMap ActionMap { get; }

        public Logger Logger => ActionMap.Logger;

        public ParameterMap(ActionMap actionMap)
        {
            ActionMap = actionMap;
            this.RawValues = new List<object>();
        }

        public string Switch { get; set; }

        public List<object> RawValues { get; }

        public ParameterInfo ParameterInfo { get; set; }
        public int Position => ParameterInfo.Position;

        public Type ParameterType => ParameterInfo.ParameterType;

        public string Name => ParameterInfo.Name;

        public bool IsOptional()
        {
            return ParameterInfo.IsOptional;
        }

        public bool IsBooleanSwitch()
        {
            return ParameterInfo.ParameterType == typeof (bool);
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

        public string GetDescription()
        {
            var attr = ParameterInfo.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
                return attr.Description;
            return "";
        }

    }
}