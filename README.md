# Odin

[![Join the chat at https://gitter.im/crmckenzie/Odin](https://badges.gitter.im/crmckenzie/Odin.svg)](https://gitter.im/crmckenzie/Odin?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/4s75vka56cos0h87/branch/master?svg=true)](https://ci.appveyor.com/project/crmckenzie/odin/branch/master)

In the .NET space there are a number of good libraries to handle run-of-the-mill command line argument parsing.
My current favorite is a nuget package called simply [CommandLineParser].
So why write a new one?

## Try it out!

```powershell
Install-Package Odin-Commands -Pre
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

## Goals

* Given `program command subcommand action --arg1 value1 --arg2 value2`
* Odin should resolve to the correct method invocation with as little wiring as possible.
    * The above example should be interpreted as:
        * find a component called `command`
        * within `command` find a component or action called `subcommand`
        * within `subcommand` find a subcommand or action called `action`
        * once a method is located, coerce the arguments to that method to the correct types and invoke the method
    * Failure to resolve a command should result in context-sensitive help
* Odin commands should support a default action.
* Odin should support custom conventions for how arguments should be formatted and routed.
  * Provide some built-in conventions for common argument styles
  * e.g. -a, --argument, --compound-argument, /argument, /argument:value
  * conventions should be overridable via attributes+
* Odin should provide auto-generated help.
  * Help should be context sensitive
  * Help format should be overridable
* Odin should communicate to the Console via an overridable Logger.
* Odin should not impose overall program structure on developers
* Odin should interoperate with arbitrary Dependency Injection frameworks.
* Odin should report its interpretation of routing in an externally testable manner.
* Odin should automatically parse arguments into common framework types
  * Odin should allow overrideble parsing algorithms.

### Show Me The Code

#### Setup Code

```csharp

public class RootCommand : Command
{
    public RootCommand() : this(new KatasCommand())
    {
    }

    public RootCommand(KatasCommand katas)
    {
        base.RegisterSubCommand(katas);
    }

    [Action]
    [Description("The proverbial 'hello world' application.")]
    public int Hello(
        [Description("Override who to say hello to. Defaults to 'World'.")]
        [Alias("w")]
        string who = "World")
    {
        this.Logger.Info($"Hello {who}!\n");
        return 0;
    }

    [Action]
    [Description("Display the current time")]
    public void Time(
        [Description("The format of the time. (default) hh:mm:ss tt")]
        [Alias("f")]
        string format = "hh:mm:ss tt")
    {
        this.Logger.Info($"The time is {DateTime.Now.ToLocalTime():format}\n");
    }
}

[Description("Provides some katas.")]
public class KatasCommand : Command
{
    [Action(IsDefault = true)]
    public int FizzBuzz(
        [Alias("i")]
        int input
        )
    {
        FizzBuzzGame.Play(this.Logger, input);
        return 0;
    }

    [Action]
    public int PrimeFactors(
      [Alias("i")]
      int input
      )
    {
        var result = PrimeFactorGenerator.Generate(input);
        var output = string.Join(" ", result.Select(row => row.ToString()));
        this.Logger.Info($"{output}\n");
        return 0;
    }
}


```

#### The Program

```csharp
class Program
{
    static void Main(string[] args)
    {
        var root = new RootCommand(new KatasCommand());
        var result = root.Execute(args);
        Environment.Exit(result);
    }
}
```

### What Do I Get For My Trouble?

You get a command line executable that can be invoked like so:

```

exe hello --who "world"                 # explicit invocation
exe hello -w "world"                    # argument alias
exe hello "world"                       # implicit argument by order

exe katas fizz-buzz --input 11          # explicit subcommand invocation
exe katas --input 11                    # subcommand + default action
exe katas -i 11                         # subcommand + default action + argument alias
exe katas 11                            # subcommand + default action + implicit argument by order
exe katas prime-factors --input 27      # subcommand + non-default action + explicit argument
```

[CommandLineParser]:https://www.nuget.org/packages/CommandLineParser
[thor]:http://whatisthor.com