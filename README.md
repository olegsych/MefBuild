MefBuild
========

MefBuild is a lightweight library powered by the [Microsoft Extensibility Framework](http://mef.codeplex.com/)
for implementing complex, extensible build systems. MefBuild mimics MSBuild.

Getting Started
---------------

* Install the [MefBuild NuGet package](https://www.nuget.org/packages/MefBuild).
* Define a class that inherits from `Command` and mark it with MEF's `Export` attribute.

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

* Create an `Engine` instance with a MEF `CompositionContext` that contains your command and call the `Execute` method with its type.

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

Defining Build Process
----------------------

`Command` is an abstract base class for implementing the concrete types that represent individual 
steps in your build process. Commands are similar to MSBuild tasks in this regard. For an old-fashioned 
C/C++ build process that has separate steps for compiler and linker, you might have the following two 
commands defined.


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

To execute these commands in correct order, you can define a Build command and list the Compile and 
Link in its `DependsOn` attribute. This is similar to defining MSBuild targets with the DependsOnTargets 
attribute. In this regard, MEFBuild commands are similar to MSBuild targets too.

```C#
[Export, DependsOn(typeof(Compile), typeof(Link))]
public class Build : Command
{
    public override void Execute()
    {
        Console.WriteLine("Build finished.");
    }
}
```

With these definitions, executing the Build command produces the following output.

    Compiling...
    Linking...
    Build finished.

Passing Build Artifacts Between Commands
----------------------------------------

Unlike in MSBuild, where build artifacts (properties and items) are passed between tasks by writing 
XML script, in MefBuild, values and objects are passed between commands via MEF composition context.

A command that produces a value or an object should define a property with the `Export` attribute. 

```C#
[Shared, Export]
public class Compile : Command
{
    [Export("ObjectFile")]
    public string ObjectFile { get; set; }

    public override void Execute()
    {
        this.ObjectFile = "mefbuild.obj";
        Console.WriteLine("Compiling {0} ...", this.ObjectFile);
    }
}
```

**NOTE**: The exporting command must be marked with the `Shared` attribute, because otherwise, MEF 2.0 
will create a new part, the Compile command in this example, for every import.

A command consuming this value or object should define a matching property with the `Import` attribute. 

```C#
[Export]
public class Link : Command
{
    [Import("ObjectFile")]
    public string ObjectFile { get; set; }

    public override void Execute()
    {
        Console.WriteLine("Linking {0} ...", this.ObjectFile);
    }
}
```

**NOTE**: When using general-purpose types, such as `string`, both `Import` and `Export` attributes 
should specify a unique contract name. When the artifact type itself is unique, a contract name is 
not necessary.

With these new definitions of the Compile and Link commands, executing the Build command defined earlier 
produces the following output. 

    Compiling mefbuild.obj ...
    Linking mefbuild.obj ...
    Build finished.

Extending Build Process
-----------------------

MefBuild's `Engine` automatically executes commands marked with the `ExecuteBefore` and `ExecuteAfter` 
attributes. Here is a command you could define to implement pre-processing of C/C++ source files. Because 
pre-processing has to happen before the compilation, this command is marked with the `ExecuteBefore` 
attribute that specifies type of the Compile command.

```C#
[Export, ExecuteBefore(typeof(Compile))]
public class Preprocess : Command
{
    public override void Execute()
    {
        Console.WriteLine("Preprocessing...");
    }
}
```

Here is another command that extends our build system by adding a packaging step after linking.

```C#
[Export, ExecuteAfter(typeof(Link))]
public class Package : Command
{
    public override void Execute()
    {
        Console.WriteLine("Packaging...");
    }
}
```

Notice that no changes are necessary in the core set of Compile, Link and Build commands to support the 
Preprocess and Package extensions. However, the extensions must be discoverable by the `Engine` through 
the MEF `CompositionContext`. A common way to achieve that is by creating the `CompositionContext` with
a set of assemblies loaded dynamically from a well-known location, such as the application directory. 

```C#
class Program
{
    static void Main(string[] args)
    {
        string[] assemblyFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        IEnumerable<Assembly> assemblies = assemblyFiles.Select(file => Assembly.LoadFrom(file));
        CompositionContext context = new ContainerConfiguration()
            .WithAssemblies(assemblies)
            .CreateContainer();

        var engine = new Engine(context);
        engine.Execute<Build>();
    }
}
```

Here is the output produced by the build system with extensions.

    Preprocessing...
    Compiling mefbuild.obj ...
    Linking mefbuild.obj ...
    Packaging...
    Build finished.
