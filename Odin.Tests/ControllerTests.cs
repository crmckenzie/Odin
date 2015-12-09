using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Odin.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommandController = Substitute.ForPartsOf<SubCommandController>();
            this.SubCommandController.Name = "SubCommand";
            this.Subject = Substitute.ForPartsOf<DefaultController>(this.SubCommandController, this.Logger);
            this.Subject.Name = "Default";
        }

        public StringBuilderLogger Logger { get; set; }

        public SubCommandController SubCommandController { get; set; }

        public DefaultController Subject { get; set; }

        #region ActionExecution

        [Test]
        public void CannotExecuteAMethodThatIsNotAnAction()
        {
            var args = new[] { "NotAnAction" };

            var result = this.Subject.Execute(args);

            Assert.That(result, Is.EqualTo(-1));
            this.Subject.Received().Help();
        }

        [Test]
        public void CanExecuteAMethodThatIsAnAction()
        {
            var args = new[] { "DoSomething" };

            var result = this.Subject.Execute(args);

            Assert.That(result, Is.EqualTo(0));
            this.Subject.DidNotReceive().Help();
        }

        [Test]
        public void ReturnsResultFromAction()
        {
            var args = new[] { "AlwaysReturnsMinus2" };

            var result = this.Subject.Execute(args);

            Assert.That(result, Is.EqualTo(-2));
            this.Subject.Received().Help();
        }

        #endregion

        #region Required arguments

        [Test]
        public void WithRequiredStringArg()
        {
            var args = new[] { "WithRequiredStringArg", "--argument", "value" };

            this.Subject.Execute(args);

            this.Subject.Received().WithRequiredStringArg("value");
        }

        [Test]
        public void WithMultipleRequiredStringArgs()
        {
            var args = new[] { "WithRequiredStringArgs", "--argument1", "value1", "--argument2", "value2" };

            this.Subject.Execute(args);

            this.Subject.Received().WithRequiredStringArgs("value1", "value2");
        }
        #endregion

        #region Switches

        [Test]
        public void SwitchWithValue()
        {
            var args = new[] { "WithSwitch", "--argument", "true" };

            this.Subject.Execute(args);
           
            Assert.That(Subject.MethodArguments, Is.EquivalentTo(new [] {true}));
        }

        [Test]
        public void SwitchWithoutValue()
        {
            var args = new[] { "WithSwitch", "--argument"};

            this.Subject.Execute(args);

            Assert.That(Subject.MethodArguments, Is.EquivalentTo(new[] { true }));
        }

        [Test]
        public void SwitchNotGiven()
        {
            var args = new[] { "WithSwitch"};

            this.Subject.Execute(args);

            Assert.That(Subject.MethodArguments, Is.EquivalentTo(new[] { false }));
        }

        #endregion

        #region Optional arguments

        [Test]
        public void WithOptionalStringArg_DoNotPassIt()
        {
            var args = new[] { "WithOptionalStringArg" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "not-passed" }));
        }

        [Test]
        public void WithOptionalStringArg_PassIt()
        {
            var args = new[] { "WithOptionalStringArg", "--argument", "value1" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "value1" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument1", "value1", "--argument2", "value2", "--argument3", "value3" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "value1", "value2", "value3" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassHead()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument1", "value1" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "value1", "value2-not-passed", "value3-not-passed" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassBody()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument2", "value2" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "value1-not-passed", "value2", "value3-not-passed" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassNone()
        {
            var args = new[] { "WithOptionalStringArgs" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "value1-not-passed", "value2-not-passed", "value3-not-passed" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassTail()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument3", "value3" };

            this.Subject.Execute(args);

            Assert.That(this.Subject.MethodArguments, Is.EquivalentTo(new[] { "value1-not-passed", "value2-not-passed", "value3" }));
        }

        #endregion

        #region SubCommands

        [Test]
        public void ExecuteSubCommand()
        {
            var args = new[] { "SubCommand" };

            this.Subject.Execute(args);

            this.SubCommandController.Received().DoSomething();
        }

        #endregion

        #region Help

        [Test]
        public void UnmatchedArgumentsDisplaysHelp()
        {
            this.Subject.GenerateHelp().Returns("this is a test");

            this.Subject.Execute(new[] {"Fredbob"});

            this.Subject.Received().Help();
        }

        [Test]
        public void HelpDisplaysControllerDescription()
        {
            // When
            var result = this.Subject.GenerateHelp();

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i], Is.EqualTo("This is the default controller"));
        }

        [Test]
        public void HelpDisplaysSubCommands()
        {
            // When
            var result = this.Subject.GenerateHelp();

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[++i], Is.EqualTo("SUB COMMANDS"));
            Assert.That(lines[++i], Is.EqualTo("SubCommand                         Provides a component of testability for subcommands."));
            Assert.That(lines[++i], Is.EqualTo("To get help for subcommands"));
            Assert.That(lines[++i], Is.EqualTo("\tDefault <subcommand> Help"));
        }

        [Test]
        public void HelpDisplaysActions()
        {
            // When
            var result = this.Subject.GenerateHelp();

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .SkipWhile(row => row != "ACTIONS")
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[++i].Trim(), Is.EqualTo("AlwaysReturnsMinus2"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("DoSomething (default)"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("SomeOtherControllerAction"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithOptionalStringArg"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithOptionalStringArgs"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithRequiredStringArg"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithRequiredStringArgs"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithSwitch"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("To get help for actions"));
            Assert.That(lines[++i], Is.EqualTo("\tDefault <action> Help"));
        }

        //[Test]
        //public void HelpDisplaysDefaultActionArguments()
        //{
        //    // When
        //    var result = this.Subject.GenerateHelp();

        //    // Then
        //    var lines = result
        //        .Split('\n')
        //        .Where(row => !string.IsNullOrWhiteSpace(row))
        //        .Select(row => row.Replace("\r", ""))
        //        .SkipWhile(row => row != "ARGUMENTS")
        //        .ToArray()
        //        ;

        //    var i = 0;
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("AlwaysReturnsMinus2"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("DoSomething (default)"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("SomeOtherControllerAction"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("WithOptionalStringArg"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("WithOptionalStringArgs"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("WithRequiredStringArg"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("WithRequiredStringArgs"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("WithSwitch"));
        //    Assert.That(lines[++i].Trim(), Is.EqualTo("To get help for actions"));
        //    Assert.That(lines[++i], Is.EqualTo("\tDefault <action> Help"));
        //}

        #endregion
    }
}
