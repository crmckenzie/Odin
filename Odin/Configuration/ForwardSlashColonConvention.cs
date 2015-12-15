using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
            return $"/{row.Name}";
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
            var token = parameter.Tokens[i];
            if (Regex.IsMatch(token, @"/\w+:\w+"))
            {
                var value = token.Split(':').Skip(1).First();
                parameter.Value = parameter.Coerce(value);
            }
            else if (Regex.IsMatch(token, @"/\w+"))
            {
                if (parameter.IsBooleanSwitch())
                {
                    parameter.Value = true;
                }
            }
            else
            {
                parameter.Value = parameter.Coerce(token);
            }

            return 1;

        }
    }
}