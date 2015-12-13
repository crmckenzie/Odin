# Odin

[![Build status](https://ci.appveyor.com/api/projects/status/4s75vka56cos0h87/branch/master?svg=true)](https://ci.appveyor.com/project/crmckenzie/odin/branch/master)

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

## Back of the Paper Napkin Usage Design

```csharp
[DefaultAction("DoSomething")]
[Description("This is the default controller")]
public class DefaultController : Controller
{
    [Action]
    [Description("A description of the DoSomething() method.")]
    public void DoSomething(
        [Description("Lorem ipsum dolor sit amet, consectetur adipiscing elit")]
        string argument1 = "value1-not-passed", 
        [Description("sed do eiusmod tempor incididunt ut labore et dolore magna aliqua")]
        string argument2 = "value2-not-passed", 
        [Description("Ut enim ad minim veniam")]
        string argument3 = "value3-not-passed")
    {
        // do something here
    }
}
```

[CommandLineParser]:https://www.nuget.org/packages/CommandLineParser
[thor]:http://whatisthor.com