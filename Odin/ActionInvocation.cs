using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Odin.Configuration;

namespace Odin
{
    public class ActionInvocation
    {
        public ActionInvocation(ActionMap actionMap, string[] tokens)
        {
            ActionMap = actionMap;
            Tokens = tokens;
            this.ParameterValues = this.ParameterMaps
                .Select(row => new ParameterValue(this, row))
                .ToList()
                .AsReadOnly()
                ;

            this.Initialize();
        }

        public Conventions Conventions => ActionMap.Conventions;

        public object Instance => ActionMap.Instance;
        public ActionMap ActionMap { get; }

        public ReadOnlyCollection<ParameterMap> ParameterMaps => ActionMap.ParameterMaps;
         
        public string[] Tokens { get;  }
        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }
        public string Name => ActionMap.Name;

        private void Initialize()
        {
            for (var i = 0; i < Tokens.Length; i++)
            {
                var arg = Tokens[i];
                var parameter = FindBySwitchOrIndex(arg, i);
                if (parameter != null)
                {
                    i += (Conventions.SetValue(parameter, i) -1);
                }
            }
        }

        private ParameterValue FindBySwitchOrIndex(string arg, int i)
        {
            var found = this.ParameterValues
                .FirstOrDefault(p => p.IsIdentifiedBy(arg))
                ;
            if (found == null && i < this.ParameterMaps.Count)
            {
                found = this.ParameterValues
                    .OrderBy(p => p.Position)
                    .ToArray()[i]
                    ;
            }
            return found;
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