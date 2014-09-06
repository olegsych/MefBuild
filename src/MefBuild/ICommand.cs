using System.Collections.Generic;

namespace MefBuild
{
    /// <summary>
    /// Represents a composable build command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets a collection of <see cref="ICommand"/> objects this command depends on.
        /// </summary>
        /// <remarks>
        /// This property mimics the DependsOnTargets attribute of MSBuild targets.
        /// http://msdn.microsoft.com/en-us/library/t50z2hka.aspx.
        /// </remarks>
        IEnumerable<ICommand> DependsOn { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}
