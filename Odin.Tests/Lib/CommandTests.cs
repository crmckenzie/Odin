using Odin.Conventions;
using System.Linq;
using NSubstitute;
using Odin.Exceptions;
using Odin.Logging;
using Shouldly;
using Xunit;

namespace Odin.Tests.Lib
{


    public class CommandTests
    {
        public CommandTests()
        {
            this.SubCommand = Substitute.ForPartsOf<SubCommand>();
            this.Subject = Substitute.ForPartsOf<DefaultCommand>(this.SubCommand)
                ;

            this.Logger = new StringBuilderLogger();
            this.Subject.Use(this.Logger);
        }

        public StringBuilderLogger Logger { get; set; }

        public SubCommand SubCommand { get; set; }

        public DefaultCommand Subject { get; set; }

        [Fact]
        public void CanExecuteAMethodThatIsAnAction()
        {
            var args = new[] { "do-something" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(0);
            this.Subject.DidNotReceive().Help();
        }

        [Fact]
        public void ReturnsResultFromAction()
        {
            var args = new[] { "always-returns-minus2" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(-2);
            this.Subject.Received().Help();
        }

        [Fact]
        public void BooleanActions_ReturningFalse()
        {
            var args = new[] { "always-returns-false" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(-1);
        }

        [Fact]
        public void BooleanActions_ReturningTrue()
        {
            var args = new[] { "always-returns-true" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(0);
        }

        [Fact]
        public void SubCommandUsesParentsLogger()
        {
            this.Subject.Execute("sub-proxy");
            this.Logger.InfoBuilder.ToString().ShouldBe("Do some SubCommand stuff!");
        }

        [Fact]
        public void ChangingConventionsRecalculatesSubCommandDictionary()
        {
            this.Subject.Use(new SlashColonConvention());
            this.Subject.Name.ShouldBe("DefaultProxy");
            this.SubCommand.Name.ShouldBe("SubProxy");
            this.Subject.SubCommands.ElementAt(0).ShouldBe(this.SubCommand);
        }

        [Fact]
        public void GenerateInvocation_WhenArgumentsAreInvalid()
        {
            // When
            var result = this.Subject.Execute("too", "many", "arguments", "passed");

            result.ShouldBe(-1);
            this.Logger.ErrorBuilder.ToString().ShouldBe("Could not interpret the command. You sent [too, many, arguments, passed].");
        }

        [Fact]
        public void GenerateInvocation_Execute()
        {
            // Given

            // When
            var result = this.Subject.Execute("too", "many", "arguments", "passed");

            // Then
            result.ShouldBe(-1);


            var info = Logger.InfoBuilder.ToString();
            info.ShouldStartWith("This is the default command");

            var error = Logger.ErrorBuilder.ToString();
            error.ShouldStartWith("Could not interpret the command.");
        }
    }
}
