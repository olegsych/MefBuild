namespace MefBuild
{
    /// <summary>
    /// Represents a composable command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// When overridden in a derived class, executes the command logic.
        /// </summary>
        /// <remarks>
        /// Override this method when implementing a concrete command. Commands that only define an 
        /// injection point for commands marked with the <see cref="ExecuteBeforeAttribute"/> or 
        /// <see cref="ExecuteAfterAttribute"/>, as well as commands that define execution order 
        /// for a group of commands using the <see cref="DependsOnAttribute"/> don't have to override 
        /// this method. 
        /// </remarks>
        public virtual void Execute()
        {
        }
    }
}
