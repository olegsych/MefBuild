namespace MefBuild
{
    /// <summary>
    /// Represents a composable command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}
