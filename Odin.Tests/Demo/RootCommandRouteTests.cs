using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Odin.Demo;
using Shouldly;

namespace Odin.Tests.Demo
{
    [TestFixture]
    public class RootCommandRouteTests
    {

        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new RootCommand();
            this.Subject.Use(this.Logger);
        }

        public StringBuilderLogger Logger { get; set; }

        public RootCommand Subject { get; set; }

        #region FizzBuzzCommandRoute.FizzBuzz

        [Test]
        public void Execute_FizzBuzz_UsingExplicitParameterName()
        {
            // Given

            // When
            var result = this.Subject.Execute("katas", "fizz-buzz", "--input", "3");

            // Then
            result.ShouldBe(0);
            this.Logger.InfoBuilder.ToString().Trim().ShouldBe("Fizz");
        }

        [Test]
        public void Execute_FizzBuzz_UsingParamterAlias()
        {
            // Given

            // When
            var result = this.Subject.Execute("katas", "fizz-buzz", "-i", "3");

            // Then
            result.ShouldBe(0);
            this.Logger.InfoBuilder.ToString().Trim().ShouldBe("Fizz");
        }


        [Test]
        public void Execute_FizzBuzz_UsingExplicitActionName()
        {
            // Given

            // When
            var result = this.Subject.Execute("katas", "fizz-buzz", "3");

            // Then
            result.ShouldBe(0);
            this.Logger.InfoBuilder.ToString().Trim().ShouldBe("Fizz");
        }

        [Test]
        public void Execute_FizzBuzz_UsingDefaultAction()
        {
            // Given

            // When
            var result = this.Subject.Execute("katas", "3");

            // Then
            result.ShouldBe(0, this.Logger.InfoBuilder.ToString());
            this.Logger.InfoBuilder.ToString().Trim().ShouldBe("Fizz");
        }

        [Test]
        public void Execute_FizzBuzz_WithParsingError_FailsGracefully()
        {
            // Given

            // When
            var result = this.Subject.Execute("katas", "fredbob");

            // Then
            result.ShouldBe(-1, this.Logger.InfoBuilder.ToString());
        }

        #endregion

    }
}
