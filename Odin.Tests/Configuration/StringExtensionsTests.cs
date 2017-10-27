namespace Odin.Tests.Configuration
{
    using Odin.Configuration;

    using Shouldly;

    using Xunit;

    public class StringExtensionsTests
    {
        [Theory]

        [InlineData("Foo", "foo")]
        public void HyphenCase(string input, string output)
        {
            input.HyphenCase().ShouldBe(output);
        }

    }
}
