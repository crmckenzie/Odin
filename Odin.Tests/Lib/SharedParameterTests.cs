using System.Linq;
using Shouldly;

namespace Odin.Tests.Lib
{
    using Xunit;

    public class SharedParameterTests
    {
        public SharedParameterTests()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new CommandWithSharedParmeters();
            this.Subject.Use(this.Logger);
        }

        public CommandWithSharedParmeters Subject { get; set; }

        public StringBuilderLogger Logger{ get; set; }


        [Fact]
        public void ParameterNotSet()
        {
            // Given

            // When
            Subject.Execute("display");

            // Then
            this.Logger.InfoBuilder.ToString().ShouldBe("none");
        }

        [Fact]
        public void SetParameter()
        {
            Subject.Execute("display", "--text", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("awesome!");
        }

        [Fact]
        public void SetParameterUsingAlias()
        {
            Subject.Execute("display", "-t", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            this.Logger.InfoBuilder.ToString().ShouldBe("awesome!");
        }

        [Fact]
        public void WithActionParameter()
        {
            Subject.Execute("display", "--text", "awesome!", "--subject", "fredbob");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("fredbob");
        }

        [Fact]
        public void WithActionParameter_InvertOrder()
        {
            Subject.Execute("display",  "--subject", "fredbob", "--text", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("fredbob");
        }
    }
}

