using Odin.Logging;

namespace Odin.Demo
{
    public static class FizzBuzzGame
    {
        public static void Play(Logger logger, int input)
        {
            if (input % 3 == 0 && input % 5 == 0)
            {
                Logger.Info("FizzBuzz");
            }
            else if (input % 3 == 0)
            {
                Logger.Info("Fizz");
            }
            else if (input % 5 == 0)
            {
                Logger.Info("Buzz");
            }
            else
            {
                Logger.Info(input.ToString());
            }
            Logger.Info("\n");
        }
    }
}