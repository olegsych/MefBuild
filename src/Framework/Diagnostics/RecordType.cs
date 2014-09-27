namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Defines type of a <see cref="Record"/> written to a <see cref="Log"/>.
    /// </summary>
    /// <remarks>
    /// Record types are defined in the order of increasing significance, with <see cref="Message"/> 
    /// defined as the default record type. <see cref="RecordType"/> combined with <see cref="Importance"/>
    /// determines the <see cref="Verbosity"/> required for the record to show up in the <see cref="Output"/>.
    /// </remarks>
    public enum RecordType
    {
        /// <summary>
        /// Indicates that a logical operation finished.
        /// </summary>
        Stop = -2,

        /// <summary>
        /// Indicates that a logical operation started.
        /// </summary>
        Start = -1,

        /// <summary>
        /// Informational message.
        /// </summary>
        Message = 0, // default

        /// <summary>
        /// Indicates a problem that does not prevent completion of a logical operation.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Indicates a problem that prevents completion of a logical operation.
        /// </summary>
        Error = 2,
    }
}
