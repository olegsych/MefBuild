namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Defines type of an event written to a <see cref="Log"/>.
    /// </summary>
    /// <remarks>
    /// Event types are defined in the order of increasing significance, with <see cref="Message"/> 
    /// defined as the default event type. <see cref="EventType"/> combined with <see cref="EventImportance"/>
    /// determines the <see cref="Verbosity"/> required for the event to show up in the <see cref="Log"/>.
    /// </remarks>
    public enum EventType
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
