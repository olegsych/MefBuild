using System;
using System.Collections.Generic;
using System.Linq;

namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Represents an object that writes diagnostics records to one or more outputs.
    /// </summary>
    public sealed class Log
    {
        private readonly IEnumerable<Output> outputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class with an array of 
        /// <see cref="Output"/> objects responsible for writing records to specific 
        /// outputs.
        /// </summary>
        public Log(IEnumerable<Output> outputs)
        {
            const string ParamName = "outputs";

            if (outputs == null)
            {
                throw new ArgumentNullException(ParamName);
            }

            int i = 0;
            foreach (Output output in outputs)
            {
                if (output == null)
                {
                    throw new ArgumentNullException(ParamName + "[" + i + "]");
                }

                i++;
            }

            this.outputs = outputs;
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
