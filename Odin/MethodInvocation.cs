using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Odin.Attributes;
using Odin.Configuration;

namespace Odin
{
    public class MethodInvocation
    {
        public MethodInvocation(Command command, MethodInfo methodInfo)
        {
            this.Command = command;
            this.MethodInfo = methodInfo;

            IsDefault = methodInfo.GetCustomAttribute<ActionAttribute>().IsDefault;
            ParameterValues = GenerateParameteValues().ToList().AsReadOnly();
            Description = GetDescription();


            this.ParameterValues = this.MethodInfo.GetParameters()
                .Select(row => new ParameterValue(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        public string Description { get; }

        public bool IsDefault { get; }

        public MethodInfo MethodInfo { get; }

        public Command Command { get; }

        private IEnumerable<ParameterValue> GenerateParameteValues()
        {
            return MethodInfo
                .GetParameters()
                .OrderBy(row => row.Position)
                .Select(row => new ParameterValue(this, row))
                ;
        }

        public Conventions Conventions => Command.Conventions;

        
        public string[] Tokens { get; internal set; }
        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }
        public string Name => Conventions.GetActionName(MethodInfo);

        private string GetDescription()
        {
            var descriptionAttr = MethodInfo.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttr == null ? "" : descriptionAttr.Description;
        }

        private void Initialize()
        {
            for (var i = 0; i < Tokens.Length; i++)
            {
                var arg = Tokens[i];
                var parameter = FindBySwitch(arg) ?? FindByIndex(i);
                if (parameter != null)
                {
                    i += (Conventions.SetValue(parameter, i) -1);
                }
            }
        }

        private ParameterValue FindBySwitch(string arg)
        {
            return this.ParameterValues
                .FirstOrDefault(p => p.IsIdentifiedBy(arg))
                ;
        }

        private ParameterValue FindByIndex(int i)
        {
            if (i >= this.ParameterValues.Count)
                return null;
            return  this.ParameterValues
                .OrderBy(p => p.Position)
                .ToArray()[i]
                ;
        }

        public bool CanInvoke()
        {
            return this.ParameterValues.All(row => row.IsValueSet()) ;
        }

        public int Invoke()
        {
            var args = this.ParameterValues
                .OrderBy(map => map.ParameterInfo.Position)
                .Select(row => row.Value)
                .ToArray()
                ;

            var result = this.MethodInfo.Invoke(this.Command, args);
            if (result is int)
            {
                return (int)result;
            }
            return 0;
        }

        public string Help()
        {
            var builder = new StringBuilder();
            var name = IsDefault ? $"{Name} (default)" : Name;

            builder.AppendLine($"{name,-30}{Description}");

            foreach (var parameter in this.ParameterValues)
            {
                var description = parameter.GetDescription();
                builder.Append($"\t{parameter.Switch,-26}");
                if (parameter.HasAliases())
                {
                    var aliases = string.Join(", ", parameter.GetAliases());
                    var text = $"aliases: {aliases}";
                    builder.AppendLine(text)
                        .Append($"\t{new string(' ', 26)}")
                        ;
                }

                builder.AppendLine(description);
            }
            return builder.ToString();
        }

        internal void SetParameterValues(string[] tokens)
        {
            this.Tokens = tokens;
            for (var i = 0; i < Tokens.Length; i++)
            {
                var arg = Tokens[i];
                var parameter = FindBySwitch(arg) ?? FindByIndex(i);
                if (parameter != null)
                {
                    i += (Conventions.SetValue(parameter, i) - 1);
                }
            }
        }
    }
}