namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can write diagnostics events to a specific output.
    /// </summary>
    public abstract class Logger : Log
    {
        /// <summary>
        /// Gets or sets the verbosity level, which determines type and importance of events that will be written to the output.
        /// </summary>
        public Verbosity Verbosity { get; set; }
    }
}
