MefBuild
========

MefBuild is a lightweight library powered by the [Microsoft Extensibility Framework](http://mef.codeplex.com/)
for implementing powerful and extensible build systems in .NET code. MefBuild mimics MSBuild.

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

* Behold!

```
Hello, World!
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
XML script, in MefBuild, values and objects are passed between commands via MEF `CompositionContext`.

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

**NOTE**: The exporting command must be marked with the `Shared` attribute, because otherwise, due to 
the default part creation policy in MEF 2.0, `CompositionContext` will create a new part, the Compile 
command in this example, for every import.

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
the MEF `CompositionContext`. A common way to achieve that is by creating a `ContainerConfiguration` with
a set of assemblies loaded dynamically from a well-known location, such as the application directory. 

```C#
class Program
{
    static void Main(string[] args)
    {
        string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        IEnumerable<Assembly> assemblies = files.Select(file => Assembly.LoadFrom(file));
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

Command Execution Order
-----------------------

When executing a `Command` of given type, MefBuild `Engine`, will execute its dependencies and 
extensions in the following order.

1. Before a command is executed, the `Engine` executes its `DependsOn` commands.
2. Before a command is executed, the `Engine` executes commands that specify it in their `ExecuteBefore` attribute.
3. Before a command is executed, the `Engine` checks if it has already executed earlier. If the command has *not* 
executed earlier, the `Engine` executes it. Otherwise, the `Engine` skips the command.
4. After a command is executed, the `Engine` executes commands that specify it in their `ExecuteAfter` attribute.

MefBuild command execution order mimics the [MSBuild target build order](http://msdn.microsoft.com/en-us/library/ee216359.aspx)
with one significant difference. MefBuild `Engine` always imports the command from the MEF `CompositionContext`. 
With the default part creation policy in MEF 2.0, `CompositionContext` returns a new instance of given 
command type for every import. Therefore, if a given command type appears in multiple locations of the 
execution order, the `Engine` will create multiple instances of this command types and execute respectively.

To prevent execution of multiple instances of a given command type, mark your command classes with 
MEF's `Shared` attribute. MSBuild explicitly guarantees that a target is never run more than once. 
Because of additional flexibility MEF offers, you can choose either option for any of your MefBuild
commands. However, as a best practice, consider **always** marking your commands with the `Shared`
attribute. This not only makes your command execution easier to follow, but also ensures correct 
passing of build artifacts between producing and consuming commands.