using Odin.Logging;

namespace Odin.Demo
{
    public static class FizzBuzzGame
    {
        public static void Play(ILogger logger, int input)
        {
            if (input % 3 == 0 && input % 5 == 0)
            {
                logger.Info("FizzBuzz");
            }
            else if (input % 3 == 0)
            {
                logger.Info("Fizz");
            }
            else if (input % 5 == 0)
            {
                logger.Info("Buzz");
            }
            else
            {
                logger.Info(input.ToString());
            }
            logger.Info("\n");
        }
    }
}