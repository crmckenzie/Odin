# Odin

[![Build status](https://ci.appveyor.com/api/projects/status/4s75vka56cos0h87/branch/master?svg=true)](https://ci.appveyor.com/project/crmckenzie/odin/branch/master)  [![Gitter](https://badges.gitter.im/crmckenzie/Odin.svg)](https://gitter.im/crmckenzie/Odin?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

In the .NET space there are a number of good libraries to handle run-of-the-mill command line argument parsing.
My current favorite is a nuget package called simply [CommandLineParser].
So why write a new one?

## Inspired By Thor

I've done some work with Ruby over the last couple of years and I was really impressed with the feature set offered by a ruby project called [thor]
In addition to a declarative approach to binding options to actions and arguments, thor supports the concept of subcommands.
We've seen subcommands used to great effect in command-line programs such as `git` and `nuget`, but current command line parser packages
offer little help with this feature.

## Inspired by Convention Over Configuration

In ASP .NET MVC, urls are routed to the appropriate controller and action by convention. `http://mysite.domain/Home/Index` is understood to route to a controller called "Home" and invoke a method called "Index."
In addition, little manual wiring is required because ASP .NET MVC can discover and instantiate the controller easily at runtime.
I wondered if it would be possible to use a combination of reflection and convention to create a command-line application in C#.

## Try it out!

```powershell
Install-Package Odin-Commands
```

## Contents

* [Features](#features)
  * [Aliases](#aliases)
  * [Descriptions](#descriptions)
  * [Default Parameter Values](#default-parameter-values)
  * [Shared Parameters](#shared-parameters)
  * [Sub Commands](#sub-commands)
  * [Custom Conventions](#custom-conventions)
  * [Custom Help Writer](#custom-help-writer)
  * [Custom Logger](#custom-logger)
* [Samples](#samples)
  * [A Simple Example: FizzBuzz](#a-simple-example-fizzbuzz)
  * [A More Sophisticated Example: Prime Factors](#A-More-Sophisticated-Example-Prime-Factors)
  * [A Sophisticated Example: Books](#A-Sophisticated-Example-Books)

## Features

### Aliases

Odin supports aliases for parameters by means of the `[Alias]` attribute.

```csharp
    [Action(IsDefault=true)]
    public void Generate(
        [Alias("t")]
        int target)
    {
        // do stuff
    }
```

### Descriptions

Odin supports extending the default help text by through the application of the `[Description]` attribute
to both parameters and actions.

```csharp
    [Action(IsDefault=true)]
    [Description("Outputs the list of prime factors.")]
    public void Generate(
        [Alias("t")]
        [Description("The target for which to calculate prime factors.")]
        int target)
    {
        // do stuff
    }
```

### Default Parameter Values

Odin supports default values for parameters. If an explicit value for a parameter is not supplied in the
command-line arguments, Odin will use the default value instead.

```csharp
    [Action(IsDefault=true)]
    [Description("Outputs the list of prime factors.")]
    public void Generate(
        [Alias("t")]
        [Description("The target for which to calculate prime factors.")]
        int target = 1000)
    {
        // do stuff
    }
```

### Shared Parameters

Odin supports sharing parameters over several actions in the same command.
Simply declare a property at the class level and decorate it with the `[Parameter]` attribute.

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
    // snip
}
```

### Sub Commands

Odin supports more complex command structures via sub commands.
Simply register the sub command with the parent command and Odin will interpret the arguments appropriately.

```csharp
public class BooksCommand : Command
{
    public BooksCommand()
    {
        base.RegisterSubCommand(new SearchCommand());
    }
    // snip
}
```

### Custom Conventions

Odin uses the `KebabCase` convention by default. 
It also ships with `SlashColon` and `SlashEquals` conventions.

The `SlashColon` convention expectes arguments to be formatted as follows:

```cmd
cli.exe action /argument:value
```

The `SlashEquals` convention expects arguments to be formatted as follows:

```cmd
cle.exe action /argument=value
```

You can implement your own custom conventions by implementing `IConvention`, though if you do
please consider submitting them back to the project. Once you've implemented your own convention
you must register it with the root command in your command tree.

```csharp
    var command = new RootCommand().Use(new MyCustomConvention());
```

### Custom Help Writer

If you would like to control the help documentation that Odin generates automatically, 
you can create your own help text by implementing `IHelpWriter`. 
Once you've  written your own custom help writer you must register it with the root command of your command tree.

```csharp
    var command = new RootCommand().Use(new MyCustomHelpWriter());
```

### Custom Logger

If you would like to control the logging facilitites that Odin provides,
you must implement the `ILogger` interface and register your implemention with the
root command of your command tree.

```csharp
    var command = new RootCommand().Use(new MyCustomLogger());
```

## Samples

### A Simple Example: FizzBuzz

#### Code
```csharp
public class FizzBuzzCommand : Command
{
    [Action(IsDefault = true)]
    public void Play(int input)
    {
        if (input % 3 == 0 && input % 5 == 0)
        {
            Logger.Info("FizzBuzz");
        }
        else if (input % 3 == 0)
        {
            Logger.Info("Fizz");
        }
        else if (input % 5 == 0)
        {
            Logger.Info("Buzz");
        }
        else
        {
            Logger.Info(input.ToString());
        }
        Logger.Info("\n");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var root = new FizzBuzzCommand();
        var result = root.Execute(args);
        Environment.Exit(result);
    }
}
```

#### Invocations

With the above code, will attempt to map arguments to the appropriate method.
In order to expose a method to the CLI it must be marked with the `Action` attribute.
"play" maps to the `FizzBuzzCommand.Play`.


```cmd
FizzBuzz.exe play --input 5
Buzz
```

It is not necessary to explicitly label the arguments in the command-line invocation.
Odin will interpret arguments as ordered parameters if the parameter names are not specified.

```cmd
FizzBuzz.exe play 5
Buzz
```

If the `Action` is marked as the `Default` action, it will not be necessary to specify the action name.
A `Command` implementation can only have 1 default action.

```cmd
FizzBuzz.exe 5
Buzz
```

#### Help Text

```cmd
default action: play

ACTIONS
help

    --action-name       default value:
                        The name of the action to provide help for.

play*

    --input


To get help for actions
        help <action>

```

### A More Sophisticated Example: Prime Factors

#### Code

In this example we will see the impact of the `[Alias]` and `[Description]` attributes as well as the impact of parameters with default values.

```csharp
class PrimeFactorsCommand : Command
{
    [Action(IsDefault=true)]
    [Description("Outputs the list of prime factors.")]
    public void Generate(
        [Alias("t")]
        [Description("The target for which to calculate prime factors.")]
        int target = 1000)
    {
        IList<int> primes = new List<int>();

        for (var candidate = 2; target > 1; candidate++)
        {
            for (; target % candidate == 0; target /= candidate)
                primes.Add(candidate);

        }

        var primesAsStrings= primes.Select(p => p.ToString()).ToArray();
        var output = string.Join(", ", primesAsStrings);
        Logger.Info(output);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var root = new FizzBuzzCommand();
        var result = root.Execute(args);
        Environment.Exit(result);
    }
}


```

#### Invocations

The `target` parameter has a default value of 1000 and an alias of '-t`. 
The following invocations are equivalent.

```cmd
PrimeFactors.exe generate --target 1000
2, 2, 2, 5, 5, 5
PrimeFactors.exe generate -t 1000
2, 2, 2, 5, 5, 5
PrimeFactors.exe generate
2, 2, 2, 5, 5, 5
PrimeFactors.exe 1000
2, 2, 2, 5, 5, 5
```

#### Help Text

```cmd
default action: generate

ACTIONS
generate*               Outputs the list of prime factors.

    --target            default value: 1000
                        aliases: -t
                        The target for which to calculate prime factors.

help

    --action-name       default value:
                        The name of the action to provide help for.


To get help for actions
        help <action>
```


### A Sophisticated Example: Books

In this example, we will see that Odin supports more complex command structures.
This CLI supports adding and removing books to the library.
It features a subcommand that allows us to search the library and sort the results.

#### Code

```csharp
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

class Program
{
    static void Main(string[] args)
    {
        var root = new BooksCommand();
        var result = root.Execute(args);
        Environment.Exit(result);
    }
}

```

#### Invocations

```cmd

Books.exe add --title "Oathbringer" --author "Branden Sanderson" --release-date 2017-11-24
Adding 'Oathbringer' by 'Branden Sanderson' - released on '11/24/2017' to the index.

Books.exe search "OathBringer"
Find books by title 'OathBringer'; sort results by 'Author'.

Books.exe search author --author "Branden Sanderson"
Find books by author 'Branden Sanderson'; sort results by 'Author'.

Books.exe search author "Branden Sanderson" --sort-by ReleaseDate
Find books by author 'Branden Sanderson'; sort results by 'ReleaseDate'.

```

#### Help Text

Here is the help text for the main `BooksCommand`:

```csharp
ACTIONS
add                     Adds a book to the index.

    --release-date      aliases: -r
                        The date the book was released.

delete                  Removes a book from the index.

help

    --action-name       default value:
                        The name of the action to provide help for.


SHARED PARAMETERS
    --title             default value:
                        aliases: -t
                        The title of the book being added.

    --author            default value:
                        aliases: -a
                        The author of the book being added.


To get help for actions
        help <action>


SUB COMMANDS
search                  Provides search capabilities for books.
To get help for subcommands
        help <subcommand>
```

And here is the help text for the "search" sub command:

```cmd
Provides search capabilities for books.

default action: title

ACTIONS
author                  Searches books by author.

    --author            aliases: -a
                        The author of the book being searched for.

help

    --action-name       default value:
                        The name of the action to provide help for.

release-date            Searches books by release date.

    --from              default value:
                        aliases: -f
                        The release date from which to search.

    --to                default value:
                        aliases: -t
                        The release date to which to search.

title*                  Searches books by title.

    --title             aliases: -t
                        The title of the book being searched for.


SHARED PARAMETERS
    --sort-by           valid values: Author, Title, ReleaseDate
                        default value: Author
                        aliases: -s
                        The order to sort the results of the search.


To get help for actions
        search help <action>

```

[CommandLineParser]:https://www.nuget.org/packages/CommandLineParser
[thor]:http://whatisthor.com
