using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Books;
using Odin.Tests.Lib;
using Shouldly;
using Xunit;

namespace Odin.Tests.Samples.Books
{
    public class BooksCommandTests
    {
        public BooksCommandTests()
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
            var exitCode = this.Command.Execute("add", "--release-date", "2017-11-14", "--author", "Branden Sanderson", "--title", "Oathbringer");

            exitCode.ShouldBe(0);
        }
    }
}
