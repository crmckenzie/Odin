using Odin.Configuration;

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
        /// <param name="tokens"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        ParseResult Parse(string[] tokens, int i);
    }
}
