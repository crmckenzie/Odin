using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzbuzz;

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
