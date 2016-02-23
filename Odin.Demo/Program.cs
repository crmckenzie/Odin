using Odin.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    [Description("Provides search capabilities for books.")]
    public class SearchCommand : Command
    {
        [Parameter]
        [Alias("s")]
        [Description("The order to sort the results of the search.")]
        public SortBooksBy SortBy { get; set; }

        [Action]
        [Description("Searches books by author.")]
        public void Author(
            [Alias("a")]
            [Description("The author of the book being searched for.")]
            string author)
        {
            Logger.Info("Find books by author '{0}'; sort results by '{1}'.\n", author, SortBy);
        }

        [Action(IsDefault = true)]
        [Description("Searches books by title.")]
        public void Title(
            [Alias("t")]
            [Description("The title of the book being searched for.")]
            string title)
        {
            Logger.Info("Find books by title '{0}'; sort results by '{1}'.\n", title, SortBy);
        }

        [Action]
        [Description("Searches books by release date.")]
        public void ReleaseDate(
            [Alias("f")]
            [Description("The release date from which to search.")]
            DateTime? from = null, 
            
            [Alias("t")]
            [Description("The release date to which to search.")]
            DateTime? to = null)
        {
            Logger.Info("Find books by release date: '{0}' - '{1}'; sort results by '{2}'.\n", from, to, SortBy);
        }
    }

    public enum SortBooksBy
    {
        Author,
        Title,
        ReleaseDate
    }
}
