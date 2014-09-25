namespace MefBuild.Hosting
{
    /// <summary>
    /// Represents an object that can trace diagnostics information.
    /// </summary>
    public abstract class Log
    {
        /// <summary>
        /// Writes an <see cref="EventType.Error"/> with the specified <paramref name="message"/> to the log.
        /// </summary>
        public void Error(string message)
        {
            this.Write(EventType.Error, message);
        }

        /// <summary>
        /// Writes a <see cref="EventType.Message"/> with the specified <paramref name="text"/> to the log.
        /// </summary>
        public void Message(string text)
        {
            this.Write(EventType.Message, text);
        }

        /// <summary>
        /// Writes a <see cref="EventType.Warning"/> with the specified <paramref name="message"/> to the log.
        /// </summary>
        public void Warning(string message)
        {
            this.Write(EventType.Warning, message);
        }

        /// <summary>
        /// Writes message of specified type to the log.
        /// </summary>
        protected abstract void Write(EventType eventType, string message);
    }
}
