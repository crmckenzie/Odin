using System;
using System.ComponentModel;
using Odin.Attributes;

namespace Odin.Demo
{
    public class BooksCommand : Command
    {
        public BooksCommand()
        {
            base.RegisterSubCommand(new SearchCommand());
        }

        [Parameter]
        [Alias("t")]
        [Description("The title of the book being added.")]
        public string Title { get; set; }

        [Parameter]
        [Alias("a")]
        [Description("The author of the book being added.")]
        public string Author { get; set; }

        [Action]
        [Description("Adds a book to the index.")]
        public void Add(
            [Alias("r")]
            [Description("The date the book was released.")]
            DateTime releaseDate)
        {   
            Logger.Info("Adding '{0}' by '{1}' - released on '{2:MM/dd/yyyy}' to the index.\n", Title, Author, releaseDate);
        }

        [Action]
        [Description("Removes a book from the index.")]
        public void Delete()
        {
            Logger.Info("Remvoing '{0}' by '{1}' from the index.\n", Title, Author);
        }
    }
}