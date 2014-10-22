using System.Composition;

namespace MefBuild
{
    [Export, Export("Execute", typeof(Command))]
    [Summary("Executes a specified command.")]
    [DependsOn(typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands))]
    internal class Execute : Command
    {
    }
}
