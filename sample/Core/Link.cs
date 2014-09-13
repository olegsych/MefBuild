using System;
using System.Composition;
using MefBuild;

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
