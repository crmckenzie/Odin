using System;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests.Parsing
{
    using Odin.Tests.Lib;

    using Xunit;

    public class ActionParameterTypeConversionTests
    {
        public ActionParameterTypeConversionTests()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new ArgumentTypesCommand()
                .Use(this.Logger)
                ;
        }

        public Command Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Fact]
        public void WithBoolean()
        {
            // When
            var result = this.Subject.GetAction("with-boolean", "--input", "true");

            // Then
            result.Parameters[0].Value.ShouldBe(true);
        }

        [Fact]
        public void WithNegatedBoolean()
        {
            // When
            var result = this.Subject.GetAction("with-boolean", "--no-input");

            // Then
            result.Parameters[0].Value.ShouldBe(false);
        }


        [Fact]
        public void WithBoolean_False()
        {
            // When
            var result = this.Subject.GetAction("with-boolean", "--input", "false");

            // Then
            result.Parameters[0].Value.ShouldBe(false);
        }

        [Fact]
        public void WithBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GetAction("with-boolean", "--input");

            // Then
            result.Parameters[0].Value.ShouldBe(true);
        }


        [Fact]
        public void WithBoolean_NoArgumentIdentifier()
        {
            // When
            var result = this.Subject.GetAction("with-boolean");

            // Then
            result.Parameters[0].Value.ShouldBe(false);
        }


        [Fact]
        public void WithInt32()
        {
            // When
            var result = this.Subject.GetAction("with-int32", "--input", "37");

            // Then
            result.Parameters[0].Value.ShouldBe(37);
        }

        [Fact]
        public void WithInt64()
        {
            // When
            var result = this.Subject.GetAction("with-int64", "--input", "37");

            // Then
            result.Parameters[0].Value.ShouldBe(37);
        }

        [Fact]
        public void WithDouble()
        {
            // When
            var result = this.Subject.GetAction("with-double", "--input", "37.45");

            // Then
            result.Parameters[0].Value.ShouldBe(37.45);
        }

        [Fact]
        public void WithDecimal()
        {
            // When
            var result = this.Subject.GetAction("with-decimal", "--input", "37.45");

            // Then
            result.Parameters[0].Value.ShouldBe(37.45m);
        }

        [Fact]
        public void WithEnum()
        {
            // When
            var result = this.Subject.GetAction("with-enum", "--input", "Two");

            // Then
            result.Parameters[0].Value.ShouldBe(Numbers.Two);
        }

        [Fact]
        public void WithDateTime()
        {
            // When
            var result = this.Subject.GetAction("with-date-time", "--input", "07/29/1975");

            // Then
            result.Parameters[0].Value.ShouldBe(new DateTime(1975,07,29));
        }

        [Fact]
        public void WithNullableBoolean()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-boolean", "--input", "true");

            // Then
            result.Parameters[0].Value.ShouldBe(true);
        }

        [Fact]
        public void WithNullableBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-boolean", "--input");

            // Then
            result.Parameters[0].Value.ShouldBe(true);
        }

        [Fact]
        public void WithNullableBoolean_False()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-boolean", "--input", "false");

            // Then
            result.Parameters[0].Value.ShouldBe(false);
        }

        [Fact]
        public void WithNullableBoolean_NoArgumentIdentifier()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-boolean");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
            result.Parameters[0].IsValueSet().ShouldBe(true);
        }


        [Fact]
        public void WithNullableInt32()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-int32", "--input", "37");

            // Then
            result.Parameters[0].Value.ShouldBe(37);
        }

        [Fact]
        public void WithNullableInt32_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-int32");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
        }

        [Fact]
        public void WithNullableInt64()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-int64", "--input", "37");

            // Then
            result.Parameters[0].Value.ShouldBe(37);
        }

        [Fact]
        public void WithNullableInt64_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-int64");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
        }

        [Fact]
        public void WithNullableDouble()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-double", "--input", "37");

            // Then
            result.Parameters[0].Value.ShouldBe(37);
        }

        [Fact]
        public void WithNullableDouble_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-double");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
        }

        [Fact]
        public void WithNullableDecimal()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-decimal", "--input", "37");

            // Then
            result.Parameters[0].Value.ShouldBe(37);
        }

        [Fact]
        public void WithNullableDecimal_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-decimal");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
        }

        [Fact]
        public void WithNullableEnum()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-enum", "--input", "Two");

            // Then
            result.Parameters[0].Value.ShouldBe(Numbers.Two);
        }

        [Fact]
        public void WithNullableEnum_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-enum");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
        }


        [Fact]
        public void WithNullableDateTime()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-date-time", "--input", "07/29/1975");

            // Then
            result.Parameters[0].Value.ShouldBe(new DateTime(1975,07,29));
        }

        [Fact]
        public void WithNullableDateTime_ArgumentIdentifierUnspecified()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-date-time");

            // Then
            result.Parameters[0].Value.ShouldBe(null);
        }

        [Fact]
        public void WithBooleanCustomParser()
        {
            // When
            var result = this.Subject.GetAction("with-boolean-yes-no-parser", "--input", "yes", "--input2", "42", "--input3", "fredbob");

            // Then
            result.Parameters[0].Value.ShouldBe(true);
            result.Parameters[1].Value.ShouldBe(42);
            result.Parameters[2].Value.ShouldBe("fredbob");
        }

        [Fact]
        public void WithBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.GetAction("with-boolean-yes-no-parser", "--input", "no", "--input2", "42", "--input3", "fredbob");

            // Then
            result.Parameters[0].Value.ShouldBe(false);
            result.Parameters[1].Value.ShouldBe(42);
            result.Parameters[2].Value.ShouldBe("fredbob");
        }

        [Fact]
        public void WithAliasedBooleanCustomParser()
        {
            // When
            var result = this.Subject.GetAction("with-boolean-yes-no-parser", "-i", "yes", "--input2", "42", "--input3", "fredbob");

            // Then
            result.Parameters[0].Value.ShouldBe(true);
            result.Parameters[1].Value.ShouldBe(42);
            result.Parameters[2].Value.ShouldBe("fredbob");
        }

        [Fact]
        public void WithAliasedBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.GetAction("with-boolean-yes-no-parser", "-i", "no", "--input2", "42", "--input3", "fredbob");

            // Then
            result.ShouldNotBeNull(this.Logger.ErrorBuilder.ToString);
            result.Parameters[0].Value.ShouldBe(false);
            result.Parameters[1].Value.ShouldBe(42);
            result.Parameters[2].Value.ShouldBe("fredbob");
        }

        [Fact]
        public void WithStringArray()
        {
            // When
            var result = this.Subject.GetAction("with-string-array", "--file-names", "file1", "file2", "file3", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] {"file1", "file2", "file3"});
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithBoolArray()
        {
            // When
            var result = this.Subject.GetAction("with-bool-array", "--values", "true", "false", "true", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { true, false, true});
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithNullableBoolArray()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-bool-array", "--values", "true", "", "false", "--some-other-input", "42");

            // Then
            result.ShouldNotBeNull();
            result.Parameters[0].Value.ShouldBe(new[] { true, (bool?)null, false });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithInt32Array()
        {
            // When
            var result = this.Subject.GetAction("with-int32-array", "--numbers", "4", "8", "15", "16","28","42", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithNullableInt32Array()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-int32-array", "--numbers", "4", "", "15", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, (int?)null, 15});
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithInt64Array()
        {
            // When
            var result = this.Subject.GetAction("with-int64-array", "--numbers", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithNullableInt64Array()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-int64-array", "--numbers", "4", "", "15", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, (int?)null, 15 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithDateTimeArray()
        {
            // When
            var result = this.Subject.GetAction("with-date-time-array", "--values", "01/01/2015", "12/25/2015", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { new DateTime(2015,1,1), new DateTime(2015,12,25),  });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithNullableDateTimeArray()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-date-time-array", "--values", "01/01/2016", "", "12/25/2016", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { new DateTime(2016, 01, 01), (DateTime?)null, new DateTime(2016, 12, 25) });
            result.Parameters[1].Value.ShouldBe(42);
        }


        [Fact]
        public void WithDecimalArray()
        {
            // When
            var result = this.Subject.GetAction("with-decimal-array", "--values", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithNullableDecimalArray()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-decimal-array", "--values", "4", "", "15", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, (decimal?)null, 15 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithDoubleArray()
        {
            // When
            var result = this.Subject.GetAction("with-double-array", "--values", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithNullableDoubleArray()
        {
            // When
            var result = this.Subject.GetAction("with-nullable-double-array", "--values", "4", "", "15", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { 4, (double?)null, 15 });
            result.Parameters[1].Value.ShouldBe(42);
        }

        [Fact]
        public void WithEnumArray()
        {
            // When
            var result = this.Subject.GetAction("with-enum-array", "--values", "One", "Three", "Two", "--some-other-input", "42");

            // Then
            result.Parameters[0].Value.ShouldBe(new[] { Numbers.One, Numbers.Three, Numbers.Two});
            result.Parameters[1].Value.ShouldBe(42);
        }
    }
}
