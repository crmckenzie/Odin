---
layout: post
title:  "Common Parameters"
date:   2016-02-23
categories: .NET, CLI
---

## What are Common Parameters?

Common parameters are parameters that are repeated across multiple actions in a CLI context. For example, I might have a CLI that has takes a `--verbose` flag to switch on verbose output for all of my actions. Rather than require that the CLI developer add a `bool verbose` switch to every action in his/her `Command` implementation, Odin allows you declare a property on the `Command` implementation as a `[Parameter]`.

## Example

```csharp
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
```

In the above example, `SortBy` is available to all of the actions defined on the `Command`. It can be set at the command-line by passing the `--sort-by <value>` switch. Odin will parse and set the switch before executing the action.

This functionality is now available in version 0.2 of the Odin-Commands nuget package.
