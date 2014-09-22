using System.Composition;

namespace MefBuild
{
    [Export, Shared, DependsOn(typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands))]
    internal class Execute : Command
    {
    }
}
