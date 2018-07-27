namespace Odin.Tests.Configuration
{
    using Odin.Configuration;

    using Shouldly;

    using Xunit;

    public class StringExtensionsTests
    {
        [Theory]

        [InlineData("Foo", "foo")]
        [InlineData("FooBar", "foo-bar")]
        public void KebabCase(string input, string output)
        {
            input.KebabCase().ShouldBe(output);
        }

    }
}
