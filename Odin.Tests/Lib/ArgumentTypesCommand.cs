using Odin.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Tests.Lib
{
    public enum Numbers
    {
        One,
        Two,
        Three
    };

    public class ArgumentTypesCommand : Command
    {
        [Action]
        public void WithBoolean(bool input)
        {
            
        }

        [Action]
        public void WithNullableBoolean(bool? input)
        {

        }

        [Action]
        public void WithInt32(int input)
        {
        }

        [Action]
        public void WithNullableInt32(int? input)
        {
        }

        [Action]
        public void WithInt64(long input)
        {
        }

        [Action]
        public void WithNullableInt64(long? input)
        {
        }

        [Action]
        public void WithDouble(double input)
        {
        }
        [Action]
        public void WithNullableDouble(double? input)
        {
        }

        [Action]
        public void WithDecimal(double input)
        {
        }
        [Action]
        public void WithNullableDecimal(decimal? input)
        {
        }
        [Action]
        public void WithEnum(Numbers input)
        {   
        }
        [Action]
        public void WithNullableEnum(Numbers? input)
        {
        }

        [Action]
        public void WithDateTime(DateTime input)
        {
        }

        [Action]
        public void WithNullableDateTime(DateTime? input)
        {
        }
    }
}
