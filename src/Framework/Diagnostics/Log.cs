using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Represents an object that can collect diagnostics events.
    /// </summary>
    [Export, Shared]
    public sealed class Log
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Private fields should be camelCased")]
        private static readonly Log empty = new Log();
        private readonly IReadOnlyCollection<Output> outputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class with an array of 
        /// <see cref="Output"/> objects responsible for writing events to one or more 
        /// outputs.
        /// </summary>
        [ImportingConstructor]
        public Log([ImportMany] params Output[] outputs)
        {
            const string ParamName = "outputs";

            if (outputs == null)
            {
                throw new ArgumentNullException(ParamName);
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(ParamName + "[" + i + "]");
                }
            }

            this.outputs = outputs;
        }

        /// <summary>
        /// Gets an empty log without outputs.
        /// </summary>
        public static Log Empty 
        {
            get { return empty; }
        }

        /// <summary>
        /// Writes event of specified type and importance to the log.
        /// </summary>
        public void Write(Record record)
        {
            if (object.ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            foreach (Output output in this.outputs)            
            {
                if (IsEventAllowedByVerbosity(output.Verbosity, record.RecordType, record.Importance))
                {
                    output.Write(record);
                }
            }
        }

        private static bool IsEventAllowedByVerbosity(Verbosity verbosity, EventType eventType, EventImportance importance)
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
