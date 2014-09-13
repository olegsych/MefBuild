MEF Build
=========

MEF Build is a lightweight build framework based on the Microsoft Extensibility Framework.
      
Getting Started
---------------

* Install the [MefBuild NuGet package](https://www.nuget.org/packages/MefBuild).
* Define a class that inherits from Command and mark it with MEF's Export attribute.

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

* Create an Engine instance with a MEF CompositionContext that contains your command and call the Execute method with its type.

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
