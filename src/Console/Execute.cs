using System.Composition;

namespace MefBuild
{
    [Shared, Command(
        typeof(Execute), 
        DependsOn = new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) })]
    internal class Execute : Command
    {
    }
}
