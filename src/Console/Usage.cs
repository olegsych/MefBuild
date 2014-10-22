using System.Composition;

namespace MefBuild
{
    [Export]
    [DependsOn(
        typeof(PrintProgramUsage), 
        typeof(GetProgramCommandTypes), 
        typeof(PrintNamedCommands))]
    internal class Usage : Command
    {
    }
}
