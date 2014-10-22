using System;
using System.Composition;
using MefBuild.Properties;

namespace MefBuild
{
    [Export]
    internal class PrintProgramUsage : Command
    {
        public override void Execute()
        {
            Console.WriteLine(Resources.UsageHeader);
        }
    }
}