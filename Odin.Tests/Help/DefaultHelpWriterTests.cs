using System.Linq;
using NUnit.Framework;
using Odin.Configuration;
using Odin.Demo;
using Odin.Tests.Lib;
using Shouldly;

namespace Odin.Tests.Help
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
                .Select(row => row.Replace("\r", "").TrimEnd())
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
                .Select(row => row.Replace("\r", "").TrimEnd())
                .SkipUntil(row => row.StartsWith("enum-action"))
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("enum-action             aliases: enum");
            lines[++i].ShouldBe("                        Enumerated parameters should be listed before");
            lines[++i].ShouldBe("                        default value.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("    --input             valid values: One, Two, Three");
            lines[++i].ShouldBe("                        default value: One");
            lines[++i].ShouldBe("");
        }

        [Test]
        public void BooleanParameters()
        {
            // When
            var cmd = new HelpWriterTestCommand();

            // Then
            var result = this.Subject.Write(cmd);

            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", "").TrimEnd())
                .SkipUntil(row => row.StartsWith("boolean-action"))
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("boolean-action          Boolean parameters should list the negative option");
            lines[++i].ShouldBe("                        before default value.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("    --input             --no-input to negate");
            lines[++i].ShouldBe("                        default value: False");
            lines[++i].ShouldBe("");
        }

        [Test]
        public void CommonParameters()
        {
            // When
            var cmd = new HelpWriterTestCommand();

            // Then
            var result = this.Subject.Write(cmd);

            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", "").TrimEnd())
                .SkipUntil(row => row.StartsWith("COMMON PARAMETERS"))
                .ToArray()
                ;
            int i = 0;
            lines[i++].ShouldBe("COMMON PARAMETERS");
            lines[i++].ShouldBe("    --common1           default value:");
            lines[i++].ShouldBe("                        aliases: -c");
            lines[i++].ShouldBe("                        Common parameters are displayed after all of the");
            lines[i++].ShouldBe("                        actions");
        }

        #region actions

        [Test]
        public void HelpForAGivenAction()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .Use(logger)
                ;

            // When
            var result = this.Subject.Write(root, "enum-action");

            // Then
            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", "").TrimEnd())
                .ToArray()
                ;

            var i = 0;
            lines[i++].ShouldBe("enum-action             aliases: enum");
            lines[i++].ShouldBe("                        Enumerated parameters should be listed before");
            lines[i++].ShouldBe("                        default value.");
            lines[i++].ShouldBe("");
            lines[i++].ShouldBe("    --input             valid values: One, Two, Three");
            lines[i++].ShouldBe("                        default value: One");
            lines[i++].ShouldBe("");
            lines[i++].ShouldBe("");
            lines[i++].ShouldBe("");
            lines[i++].ShouldBe("COMMON PARAMETERS");
            lines[i++].ShouldBe("    --common1           default value:");
            lines[i++].ShouldBe("                        aliases: -c");
            lines[i++].ShouldBe("                        Common parameters are displayed after all of the");
            lines[i++].ShouldBe("                        actions");
        }

        [Test]
        public void HelpForAGivenActionUsingAlias()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .Use(logger)
                ;

            // When
            var result = this.Subject.Write(root, "enum");

            // Then
            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", "").TrimEnd())
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("enum-action             aliases: enum");
            lines[++i].ShouldBe("                        Enumerated parameters should be listed before");
            lines[++i].ShouldBe("                        default value.");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("    --input             valid values: One, Two, Three");
            lines[++i].ShouldBe("                        default value: One");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("");
            lines[++i].ShouldBe("");
        }

        #endregion

        #region subcommands

        [Test]
        public void HelpDisplaysSubCommands()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .RegisterSubCommand(new KatasCommand())
                .Use(logger)
                ;

            // When
            var result = this.Subject.Write(root);

            // Then
            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", "").TrimEnd())
                .SkipUntil(row => row == "SUB COMMANDS")
                .ToArray()
                ;

            var i = -1;
            lines[++i].ShouldBe("SUB COMMANDS");
            lines[++i].ShouldBe("katas                   This command is intended for demonstration purposes.");
            lines[++i].ShouldBe("                        It provides some katas which can be executed as");
            lines[++i].ShouldBe("                        actions.");
            lines[++i].ShouldBe("To get help for subcommands");
            lines[++i].ShouldBe("\thelp <subcommand>");
        }

        [Test]
        public void HelpInvokedAsSubCommandAction()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .RegisterSubCommand(new KatasCommand())
                .Use(logger)
                ;

            // When
            var result = root.Execute("katas", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = -1;
            lines[++i].Trim().ShouldBe("This command is intended for demonstration purposes. It provides some katas");
            lines[++i].Trim().ShouldBe("which can be executed as actions.");
            lines[++i].Trim().ShouldBe("");
            lines[++i].Trim().ShouldBe("default action: fizz-buzz");
        }

        [Test]
        public void HelpForSubCommandUsingAlias()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .RegisterSubCommand(new KatasCommand())
                .Use(logger)
                ;

            // When
            var result = this.Subject.Write(root, "exercise");

            // Then
            var lines = result
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = -1;
            lines[++i].Trim().ShouldBe("This command is intended for demonstration purposes. It provides some katas");
            lines[++i].Trim().ShouldBe("which can be executed as actions.");
            lines[++i].Trim().ShouldBe("");
            lines[++i].Trim().ShouldBe("default action: fizz-buzz");
        }

        [Test]
        public void RootSubCommandSectionFooter()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .RegisterSubCommand(new KatasCommand())
                .Use(logger)
                ;

            // When
            var result = root.Execute("help");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .SkipUntil(row => row == "To get help for subcommands")
                .ToArray()
                ;

            var i = -1;
            lines[++i].Trim().ShouldBe("To get help for subcommands");
            lines[++i].Trim().ShouldBe("help <subcommand>");
        }

        [Test]
        public void HelpInvokedAsActionOnParent()
        {
            // Given
            var logger = new StringBuilderLogger();
            var root = new HelpWriterTestCommand()
                .RegisterSubCommand(new KatasCommand())
                .Use(logger)
                ;

            // When
            var result = root.Execute("help", "katas");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = -1;
            lines[++i].Trim().ShouldBe("This command is intended for demonstration purposes. It provides some katas");
            lines[++i].Trim().ShouldBe("which can be executed as actions.");
            lines[++i].Trim().ShouldBe("");
            lines[++i].Trim().ShouldBe("default action: fizz-buzz");
        }

        [Test]
        public void HelpForSubSubCommands()
        {
            // Given
            var logger = new StringBuilderLogger();
            var subCommand = new SubCommand()
                .RegisterSubCommand(new KatasCommand())
                ;
            var root = new DefaultCommand()
                .RegisterSubCommand(subCommand)
                .Use(logger)
                ;

            // When
            var result = root.Execute("sub", "katas", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row.Replace("\r", ""))
                .ToArray()
                ;

            var i = -1;
            lines[++i].Trim().ShouldBe("This command is intended for demonstration purposes. It provides some katas");
        }

        [Test]
        public void SubCommandSectionFooter()
        {
            // Given
            var logger = new StringBuilderLogger();
            var subCommand = new SubCommand()
                .RegisterSubCommand(new KatasCommand())
                ;
            var root = new DefaultCommand()
                .RegisterSubCommand(subCommand)
                .Use(logger)
                ;

            // When
            var result = root.Execute("sub", "help");

            // Then
            Assert.That(result, Is.EqualTo(0), logger.ErrorBuilder.ToString());

            var lines = logger.InfoBuilder.ToString()
                .Split('\n')
                .Select(row => row.Replace("\r", ""))
                .SkipUntil(row => row == "To get help for subcommands")
                .ToArray()
                ;

            var i = -1;
            lines[++i].Trim().ShouldBe("To get help for subcommands");
            lines[++i].Trim().ShouldBe("sub help <subcommand>");
        }
        #endregion
    }
}