using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests.Validation
{
    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void WithMultipleDefaultActions()
        {
            // Given
            var cmd =new CommandWithMultipleDefaultActions();
            
            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].CommandName.ShouldBe("with-multiple-default-actions");
            results[0].Messages[0].ShouldBe("There is more than one default action: action1, action2.");
        }

        [Test]
        public void SubCommandActionNamingConflict()
        {
            // Given
            var cmd =new CommandWithNamingConflictBetweenSubCommandAndAction();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].CommandName.ShouldBe("with-naming-conflict-between-sub-and-action");
            results[0].Messages[0].ShouldBe("There is more than one executable action named 'katas'.");
        }

        [Test]
        public void ValidationIncludesResultsFromSubCommands()
        {
            // Given
            var cmd = new CommandWithNamingConflictBetweenSubCommandAndAction()
                .RegisterSubCommand(new CommandWithMultipleDefaultActions())
                ;

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(2);
            results[1].CommandName.ShouldBe("with-multiple-default-actions");
            results[1].Messages[0].ShouldBe("There is more than one default action: action1, action2.");
        }

        [Test]
        public void DuplicatedParameterAliases()
        {
            // Given
            var cmd = new CommandWithMethodParameterAliasDuplication();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The alias '-p' is duplicated for action 'action1'.");
        }

        [Test]
        public void CommandAndMethodParameterNameConflict()
        {
            // Given
            var cmd = new CommandWithCommonParameterAndMethodParameterNameConflict();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The common parameter name '--param1' conflicts with a parameter defined for action 'action1'.");
        }

        [Test]
        public void CommonParameterAliasDuplication()
        {
            // Given
            var cmd = new CommandWithCommonParameterAliasDuplication();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The alias '-p' is duplicated amongst common parameters.");
        }

        [Test]
        public void CommonParameterAndMethodParameterAliasConflict()
        {
            // Given
            var cmd = new CommandWithCommonParameterAndMethodParameterAliasConflict();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The alias '-p' for common parameter '--common-parameter' is duplicated for parameter '--param1' on action 'action1'.");
        }

    }
}
