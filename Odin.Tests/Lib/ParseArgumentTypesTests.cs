using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Odin.Demo;
using Shouldly;

namespace Odin.Tests.Lib
{
    [TestFixture]
    public class ParseArgumentTypesTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Subject = new ArgumentTypesCommand();
        }

        public Command Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Test]
        public void WithInt32()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-int32", "--input", "37");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37);
        }

        [Test]
        public void WithInt64()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-int64", "--input", "37");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37);
        }

        [Test]
        public void WithDouble()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-double", "--input", "37.45");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37.45);
        }

        [Test]
        public void WithDecimal()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-decimal", "--input", "37.45");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37.45m);
        }

        [Test]
        public void WithEnum()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-enum", "--input", "Two");

            // Then
            result.ParameterValues[0].Value.ShouldBe(Numbers.Two);
        }

        [Test]
        public void WithDateTime()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-date-time", "--input", "07/29/1975");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new DateTime(1975,07,29));
        }

    }
}
