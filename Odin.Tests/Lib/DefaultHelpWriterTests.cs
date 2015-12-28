using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Odin.Configuration;
using Odin.Demo;
using Odin.Tests.Lib;
using Shouldly;

namespace Odin.Tests
{
    [TestFixture]
    public class DefaultHelpWriterTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Subject = new DefaultHelpWriter()
            {
                IdentifierWidth = 20,
                SpacerWidth = 4,
                IndentWidth = 4,
                DescriptionWidth = 52
            };
        }

        public IHelpWriter Subject { get; set; }

        [Test]
        public void DisplayRootCommandDescription()
        {
            // When
            var cmd = new HelpWriterTestCommand();
 
            // Then
            var result = this.Subject.Write(cmd);

            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("This is a demo of the Odin-Commands NuGet package.");
            lines[++i].ShouldBe("You can use this package to easily create command line applications that");
            lines[++i].ShouldBe("automatically route command-line arguments to the correct command based on");
            lines[++i].ShouldBe("customizable conventions.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("default action: default-action");
        }

        [Test]
        public void DisplayDefaultAction()
        {
            // When
            var cmd = new HelpWriterTestCommand();

            // Then
            var result = this.Subject.Write(cmd);

            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .SkipUntil(row => row.StartsWith("default-action"))
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("default-action*         aliases: default");
            lines[++i].ShouldBe("                        Use the ActionAttribute to indicate which methods");
            lines[++i].ShouldBe("                        should be mapped to command line arguments. Set");
            lines[++i].ShouldBe("                        IsDefault=True to mark an action as the default.");
            lines[++i].ShouldBe("                        Only one action per command can be marked default.");
            lines[++i].ShouldBe("                        Use the AliasAttribute to indicate an alias for the");
            lines[++i].ShouldBe("                        action.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("    --param1            aliases: -p, -p1");
            lines[++i].ShouldBe("                        Use the AliasAttribute to indicate one or more");
            lines[++i].ShouldBe("                        aliases for the parameter.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("    --param2            default value: 42");
            lines[++i].ShouldBe("                        aliases: -r, -p2");
            lines[++i].ShouldBe("                        Default values are displayed first, followed by");
            lines[++i].ShouldBe("                        aliases. Each parameter can have its own");
            lines[++i].ShouldBe("                        DescriptionAttribute.");
        }

        [Test]
        public void EnumParamters()
        {
            // When
            var cmd = new HelpWriterTestCommand();

            // Then
            var result = this.Subject.Write(cmd);

            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .SkipUntil(row => row.StartsWith("enum-action"))
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("enum-action             Enumerated values should be listed before default");
            lines[++i].ShouldBe("                        values.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("    --input             valid values: One, Two, Three");
            lines[++i].ShouldBe("                        default value: One");
            lines[++i].ShouldBe("");
        }

        [Test]
        public void HelpDisplaysSubCommands()
        {
            // When
            var logger = new StringBuilderLogger();
            var subCommand = new SubCommand().RegisterSubCommand(new KatasCommand());
            var root = new DefaultCommand().RegisterSubCommand(subCommand).Use(logger);

            var result = this.Subject.Write(root);

            // Then
            var lines = result
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .SkipUntil(row => row == "SUB COMMANDS")
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i], Is.EqualTo("SUB COMMANDS"));
            Assert.That(lines[++i], Is.EqualTo("sub                           Provides a component of testability for subcommands."));
            Assert.That(lines[++i], Is.EqualTo("To get help for subcommands"));
            Assert.That(lines[++i], Is.EqualTo("\thelp <subcommand>"));
        }

        [Test]
        public void HelpForSubCommands()
        {
            // When
            var logger = new StringBuilderLogger();
            var subCommand = new SubCommand().RegisterSubCommand(new KatasCommand());
            var root = new DefaultCommand().RegisterSubCommand(subCommand).Use(logger);
            var result = root.Execute("sub", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .SkipUntil(row =>row== "SUB COMMANDS")
                .ToArray()
                ;

            var i = 0;
            lines[++i].Trim().ShouldBe("katas                         Provides some katas.");
            lines[++i].Trim().ShouldBe("To get help for subcommands");
            lines[++i].Trim().ShouldBe("sub help <subcommand>");
        }

        [Test]
        public void AlternativeHelpForSubCommands()
        {
            // When
            var logger = new StringBuilderLogger();
            var subCommand = new SubCommand().RegisterSubCommand(new KatasCommand());
            var root = new DefaultCommand().RegisterSubCommand(subCommand).Use(logger);
            var result = root.Execute("help", "sub");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
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
            var logger = new StringBuilderLogger();
            var subCommand = new SubCommand().RegisterSubCommand(new KatasCommand());
            var root = new DefaultCommand().RegisterSubCommand(subCommand).Use(logger);
            var result = root.Execute("sub", "katas", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = 0;
            Assert.That(lines[i].Trim(), Is.EqualTo("Provides some katas."));
        }

    }
}