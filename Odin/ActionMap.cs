using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Logging;

namespace Odin
{
    public class ActionMap
    {
        public ActionMap(Command instance, MethodInfo method)
        {
            Instance = instance;
            MethodInfo = method;
            IsDefaultAction = method.GetCustomAttribute<ActionAttribute>().IsDefault;
            ParameterMaps = GenerateParameterMap().ToList().AsReadOnly();
            Description = GetDescription();
        }

        public string Name => Conventions.GetActionName(MethodInfo);
        public Conventions Conventions => Instance.Conventions;

        public Logger Logger => Instance.Logger;

        public Command Instance { get; }
        public MethodInfo MethodInfo { get; }
        public bool IsDefaultAction { get; }

        public string Description { get; }

        protected internal ReadOnlyCollection<ParameterMap> ParameterMaps { get; }

        private string GetDescription()
        {
            var descriptionAttr = MethodInfo.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttr == null ? "" : descriptionAttr.Description;
        }

        private IEnumerable<ParameterMap> GenerateParameterMap()
        {
            return MethodInfo
                .GetParameters()
                .OrderBy(row => row.Position)
                .Select(row => new ParameterMap(this)
                {
                    ParameterInfo = row,
                    Switch = Conventions.GetArgumentName(row),
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