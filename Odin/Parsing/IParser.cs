using Odin.Configuration;

namespace Odin.Parsing
{
    public interface IParser
    {
        ParseResult Parse(string[] tokens, int i);
    }
}
