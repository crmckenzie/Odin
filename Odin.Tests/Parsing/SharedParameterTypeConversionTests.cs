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
            this.Subject = new SharedParameterTypesCommand()
                .Use(this.Logger)
                ;
        }

        public Command Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Fact]
        public void WithBoolean()
        {
            // When
            var result = this.Subject.GetAction("execute", "--boolean", "true");

            // Then
            result.ShouldNotBe(null);
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(true);
        }

        [Fact]
        public void WithNegatedBoolean()
        {
            // When
            var result = this.Subject.GetAction("execute", "--no-boolean");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(false);
        }


        [Fact]
        public void WithBoolean_False()
        {
            // When
            var result = this.Subject.GetAction("execute", "--boolean", "false");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(false);
        }

        [Fact]
        public void WithBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GetAction("execute", "--boolean");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(true);
        }


        [Fact]
        public void WithInt32()
        {
            // When
            var result = this.Subject.GetAction("execute", "--int32", "37");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Int32"]
                .Value.ShouldBe(37);
        }

        [Fact]
        public void WithInt64()
        {
            // When
            var result = this.Subject.GetAction("execute", "--int64", "37");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Int64"]
                .Value.ShouldBe(37);
        }

        [Fact]
        public void WithDouble()
        {
            // When
            var result = this.Subject.GetAction("execute", "--double", "37.45");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Double"]
                .Value.ShouldBe(37.45);
        }

        [Fact]
        public void WithDecimal()
        {
            // When
            var result = this.Subject.GetAction("execute", "--decimal", "37.45");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Decimal"]
                .Value.ShouldBe(37.45m);
        }

        [Fact]
        public void WithEnum()
        {
            // When
            var result = this.Subject.GetAction("execute", "--enum", "Two");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Enum"]
                .Value.ShouldBe(Numbers.Two);
        }

        [Fact]
        public void WithDateTime()
        {
            // When
            var result = this.Subject.GetAction("execute", "--date-time", "07/29/1975");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["DateTime"]
                .Value.ShouldBe(new DateTime(1975, 07, 29));
        }

        [Fact]
        public void WithNullableBoolean()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-boolean", "true");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableBoolean"]
                .Value.ShouldBe(true);
        }

        [Fact]
        public void WithNullableBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-boolean");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableBoolean"]
                .Value.ShouldBe(true);
        }

        [Fact]
        public void WithNullableBoolean_False()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-boolean", "false");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableBoolean"]
                .Value.ShouldBe(false);
        }


        [Fact]
        public void WithNullableInt32()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-int32", "37");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableInt32"]
                .Value.ShouldBe(37);
        }



        [Fact]
        public void WithNullableInt64()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-int64", "37");

            // Then
            result.ShouldNotBe(null);
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableInt64"]
                .Value.ShouldBe(37);
        }

        [Fact]
        public void WithNullableDouble()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-double", "37.45");

            // Then
            result.ShouldNotBe(null);
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableDouble"]
                .Value.ShouldBe(37.45);
        }


        [Fact]
        public void WithNullableDecimal()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-decimal", "37.45");

            // Then
            result.ShouldNotBe(null);
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableDecimal"]
                .Value.ShouldBe(37.45);
        }


        [Fact]
        public void WithNullableEnum()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-enum", "Two");

            // Then
            result.ShouldNotBe(null);
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableEnum"]
                .Value.ShouldBe(Numbers.Two);
        }


        [Fact]
        public void WithNullableDateTime()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-date-time", "07/29/1975");

            // Then
            result.ShouldNotBe(null);
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableDateTime"]
                .Value.ShouldBe(new DateTime(1975, 07, 29));
        }


        [Fact]
        public void WithBooleanCustomParser()
        {
            // When
            var result = this.Subject.GetAction("execute", "--custom-parser", "yes");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["CustomParser"]
                .Value.ShouldBe(true);
        }

        [Fact]
        public void WithBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.GetAction("execute", "--custom-parser", "no");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["CustomParser"]
                .Value.ShouldBe(false);
        }


        [Fact]
        public void WithStringArray()
        {
            // When
            var result = this.Subject.GetAction("execute" ,"--string-array", "file1", "file2", "file3", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["StringArray"]
                .Value.ShouldBe(new [] {"file1", "file2", "file3"});
        }

        [Fact]
        public void WithBoolArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--bool-array", "true", "false", "true", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["BoolArray"]
                .Value.ShouldBe(new[] { true, false, true });
        }

        [Fact]
        public void WithNullableBoolArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-bool-array", "true", "", "false", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableBoolArray"]
                .Value.ShouldBe(new[] { true, (bool?)null, false});
        }

        [Fact]
        public void WithInt32Array()
        {
            // When
            var result = this.Subject.GetAction("execute", "--int32-array", "4", "8", "15", "16","28","42", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Int32Array"]
                .Value.ShouldBe(new[] { 4,8,15,16,28,42 });
        }

        [Fact]
        public void WithNullableInt32Array()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-int32-array", "4", "", "15", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableInt32Array"]
                .Value.ShouldBe(new[] { 4, (int?)null, 15});
        }

        [Fact]
        public void WithInt64Array()
        {
            // When
            var result = this.Subject.GetAction("execute", "--int64-array", "4", "8", "15", "16", "28", "42", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["Int64Array"]
                .Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
        }

        [Fact]
        public void WithNullableInt64Array()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-int64-array", "4", "", "15", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableInt64Array"]
                .Value
                .ShouldBe(new[] { 4, (long?)null, 15 });
        }

        [Fact]
        public void WithDateTimeArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--date-time-array", "01/01/2015", "12/25/2015", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["DateTimeArray"]
                .Value.ShouldBe(new[] { new DateTime(2015,1,1), new DateTime(2015,12,25),  });
        }

        [Fact]
        public void WithNullableDateTimeArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-date-time-array", "01/01/2015", "", "12/25/2015", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableDateTimeArray"]
                .Value.ShouldBe(new[] { new DateTime(2015, 1, 1), (DateTime?)null, new DateTime(2015, 12, 25), });
        }


        [Fact]
        public void WithDecimalArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--decimal-array", "4", "8", "15", "16", "28", "42", "--int32", "42");

            // Then
            result.SharedParameters
               .ToDictionary(cp => cp.Name)["DecimalArray"]
               .Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
        }

        [Fact]
        public void WithNullableDecimalArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-decimal-array", "4", "", "15", "--int32", "42");

            // Then
            result.SharedParameters
               .ToDictionary(cp => cp.Name)["NullableDecimalArray"]
               .Value.ShouldBe(new[] { 4, (decimal?)null, 15 });
        }

        [Fact]
        public void WithDoubleArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--double-array", "4", "8", "15", "16", "28", "42", "--double", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["DoubleArray"]
                .Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
        }

        [Fact]
        public void WithNullableDoubleArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--nullable-double-array", "4", "", "15", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["NullableDoubleArray"]
                .Value.ShouldBe(new[] { 4, (double?)null, 15 });
        }

        [Fact]
        public void WithEnumArray()
        {
            // When
            var result = this.Subject.GetAction("execute", "--enum-array", "One", "Three", "Two", "--int32", "42");

            // Then
            result.SharedParameters
                .ToDictionary(cp => cp.Name)["EnumArray"]
                .Value.ShouldBe(new[] { Numbers.One, Numbers.Three, Numbers.Two});
        }
    }
}