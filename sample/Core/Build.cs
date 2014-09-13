using System;
using System.Composition;
using MefBuild;

[Shared, Export, DependsOn(typeof(Compile), typeof(Link))]
public class Build : Command
{
    public override void Execute()
    {
        Console.WriteLine("Build finished.");
    }
}