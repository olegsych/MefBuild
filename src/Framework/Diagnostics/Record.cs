using System;

namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Encapsulates a <see cref="Log"/> record.
    /// </summary>
    public sealed class Record
    {
        private readonly Tuple<string, RecordType, Importance> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class with the specified <paramref name="text"/>, 
        /// <paramref name="recordType"/> and <paramref name="importance"/>.
        /// </summary>
        public Record(string text, RecordType recordType, Importance importance)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            this.data = new Tuple<string, RecordType, Importance>(text, recordType, importance);
        }

        /// <summary>
        /// Gets the record text.
        /// </summary>
        public string Text 
        {
            get { return this.data.Item1; }
        }

        /// <summary>
        /// Gets the record type.
        /// </summary>
        public RecordType RecordType 
        {
            get { return this.data.Item2; }
        }

        /// <summary>
        /// Gets the record importance.
        /// </summary>
        public Importance Importance 
        {
            get { return this.data.Item3; }
        }

        /// <summary>
        /// Returns true if the specified object is a <see cref="Record"/> that has the same property values.
        /// </summary>
        public override bool Equals(object obj)
        {
            var record = obj as Record;
            return record != null && this.data.Equals(record.data);
        }

        /// <summary>
        /// Returns a hash code computed based on property values of this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }
    }
}
