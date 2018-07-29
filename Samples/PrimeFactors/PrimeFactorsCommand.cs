using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Odin;
using Odin.Attributes;

namespace PrimeFactors
{
    class PrimeFactorsCommand : Command
    {
        [Action(IsDefault=true)]
        [Description("Outputs the list of prime factors.")]
        public void Generate(
            [Alias("t")]
            [Description("The target for which to calculate prime factors.")]
            int target = 1000)
        {
            IList<int> primes = new List<int>();

            for (var candidate = 2; target > 1; candidate++)
            {
                for (; target % candidate == 0; target /= candidate)
                    primes.Add(candidate);

            }

            var primesAsStrings= primes.Select(p => p.ToString()).ToArray();
            var output = string.Join(", ", primesAsStrings);
            Logger.Info(output);
        }
    }
}