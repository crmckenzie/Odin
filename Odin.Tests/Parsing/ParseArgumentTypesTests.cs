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

        [Test]
        public void WithBoolArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-bool-array", "--values", "true", "false", "true", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { true, false, true});
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithNullableBoolArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-bool-array", "--values", "true", "", "false", "--some-other-input", "42");

            // Then
            result.ShouldNotBeNull();
            result.ParameterValues[0].Value.ShouldBe(new[] { true, (bool?)null, false });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithInt32Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-int32-array", "--numbers", "4", "8", "15", "16","28","42", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithNullableInt32Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-int32-array", "--numbers", "4", "", "15", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, (int?)null, 15});
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithInt64Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-int64-array", "--numbers", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithNullableInt64Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-int64-array", "--numbers", "4", "", "15", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, (int?)null, 15 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithDateTimeArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-date-time-array", "--values", "01/01/2015", "12/25/2015", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { new DateTime(2015,1,1), new DateTime(2015,12,25),  });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithNullableDateTimeArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-date-time-array", "--values", "01/01/2016", "", "12/25/2016", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { new DateTime(2016, 01, 01), (DateTime?)null, new DateTime(2016, 12, 25) });
            result.ParameterValues[1].Value.ShouldBe(42);
        }


        [Test]
        public void WithDecimalArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-decimal-array", "--values", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithNullableDecimalArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-decimal-array", "--values", "4", "", "15", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, (decimal?)null, 15 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithDoubleArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-double-array", "--values", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithNullableDoubleArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-nullable-double-array", "--values", "4", "", "15", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { 4, (double?)null, 15 });
            result.ParameterValues[1].Value.ShouldBe(42);
        }

        [Test]
        public void WithEnumArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("with-enum-array", "--values", "One", "Three", "Two", "--some-other-input", "42");

            // Then
            result.ParameterValues[0].Value.ShouldBe(new[] { Numbers.One, Numbers.Three, Numbers.Two});
            result.ParameterValues[1].Value.ShouldBe(42);
        }
    }
}
