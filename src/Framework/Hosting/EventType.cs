namespace MefBuild.Hosting
{
    /// <summary>
    /// Defines type of log message.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// By the way...
        /// </summary>
        Information,

        /// <summary>
        /// Hmm... Something's fishy.
        /// </summary>
        Warning,

        /// <summary>
        /// Oops! You might want to fix this one.
        /// </summary>
        Error,
    }
}
