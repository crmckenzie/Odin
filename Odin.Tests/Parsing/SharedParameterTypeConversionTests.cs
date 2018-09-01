using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests.Parsing
{
    using Odin.Tests.Lib;

    using Xunit;

    public class SharedParameterTypeConversionTests
    {
        public SharedParameterTypeConversionTests()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new SharedParameterTypesCommand();
            this.Subject.Use(this.Logger)
                ;
        }

        public SharedParameterTypesCommand Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Fact]
        public void WithBoolean()
        {
            // When
            this.Subject.Execute("execute", "--boolean", "true");

            // Then
            this.Subject.Boolean.ShouldBe(true);
        }

        [Fact]
        public void WithNegatedBoolean()
        {
            // When
            var result = this.Subject.Execute("execute", "--no-boolean");

            // Then
            this.Subject.Boolean.ShouldBe(false);
        }


        [Fact]
        public void WithBoolean_False()
        {
            // When
            var result = this.Subject.Execute("execute", "--boolean", "false");

            // Then
            this.Subject.Boolean.ShouldBe(false);
        }

        [Fact]
        public void WithBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.Execute("execute", "--boolean");

            // Then
            this.Subject.Boolean.ShouldBe(true);
        }


        [Fact]
        public void WithInt32()
        {
            // When
            var result = this.Subject.Execute("execute", "--int32", "37");

            // Then
            this.Subject.Int32.ShouldBe(37);
        }

        [Fact]
        public void WithInt64()
        {
            // When
            var result = this.Subject.Execute("execute", "--int64", "37");

            // Then
            this.Subject.Int64.ShouldBe(37);
        }

        [Fact]
        public void WithDouble()
        {
            // When
            var result = this.Subject.Execute("execute", "--double", "37.45");

            // Then
            this.Subject.Double.ShouldBe(37.45);
        }

        [Fact]
        public void WithDecimal()
        {
            // When
            var result = this.Subject.Execute("execute", "--decimal", "37.45");

            // Then
            this.Subject.Decimal.ShouldBe(37.45m);
        }

        [Fact]
        public void WithEnum()
        {
            // When
            var result = this.Subject.Execute("execute", "--enum", "Two");

            // Then
            this.Subject.Enum.ShouldBe(Numbers.Two);
        }

        [Fact]
        public void WithDateTime()
        {
            // When
            var result = this.Subject.Execute("execute", "--date-time", "07/29/1975");

            // Then
            this.Subject.DateTime.ShouldBe(new DateTime(1975, 07, 29));
        }

        [Fact]
        public void WithNullableBoolean()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-boolean", "true");

            // Then
            this.Subject.NullableBoolean.ShouldBe(true);
        }

        [Fact]
        public void WithNullableBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-boolean");

            // Then
            this.Subject.NullableBoolean.ShouldBe(true);
        }

        [Fact]
        public void WithNullableBoolean_False()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-boolean", "false");

            // Then
            this.Subject.NullableBoolean.ShouldBe(false);
        }


        [Fact]
        public void WithNullableInt32()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-int32", "37");

            // Then
            this.Subject.NullableInt32.ShouldBe(37);
        }

        [Fact]
        public void WithNullableInt64()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-int64", "37");

            // Then
            this.Subject.NullableInt64.ShouldBe(37);
        }

        [Fact]
        public void WithNullableDouble()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-double", "37.45");

            // Then
            this.Subject.NullableDouble.ShouldBe(37.45);
        }


        [Fact]
        public void WithNullableDecimal()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-decimal", "37.45");

            // Then
            this.Subject.NullableDecimal.ShouldBe(37.45m);
        }


        [Fact]
        public void WithNullableEnum()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-enum", "Two");

            // Then
            this.Subject.NullableEnum.ShouldBe(Numbers.Two);
        }


        [Fact]
        public void WithNullableDateTime()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-date-time", "07/29/1975");

            // Then
            this.Subject.NullableDateTime.ShouldBe(new DateTime(1975, 07, 29));
        }


        [Fact]
        public void WithBooleanCustomParser()
        {
            // When
            var result = this.Subject.Execute("execute", "--custom-parser", "yes");

            // Then
            this.Subject.CustomParser.ShouldBe(true);
        }

        [Fact]
        public void WithBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.Execute("execute", "--custom-parser", "no");

            // Then
            this.Subject.CustomParser.ShouldBe(false);
        }


        [Fact]
        public void WithStringArray()
        {
            // When
            var result = this.Subject.Execute("execute" ,"--string-array", "file1", "file2", "file3", "--int32", "42");

            // Then
            this.Subject.StringArray.ShouldBe(new [] {"file1", "file2", "file3"});
        }

        [Fact]
        public void WithBoolArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--bool-array", "true", "false", "true", "--int32", "42");

            // Then
            this.Subject.BoolArray.ShouldBe(new[] { true, false, true });
        }

        [Fact]
        public void WithNullableBoolArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-bool-array", "true", "", "false", "--int32", "42");

            // Then
            this.Subject.NullableBoolArray.ShouldBe(new[] { true, (bool?)null, false});
        }

        [Fact]
        public void WithInt32Array()
        {
            // When
            var result = this.Subject.Execute("execute", "--int32-array", "4", "8", "15", "16","28","42", "--int32", "42");

            // Then
            this.Subject.Int32Array.ShouldBe(new[] { 4,8,15,16,28,42 });
        }

        [Fact]
        public void WithNullableInt32Array()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-int32-array", "4", "", "15", "--int32", "42");

            // Then
            this.Subject.NullableInt32Array.ShouldBe(new[] { 4, (int?)null, 15});
        }

        [Fact]
        public void WithInt64Array()
        {
            // When
            var result = this.Subject.Execute("execute", "--int64-array", "4", "8", "15", "16", "28", "42", "--int32", "42");

            // Then
            this.Subject.Int64Array.ShouldBe(new long[] { 4, 8, 15, 16, 28, 42 });
        }

        [Fact]
        public void WithNullableInt64Array()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-int64-array", "4", "", "15", "--int32", "42");

            // Then
            this.Subject.NullableInt64Array.ShouldBe(new[] { 4, (long?)null, 15 });
        }

        [Fact]
        public void WithDateTimeArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--date-time-array", "01/01/2015", "12/25/2015", "--int32", "42");

            // Then
            this.Subject.DateTimeArray.ShouldBe(new[] { new DateTime(2015,1,1), new DateTime(2015,12,25),  });
        }

        [Fact]
        public void WithNullableDateTimeArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-date-time-array", "01/01/2015", "", "12/25/2015", "--int32", "42");

            // Then
            this.Subject.NullableDateTimeArray.ShouldBe(new[] { new DateTime(2015, 1, 1), (DateTime?)null, new DateTime(2015, 12, 25), });
        }


        [Fact]
        public void WithDecimalArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--decimal-array", "4", "8", "15", "16", "28", "42", "--int32", "42");

            // Then
            this.Subject.DecimalArray.ShouldBe(new decimal[] { 4, 8, 15, 16, 28, 42 });
        }

        [Fact]
        public void WithNullableDecimalArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-decimal-array", "4", "", "15", "--int32", "42");

            // Then
            this.Subject.NullableDecimalArray.ShouldBe(new[] { 4, (decimal?)null, 15 });
        }

        [Fact]
        public void WithDoubleArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--double-array", "4", "8", "15", "16", "28", "42", "--double", "42");

            // Then
            this.Subject.DoubleArray.ShouldBe(new double[] { 4, 8, 15, 16, 28, 42 });
        }

        [Fact]
        public void WithNullableDoubleArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--nullable-double-array", "4", "", "15", "--int32", "42");

            // Then
            this.Subject.NullableDoubleArray.ShouldBe(new[] { 4, (double?)null, 15 });
        }

        [Fact]
        public void WithEnumArray()
        {
            // When
            var result = this.Subject.Execute("execute", "--enum-array", "One", "Three", "Two", "--int32", "42");

            // Then
            this.Subject.EnumArray.ShouldBe(new[] { Numbers.One, Numbers.Three, Numbers.Two});
        }
    }
}