using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Odin.Attributes;
using Odin.Demo;
using Shouldly;

namespace Odin.Tests.Lib
{
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

    [TestFixture]
    public class AliasTests
    {
        [SetUp]
        public void BeforeEachTest()
        {
            this.Logger =new StringBuilderLogger();
            this.NeedsAnAlias = new NeedsAnAliasCommand();
            this.Root = new RootCommand()
                .Use(this.Logger)
                .RegisterSubCommand(this.NeedsAnAlias)
                ;
        }

        public StringBuilderLogger Logger { get; set; }

        public Command Root { get; set; }

        public NeedsAnAliasCommand NeedsAnAlias { get; set; }

        [Test]
        public void InvokeWithoutAliases()
        {
            // When
            var result = this.Root.Execute("needs-an-alias", "needs-an-alias-method", "--input", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }

        [Test]
        public void InvokeWithCommandAlias()
        {
            // When
            var result = this.Root.Execute("foo", "needs-an-alias-method", "--input", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }


        [Test]
        public void InvokeWithMethodAlias()
        {
            // When
            var result = this.Root.Execute("needs-an-alias", "bar", "--input", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }


        [Test]
        public void InvokeWithArgumentAliases()
        {
            // When
            var result = this.Root.Execute("needs-an-alias", "needs-an-alias-method", "-i", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }

        [Test]
        public void InvokeWithAllAliases()
        {
            // When
            var result = this.Root.Execute("foo", "bar", "-i", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }

    }
}
