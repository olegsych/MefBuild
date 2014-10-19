namespace MefBuild
{
    [Summary("Executes a specified command.")]
    [Command, DependsOn(typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands))]
    internal class Execute : Command
    {
    }
}
