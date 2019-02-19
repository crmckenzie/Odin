using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new BooksCommand();
            var result = root.Execute(args);
            Environment.Exit(result);
        }
    }
}
