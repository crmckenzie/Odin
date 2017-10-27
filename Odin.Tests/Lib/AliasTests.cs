using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Odin.Demo;
using Shouldly;

namespace Odin.Tests.Lib
{
    using Xunit;

    public class AliasTests
    {
        public AliasTests()
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

        [Fact]
        public void InvokeWithoutAliases()
        {
            // When
            var result = this.Root.Execute("needs-an-alias", "needs-an-alias-method", "--input", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }

        [Fact]
        public void InvokeWithCommandAlias()
        {
            // When
            var result = this.Root.Execute("foo", "needs-an-alias-method", "--input", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }


        [Fact]
        public void InvokeWithMethodAlias()
        {
            // When
            var result = this.Root.Execute("needs-an-alias", "bar", "--input", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }


        [Fact]
        public void InvokeWithArgumentAliases()
        {
            // When
            var result = this.Root.Execute("needs-an-alias", "needs-an-alias-method", "-i", "hello world!");

            // Then
            result.ShouldNotBeNull();
            this.Logger.InfoBuilder.ToString().ShouldBe("hello world!");
        }

        [Fact]
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
