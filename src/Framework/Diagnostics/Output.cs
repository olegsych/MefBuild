namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Represents an object that can write diagnostics events to a specific output.
    /// </summary>
    public abstract class Output
    {
        /// <summary>
        /// Gets or sets the verbosity level, which determines type and importance of records that will be written to the output.
        /// </summary>
        public Verbosity Verbosity { get; set; }

        /// <summary>
        /// When implemented in a derived class, writes the specified <paramref name="record"/> to the output.
        /// </summary>
        public abstract void Write(Record record);
    }
}
