MEFBuild
=========

MEFBuild is a lightweight framework for implementing complex, multi-step build processes using .NET code.
MEFBuild mimics MSBuild and relies on the Microsoft Extensibility Framework to make builds loosely-coupled and extensible.
      
Getting Started
---------------

* Install the [MefBuild NuGet package](https://www.nuget.org/packages/MefBuild).
* Define a class that inherits from *Command* and mark it with MEF's *Export* attribute.

```C#
using System;
using System.Composition;
using MefBuild;

[Export]
public class HelloWorld : Command
{
    public override void Execute()
    {
        Console.WriteLine("Hello, World!");
    }
}
```

* Create an *Engine* instance with a MEF *CompositionContext* that contains your command and call the *Execute* method with its type.

```C#
using System.Composition;
using System.Composition.Hosting;
using MefBuild;

class Program
{
    static void Main(string[] args)
    {
        CompositionContext context = new ContainerConfiguration()
            .WithAssembly(typeof(Program).Assembly)
            .CreateContainer();
        var engine = new Engine(context);
        engine.Execute<HelloWorld>();
    }
}
``` 

Defining Build Structure
------------------------

MEFBuild includes *Command*, an abstract base class for implementing the concrete types that represent individual steps in your build process. 
Commands are similar to MSBuild tasks in this regard. For an old-fashioned C/C++ build process that has separate steps for compiler and linker, 
you might have the following two commands defined.


```C#
[Export]
public class Compile : Command
{
    public override void Execute()
    {
        Console.WriteLine("Compiling...");
    }
}

[Export]
public class Link : Command
{
    public override void Execute()
    {
        Console.WriteLine("Linking...");
    }
}
```

To define correct order for executing these commands, you can define a Build command and list Compile and Link in its *DependsOn* attribute. 
This is similar to defining MSBuild targets with DependsOnTargets attribute. In this regard, MEFBuild commands are similar to 
MSBuild targets too.

```C#
[Export, DependsOn(typeof(Compile), typeof(Link))]
public class Build : Command
{
    public override void Execute()
    {
        Console.WriteLine("Done.");
    }
}
```

With these definitions, executing the *Build* command produces the following output.

> Compiling...
> Linking...
> Done.