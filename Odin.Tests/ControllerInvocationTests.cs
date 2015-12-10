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
            this.SubCommandCommandRoute = new SubCommandCommandRoute(this.Logger)
            {
                Name = "SubCommand"
            };

            this.Subject = new DefaultCommandRoute(this.SubCommandCommandRoute, this.Logger);
            this.Subject.Name = "Default";
        }

        public StringBuilderLogger Logger { get; set; }

        public SubCommandCommandRoute SubCommandCommandRoute { get; set; }

        public DefaultCommandRoute Subject { get; set; }

        #region ActionExecution

        [Test]
        public void CannotExecuteAMethodThatIsNotAnAction()
        {
            // Given
            var args = new[] { "NotAnAction" };

            // When
            var result = this.Subject.GenerateInvocation(args);

            // Then
            result.ShouldBeNull();
        }

        [Test]
        public void CanExecuteAMethodThatIsAnAction()
        {
            // Given
            var args = new[] { "DoSomething" };

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
            var args = new[] { "AlwaysReturnsMinus2" };

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
            var args = new[] { "WithRequiredStringArg", "--argument", "value" };

            var result =this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe("value");
        }

        [Test]
        public void WithMultipleRequiredStringArgs()
        {
            var args = new[] { "WithRequiredStringArgs", "--argument1", "value1", "--argument2", "value2" };

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
            var args = new[] { "WithRequiredStringArgs", "value1", "value2" };

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
            var args = new[] { "WithSwitch", "--argument", "true" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void SwitchWithoutValue()
        {
            var args = new[] { "WithSwitch", "--argument"};

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void SwitchNotGiven()
        {
            var args = new[] { "WithSwitch"};

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
            var args = new[] { "WithOptionalStringArg" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe(Type.Missing);
        }

        [Test]
        public void WithOptionalStringArg_PassIt()
        {
            var args = new[] { "WithOptionalStringArg", "--argument", "value1" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.ParameterValues.Count.ShouldBe(1);
            result.ParameterValues[0].Name.ShouldBe("argument");
            result.ParameterValues[0].Value.ShouldBe("value1");
        }

        [Test]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument1", "value1", "--argument2", "value2", "--argument3", "value3" };

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
            var args = new[] { "WithOptionalStringArgs", "--argument1", "value1" };

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
            var args = new[] { "WithOptionalStringArgs", "--argument2", "value2" };

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
            var args = new[] { "WithOptionalStringArgs" };

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
            var args = new[] { "WithOptionalStringArgs", "--argument3", "value3" };

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
            var args = new[] { "SubCommand" };

            var result = this.Subject.GenerateInvocation(args);

            result.ShouldNotBeNull();
            result.Instance.ShouldBe(this.SubCommandCommandRoute);
            result.ParameterValues.Count.ShouldBe(0);
        }

        #endregion
    }
}