using System.Linq;
using System.Text.RegularExpressions;

namespace Odin.Configuration
{
    internal class SlashEqualsValueParser : IValueParser
    {
        private readonly ParameterValue _parameterValue;

        public SlashEqualsValueParser(ParameterValue parameterValue)
        {
            _parameterValue = parameterValue;
        }

        public ParseResult Parse(string[] tokens, int i)
        {
            var token = tokens[i];
            if (IsNameValuePair(token))
            {
                var value = token.Split('=').Skip(1).First();
                return new ParseResult()
                {
                    Value = _parameterValue.Coerce(value),
                    TokensProcessed = 1,
                };
            }

            if (!IsArgumentName(token))
                return new ParseResult()
                {
                    Value = _parameterValue.Coerce(token),
                    TokensProcessed = 1,
                };

            if (_parameterValue.IsBoolean())
            {
                return new ParseResult()
                {
                    Value = true,
                    TokensProcessed = 1,
                };
            }
            return new ParseResult()
            {
                Value = _parameterValue.Coerce(token),
                TokensProcessed = 1,
            };
        }

        private static bool IsArgumentName(string token)
        {
            return Regex.IsMatch(token, @"/\w+");
        }

        private static bool IsNameValuePair(string token)
        {
            return Regex.IsMatch(token, @"/\w+=\w+");
        }
    }
}