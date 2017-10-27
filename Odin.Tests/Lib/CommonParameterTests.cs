using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;

using Odin.Configuration;
using Shouldly;

namespace Odin.Tests.Lib
{
    using Xunit;

    public class CommonParameterTests
    {
        public CommonParameterTests()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new CommandWithCommonParmeters();
            this.Subject.Use(this.Logger);
        }

        public CommandWithCommonParmeters Subject { get; set; }

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
        public void GetMethodInvocation_DoesNot_SetParameter()
        {

            var invocation = Subject.GenerateInvocation("display", "--text", "awesome!");
            Subject.Text.ShouldBe(null);

            invocation.CommonParameters.ToDictionary(p => p.Name)["Text"].Value.ShouldBe("awesome!");
        }

        [Fact]
        public void SetParameterUsingAlias()
        {
            Subject.Execute("display", "-t", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            this.Logger.InfoBuilder.ToString().ShouldBe("awesome!");
        }

        [Fact]
        public void WithMethodParameter()
        {
            Subject.Execute("display", "--text", "awesome!", "--subject", "fredbob");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("fredbob");
        }

        [Fact]
        public void WithMethodParameter_InvertOrder()
        {
            Subject.Execute("display",  "--subject", "fredbob", "--text", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("fredbob");
        }
    }
}

