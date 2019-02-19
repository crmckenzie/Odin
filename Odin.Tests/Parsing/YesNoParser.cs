using Odin.Parsing;

namespace Odin.Tests.Parsing
{
    public class YesNoParser : IParser
    {
        private readonly Parameter _parameterValue;

        public ParseResult Parse(string[] tokens, int tokenIndex)
        {
            var token = tokens[tokenIndex];
            var result = new ParseResult()
            {
                TokensProcessed = 1
            };

            if (this._parameterValue.IsIdentifiedBy(token))
            {
                token = tokens[tokenIndex + 1];
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