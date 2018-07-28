# Odin

[![Build status](https://ci.appveyor.com/api/projects/status/4s75vka56cos0h87/branch/master?svg=true)](https://ci.appveyor.com/project/crmckenzie/odin/branch/master)  [![Gitter](https://badges.gitter.im/crmckenzie/Odin.svg)](https://gitter.im/crmckenzie/Odin?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

In the .NET space there are a number of good libraries to handle run-of-the-mill command line argument parsing.
My current favorite is a nuget package called simply [CommandLineParser].
So why write a new one?

## Try it out!

```powershell
Install-Package Odin-Commands
```

## Inspired By Thor

I've done some work with Ruby over the last couple of years and I was really impressed with the feature set offered by a ruby project called [thor]
In addition to a declarative approach to binding options to actions and arguments, thor supports the concept of subcommands.
We've seen subcommands used to great effect in command-line programs such as `git` and `nuget`, but current command line parser packages
offer little help with this feature.

## Inspired by Convention Over Configuration

In ASP .NET MVC, urls are routed to the appropriate controller and action by convention. `http://mysite.domain/Home/Index` is understood to route to a controller called "Home" and invoke a method called "Index."
In addition, little manual wiring is required because ASP .NET MVC can discover and instantiate the controller easily at runtime.
I wondered if it would be possible to use a combination of reflection and convention to create a command-line application in C#.

## A Simple Example

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

### Invocations

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



### Help Text

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

### Show Me The Code



#### Setup Code

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

    public enum SortBooksBy
    {
        Author,
        Title,
        ReleaseDate
    }


```

#### The Program

```csharp
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

### What Do I Get For My Trouble?

You get a command line executable that can be invoked like so:

```

exe add --author "Ayn Rand" --title "Atlas Shrugged" --release-date  1957/10/10   # explicit invocation
exe add -a "Ayn Rand" -t "Atlas Shrugged" -r 1957/10/10                           # argument alias

exe search title --title "Atlas Shrugged" --sort-by author                        # explicit subcommand invocation
exe search --title "Atlas Shrugged" --sort-by author                              # subcommand + default action
exe search -t "Atlas Shrugged"  --sort-by author                                  # subcommand + default action + argument alias
exe search "Atlas Shrugged"                                                       # subcommand + default action + implicit argument by order
exe search author --author "Ayn Rand" --sort-by author                            # subcommand + non-default action + explicit argument
```

And help output that looks like:

```
ACTIONS
add                     Adds a book to the index.

    --release-date      aliases: -r
                        The date the book was released.

delete                  Removes a book from the index.

help

    --action-name       default value:
                        The name of the action to provide help for.


COMMON PARAMETERS
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

[CommandLineParser]:https://www.nuget.org/packages/CommandLineParser
[thor]:http://whatisthor.com
