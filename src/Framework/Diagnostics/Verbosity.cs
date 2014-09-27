namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Defines verbosity level of a <see cref="Output"/>.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// Displays only errors of high importance.
        /// </summary>
        Quiet = -2,

        /// <summary>
        /// Displays errors of normal and high importance, warnings of high importance and 
        /// messages of high importance.
        /// </summary>
        Minimal = -1,

        /// <summary>
        /// Displays errors of normal and high importance, warnings of normal and high importance, 
        /// messages of high importance and start events of high importance.
        /// </summary>
        Normal = 0, // default

        /// <summary>
        /// Displays all record types with high and normal importance.
        /// </summary>
        Detailed = 1,

        /// <summary>
        /// Displays all record types of all importance levels.
        /// </summary>
        Diagnostic = 2,
    }
}
