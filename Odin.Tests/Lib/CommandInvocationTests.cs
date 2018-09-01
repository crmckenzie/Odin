using Odin.Tests.Samples.Demo;

namespace Odin.Tests.Lib
{
    using System;
    using Shouldly;

    using Xunit;

    public class CommandInvocationTests
    {
        public CommandInvocationTests()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommand = new SubCommand();
            this.Subject = new DefaultCommand(this.SubCommand);
            this.Subject
                .Use(this.Logger)
                ;
        }

        public StringBuilderLogger Logger { get; set; }

        public SubCommand SubCommand { get; set; }

        public DefaultCommand Subject { get; set; }

        #region ActionExecution

        [Fact]
        public void OnlyActionMethodsAreInterpretedAsActions()
        {
            // Given
            var methodThatExistsButNotDecoratedWithActionAttribute = "NotAnAction";
            var args = new[] { methodThatExistsButNotDecoratedWithActionAttribute };

            // When
            var result = this.Subject.Execute(args);

            // Then
            var defaultAction = "DoSomething";
            this.Subject.MethodCalled.ShouldBe(defaultAction);

            this.Subject.MethodArguments[0].ShouldBe(methodThatExistsButNotDecoratedWithActionAttribute);
            this.Subject.MethodArguments[1].ShouldBe("value2-not-passed");
            this.Subject.MethodArguments[2].ShouldBe("value3-not-passed");
        }

        [Fact]
        public void CanExecuteAMethodThatIsAnAction()
        {
            // Given
            var args = new[] { "DoSomething" };

            // When
            var result = this.Subject.Execute(args);

            // Then
            this.Subject.MethodCalled.ShouldBe("DoSomething");
        }

        [Fact]
        public void ReturnsResultFromAction()
        {
            // Given
            var args = new[] { "always-returns-minus2" };

            // When
            var result = this.Subject.Execute(args);

            // Then
            this.Subject.MethodCalled.ShouldBe("AlwaysReturnsMinus2");
            result.ShouldBe(-2);
        }

        [Fact]
        public void BeforeAndAfterEventsAreExecuted()
        {
            // Given
            var subject = new ExecutionLifecycleCommand();

            // When
            subject.Execute("do-stuff");

            // Then
            subject.Before.ShouldNotBe(DateTime.MinValue);
            subject.Begin.ShouldBeGreaterThan(subject.Before);
            subject.After.ShouldBeGreaterThan(subject.Begin);
        }


        [Fact]
        public void BeforeAndAfterEventsAreExecutedOnSubCommand()
        {
            // Given
            var root = new RootCommand();
            var subject = new ExecutionLifecycleCommand();
            root.RegisterSubCommand(subject);

            // When
            root.Execute("execution-lifecycle", "do-stuff");

            // Then
            subject.Before.ShouldNotBe(DateTime.MinValue);
            subject.Begin.ShouldBeGreaterThan(subject.Before);
            subject.After.ShouldBeGreaterThan(subject.Begin);
        }

        #endregion

        #region Required arguments

        [Fact]
        public void WithRequiredStringArg()
        {
            var args = new[] { "with-required-string-arg", "--argument", "value" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithRequiredStringArg");
            this.Subject.MethodArguments[0].ShouldBe("value");
        }

        [Fact]
        public void WithMultipleRequiredStringArgs()
        {
            var args = new[] { "with-required-string-args", "--argument1", "value1", "--argument2", "value2" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithRequiredStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2" });
        }

        [Fact]
        public void CanMatchArgsByParameterOrder()
        {
            var args = new[] { "with-required-string-args", "value1", "value2" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithRequiredStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2" });
        }

        #endregion

        #region Switches

        [Fact]
        public void SwitchWithValue()
        {
            var args = new[] { "with-switch", "--argument", "true" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { true });
        }

        [Fact]
        public void SwitchWithoutValue()
        {
            var args = new[] { "with-switch", "--argument"};

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { true });
        }

        [Fact]
        public void NegativeSwitch()
        {
            var args = new[] { "with-switch", "--no-argument" };

            var result = this.Subject.Execute(args);

            result.ShouldBe(0);
            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { false });
        }

        [Fact]
        public void SwitchNotGiven()
        {
            var args = new[] { "with-switch"};


            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { false });
        }

        #endregion

        #region Optional arguments

        [Fact]
        public void WithOptionalStringArg_DoNotPassIt()
        {
            var args = new[] { "with-optional-string-arg" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArg");
            var defaultValueForMethod = "not-passed";
            this.Subject.MethodArguments.ShouldBe(new[] { defaultValueForMethod });
        }

        [Fact]
        public void WithOptionalStringArg_PassIt()
        {
            var args = new[] { "with-optional-string-arg", "--argument", "value1" };


            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArg");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "with-optional-string-args", "--argument1", "value1", "--argument2", "value2", "--argument3", "value3" };

            this.Subject.Execute(args);

            this.Subject.Execute(args);

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2", "value3" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassHead()
        {
            var args = new[] { "with-optional-string-args", "--argument1", "value1" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2-not-passed", "value3-not-passed" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassBody()
        {
            var args = new[] { "with-optional-string-args", "--argument2", "value2" };

            var result = this.Subject.Execute(args);
            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1-not-passed", "value2", "value3-not-passed" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassNone()
        {
            var args = new[] { "with-optional-string-args" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1-not-passed", "value2-not-passed", "value3-not-passed" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassTail()
        {
            var args = new[] { "with-optional-string-args", "--argument3", "value3" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1-not-passed", "value2-not-passed", "value3" });
        }

        #endregion

        #region SubCommands

        [Fact]
        public void ExecuteSubCommand()
        {
            var args = new[] { "sub" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe(null);
            this.Subject.MethodArguments.ShouldBe(null);

            this.SubCommand.MethodCalled.ShouldBe("DoSomething");
        }

        #endregion
    }
}