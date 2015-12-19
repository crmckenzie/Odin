using System.Reflection;
using Odin.Attributes;

namespace Odin.Configuration
{
    public abstract class Conventions
    {
        public abstract string GetCommandName(Command command);
        public abstract string GetLongOptionName(ParameterInfo row);
        public abstract string GetActionName(MethodInfo methodInfo);
        public abstract string GetShortOptionName(string rawAlias);
        public abstract bool IsIdentifiedBy(ParameterValue parameterMap, string arg);

        public abstract ParseResult Parse(ParameterValue parameter, string[] tokens, int i)
    }
}