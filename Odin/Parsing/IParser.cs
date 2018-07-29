namespace Odin.Parsing
{
    /// <summary>
    /// Interface for parsers.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Returns a  <see cref="ParseResult"/> for a range of tokens beginning at position i.
        /// </summary>
        /// <param name="tokens">The complete list of tokens that initiated the command.</param>
        /// <param name="tokenIndex">The current parsing position in the tokens list.</param>
        /// <returns></returns>
        ParseResult Parse(string[] tokens, int tokenIndex);
    }
}
