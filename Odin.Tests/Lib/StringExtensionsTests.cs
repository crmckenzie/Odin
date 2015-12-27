using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Odin.Configuration;
using Shouldly;

namespace Odin.Tests.Lib
{

    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void PageinateSolidText()
        {
            var str = "abcdefghijklmnopqrstuvwxyz";

            var results = str.Paginate(5).ToArray();

            results.Length.ShouldBe(1);
            results[0].ShouldBe(str);
        }

        [Test]
        public void PaginateNormalText()
        {
            var str = "The quick brown fox jumped over the lazy dog.";

            var results = str.Paginate(10).ToArray();

            results.Length.ShouldBe(5);
            results[0].ShouldBe("The quick");
            results[1].ShouldBe("brown fox");
            results[2].ShouldBe("jumped");
            results[3].ShouldBe("over the");
            results[4].ShouldBe("lazy dog.");
        }
    }
}
