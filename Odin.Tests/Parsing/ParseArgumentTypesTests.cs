using System;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests.Parsing
{
    [TestFixture]
    public class ParseArgumentTypesTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new ArgumentTypesCommand()
                .Use(this.Logger)
                ;
        }

        public Command Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Test]
        public void WithBoolean()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean", "--input", "true");

            // Then
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void WithBoolean_False()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean", "--input", "false");

            // Then
            result.ParameterValues[0].Value.ShouldBe(false);
        }

        [Test]
        public void WithBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean", "--input");

            // Then
            result.ParameterValues[0].Value.ShouldBe(true);
        }


        [Test]
        public void WithBoolean_NoArgumentIdentifier()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean");

            // Then
            result.ParameterValues[0].Value.ShouldBe(false);
        }


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

        [Test]
        public void WithNullableBoolean()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-boolean", "--input", "true");

            // Then
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void WithNullableBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-boolean", "--input");

            // Then
            result.ParameterValues[0].Value.ShouldBe(true);
        }

        [Test]
        public void WithNullableBoolean_False()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-boolean", "--input", "false");

            // Then
            result.ParameterValues[0].Value.ShouldBe(false);
        }

        [Test]
        public void WithNullableBoolean_NoArgumentIdentifier()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-boolean");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
            result.ParameterValues[0].IsValueSet().ShouldBe(true);
        }


        [Test]
        public void WithNullableInt32()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-int32", "--input", "37");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37);
        }

        [Test]
        public void WithNullableInt32_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-int32");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
        }

        [Test]
        public void WithNullableInt64()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-int64", "--input", "37");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37);
        }

        [Test]
        public void WithNullableInt64_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-int64");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
        }

        [Test]
        public void WithNullableDouble()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-double", "--input", "37");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37);
        }

        [Test]
        public void WithNullableDouble_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-double");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
        }

        [Test]
        public void WithNullableDecimal()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-decimal", "--input", "37");

            // Then
            result.ParameterValues[0].Value.ShouldBe(37);
        }

        [Test]
        public void WithNullableDecimal_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-decimal");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
        }

        [Test]
        public void WithNullableEnum()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-enum", "--input", "Two");

            // Then
            result.ParameterValues[0].Value.ShouldBe(Numbers.Two);
        }

        [Test]
        public void WithNullableEnum_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-enum");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
        }


        [Test]
        public void WithNullableDateTime()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-date-time", "--input", "07/29/1975");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new DateTime(1975,07,29));
        }

        [Test]
        public void WithNullableDateTime_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-date-time");

            // Then
            result.ParameterValues[0].Value.ShouldBe(null);
        }

        [Test]
        public void WithBooleanCustomParser()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean-yes-no-parser", "--input", "yes", "--input2", "42", "--input3", "fredbob");

            // Then
            result.ParameterValues[0].Value.ShouldBe(true);
            result.ParameterValues[1].Value.ShouldBe(42);
            result.ParameterValues[2].Value.ShouldBe("fredbob");
        }

        [Test]
        public void WithBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean-yes-no-parser", "--input", "no", "--input2", "42", "--input3", "fredbob");

            // Then
            result.ParameterValues[0].Value.ShouldBe(false);
            result.ParameterValues[1].Value.ShouldBe(42);
            result.ParameterValues[2].Value.ShouldBe("fredbob");
        }

        [Test]
        public void WithAliasedBooleanCustomParser()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean-yes-no-parser", "-i", "yes", "--input2", "42", "--input3", "fredbob");

            // Then
            result.ParameterValues[0].Value.ShouldBe(true);
            result.ParameterValues[1].Value.ShouldBe(42);
            result.ParameterValues[2].Value.ShouldBe("fredbob");
        }

        [Test]
        public void WithAliasedBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-boolean-yes-no-parser", "-i", "no", "--input2", "42", "--input3", "fredbob");

            // Then
            result.ShouldNotBeNull(this.Logger.ErrorBuilder.ToString);
            result.ParameterValues[0].Value.ShouldBe(false);
            result.ParameterValues[1].Value.ShouldBe(42);
            result.ParameterValues[2].Value.ShouldBe("fredbob");
        }

        [Test]
        public void WithStringArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-string-array", "--file-names", "file1", "file2", "file3", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] {"file1", "file2", "file3"});
            result.ParameterValues[1].Value.ShouldBe(42);
        }


    }
}
