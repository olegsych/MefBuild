using System;

namespace MefBuild.Hosting
{
    /// <summary>
    /// Encapsulates a <see cref="Log"/> record.
    /// </summary>
    public sealed class Record
    {
        private readonly string text;
        private readonly EventType recordType;
        private readonly EventImportance importance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class with the specified <paramref name="text"/>, 
        /// <paramref name="recordType"/> and <paramref name="importance"/>.
        /// </summary>
        public Record(string text, EventType recordType, EventImportance importance)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            this.text = text;
            this.recordType = recordType;
            this.importance = importance;
        }

        /// <summary>
        /// Gets the record text.
        /// </summary>
        public string Text 
        {
            get { return this.text; }
        }

        /// <summary>
        /// Gets the record type.
        /// </summary>
        public EventType RecordType 
        {
            get { return this.recordType; }
        }

        /// <summary>
        /// Gets the record importance.
        /// </summary>
        public EventImportance Importance 
        {
            get { return this.importance; }
        }
    }
}
