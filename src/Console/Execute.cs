using System.Composition;

namespace MefBuild
{
    [Command(DependsOn = new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) })]
    internal class Execute : Command
    {
    }
}
