---
layout: post
title:  "Announcing Odin-Commands"
date:   2016-01-01 21:16:08 -0800
categories: .NET, CLI
---
In the .NET space there are a number of good libraries to handle run-of-the-mill command line argument parsing.
My current favorite is a nuget package called simply [CommandLineParser].
So why write a new one?

In short, current .NET command-line libraries focus only on parsing the args.
You are left reponsible for interpreting the args to call the right methods with the correct arguments.
What if CLI's could be as easy to structure and execute as ASP .NET MVC applications?

## Try it out!

```powershell
Install-Package Odin-Commands -Pre
```

Then read [Getting Started](https://github.com/crmckenzie/Odin/wiki/Getting-Started)

Read the [wiki](https://github.com/crmckenzie/Odin/wiki).

## Inspired By Thor

I've done some work with Ruby over the last couple of years and I was really impressed with the feature set offered by a ruby project called [thor]
In addition to a declarative approach to binding options to actions and arguments, thor supports the concept of subcommands.
We've seen subcommands used to great effect in command-line programs such as `git` and `nuget`, but current command line parser packages
offer little help with this feature.

## Inspired by Convention Over Configuration

In ASP .NET MVC, urls are routed to the appropriate controller and action by convention. `http://mysite.domain/Home/Index` is understood to route to a controller called "Home" and invoke a method called "Index."
In addition, little manual wiring is required because ASP .NET MVC can discover and instantiate the controller easily at runtime.
I wondered if it would be possible to use a combination of reflection and convention to create a command-line application in C#.

### Show Me The Code

#### Setup Code

Write a class that inherits `Command`. You can also register SubCommands.
Add methods with `[Action]` attributes to indicate that they are executable.

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

To execute the program, just take call `Execute(args)`.

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
