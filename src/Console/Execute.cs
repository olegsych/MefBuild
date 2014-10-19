namespace MefBuild
{
    [Summary("Executes a specified command.")]
    [Command(DependsOn = new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) })]
    internal class Execute : Command
    {
    }
}
