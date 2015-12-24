using System.Reflection;
using System.Threading;
using Odin.Attributes;
using Odin.Parsing;

namespace Odin.Configuration
{
    public class SlashColonConvention : Conventions
    {
        public override string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            return name.Replace("Command", "");
        }

        public override string GetLongOptionName(ParameterInfo row)
        {
            return $"/{row.Name}";
        }

        public override string GetActionName(MethodInfo methodInfo)
        {
            return methodInfo.Name;
        }

        public override string GetShortOptionName(string rawAlias)
        {
            return $"/{rawAlias}";
        }

        public override bool IsMatchingParameter(ParameterValue parameterMap, string arg)
        {
            return arg.StartsWith(parameterMap.LongOptionName);
        }

        public override IParser CreateParser(ParameterValue parameter)
        {
            return new SlashColonParser(parameter);
        }
    }
}