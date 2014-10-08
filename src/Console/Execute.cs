namespace MefBuild
{
    [Command(
        Summary = "Executes a specified command.",
        DependsOn = new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) })]
    internal class Execute : Command
    {
    }
}
