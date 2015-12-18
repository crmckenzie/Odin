namespace Odin.Configuration
{
    public interface IValueParser
    {
        ParseResult Parse(string[] tokens, int i);
    }
}