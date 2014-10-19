using System.Composition;

namespace MefBuild
{
    [Export]
    [Summary("Executes a specified command.")]
    [DependsOn(typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands))]
    internal class Execute : Command
    {
    }
}
