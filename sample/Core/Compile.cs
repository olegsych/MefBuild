using System;
using System.Composition;
using MefBuild;

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
