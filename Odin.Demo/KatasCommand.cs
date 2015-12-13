using System.ComponentModel;
using System.Linq;
using Odin.Attributes;

namespace Odin.Demo
{
    [Description("Provides some katas.")]
    public class KatasCommand : Command
    {
        [Action(IsDefault = true)]
        public int FizzBuzz(
            [Alias("-i")]
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
        public int PrimeFactors(int input)
        {
            var result = PrimeFactorGenerator.Generate(input);
            var output = string.Join(" ", result.Select(row => row.ToString()));
            this.Logger.Info($"{output}\n");
            return 0;
        }
    }
}