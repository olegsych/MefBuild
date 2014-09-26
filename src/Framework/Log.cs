using System;
using System.Collections.Generic;
using System.Composition;
using MefBuild.Hosting;

namespace MefBuild
{
    /// <summary>
    /// Represents an object that can collect diagnostics events.
    /// </summary>
    [Export, Shared]
    public sealed class Log
    {
        private readonly IReadOnlyCollection<Logger> loggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class with an array of 
        /// <see cref="Logger"/> objects responsible for writing events to one or more 
        /// outputs.
        /// </summary>
        [ImportingConstructor]
        public Log([ImportMany] params Logger[] loggers)
        {
            if (loggers == null)
            {
                throw new ArgumentNullException("loggers");
            }

            for (int i = 0; i < loggers.Length; i++)
            {
                if (loggers[i] == null)
                {
                    throw new ArgumentNullException("loggers[" + i + "]");
                }
            }

            this.loggers = loggers;
        }

        /// <summary>
        /// Writes message of specified type to the log.
        /// </summary>
        public void Write(string text, EventType eventType, EventImportance importance)
        {
            foreach (Logger logger in this.loggers)            
            {
                if (IsEventAllowedByLoggerVerbosity(logger.Verbosity, eventType, importance))
                {
                    logger.Write(text, eventType, importance);
                }
            }
        }

        private static bool IsEventAllowedByLoggerVerbosity(Verbosity verbosity, EventType eventType, EventImportance importance)
        {
            switch (verbosity)
            {
                case Verbosity.Quiet:
                    return eventType == EventType.Error && importance == EventImportance.High;
                case Verbosity.Minimal:
                    return (eventType == EventType.Error   && importance >= EventImportance.Normal)
                        || (eventType >= EventType.Message && importance == EventImportance.High);
                case Verbosity.Normal:
                    return (eventType >= EventType.Warning && importance >= EventImportance.Normal)
                        || (eventType >= EventType.Start   && importance == EventImportance.High);
                case Verbosity.Detailed:
                    return importance > EventImportance.Low;
                default:
                    return true;
            }
        }
    }
}
