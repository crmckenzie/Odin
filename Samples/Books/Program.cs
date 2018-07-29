using System;

namespace Books
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
