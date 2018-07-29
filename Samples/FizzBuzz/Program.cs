using System;

namespace FizzBuzz
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new FizzBuzzCommand();
            var result = root.Execute(args);
            Environment.Exit(result);
        }
    }

}
