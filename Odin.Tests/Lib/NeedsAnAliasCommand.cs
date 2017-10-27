namespace Odin.Tests.Lib
{
    using Odin.Attributes;

    [Alias("foo")]
    public class NeedsAnAliasCommand : Command
    {
        [Action]
        [Alias("bar")]
        public void NeedsAnAliasMethod([Alias("i")] string input)
        {
            this.Logger.Info(input);
        }
    }
}