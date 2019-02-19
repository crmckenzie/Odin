using Odin.Conventions;
using Shouldly;
using Xunit;

namespace Odin.Tests.Conventions
{
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
