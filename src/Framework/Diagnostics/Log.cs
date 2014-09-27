using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Represents an object that writes diagnostics records to one or more outputs.
    /// </summary>
    [Export, Shared]
    public sealed class Log
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Private fields should be camelCased")]
        private static readonly Log empty = new Log();
        private readonly IReadOnlyCollection<Output> outputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class with an array of 
        /// <see cref="Output"/> objects responsible for writing records to specific 
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
        /// Writes the specified <paramref name="record"/> to the <see cref="Output"/> objects associated with this log.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="record"/> is null.</exception>
        public void Write(Record record)
        {
            if (object.ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            foreach (Output output in this.outputs)            
            {
                if (IsRecordAllowedByVerbosity(output.Verbosity, record.RecordType, record.Importance))
                {
                    output.Write(record);
                }
            }
        }

        private static bool IsRecordAllowedByVerbosity(Verbosity verbosity, RecordType recordType, Importance importance)
        {
            switch (verbosity)
            {
                case Verbosity.Quiet:
                    return recordType == RecordType.Error && importance == Importance.High;
                case Verbosity.Minimal:
                    return (recordType == RecordType.Error   && importance >= Importance.Normal)
                        || (recordType >= RecordType.Message && importance == Importance.High);
                case Verbosity.Normal:
                    return (recordType >= RecordType.Warning && importance >= Importance.Normal)
                        || (recordType >= RecordType.Start   && importance == Importance.High);
                case Verbosity.Detailed:
                    return importance > Importance.Low;
                default:
                    return true;
            }
        }
    }
}
