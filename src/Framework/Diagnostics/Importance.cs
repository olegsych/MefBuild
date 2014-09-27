namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Defines importance of a log <see cref="Record"/>.
    /// </summary>
    public enum Importance
    {
        /// <summary>
        /// Indicates a record of low importance.
        /// </summary>
        /// <remarks>
        /// Records of low importance are intended for developers building new commands. They are written out
        /// when verbosity level is set to <see cref="Verbosity.Diagnostic"/>. Console output displays 
        /// low-importance records using darker colors than normal and high-importance records.
        /// </remarks>
        Low = -1,

        /// <summary>
        /// Indicates a record of normal importance.
        /// </summary>
        /// <remarks>
        /// Records of normal importance are intended for developers troubleshooting execution of pre-built 
        /// commands. Depending on significance of an <see cref="RecordType"/>, it is typically written out 
        /// when verbosity level is lower, such as <see cref="Verbosity.Detailed"/> and <see cref="Verbosity.Normal"/>.
        /// </remarks>
        Normal = 0, // default

        /// <summary>
        /// Indicates a record of high importance.
        /// </summary>
        /// <remarks>
        /// Records of high importance are intended for developers using pre-built commands. A record of lower
        /// significance, such as <see cref="RecordType.Message"/>, with high importance will appear in the 
        /// output at lower verbosity levels, such as <see cref="Verbosity.Normal"/>, that typically filter
        /// them out.
        /// </remarks>
        High = 1,
    }
}
