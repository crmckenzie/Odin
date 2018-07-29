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
            var args = new[] { "NotAnAction" };

            // When
            var result = this.Subject.GetAction(args);

            // Then
            result.ShouldNotBeNull();
            result.Name.ShouldBe("do-something");
            result.Parameters[0].Value.ShouldBe("NotAnAction");
            result.Parameters[1].Value.ShouldBe(Type.Missing);
            result.Parameters[2].Value.ShouldBe(Type.Missing);
        }

        [Fact]
        public void CanExecuteAMethodThatIsAnAction()
        {
            // Given
            var args = new[] { "do-something" };

            // When
            var result = this.Subject.GetAction(args);

            // Then
            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(3);
            result.Parameters.ShouldAllBe(pv=> pv.Value == Type.Missing);
        }

        [Fact]
        public void ReturnsResultFromAction()
        {
            // Given
            var args = new[] { "always-returns-minus2" };

            // When
            var result = this.Subject.GetAction(args);

            // Then
            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(0);
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

            var result =this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Name.ShouldBe("argument");
            result.Parameters[0].Value.ShouldBe("value");
        }

        [Fact]
        public void WithMultipleRequiredStringArgs()
        {
            var args = new[] { "with-required-string-args", "--argument1", "value1", "--argument2", "value2" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(2);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe("value1");
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe("value2");
        }

        [Fact]
        public void CanMatchArgsByParameterOrder()
        {
            var args = new[] { "with-required-string-args", "value1", "value2" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(2);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe("value1");
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe("value2");
        }

        #endregion

        #region Switches

        [Fact]
        public void SwitchWithValue()
        {
            var args = new[] { "with-switch", "--argument", "true" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Name.ShouldBe("argument");
            result.Parameters[0].Value.ShouldBe(true);
        }

        [Fact]
        public void SwitchWithoutValue()
        {
            var args = new[] { "with-switch", "--argument"};

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Name.ShouldBe("argument");
            result.Parameters[0].Value.ShouldBe(true);
        }

        [Fact]
        public void SwitchNotGiven()
        {
            var args = new[] { "with-switch"};

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Name.ShouldBe("argument");
            result.Parameters[0].Value.ShouldBe(false);
        }

        #endregion

        #region Optional arguments

        [Fact]
        public void WithOptionalStringArg_DoNotPassIt()
        {
            var args = new[] { "with-optional-string-arg" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Name.ShouldBe("argument");
            result.Parameters[0].Value.ShouldBe(Type.Missing);
        }

        [Fact]
        public void WithOptionalStringArg_PassIt()
        {
            var args = new[] { "with-optional-string-arg", "--argument", "value1" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Name.ShouldBe("argument");
            result.Parameters[0].Value.ShouldBe("value1");
        }

        [Fact]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "with-optional-string-args", "--argument1", "value1", "--argument2", "value2", "--argument3", "value3" };

            this.Subject.Execute(args);

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe("value1");
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe("value2");
            result.Parameters[2].Name.ShouldBe("argument3");
            result.Parameters[2].Value.ShouldBe("value3");
        }

        [Fact]
        public void WithOptionalStringArgs_PassHead()
        {
            var args = new[] { "with-optional-string-args", "--argument1", "value1" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe("value1");
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe(Type.Missing);
            result.Parameters[2].Name.ShouldBe("argument3");
            result.Parameters[2].Value.ShouldBe(Type.Missing);
        }

        [Fact]
        public void WithOptionalStringArgs_PassBody()
        {
            var args = new[] { "with-optional-string-args", "--argument2", "value2" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe(Type.Missing);
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe("value2");
            result.Parameters[2].Name.ShouldBe("argument3");
            result.Parameters[2].Value.ShouldBe(Type.Missing);
        }

        [Fact]
        public void WithOptionalStringArgs_PassNone()
        {
            var args = new[] { "with-optional-string-args" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe(Type.Missing);
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe(Type.Missing);
            result.Parameters[2].Name.ShouldBe("argument3");
            result.Parameters[2].Value.ShouldBe(Type.Missing);
        }

        [Fact]
        public void WithOptionalStringArgs_PassTail()
        {
            var args = new[] { "with-optional-string-args", "--argument3", "value3" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Name.ShouldBe("argument1");
            result.Parameters[0].Value.ShouldBe(Type.Missing);
            result.Parameters[1].Name.ShouldBe("argument2");
            result.Parameters[1].Value.ShouldBe(Type.Missing);
            result.Parameters[2].Name.ShouldBe("argument3");
            result.Parameters[2].Value.ShouldBe("value3");
        }

        #endregion

        #region SubCommands

        [Fact]
        public void ExecuteSubCommand()
        {
            var args = new[] { "sub" };

            var result = this.Subject.GetAction(args);

            result.ShouldNotBeNull();
            result.Command.ShouldBe(this.SubCommand);
            result.Parameters.Count.ShouldBe(0);
        }

        #endregion
    }
}