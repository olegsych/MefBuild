namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can trace diagnostics information.
    /// </summary>
    public abstract class Log
    {
        /// <summary>
        /// Writes an error <paramref name="message"/> to the log.
        /// </summary>
        public void Error(string message)
        {
            this.Write(EventType.Error, message);
        }

        /// <summary>
        /// Writes an informational <paramref name="message"/> to the log.
        /// </summary>
        public void Information(string message)
        {
            this.Write(EventType.Information, message);
        }

        /// <summary>
        /// Writes a warning <paramref name="message"/> to the log.
        /// </summary>
        public void Warning(string message)
        {
            this.Write(EventType.Warning, message);
        }

        /// <summary>
        /// Writes message of specified type to the log.
        /// </summary>
        protected abstract void Write(EventType messageType, string message);
    }
}
