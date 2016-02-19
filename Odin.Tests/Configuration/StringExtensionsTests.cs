using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Odin.Configuration;
using Shouldly;

namespace Odin.Tests.Lib.Configuration
{
    [TestFixture]
    public class StringExtensionsTests
    {

        [Test]
        [TestCase("Foo", "foo")]
        public void HyphenCase(string input, string output)
        {
            input.HyphenCase().ShouldBe(output);
        }

    }
}
