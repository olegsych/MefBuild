namespace MefBuild.Hosting
{
    /// <summary>
    /// Defines verbosity level of the <see cref="Log"/>.
    /// </summary>
    public enum LogVerbosity
    {
        /// <summary>
        /// Displays only errors of high importance.
        /// </summary>
        Quiet = -2,

        /// <summary>
        /// Displays errors of normal and high importance, warnings of high importance and 
        /// messages of high importance.
        /// </summary>
        Minimal,

        /// <summary>
        /// Displays errors of normal and high importance, warnings of normal and high importance, 
        /// messages of high importance and start events of high importance.
        /// </summary>
        Normal,

        /// <summary>
        /// Displays all event types with high and normal importance.
        /// </summary>
        Detailed,

        /// <summary>
        /// Displays all event types of all importance levels.
        /// </summary>
        Diagnostic,
    }
}
