namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can write diagnostics events to a specific output.
    /// </summary>
    public abstract class Output
    {
        /// <summary>
        /// Gets or sets the verbosity level, which determines type and importance of events that will be written to the output.
        /// </summary>
        public Verbosity Verbosity { get; set; }

        /// <summary>
        /// When implemented in a derived class, writes <paramref name="text"/> with the specified <paramref name="eventType"/> 
        /// and <paramref name="importance"/> to the output.
        /// </summary>
        public abstract void Write(string text, EventType eventType, EventImportance importance);
    }
}
