using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests
{
    //TODO: Support aliases on controllers
    //TODO: Support aliases on methods
    //TODO: Support aliases on parameters
    //TODO: Support custom conventions
    //TODO: Support custom argument parsers

    [TestFixture]
    public class ControllerTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommand = Substitute.ForPartsOf<SubCommand>();
            this.Subject = Substitute.ForPartsOf<DefaultCommand>(this.SubCommand, this.Logger);
        }

        public StringBuilderLogger Logger { get; set; }

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
        public void SubCommandUsesParentsLogger()
        {
            this.SubCommand.Logger.ShouldBe(this.Subject.Logger);
        }
    }
}
