using System;

namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Encapsulates a <see cref="Log"/> record.
    /// </summary>
    public sealed class Record : IEquatable<Record>
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

        /// <summary>
        /// Determines whether two specified records have the same property values.
        /// </summary>
        public static bool operator ==(Record left, Record right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified records have different property values.
        /// </summary>
        public static bool operator !=(Record left, Record right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns true if this record has the same property values as the <paramref name="other"/>.
        /// </summary>
        public bool Equals(Record other)
        {
            return !object.ReferenceEquals(other, null)
                && this.text.Equals(other.text) 
                && this.recordType.Equals(other.recordType) 
                && this.importance.Equals(other.Importance);
        }

        /// <summary>
        /// Returns true if the specified object is a <see cref="Record"/> that has the same property values.
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Record);
        }

        /// <summary>
        /// Returns a hash code computed based on property values of this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return CombineHashCodes(
                this.text.GetHashCode(),
                this.recordType.GetHashCode(),
                this.importance.GetHashCode());
        }

        // http://referencesource.microsoft.com/#mscorlib/system/tuple.cs
        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        private static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }
    }
}
