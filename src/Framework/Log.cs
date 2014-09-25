namespace MefBuild
{
    /// <summary>
    /// Represents an object that can collect diagnostics events.
    /// </summary>
    public abstract class Log
    {
        /// <summary>
        /// Writes an <see cref="EventType.Error"/> with the specified <paramref name="message"/> to the log.
        /// </summary>
        public void Error(string message)
        {
            this.Write(message, EventType.Error, EventImportance.Normal);
        }

        /// <summary>
        /// Writes a <see cref="EventType.Message"/> with the specified <paramref name="text"/> to the log.
        /// </summary>
        public void Message(string text)
        {
            this.Write(text, EventType.Message, EventImportance.Normal);
        }

        /// <summary>
        /// Writes a <see cref="EventType.Warning"/> with the specified <paramref name="message"/> to the log.
        /// </summary>
        public void Warning(string message)
        {
            this.Write(message, EventType.Warning, EventImportance.Normal);
        }

        /// <summary>
        /// Writes message of specified type to the log.
        /// </summary>
        public abstract void Write(string message, EventType eventType, EventImportance importance);
    }
}
