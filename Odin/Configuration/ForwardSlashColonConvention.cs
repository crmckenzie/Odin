using System.Linq;
using System.Reflection;
using Odin.Attributes;

namespace Odin.Configuration
{
    public class ForwardSlashColonConvention : Conventions
    {
        public override string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            return name.Replace("Command", "");
        }

        public override string GetArgumentName(ParameterInfo row)
        {
            return $"/{row.Name.HyphenCase()}";
        }

        public override bool IsArgumentIdentifier(string value)
        {
            return value.StartsWith("/");
        }

        public override string GetActionName(MethodInfo methodInfo)
        {
            return methodInfo.Name;
        }

        public override bool MatchesAlias(AliasAttribute aliasAttribute, string arg)
        {
            if (aliasAttribute == null)
                return false;

            var aliases = aliasAttribute.Aliases.Select(GetFormattedAlias);
            return aliases.Contains(arg);
        }

        public override string GetFormattedAlias(string rawAlias)
        {
            return $"/{rawAlias}";
        }

        public override bool IsIdentifiedBy(ParameterMap parameterMap, string arg)
        {
            return arg.StartsWith(parameterMap.Switch);
        }

        public override int SetValue(ParameterValue parameter, int i)
        {
            var arg = parameter.Args[i];
            if (parameter.IsBooleanSwitch())
            {
                parameter.Value = true;
            }
            else if (parameter.NextArgIsIdentifier(i))
            {
                return 1;
            }
            else if (parameter.IsIdentifiedBy(arg) && parameter.HasNextValue(i))
            {
                var value = parameter.Args[i + 1];
                parameter.Value = parameter.Coerce(value);
                return 2;
            }
            else
            {
                parameter.Value = parameter.Coerce(arg);
            }

            return 1;
        }
    }
}