using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

            this.ParameterValues = this.MethodInfo.GetParameters()
                .Select(row => new ParameterValue(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        public bool IsDefault { get; }

        private MethodInfo MethodInfo { get; }

        public Command Command { get; }
        public Conventions Conventions => Command.Conventions;
        public string Name => Conventions.GetActionName(MethodInfo);
        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }

        private IEnumerable<ParameterValue> GenerateParameteValues()
        {
            return MethodInfo
                .GetParameters()
                .OrderBy(row => row.Position)
                .Select(row => new ParameterValue(this, row))
                ;
        }

        public string GetDescription()
        {
            var descriptionAttr = MethodInfo.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttr == null ? "" : descriptionAttr.Description;
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

            if (result is bool)
            {
                return ((bool) result) ? 0 : -1;
            }

            return 0;
        }

        internal void SetParameterValues(string[] tokens)
        {
            var i = 0;
            while (i < tokens.Length)
            {
                var arg = tokens[i];
                var parameter = FindBySwitch(arg) ?? FindByIndex(i);
                if (parameter == null)
                {
                    i++;
                    continue;
                };
                ParseResult result;
                if (parameter.HasCustomParser())
                {
                    result = parameter.CustomParser.Parse(tokens, i);
                }
                else
                {
                    result = Conventions.Parse(parameter, tokens, i);
                }

                if (result.TokensProcessed <= 0)
                {
                    i++;
                    continue;
                };

                parameter.Value = result.Value;
                i += result.TokensProcessed;

            }

        }
    }
}