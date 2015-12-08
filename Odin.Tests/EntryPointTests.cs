using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Odin.Tests
{
    [TestFixture]
    public class EntryPointTests
    {
        [Test]
        public void ExecutesDefaultControllerAction()
        {
            var args = new string[] {};

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            controller.Received().DoSomething();
        }

        [Test]
        public void ExecuteDifferentControllerAction()
        {
            var args = new string[] { "SomeOtherControllerAction" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            controller.Received().SomeOtherControllerAction();
        }

        [Test]
        public void ActionWithRequiredStringArg()
        {
            var args = new[] { "WithRequiredStringArg", "--argument", "value"};

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            controller.Received().WithRequiredStringArg("value");
        }

        [Test]
        public void ActionWithMultipleRequiredStringArgs()
        {
            var args = new[] { "WithRequiredStringArgs", "--argument1", "value1", "--argument2", "value2" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            controller.Received().WithRequiredStringArgs("value1", "value2");
        }

        [Test]
        public void ActionWithOptionalStringArg_DoNotPassIt()
        {
            var args = new[] { "WithOptionalStringArg" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            Assert.That(controller.MethodArguments, Is.EquivalentTo(new [] { "not-passed"}));
        }

        [Test]
        public void ActionWithOptionalStringArg_PassIt()
        {
            var args = new[] { "WithOptionalStringArg", "--argument", "value1" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            Assert.That(controller.MethodArguments, Is.EquivalentTo(new[] { "value1" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassThemAll()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument1", "value1", "--argument2", "value2", "--argument3", "value3" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            Assert.That(controller.MethodArguments, Is.EquivalentTo(new[] { "value1", "value2", "value3" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassHead()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument1", "value1"};

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            Assert.That(controller.MethodArguments, Is.EquivalentTo(new[] { "value1", "value2-not-passed", "value3-not-passed" }));
        }
        [Test]
        public void WithOptionalStringArgs_PassBody()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument2", "value2" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            Assert.That(controller.MethodArguments, Is.EquivalentTo(new[] { "value1-not-passed", "value2", "value3-not-passed" }));
        }

        [Test]
        public void WithOptionalStringArgs_PassTail()
        {
            var args = new[] { "WithOptionalStringArgs", "--argument3", "value3" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            Assert.That(controller.MethodArguments, Is.EquivalentTo(new[] { "value1-not-passed", "value2-not-passed", "value3" }));
        }

        [Test]
        public void ExecuteNamedControllerDefaultAction()
        {
            var args = new[] { "NotTheDefault" };

            var defaultController = Substitute.ForPartsOf<DefaultController>();
            var notTheDefaultController = Substitute.ForPartsOf<NotTheDefaultController>();
            notTheDefaultController.Name = "NotTheDefault";

            var entryPoint = new EntryPoint(defaultController, notTheDefaultController);
            entryPoint.Execute(args);

            defaultController.DidNotReceive().DoSomething();
            notTheDefaultController.Received().DoSomething();
        }

        [Test]
        public void ExecuteDefaultControllerWithNamedAction()
        {
            var args = new[] { "DoSomething" };

            var defaultController = Substitute.ForPartsOf<DefaultController>();
            var notTheDefaultController = Substitute.ForPartsOf<NotTheDefaultController>();
 
            var entryPoint = new EntryPoint(defaultController, notTheDefaultController);
            entryPoint.Execute(args);

            defaultController.Received().DoSomething();
        }

        [Test]
        public void ExecuteSubCommand()
        {
            var args = new[] { "SubCommand" };

            var subCommandController = Substitute.ForPartsOf<SubCommandController>();
            subCommandController.Name = "SubCommand";

            var defaultController = new DefaultController(subCommandController);
            var notTheDefaultController = Substitute.ForPartsOf<NotTheDefaultController>();

            var entryPoint = new EntryPoint(defaultController, notTheDefaultController);
            entryPoint.Execute(args);

            subCommandController.Received().DoSomething();
        }
    }
}
