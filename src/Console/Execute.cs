using System.Composition;

namespace MefBuild
{
    [Export, Shared, Command(
        typeof(Execute), 
        DependsOn = new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) })]
    internal class Execute : Command
    {
    }
}
