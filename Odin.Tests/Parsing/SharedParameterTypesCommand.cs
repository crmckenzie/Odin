using System;
using Odin.Attributes;

namespace Odin.Tests.Parsing
{
    public class SharedParameterTypesCommand : Command
    {
        [Parameter]
        public bool Boolean { get; set; }

        [Parameter]
        public bool? NullableBoolean { get; set; }

        [Parameter]
        public int Int32 { get; set; }

        [Parameter]
        public int? NullableInt32 { get; set; }

        [Parameter]
        public long Int64 { get; set; }

        [Parameter]
        public long? NullableInt64 { get; set; }


        [Parameter]
        public double Double { get; set; }

        [Parameter]
        public double? NullableDouble { get; set; }


        [Parameter]
        public decimal Decimal { get; set; }

        [Parameter]
        public decimal? NullableDecimal { get; set; }


        [Parameter]
        public Numbers Enum { get; set; }

        [Parameter]
        public Numbers? NullableEnum { get; set; }


        [Parameter]
        public DateTime DateTime { get; set; }

        [Parameter]
        public DateTime? NullableDateTime { get; set; }


        [Parameter]
        [Parser(typeof(YesNoParser))]
        public bool CustomParser { get; set; }

        
        [Parameter]
        public string[] StringArray { get; set; }

        
        [Parameter]
        public bool[] BoolArray { get; set; }

        [Parameter]
        public bool?[] NullableBoolArray { get; set; }

        [Parameter]
        public int[] Int32Array { get; set; }

        [Parameter]
        public int?[] NullableInt32Array { get; set; }

        [Parameter]
        public long[] Int64Array { get; set; }

        [Parameter]
        public long?[] NullableInt64Array { get; set; }


        [Parameter]
        public DateTime[] DateTimeArray { get; set; }

        [Parameter]
        public DateTime?[] NullableDateTimeArray { get; set; }



        [Parameter]
        public decimal[] DecimalArray { get; set; }

        [Parameter]
        public decimal?[] NullableDecimalArray { get; set; }



        [Parameter]
        public double[] DoubleArray { get; set; }

        [Parameter]
        public double?[] NullableDoubleArray { get; set; }


        [Parameter]
        public Numbers[] EnumArray { get; set; }


        [Action]
        public void Execute()
        {
            
        }
    }
}