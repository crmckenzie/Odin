using System.Collections.Generic;

namespace Odin.Tests.Samples.Demo
{
    public class PrimeFactorGenerator
    {
        public static IList<int> Generate(int generateFor)
        {
            IList<int> primes = new List<int>();

            for (int candidate = 2; generateFor > 1; candidate++)

                for (; generateFor % candidate == 0; generateFor /= candidate)

                    primes.Add(candidate);

            return primes;
        }
    }
}