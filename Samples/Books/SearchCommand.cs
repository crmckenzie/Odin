using System;
using System.ComponentModel;
using Odin;
using Odin.Attributes;

namespace Books
{
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
}