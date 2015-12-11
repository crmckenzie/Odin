using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests
{
    [TestFixture]
    public class ControllerInvocationTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.SubCommand = new SubCommand();
            this.Subject = new DefaultCommand(this.SubCommand, this.Logger);
        }

        public StringBuilderLogger Logger { get; set; }

        public SubCommand SubCommand { get; set; }

        public DefaultCommand Subject { get; set; }

        #region ActionExecution

        [Test]
        public void OnlyActionMethodsAreInterpretedAsActions()
        {
            // Given
            var args = new[] { "NotAnAction" };

            // When
            var result = this.Subject.GenerateInvocation(args);

            // Then
            result.ShouldNotBeNull();
            result.Name.ShouldBe("do-something");
            result.ParameterValues[0].Value.ShouldBe("NotAnAction");
            result.ParameterValues[1].Value.ShouldBe(Type.Missing);
            result.ParameterValues[2].Value.ShouldBe(Type.Missing);
        }

        [Test]
        public void CanExecuteAMethodThatIsAnAction()
        {
            // Given
            var args = new[] { "do-something" };

            // When
            var result = this.Subject.GenerateInvocation(args);

            // Then
            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(3);
            result.ParameterValues.ShouldAllBe(pv=> pv.Value == Type.Missing);
        }

        [Test]
        public void ReturnsResultFromAction()
        {
            // Given
            var args = new[] { "always-returns-minus2" };

            // When
            var result = this.Subject.GenerateInvocation(args);

            // Then
            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(0);
        }

        #endregion

        #region Required arguments

        [Test]
        public void WithRequiredStringArg()
        {
            var args = new[] { "with-required-string-arg", "--argument", "value" };

            var result =this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe("value");
        }

        [Test]
        public void WithMultipleRequiredStringArgs()
        {
            var args = new[] { "with-required-string-args", "--argument1", "value1", "--argument2", "value2" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(2);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe("value1");
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe("value2");
        }

        [Test]
        public void CanMatchArgsByParameterOrder()
        {
            var args = new[] { "with-required-string-args", "value1", "value2" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(2);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe("value1");
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe("value2");
        }

        #endregion

        #region Switches

        [Test]
        public void SwitchWithValue()
        {
            var args = new[] { "with-switch", "--argument", "true" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void SwitchWithoutValue()
        {
            var args = new[] { "with-switch", "--argument"};

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void SwitchNotGiven()
        {
            var args = new[] { "with-switch"};

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(false);
        }

        #endregion

        #region Optional arguments

        [Test]
        public void WithOptionalStringArg_DoNotPassIt()
        {
            var args = new[] { "with-optional-string-arg" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(Type.Missing);
        }

        [Test]
        public void WithOptionalStringArg_PassIt()
        {
            var args = new[] { "with-optional-string-arg", "--argument", "value1" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe("value1");
        }

        [Test]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "with-optional-string-args", "--argument1", "value1", "--argument2", "value2", "--argument3", "value3" };

            this.Subject.Execute(args);

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(3);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe("value1");
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe("value2");
            result.ParameterValues[2].Name.ShouldBe("argument3");
            result.ParameterValues[2].Value.ShouldBe("value3");
        }

        [Test]
        public void WithOptionalStringArgs_PassHead()
        {
            var args = new[] { "with-optional-string-args", "--argument1", "value1" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(3);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe("value1");
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe(Type.Missing);
            result.ParameterValues[2].Name.ShouldBe("argument3");
            result.ParameterValues[2].Value.ShouldBe(Type.Missing);
        }

        [Test]
        public void WithOptionalStringArgs_PassBody()
        {
            var args = new[] { "with-optional-string-args", "--argument2", "value2" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(3);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe(Type.Missing);
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe("value2");
            result.ParameterValues[2].Name.ShouldBe("argument3");
            result.ParameterValues[2].Value.ShouldBe(Type.Missing);
        }

        [Test]
        public void WithOptionalStringArgs_PassNone()
        {
            var args = new[] { "with-optional-string-args" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(3);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe(Type.Missing);
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe(Type.Missing);
            result.ParameterValues[2].Name.ShouldBe("argument3");
            result.ParameterValues[2].Value.ShouldBe(Type.Missing);
        }

        [Test]
        public void WithOptionalStringArgs_PassTail()
        {
            var args = new[] { "with-optional-string-args", "--argument3", "value3" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(3);
            result.ParameterValues[0].Name.ShouldBe("argument1");
            result.ParameterValues[0].Value.ShouldBe(Type.Missing);
            result.ParameterValues[1].Name.ShouldBe("argument2");
            result.ParameterValues[1].Value.ShouldBe(Type.Missing);
            result.ParameterValues[2].Name.ShouldBe("argument3");
            result.ParameterValues[2].Value.ShouldBe("value3");
        }

        #endregion

        #region SubCommands

        [Test]
        public void ExecuteSubCommand()
        {
            var args = new[] { "sub" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.Instance.ShouldBe(this.SubCommand);
            result.ParameterValues.Count.ShouldBe(0);
        }

        #endregion
    }
}