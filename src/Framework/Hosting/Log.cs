namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can trace diagnostics information.
    /// </summary>
    public abstract class Log
    {
        /// <summary>
        /// Writes a critical <paramref name="message"/> to the log.
        /// </summary>
        public void Critical(string message)
        {
            this.Write(MessageType.Critical, message);
        }

        /// <summary>
        /// Writes an error <paramref name="message"/> to the log.
        /// </summary>
        public void Error(string message)
        {
            this.Write(MessageType.Error, message);
        }

        /// <summary>
        /// Writes an informational <paramref name="message"/> to the log.
        /// </summary>
        public void Information(string message)
        {
            this.Write(MessageType.Information, message);
        }

        /// <summary>
        /// Writes a verbose <paramref name="message"/> to the log.
        /// </summary>
        public void Verbose(string message)
        {
            this.Write(MessageType.Verbose, message);
        }

        /// <summary>
        /// Writes a warning <paramref name="message"/> to the log.
        /// </summary>
        public void Warning(string message)
        {
            this.Write(MessageType.Warning, message);
        }

        /// <summary>
        /// Writes message of specified type to the log.
        /// </summary>
        protected abstract void Write(MessageType messageType, string message);
    }
}
