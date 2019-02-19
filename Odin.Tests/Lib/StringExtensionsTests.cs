using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Odin.Conventions;
using Shouldly;

namespace Odin.Tests.Lib
{
    using Xunit;

    public class StringExtensionsTests
    {
        [Fact]
        public void PageinateSolidText()
        {
            var str = "abcdefghijklmnopqrstuvwxyz";

            var results = str.Paginate(5).ToArray();

            results.Length.ShouldBe(1);
            results[0].ShouldBe(str);
        }

        [Fact]
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

        [Fact]
        public void PaginateTextWithLineBreaks()
        {
            var text = 
@"Out of the night that covers me
black as a pit from pole to pole
I thank whatever gods may be
for my unconquerable soul.

In the fell clutch of circumstance
I have not winced nor cried allowed
Under the bludgeonings of chance
My head is bloody but unbowed.

Beyond this place of wrath and tears
Looms but the horror of the shade
And still the menace of the years
Finds and shall find me unafraid.

It matters not how straight the gate
How charged with punishments the scroll
I am the master of my fate
I am the master of my soul.
";

            var results = text.Paginate(20).ToArray();

            var i = -1;
            results[++i].ShouldBe("Out of the night");
            results[++i].ShouldBe("that covers me");
            results[++i].ShouldBe("black as a pit from");
            results[++i].ShouldBe("pole to pole");
            results[++i].ShouldBe("I thank whatever");
            results[++i].ShouldBe("gods may be");
            results[++i].ShouldBe("for my unconquerable");
            results[++i].ShouldBe("soul.");
            results[++i].ShouldBe("");
            results[++i].ShouldBe("In the fell clutch");

        }
    }
}
