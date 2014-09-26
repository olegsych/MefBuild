namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can write diagnostics events to a specific output.
    /// </summary>
    public abstract class Logger
    {
        /// <summary>
        /// Gets or sets the verbosity level, which determines type and importance of events that will be written to the output.
        /// </summary>
        public Verbosity Verbosity { get; set; }

        /// <summary>
        /// When implemented in a derived class, writes an event with the specified 
        /// <paramref name="text"/>, <paramref name="eventType"/> and <paramref name="importance"/>
        /// to the output this logger encapsulates.
        /// </summary>
        public abstract void Write(string text, EventType eventType, EventImportance importance);
    }
}
