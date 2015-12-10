using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Odin.Tests
{
    [TestFixture]
    public class ControllerHelpTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommandCommandRoute = Substitute.ForPartsOf<SubCommandCommandRoute>(this.Logger);
            this.SubCommandCommandRoute.Name = "SubCommand";
            this.Subject = Substitute.ForPartsOf<DefaultCommandRoute>(this.SubCommandCommandRoute, this.Logger);
            this.Subject.Name = "Default";
        }

        public StringBuilderLogger Logger { get; set; }

        public SubCommandCommandRoute SubCommandCommandRoute { get; set; }

        public DefaultCommandRoute Subject { get; set; }

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
            Assert.That(lines[++i], Is.EqualTo("SubCommand                    Provides a component of testability for subcommands."));
            Assert.That(lines[++i], Is.EqualTo("To get help for subcommands"));
            Assert.That(lines[++i], Is.EqualTo("\tDefault <subcommand> Help"));
        }

        [Test]
        public void HelpDisplaysActions()
        {
            // When
            var result = this.Subject.GenerateHelp();
            Console.WriteLine(result);

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
            Assert.That(lines[++i].Trim(), Is.EqualTo("DoSomething (default)         A description of the DoSomething() method."));
            Assert.That(lines[++i], Is.EqualTo("\t--argument1               Lorem ipsum dolor sit amet, consectetur adipiscing elit"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument2               sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument3               Ut enim ad minim veniam"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("Help"));
            Assert.That(lines[++i], Is.EqualTo("\t--actionName              The name of the action to provide help for."));
            Assert.That(lines[++i].Trim(), Is.EqualTo("SomeOtherControllerAction"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithOptionalStringArg"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithOptionalStringArgs"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument1"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument2"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument3"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithRequiredStringArg"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithRequiredStringArgs"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument1"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument2"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("WithSwitch"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("To get help for actions"));
            Assert.That(lines[++i], Is.EqualTo("\tDefault Help <action>"));
        }

        [Test]
        public void HelpForIndividualAction()
        {
            // When
            var result = this.Subject.GenerateHelp("DoSomething");
            Console.WriteLine(result);

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("DoSomething (default)         A description of the DoSomething() method."));
            Assert.That(lines[++i], Is.EqualTo("\t--argument1               Lorem ipsum dolor sit amet, consectetur adipiscing elit"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument2               sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument3               Ut enim ad minim veniam"));
        }

        [Test]
        public void HelpForSubCommands()
        {
            // When
            var result = this.Subject.Execute(new [] { "SubCommand", "Help"});

            // Then
            Assert.That(result, Is.EqualTo(0), this.Logger.ErrorBuilder.ToString());
            this.SubCommandCommandRoute.Received().Help();

            var lines = this.Logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("Provides a component of testability for subcommands."));
            Assert.That(lines[++i].Trim(), Is.EqualTo("ACTIONS"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("DoSomething (default)"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("Help"));
            Assert.That(lines[++i], Is.EqualTo("\t--actionName              The name of the action to provide help for."));
            Assert.That(lines[++i].Trim(), Is.EqualTo("To get help for actions"));

            //TODO: This line should include the root controller.
            Assert.That(lines[++i], Is.EqualTo("\tSubCommand Help <action>"));

        }
    }
}