using System;
using System.Diagnostics;
using Odin.Attributes;
using Odin.Tests.Lib;

namespace Odin.Tests.Parsing
{
    public enum Numbers
    {
        One,
        Two,
        Three
    };

    public class ArgumentTypesCommand : Command
    {


        private void LogMethodCall(params object[] arguments)
        {
            var stackTrace = new StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            this.MethodCalled = methodBase.Name;
            this.MethodArguments = arguments;
        }

        public object[] MethodArguments { get; set; }

        public string MethodCalled { get; set; }

        [Action]
        public void WithBoolean(bool input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithNullableBoolean(bool? input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithInt32(int input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithNullableInt32(int? input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithInt64(long input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithNullableInt64(long? input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithDouble(double input)
        {
            LogMethodCall(input);
        }
        [Action]
        public void WithNullableDouble(double? input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithDecimal(double input)
        {
            LogMethodCall(input);
        }
        [Action]
        public void WithNullableDecimal(decimal? input)
        {
            LogMethodCall(input);
        }
        [Action]
        public void WithEnum(Numbers input)
        {
            LogMethodCall(input);
        }
        [Action]
        public void WithNullableEnum(Numbers? input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithDateTime(DateTime input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithNullableDateTime(DateTime? input)
        {
            LogMethodCall(input);
        }

        [Action]
        public void WithBooleanYesNoParser(
            [Alias("i")]
            [Parser(typeof(YesNoParser))] bool input, 
            int input2, string input3)
        {

            LogMethodCall(input, input2, input3);

        }

        [Action]
        public void WithStringArray(string[] fileNames, int someOtherInput)
        {
            LogMethodCall(fileNames, someOtherInput);

        }

        [Action]
        public void WithBoolArray(bool[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithNullableBoolArray(bool?[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithInt32Array(int[] numbers, int someOtherInput)
        {
            LogMethodCall(numbers, someOtherInput);
        }

        [Action]
        public void WithNullableInt32Array(int?[] numbers, int someOtherInput)
        {
            LogMethodCall(numbers, someOtherInput);

        }

        [Action]
        public void WithInt64Array(long[] numbers, int someOtherInput)
        {
            LogMethodCall(numbers, someOtherInput);
        }

        [Action]
        public void WithNullableInt64Array(long?[] numbers, int someOtherInput)
        {
            LogMethodCall(numbers, someOtherInput);

        }

        [Action]
        public void WithDateTimeArray(DateTime[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithNullableDateTimeArray(DateTime?[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithDecimalArray(decimal[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithNullableDecimalArray(decimal?[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithDoubleArray(double[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithNullableDoubleArray(double?[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }

        [Action]
        public void WithEnumArray(Numbers[] values, int someOtherInput)
        {
            LogMethodCall(values, someOtherInput);

        }


    }
}
