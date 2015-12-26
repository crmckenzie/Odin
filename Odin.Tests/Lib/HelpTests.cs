using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Odin.Configuration;
using Odin.Demo;
using Shouldly;

namespace Odin.Tests
{
    [TestFixture]
    public class HelpTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommand = new SubCommand();
            this.SubCommand.RegisterSubCommand(new KatasCommand());
            this.DefaultCommand = new DefaultCommand(this.SubCommand);
            this.DefaultCommand.Use(this.Logger);
            this.Subject = this.DefaultCommand.HelpGenerator;
        }

        public DefaultCommand DefaultCommand { get; set; }

        public StringBuilderLogger Logger { get; set; }

        public SubCommand SubCommand { get; set; }

        public HelpGenerator Subject { get; set; }

        [Test]
        public void HelpDisplaysControllerDescription()
        {
            // When
            var result = this.Subject.Emit(this.DefaultCommand);

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i], Is.EqualTo("This is the default command"));
        }

        [Test]
        public void HelpDisplaysSubCommands()
        {
            // When
            var result = this.Subject.Emit(this.DefaultCommand);

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[++i], Is.EqualTo("SUB COMMANDS"));
            Assert.That(lines[++i], Is.EqualTo("sub                           Provides a component of testability for subcommands."));
            Assert.That(lines[++i], Is.EqualTo("To get help for subcommands"));
            Assert.That(lines[++i], Is.EqualTo("\thelp <subcommand>"));
        }

        [Test]
        public void HelpDisplaysActions()
        {
            // When
            var result = this.Subject.Emit(this.DefaultCommand);
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
            Assert.That(lines[++i].Trim(), Is.EqualTo("always-returns-false"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("always-returns-minus2"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("always-returns-true"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("do-something (default)        A description of the DoSomething() method."));
            Assert.That(lines[++i], Is.EqualTo("\t--argument1               aliases: -a, -A"));
            Assert.That(lines[++i], Is.EqualTo("\t                          Lorem ipsum dolor sit amet, consectetur adipiscing elit"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument2               aliases: -b, -B"));
            Assert.That(lines[++i], Is.EqualTo("\t                          sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument3               aliases: -c, -C"));
            Assert.That(lines[++i], Is.EqualTo("\t                          Ut enim ad minim veniam"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("help"));
            lines[++i].ShouldBe("\t--action-name             The name of the action to provide help for.");
            Assert.That(lines[++i].Trim(), Is.EqualTo("some-other-controller-action"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("with-arguments-of-various-types"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("--i"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("--j"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("with-optional-string-arg"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("with-optional-string-args"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument1"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument2"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument3"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("with-required-string-arg"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("with-required-string-args"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument1"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument2"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("with-switch"));
            Assert.That(lines[++i].TrimEnd(), Is.EqualTo("\t--argument"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("To get help for actions"));
            Assert.That(lines[++i], Is.EqualTo("\tdefault help <action>"));
        }

        [Test]
        public void HelpForIndividualAction()
        {
            // When
            var result = this.Subject.Emit(this.DefaultCommand, "do-something");
            Console.WriteLine(result);

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("do-something (default)        A description of the DoSomething() method."));
            Assert.That(lines[++i], Is.EqualTo("\t--argument1               aliases: -a, -A"));
            Assert.That(lines[++i], Is.EqualTo("\t                          Lorem ipsum dolor sit amet, consectetur adipiscing elit"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument2               aliases: -b, -B"));
            Assert.That(lines[++i], Is.EqualTo("\t                          sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"));
            Assert.That(lines[++i], Is.EqualTo("\t--argument3               aliases: -c, -C"));
            Assert.That(lines[++i], Is.EqualTo("\t                          Ut enim ad minim veniam"));
        }

        [Test]
        public void HelpForSubCommands()
        {
            // When
            var result = this.DefaultCommand.Execute("sub", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), this.Logger.ErrorBuilder.ToString());

            var lines = this.Logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("Provides a component of testability for subcommands."));
            lines[++i].Trim().ShouldBe("SUB COMMANDS");
            lines[++i].Trim().ShouldBe("katas                         Provides some katas.");
            lines[++i].Trim().ShouldBe("To get help for subcommands");
            lines[++i].Trim().ShouldBe("sub help <subcommand>");
            Assert.That(lines[++i].Trim(), Is.EqualTo("ACTIONS"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("do-something (default)"));
            Assert.That(lines[++i].Trim(), Is.EqualTo("help"));
            Assert.That(lines[++i], Is.EqualTo("\t--action-name             The name of the action to provide help for."));
            Assert.That(lines[++i].Trim(), Is.EqualTo("To get help for actions"));

            Assert.That(lines[++i], Is.EqualTo("\tsub help <action>"));
        }

        [Test]
        public void AlternativeHelpForSubCommands()
        {
            // When
            var result = this.DefaultCommand.Execute("help", "sub");

            // Then
            Assert.That(result, Is.EqualTo(0), this.Logger.ErrorBuilder.ToString());

            var lines = this.Logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("Provides a component of testability for subcommands."));
        }

        [Test]
        public void HelpForSubSubCommands()
        {
            // When
            var result = this.DefaultCommand.Execute("sub", "katas", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), this.Logger.ErrorBuilder.ToString());

            var lines = this.Logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            Console.WriteLine(this.Logger.InfoBuilder.ToString());

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("Provides some katas."));
        }

    }
}