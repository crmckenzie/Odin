using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Odin.Configuration;
using Shouldly;

namespace Odin.Tests
{

    [TestFixture]
    public class CommandTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.SubCommand = Substitute.ForPartsOf<SubCommand>();
            this.Subject = Substitute.ForPartsOf<DefaultCommand>(this.SubCommand);
        }

        public SubCommand SubCommand { get; set; }

        public DefaultCommand Subject { get; set; }

        [Test]
        public void CanExecuteAMethodThatIsAnAction()
        {
            var args = new[] { "do-something" };

            var result = this.Subject.Execute(args);

            Assert.That(result, Is.EqualTo(0));
            this.Subject.DidNotReceive().Help();
        }

        [Test]
        public void ReturnsResultFromAction()
        {
            var args = new[] { "always-returns-minus2" };

            var result = this.Subject.Execute(args);

            Assert.That(result, Is.EqualTo(-2));
            this.Subject.Received().Help();
        }

        [Test]
        public void BooleanActions_ReturningFalse()
        {
            var args = new[] { "always-returns-false" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(-1);
        }

        [Test]
        public void BooleanActions_ReturningTrue()
        {
            var args = new[] { "always-returns-true" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(0);
        }

        [Test]
        public void SubCommandUsesParentsLogger()
        {
            this.SubCommand.Logger.ShouldBe(this.Subject.Logger);
        }

        [Test]
        public void ChangingConventionsRecalculatesSubCommandDictionary()
        {
            this.Subject.Use(new SlashColonConvention());
            this.Subject.Name.ShouldBe("DefaultProxy");
            this.SubCommand.Name.ShouldBe("SubProxy");
            this.Subject.SubCommands["SubProxy"].ShouldBe(this.SubCommand);
        }
    }
}
