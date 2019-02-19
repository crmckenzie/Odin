using Books;
using Odin.Tests.Lib;
using Shouldly;
using Xunit;

namespace Odin.Tests.Samples.Books
{
    public class SearchCommandTests
    {
        public SearchCommandTests()
        {
            this.Command = new BooksCommand();
            this.Logger = new StringBuilderLogger();
            this.Command.Use(this.Logger);
        }

        public StringBuilderLogger Logger { get; set; }

        public BooksCommand Command { get; set; }

        [Fact]
        public void InterpretsSharedParamtersToSubCommand()
        {
            var exitCode  = this.Command.Execute("search", "author", "Branden Sanderson", "--sort-by", "ReleaseDate");

            exitCode.ShouldBe(0);
        }
    }
}