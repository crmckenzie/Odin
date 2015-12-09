using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Odin
{
    public class ActionInvocation
    {
        public ActionInvocation(ActionMap actionMap, string[] args)
        {
            ActionMap = actionMap;
            Args = args;
            this.ParameterValues = this.ParameterMaps
                .Select(row => new ParameterValue(row))
                .ToList()
                .AsReadOnly()
                ;

            this.Initialize();
        }

        public object Instance => ActionMap.Instance;
        public ActionMap ActionMap { get; }

        public ReadOnlyCollection<ParameterMap> ParameterMaps => ActionMap.ParameterMaps;
         
        public string[] Args { get;  }

        private void Initialize()
        {
            var values = this.ParameterValues.ToDictionary(row => row.ParameterMap);
            for (var i = 0; i < Args.Length; i++)
            {
                var arg = Args[i];
                var map = GetParameterMap(arg, i);
                if (map == null) continue;

                var parameterValue = values[map];
                if (IsArgumentIdentifier(arg))
                {
                    if (map.IsBooleanSwitch())
                    {
                        parameterValue.Value = true;
                    }
                    else
                    {
                        var j = i + 1;
                        if (j < Args.Length)
                        {
                            var value = Args[j];
                            if (IsArgumentIdentifier(value))
                                continue;

                            parameterValue.Value = map.Coerce(value);
                        }
                        i++;
                    }
                }
                else
                {
                    parameterValue.Value = map.Coerce(arg);
                }
            }

        }

        private ParameterMap GetParameterMap(string arg, int i)
        {
            var map = this.ParameterMaps.FirstOrDefault(p => p.Switch == arg);
            if (map == null && i < this.ParameterMaps.Count)
            {
                map = this.ParameterMaps[i];
            }
            return map;
        }

        private bool IsArgumentIdentifier(string value)
        {
            return value.StartsWith("--");
        }

        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }

        public bool CanInvoke()
        {
            return this.ParameterValues.All(row => row.IsValueSet()) ;
        }

        public int Invoke()
        {
            var args = this.ParameterValues
                .OrderBy(map => map.ParameterMap.Position)
                .Select(row => row.Value)
                .ToArray()
                ;

            var result = this.ActionMap.MethodInfo.Invoke(this.Instance, args);
            if (result is int)
            {
                return (int)result;
            }
            return 0;
        }
    }
}