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
            var args = new[] { "SomeOtherControllerAction" };

            var controller = Substitute.ForPartsOf<DefaultController>();
            controller.Name = "Default";

            var entryPoint = new EntryPoint(controller);
            entryPoint.Execute(args);

            controller.Received().SomeOtherControllerAction();
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

    }
}
