namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can trace diagnostics information.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Writes a critical <paramref name="message"/> to the log.
        /// </summary>
        void WriteCritical(string message);

        /// <summary>
        /// Writes an error <paramref name="message"/> to the log.
        /// </summary>
        void WriteError(string message);

        /// <summary>
        /// Writes an informational <paramref name="message"/> to the log.
        /// </summary>
        void WriteInformation(string message);

        /// <summary>
        /// Writes a verbose <paramref name="message"/> to the log.
        /// </summary>
        void WriteVerbose(string message);

        /// <summary>
        /// Writes a warning <paramref name="message"/> to the log.
        /// </summary>
        void WriteWarning(string message);
    }
}
