using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests.Validation
{
    using Xunit;

    public class ValidationTests
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void DuplicatedParameterAliases()
        {
            // Given
            var cmd = new CommandWithActionParameterAliasDuplication();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The alias '-p' is duplicated for action 'action1'.");
        }

        [Fact]
        public void CommandAndActionParameterNameConflict()
        {
            // Given
            var cmd = new CommandWithSharedParameterAndActionParameterNameConflict();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The shared parameter name '--param1' conflicts with a parameter defined for action 'action1'.");
        }

        [Fact]
        public void SharedParameterAliasDuplication()
        {
            // Given
            var cmd = new CommandWithSharedParameterAliasDuplication();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The alias '-p' is duplicated amongst shared parameters.");
        }

        [Fact]
        public void SharedParameterAndActionParameterAliasConflict()
        {
            // Given
            var cmd = new CommandWithSharedParameterAndActionParameterAliasConflict();

            // When
            var results = cmd.Validate().ToArray();

            // Then
            results.Length.ShouldBe(1);
            results[0].Messages[0].ShouldBe("The alias '-p' for shared parameter '--shared-parameter' is duplicated for parameter '--param1' on action 'action1'.");
        }

    }
}
