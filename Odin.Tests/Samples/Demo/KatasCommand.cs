using System.ComponentModel;
using System.Linq;
using Odin.Attributes;

namespace Odin.Tests.Samples.Demo
{
    [Description(@"This command is intended for demonstration purposes. It provides some katas which can be executed as actions.")]
    [Alias("exercise")]
    public class KatasCommand : Command
    {
        [Action(IsDefault = true)]
        [Description("If <input> is a multiple of 3, emit 'Fizz'." +
                     "If it is a multiple of 5, emit 'Buzz'." +
                     "If it is a multiple of 3 and 5, emit 'FizzBuzz'." +
                     "Otherwise, emit <input>.")]
        public int FizzBuzz(
            [Alias("i")]
            int input
            )
        {
            if (input%3 == 0 && input%5 == 0)
            {
                Logger.Info("FizzBuzz");
            }
            else if (input %3 == 0)
            {
                Logger.Info("Fizz");
            }
            else if (input%5 == 0)
            {
                Logger.Info("Buzz");
            }
            else
            {
                Logger.Info(input.ToString());
            }
            Logger.Info("\n");
            return 0;
        }

        [Action]
        [Description("Emit the prime factors of <input>.")]
        public int PrimeFactors(int input)
        {
            var result = PrimeFactorGenerator.Generate(input);
            var output = string.Join(" ", result.Select(row => row.ToString()));
            this.Logger.Info($"{output}\n");
            return 0;
        }
    }
}