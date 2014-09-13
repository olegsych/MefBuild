using System;
using System.Composition;
using MefBuild;

[Export, ExecuteAfter(typeof(Link))]
public class Package : Command
{
    public override void Execute()
    {
        Console.WriteLine("Packaging...");
    }
}
