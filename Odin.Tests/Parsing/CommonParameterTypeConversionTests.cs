using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Odin.Tests.Parsing
{
    [TestFixture]
    public class CommonParameterTypeConversionTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Logger = new StringBuilderLogger();
            this.Subject = new CommonParameterTypesCommand()
                .Use(this.Logger)
                ;
        }

        public Command Subject { get; set; }

        public StringBuilderLogger Logger { get; set; }

        [Test]
        public void WithBoolean()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--boolean", "true");

            // Then
            result.ShouldNotBe(null);
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(true);
        }

        [Test]
        public void WithNegatedBoolean()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--no-boolean");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(false);
        }


        [Test]
        public void WithBoolean_False()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--boolean", "false");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(false);
        }

        [Test]
        public void WithBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--boolean");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Boolean"]
                .Value.ShouldBe(true);
        }


        [Test]
        public void WithInt32()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--int32", "37");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Int32"]
                .Value.ShouldBe(37);
        }

        [Test]
        public void WithInt64()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--int64", "37");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Int64"]
                .Value.ShouldBe(37);
        }

        [Test]
        public void WithDouble()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--double", "37.45");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Double"]
                .Value.ShouldBe(37.45);
        }

        [Test]
        public void WithDecimal()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--decimal", "37.45");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Decimal"]
                .Value.ShouldBe(37.45m);
        }

        [Test]
        public void WithEnum()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--enum", "Two");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Enum"]
                .Value.ShouldBe(Numbers.Two);
        }

        [Test]
        public void WithDateTime()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--date-time", "07/29/1975");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["DateTime"]
                .Value.ShouldBe(new DateTime(1975, 07, 29));
        }

        [Test]
        public void WithNullableBoolean()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-boolean", "true");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableBoolean"]
                .Value.ShouldBe(true);
        }

        [Test]
        public void WithNullableBoolean_ArgumentIdentifierOnly()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-boolean");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableBoolean"]
                .Value.ShouldBe(true);
        }

        [Test]
        public void WithNullableBoolean_False()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-boolean", "false");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableBoolean"]
                .Value.ShouldBe(false);
        }


        [Test]
        public void WithNullableInt32()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-int32", "37");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableInt32"]
                .Value.ShouldBe(37);
        }



        [Test]
        public void WithNullableInt64()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-int64", "37");

            // Then
            result.ShouldNotBe(null);
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableInt64"]
                .Value.ShouldBe(37);
        }

        [Test]
        public void WithNullableDouble()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-double", "37.45");

            // Then
            result.ShouldNotBe(null);
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableDouble"]
                .Value.ShouldBe(37.45);
        }


        [Test]
        public void WithNullableDecimal()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-decimal", "37.45");

            // Then
            result.ShouldNotBe(null);
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableDecimal"]
                .Value.ShouldBe(37.45);
        }


        [Test]
        public void WithNullableEnum()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-enum", "Two");

            // Then
            result.ShouldNotBe(null);
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableEnum"]
                .Value.ShouldBe(Numbers.Two);
        }


        [Test]
        public void WithNullableDateTime()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-date-time", "07/29/1975");

            // Then
            result.ShouldNotBe(null);
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableDateTime"]
                .Value.ShouldBe(new DateTime(1975, 07, 29));
        }


        [Test]
        public void WithBooleanCustomParser()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--custom-parser", "yes");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["CustomParser"]
                .Value.ShouldBe(true);
        }

        [Test]
        public void WithBoolean_FalseCustomParser()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--custom-parser", "no");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["CustomParser"]
                .Value.ShouldBe(false);
        }


        [Test]
        public void WithStringArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute" ,"--string-array", "file1", "file2", "file3", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["StringArray"]
                .Value.ShouldBe(new [] {"file1", "file2", "file3"});
        }

        [Test]
        public void WithBoolArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--bool-array", "true", "false", "true", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["BoolArray"]
                .Value.ShouldBe(new[] { true, false, true });
        }

        [Test]
        public void WithNullableBoolArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-bool-array", "true", "", "false", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableBoolArray"]
                .Value.ShouldBe(new[] { true, (bool?)null, false});
        }

        [Test]
        public void WithInt32Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--int32-array", "4", "8", "15", "16","28","42", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Int32Array"]
                .Value.ShouldBe(new[] { 4,8,15,16,28,42 });
        }

        [Test]
        public void WithNullableInt32Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-int32-array", "4", "", "15", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableInt32Array"]
                .Value.ShouldBe(new[] { 4, (int?)null, 15});
        }

        [Test]
        public void WithInt64Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--int64-array", "4", "8", "15", "16", "28", "42", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["Int64Array"]
                .Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
        }

        [Test]
        public void WithNullableInt64Array()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-int64-array", "4", "", "15", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableInt64Array"]
                .Value
                .ShouldBe(new[] { 4, (long?)null, 15 });
        }

        [Test]
        public void WithDateTimeArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--date-time-array", "01/01/2015", "12/25/2015", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["DateTimeArray"]
                .Value.ShouldBe(new[] { new DateTime(2015,1,1), new DateTime(2015,12,25),  });
        }

        [Test]
        public void WithNullableDateTimeArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-date-time-array", "01/01/2015", "", "12/25/2015", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableDateTimeArray"]
                .Value.ShouldBe(new[] { new DateTime(2015, 1, 1), (DateTime?)null, new DateTime(2015, 12, 25), });
        }


        [Test]
        public void WithDecimalArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--decimal-array", "4", "8", "15", "16", "28", "42", "--int32", "42");

            // Then
            result.CommonParameters
               .ToDictionary(cp => cp.Name)["DecimalArray"]
               .Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
        }

        [Test]
        public void WithNullableDecimalArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-decimal-array", "4", "", "15", "--int32", "42");

            // Then
            result.CommonParameters
               .ToDictionary(cp => cp.Name)["NullableDecimalArray"]
               .Value.ShouldBe(new[] { 4, (decimal?)null, 15 });
        }

        [Test]
        public void WithDoubleArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--double-array", "4", "8", "15", "16", "28", "42", "--double", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["DoubleArray"]
                .Value.ShouldBe(new[] { 4, 8, 15, 16, 28, 42 });
        }

        [Test]
        public void WithNullableDoubleArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--nullable-double-array", "4", "", "15", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["NullableDoubleArray"]
                .Value.ShouldBe(new[] { 4, (double?)null, 15 });
        }

        [Test]
        public void WithEnumArray()
        {
            // When
            var result = this.Subject.GenerateInvocation("execute", "--enum-array", "One", "Three", "Two", "--int32", "42");

            // Then
            result.CommonParameters
                .ToDictionary(cp => cp.Name)["EnumArray"]
                .Value.ShouldBe(new[] { Numbers.One, Numbers.Three, Numbers.Two});
        }
    }
}