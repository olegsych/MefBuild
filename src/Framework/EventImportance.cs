using MefBuild.Hosting;

namespace MefBuild
{
    /// <summary>
    /// Defines importance of a logged event.
    /// </summary>
    public enum EventImportance
    {
        /// <summary>
        /// Indicates a low-importance event.
        /// </summary>
        /// <remarks>
        /// Low-importance events are intended for developers building new commands. They are logged when 
        /// verbosity level is set to <see cref="Verbosity.Diagnostic"/>. Console output displays 
        /// low-importance events using darker colors than normal and high-importance events.
        /// </remarks>
        Low = -1,

        /// <summary>
        /// Indicates a normal-importance event.
        /// </summary>
        /// <remarks>
        /// Normal-importance events are intended for developers using pre-built commands. Depending on 
        /// significance of an <see cref="EventType"/>, it is typically logged when verbosity level is lower,
        /// such as <see cref="Verbosity.Detailed"/> and <see cref="Verbosity.Normal"/>.
        /// </remarks>
        Normal = 0, // default

        /// <summary>
        /// Indicates a high-importance event.
        /// </summary>
        /// <remarks>
        /// High-importance events are intended for developers using pre-built commands. An event of lower
        /// significance, such as <see cref="EventType.Message"/>, with high importance will appear in the 
        /// log at lower verbosity levels, such as <see cref="Verbosity.Normal"/>, that normally filter
        /// them out.
        /// </remarks>
        High = 1,
    }
}
