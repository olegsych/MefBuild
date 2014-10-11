namespace MefBuild.Execution
{
    /// <summary>
    /// Describes a dependency between two command types.
    /// </summary>
    internal enum DependencyType
    {
        None,
        DependsOn,
        ExecuteBefore,
        ExecuteAfter,
    }
}
