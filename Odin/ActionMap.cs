using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Odin
{
    public class ActionMap
    {
        public ActionMap(object instance, MethodInfo method, bool isDefaultAction)
        {
            Instance = instance;
            MethodInfo = method;
            IsDefaultAction = isDefaultAction;
            ParameterMaps = GenerateParameterMap().ToList().AsReadOnly();
            Description = GetDescription();
        }

        public string Name => MethodInfo.Name;

        public object Instance { get; }
        public MethodInfo MethodInfo { get; }
        public bool IsDefaultAction { get;  }

        public string Description { get; }

        public ReadOnlyCollection<ParameterMap> ParameterMaps { get; }

        private string GetDescription()
        {
            var descriptionAttr = MethodInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr == null)
                return "";

            return descriptionAttr.Description;
        }

        private IEnumerable<ParameterMap> GenerateParameterMap()
        {
            return MethodInfo
                .GetParameters()
                .OrderBy(row => row.Position)
                .Select(row => new ParameterMap
                {
                    ParameterInfo = row,
                    Switch = $"--{row.Name}"
                });
        }

        public ActionInvocation GenerateInvocation(string[] args)
        {
            return new ActionInvocation(this, args);
        }

        public string Help()
        {
            var builder = new StringBuilder();
            var name = IsDefaultAction ? $"{Name} (default)" : Name;

            builder.AppendLine($"{name,-30}{Description}");

            foreach (var parameterMap in this.ParameterMaps)
            {
                var description = parameterMap.GetDescription();
                builder.AppendLine($"\t{parameterMap.Switch,-26}{description}");
            }
            return builder.ToString();
        }
    }
}