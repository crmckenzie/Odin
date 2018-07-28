using Odin.Attributes;
using System;
using Odin;

namespace SimpleBooks
{
    public class BooksCommand : Command
    {
        [Action]
        public void Add(string title, string author, DateTime releaseDate)
        {
            Logger.Info("Adding '{0}' by '{1}' - released on '{2:MM/dd/yyyy}' to the index.\n", title, author, releaseDate);
        }

        [Action]
        public void Delete(string title, string author)
        {
            Logger.Info("Remvoing '{0}' by '{1}' from the index.\n", title, author);
        }
    }
}
