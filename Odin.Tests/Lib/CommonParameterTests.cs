using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;
using Odin.Attributes;
using Odin.Configuration;
using Shouldly;

namespace Odin.Tests.Lib
{
    public class CommandWithCommonParmeters : Command
    {   
        [Parameter]
        [Alias("t")]
        public string Text { get; set; }

        [Action]
        public void Display(string subject = null)
        {
            var text = subject ?? Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                Logger.Info("none");
            }
            else
            {
                Logger.Info(text);
            }
        }
    }

    [TestFixture]
    public class CommonParameterTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new CommandWithCommonParmeters();
            this.Subject.Use(this.Logger);
        }

        public CommandWithCommonParmeters Subject { get; set; }

        public StringBuilderLogger Logger{ get; set; }


        [Test]
        public void ParameterNotSet()
        {
            // Given

            // When
            Subject.Execute("display");

            // Then
            this.Logger.InfoBuilder.ToString().ShouldBe("none");
        }

        [Test]
        public void SetParameter()
        {
            Subject.Execute("display", "--text", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("awesome!");
        }

        [Test]
        public void GetMethodInvocation_DoesNot_SetParameter()
        {

            var invocation = Subject.GenerateInvocation("display", "--text", "awesome!");
            Subject.Text.ShouldBe(null);

            invocation.CommonParameters.ToDictionary(p => p.Name)["Text"].Value.ShouldBe("awesome!");
        }

        [Test]
        public void SetParameterUsingAlias()
        {
            Subject.Execute("display", "-t", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            this.Logger.InfoBuilder.ToString().ShouldBe("awesome!");
        }

        [Test]
        public void WithMethodParameter()
        {
            Subject.Execute("display", "--text", "awesome!", "--subject", "fredbob");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("fredbob");
        }

        [Test]
        public void WithMethodParameter_InvertOrder()
        {
            Subject.Execute("display",  "--subject", "fredbob", "--text", "awesome!");

            Subject.Text.ShouldBe("awesome!");
            Logger.InfoBuilder.ToString().ShouldBe("fredbob");
        }
    }
}

