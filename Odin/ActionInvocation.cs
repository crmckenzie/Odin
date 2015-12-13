using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Odin.Configuration;

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

        public Conventions Conventions => ActionMap.Conventions;

        public object Instance => ActionMap.Instance;
        public ActionMap ActionMap { get; }

        public ReadOnlyCollection<ParameterMap> ParameterMaps => ActionMap.ParameterMaps;
         
        public string[] Args { get;  }
        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }
        public string Name => ActionMap.Name;

        private void Initialize()
        {
            for (var i = 0; i < Args.Length; i++)
            {
                var arg = Args[i];
                var map = FindBySwitchOrIndex(arg, i);
                i += SetParameterValue(map, arg, i);
            }
        }

        private int SetParameterValue(ParameterMap map, string arg, int i)
        {
            if (map == null)
                return 0;

            var parameterValue = this.ParameterValues.FirstOrDefault(row => row.IsIdentifiedBy(arg));
            if (parameterValue == null)
            {
                parameterValue = this.ParameterValues[i];
                parameterValue.Value = map.Coerce(arg);
                return 0;
            }

            if (!parameterValue.IsIdentifiedBy(arg))
                return 0;

            if (map.IsBooleanSwitch())
            {
                parameterValue.Value = true;
            }
            else if (NextArgIsIdentifier(i))
            {
                return 0;
            }
            else if (HasNextValue(i))
            {
                var value = Args[i + 1];
                parameterValue.Value = map.Coerce(value);
                return 1;
            }

            return 0;
        }

        private bool HasNextValue(int indexOfCurrentArg)
        {
            return Args.Length > (indexOfCurrentArg + 1);
        }

        private bool NextArgIsIdentifier(int indexOfCurrentArg)
        {
            var j = indexOfCurrentArg + 1;
            if (j < Args.Length)
            {
                return Conventions.IsArgumentIdentifier(Args[j]);
            }
            return false;
        }

        private ParameterMap FindBySwitchOrIndex(string arg, int i)
        {
            var map = this.ParameterMaps.FirstOrDefault(p => p.Switch == arg || p.HasAlias(arg));
            if (map == null && i < this.ParameterMaps.Count)
            {
                map = this.ParameterMaps[i];
            }
            return map;
        }

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