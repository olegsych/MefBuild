using System;
using System.Composition;
using MefBuild;

[Export, ExecuteBefore(typeof(Compile))]
public class Preprocess : Command
{
    public override void Execute()
    {
        Console.WriteLine("Preprocessing...");
    }
}