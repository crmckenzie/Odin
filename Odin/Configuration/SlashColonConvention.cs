using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Odin.Attributes;

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

        public override ParseResult Parse(ParameterValue parameter, string[] tokens, int i)
        {
            var token = tokens[i];
            if (IsNameValuePair(token))
            {
                var value = token.Split(':').Skip(1).First();
                return new ParseResult()
                {
                    Value = parameter.Coerce(value),
                    TokensProcessed = 1,
                };
            }

            if (!IsArgumentName(token))
                return new ParseResult()
                {
                    Value = parameter.Coerce(token),
                    TokensProcessed = 1,
                };

            if (parameter.IsBoolean())
            {
                return new ParseResult()
                {
                    Value = true,
                    TokensProcessed = 1,
                };
            }
            return new ParseResult()
            {
                Value = parameter.Coerce(token),
                TokensProcessed = 1,
            };
        }

        private static bool IsArgumentName(string token)
        {
            return Regex.IsMatch(token, @"/\w+");
        }

        private static bool IsNameValuePair(string token)
        {
            return Regex.IsMatch(token, @"/\w+:\w+");
        }

    }
}