using Shouldly;
using System;

namespace Odin.Tests.Parsing
{
    using Odin.Tests.Lib;

    using Xunit;

    public class ActionParameterTypeConversionTests
    {
        public ActionParameterTypeConversionTests()
        {
            Logger = new StringBuilderLogger();
            Subject = new ArgumentTypesCommand();
            Subject
            .Use(Logger)
            ;
        }

        public ArgumentTypesCommand Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Fact]
        public void WithBoolean()
        {
            // When
            var result = Subject.Execute("with-boolean", "--input", "true");

            // Then
            Subject.MethodArguments.ShouldBe(new object[]{true});
       }

        [Fact]
        public void WithNegatedBoolean()
        {
            // When
            var result = Subject.Execute("with-boolean", "--no-input");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { false });
        }


        [Fact]
        public void WithBoolean_False()
        {
            // When
            var result = Subject.Execute("with-boolean", "--input", "false");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { false });
        }

        [Fact]
        public void WithBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = Subject.Execute("with-boolean", "--input");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { true });
        }


        [Fact]
        public void WithBoolean_NoArgumentIdentifier()
        {
            // When
            var result = Subject.Execute("with-boolean");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { false });
        }


        [Fact]
        public void WithInt32()
        {
            // When
            var result = Subject.Execute("with-int32", "--input", "37");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37});

        }

        [Fact]
        public void WithInt64()
        {
            // When
            var result = Subject.Execute("with-int64", "--input", "37");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37 });
        }

        [Fact]
        public void WithDouble()
        {
            // When
            var result = Subject.Execute("with-double", "--input", "37.45");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37.45 });
        }

        [Fact]
        public void WithDecimal()
        {
            // When
            var result = Subject.Execute("with-decimal", "--input", "37.45");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37.45m });
        }

        [Fact]
        public void WithEnum()
        {
            // When
            var result = Subject.Execute("with-enum", "--input", "Two");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { Numbers.Two });
        }

        [Fact]
        public void WithDateTime()
        {
            // When
            var result = Subject.Execute("with-date-time", "--input", "07/29/1975");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { new DateTime(1975, 7, 29) });
        }

        [Fact]
        public void WithNullableBoolean()
        {
            // When
            var result = Subject.Execute("with-nullable-boolean", "--input", "true");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { true });
        }

        [Fact]
        public void WithNullableBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = Subject.Execute("with-nullable-boolean", "--input");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { true });
        }

        [Fact]
        public void WithNullableBoolean_False()
        {
            // When
            var result = Subject.Execute("with-nullable-boolean", "--input", "false");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] {false});
        }

        [Fact]
        public void WithNullableBoolean_NoArgumentIdentifier()
        {
            // When
            var result = Subject.Execute("with-nullable-boolean");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }


        [Fact]
        public void WithNullableInt32()
        {
            // When
            var result = Subject.Execute("with-nullable-int32", "--input", "37");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37 });
        }

        [Fact]
        public void WithNullableInt32_ArgumentIdentifierUnspecified()
        {
            // When
            var result = Subject.Execute("with-nullable-int32");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }

        [Fact]
        public void WithNullableInt64()
        {
            // When
            var result = Subject.Execute("with-nullable-int64", "--input", "37");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37 });
        }

        [Fact]
        public void WithNullableInt64_ArgumentIdentifierUnspecified()
        {
            // When
            var result = Subject.Execute("with-nullable-int64");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }

        [Fact]
        public void WithNullableDouble()
        {
            // When
            var result = Subject.Execute("with-nullable-double", "--input", "37");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37m });
        }

        [Fact]
        public void WithNullableDouble_ArgumentIdentifierUnspecified()
        {
            // When
            var result = Subject.Execute("with-nullable-double");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }

        [Fact]
        public void WithNullableDecimal()
        {
            // When
            var result = Subject.Execute("with-nullable-decimal", "--input", "37");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { 37 });
        }

        [Fact]
        public void WithNullableDecimal_ArgumentIdentifierUnspecified()
        {
            // When
            var result = Subject.Execute("with-nullable-decimal");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }

        [Fact]
        public void WithNullableEnum()
        {
            // When
            var result = Subject.Execute("with-nullable-enum", "--input", "Two");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { Numbers.Two });
        }

        [Fact]
        public void WithNullableEnum_ArgumentIdentifierUnspecified()
        {
            // When
            var result = Subject.Execute("with-nullable-enum");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }


        [Fact]
        public void WithNullableDateTime()
        {
            // When
            var result = Subject.Execute("with-nullable-date-time", "--input", "07/29/1975");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { new DateTime(1975, 7, 29) });
        }

        [Fact]
        public void WithNullableDateTime_ArgumentIdentifierUnspecified()
        {
            // When
            var result = Subject.Execute("with-nullable-date-time");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { null });
        }

        [Fact]
        public void WithBooleanCustomParser()
        {
            // When
            var result = Subject.Execute("with-boolean-yes-no-parser", "--input", "yes", "--input2", "42", "--input3", "fredbob");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { true, 42, "fredbob" });

        }

        [Fact]
        public void WithBoolean_FalseCustomParser()
        {
            // When
            var result = Subject.Execute("with-boolean-yes-no-parser", "--input", "no", "--input2", "42", "--input3", "fredbob");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { false, 42, "fredbob" });
        }

        [Fact]
        public void WithAliasedBooleanCustomParser()
        {
            // When
            var result = Subject.Execute("with-boolean-yes-no-parser", "-i", "yes", "--input2", "42", "--input3", "fredbob");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { true, 42, "fredbob" });
        }

        [Fact]
        public void WithAliasedBoolean_FalseCustomParser()
        {
            // When
            var result = Subject.Execute("with-boolean-yes-no-parser", "-i", "no", "--input2", "42", "--input3", "fredbob");

            // Then
            Subject.MethodArguments.ShouldBe(new object[] { false, 42, "fredbob" });
        }

        [Fact]
        public void WithStringArray()
        {
            // When
            var result = Subject.Execute("with-string-array", "--file-names", "file1", "file2", "file3", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { "file1", "file2", "file3" };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });

        }

        [Fact]
        public void WithBoolArray()
        {
            // When
            var result = Subject.Execute("with-bool-array", "--values", "true", "false", "true", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { true, false, true};
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithNullableBoolArray()
        {
            // When
            var result = Subject.Execute("with-nullable-bool-array", "--values", "true", "", "false", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { true, (bool?)null, false };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithInt32Array()
        {
            // When
            var result = Subject.Execute("with-int32-array", "--numbers", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, 8, 15, 16, 28, 42 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithNullableInt32Array()
        {
            // When
            var result = Subject.Execute("with-nullable-int32-array", "--numbers", "4", "", "15", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, (int?)null, 15 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithInt64Array()
        {
            // When
            var result = Subject.Execute("with-int64-array", "--numbers", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, 8, 15, 16, 28, 42 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithNullableInt64Array()
        {
            // When
            var result = Subject.Execute("with-nullable-int64-array", "--numbers", "4", "", "15", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, (int?)null, 15 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithDateTimeArray()
        {
            // When
            var result = Subject.Execute("with-date-time-array", "--values", "01/01/2015", "12/25/2015", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { new DateTime(2015, 1, 1), new DateTime(2015, 12, 25), };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithNullableDateTimeArray()
        {
            // When
            var result = Subject.Execute("with-nullable-date-time-array", "--values", "01/01/2016", "", "12/25/2016", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { new DateTime(2016, 01, 01), (DateTime?)null, new DateTime(2016, 12, 25) };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }


        [Fact]
        public void WithDecimalArray()
        {
            // When
            var result = Subject.Execute("with-decimal-array", "--values", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, 8, 15, 16, 28, 42 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });

        }

        [Fact]
        public void WithNullableDecimalArray()
        {
            // When
            var result = Subject.Execute("with-nullable-decimal-array", "--values", "4", "", "15", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, (decimal?)null, 15 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithDoubleArray()
        {
            // When
            var result = Subject.Execute("with-double-array", "--values", "4", "8", "15", "16", "28", "42", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, 8, 15, 16, 28, 42 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithNullableDoubleArray()
        {
            // When
            var result = Subject.Execute("with-nullable-double-array", "--values", "4", "", "15", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { 4, (double?)null, 15 };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }

        [Fact]
        public void WithEnumArray()
        {
            // When
            var result = Subject.Execute("with-enum-array", "--values", "One", "Three", "Two", "--some-other-input", "42");

            // Then
            var arrayValue = new[] { Numbers.One, Numbers.Three, Numbers.Two };
            Subject.MethodArguments.ShouldBe(new object[] { arrayValue, 42 });
        }
    }
}
