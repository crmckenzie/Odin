---
layout: post
title:  "Before And After Execute Events"
date:   2016-03-01
categories: .NET, CLI
---

Today I released Odin-Commands 0.2.1 on [nuget.org]. 

# What's New?

I was writing a CLI command when I released it would be nice to be able to set default values for the `Common Parameters` on the command prior to executing the action. 
The difficulty is that some of the default parameter values are composed from other parameter values but all of them are settable by the user.
To achieve this goal I added overridable `OnBeforeExecute` and `OnAfterExecute` methods to the `Command` class.

# How do I use it?

```csharp

public class MyCommand: Command
{

  protected override void OnBeforeExecute(MethodInvocation invocation)
  {
     ApplyDefaultValues(); // or do other stuff prior to executing the invocation.
  }

  protected override int OnAfterExecute(MethodInvocation invocation, int result)
  {
    // you can return a different exit code if you need to.
    return base.OnAfterExecute(invocation, result);
  }

}

```

[nuget.org]: http://nuget.org

 