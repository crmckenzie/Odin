using Odin.Configuration;
using Odin.Parsing;

namespace Odin.Tests.Parsing
{
    public class YesNoParser : IParser
    {
        private readonly Parameter _parameterValue;

        public ParseResult Parse(string[] tokens, int i)
        {
            var token = tokens[i];
            var result = new ParseResult()
            {
                TokensProcessed = 1
            };

            if (this._parameterValue.IsIdentifiedBy(token))
            {
                token = tokens[i + 1];
                result.TokensProcessed++;
            }

            result.Value = token.Contains("yes");
            return result;
        }

        public YesNoParser(Parameter parameterValue)
        {
            _parameterValue = parameterValue;
        }
    }
}