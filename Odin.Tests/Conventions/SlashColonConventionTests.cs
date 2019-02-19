using System;
using Odin.Conventions;
using Odin.Tests.Lib;
using Shouldly;
using Xunit;

namespace Odin.Tests.Conventions
{
    public class SlashColonConventionTests
    {
        public SlashColonConventionTests()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommand = new SubCommand();
            this.Subject = new DefaultCommand(this.SubCommand);
            this.Subject
                .Use(this.Logger)
                .Use(new SlashColonConvention())
                ;
        }

        private StringBuilderLogger Logger { get; }

        private SubCommand SubCommand { get; }

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

        #endregion

        #region Required arguments

        [Fact]
        public void WithRequiredStringArg()
        {
            var args = new[] { "WithRequiredStringArg", "/argument:value" };


            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithRequiredStringArg");
            this.Subject.MethodArguments[0].ShouldBe("value");
        }

        [Fact]
        public void WithMultipleRequiredStringArgs()
        {
            var args = new[] { "WithRequiredStringArgs", "/argument1:value1", "/argument2:value2"};

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithRequiredStringArgs");
            this.Subject.MethodArguments.ShouldBe(new []{"value1", "value2"});
        }

        [Fact]
        public void CanMatchArgsByParameterOrder()
        {
            var args = new[] { "WithRequiredStringArgs", "value1", "value2" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithRequiredStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2" });
        }

        #endregion

        #region Switches

        [Fact]
        public void SwitchWithValue()
        {
            var args = new[] { "WithSwitch", "/argument:true"};

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] {true});
        }

        [Fact]
        public void SwitchWithoutValue()
        {
            var args = new[] { "WithSwitch", "/argument" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { true });
        }

        [Fact]
        public void NegativeSwitch()
        {
            var args = new[] { "WithSwitch", "/no-argument" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { false });
        }

        [Fact]
        public void SwitchNotGiven()
        {
            var args = new[] { "WithSwitch" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithSwitch");
            this.Subject.MethodArguments.ShouldBe(new object[] { false });
        }

        #endregion

        #region Optional arguments

        [Fact]
        public void WithOptionalStringArg_DoNotPassIt()
        {
            var args = new[] { "WithOptionalStringArg" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArg");
            var defaultValueForMethod = "not-passed";
            this.Subject.MethodArguments.ShouldBe(new []{defaultValueForMethod});

        }

        [Fact]
        public void WithOptionalStringArg_PassIt()
        {
            var args = new[] { "WithOptionalStringArg", "/argument:value1"};

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArg");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "WithOptionalStringArgs", "/argument1:value1", "/argument2:value2", "/argument3:value3"};

            this.Subject.Execute(args);

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2", "value3" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassHead()
        {
            var args = new[] { "WithOptionalStringArgs", "/argument1:value1"};

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1", "value2-not-passed", "value3-not-passed" });

        }

        [Fact]
        public void WithOptionalStringArgs_PassBody()
        {
            var args = new[] { "WithOptionalStringArgs", "/argument2:value2"};

            var result = this.Subject.Execute(args);
            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1-not-passed", "value2", "value3-not-passed" });

        }

        [Fact]
        public void WithOptionalStringArgs_PassNone()
        {
            var args = new[] { "WithOptionalStringArgs" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1-not-passed", "value2-not-passed", "value3-not-passed" });
        }

        [Fact]
        public void WithOptionalStringArgs_PassTail()
        {
            var args = new[] { "WithOptionalStringArgs", "/argument3:value3"};

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe("WithOptionalStringArgs");
            this.Subject.MethodArguments.ShouldBe(new[] { "value1-not-passed", "value2-not-passed", "value3" });
        }

        #endregion

        #region SubCommands

        [Fact]
        public void ExecuteSubCommand()
        {
            var args = new[] { "Sub" };

            var result = this.Subject.Execute(args);

            this.Subject.MethodCalled.ShouldBe(null);
            this.Subject.MethodArguments.ShouldBe(null);

            this.SubCommand.MethodCalled.ShouldBe("DoSomething");
        }

        #endregion
    }
}