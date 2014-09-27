#define DEBUG

using System;
using System.Composition;
using System.Diagnostics;

namespace MefBuild.Diagnostics
{
    /// <summary>
    /// Similar to the default trace listener, this <see cref="Output"/> writes text to the <see cref="Debug"/> output.
    /// </summary>
    [Export(typeof(Output))]
    public class DebugOutput : Output
    {
        /// <summary>
        /// Writes the specified <paramref name="record"/> to the <see cref="Debug"/> output.
        /// </summary>
        public override void Write(Record record)
        {
            if (object.ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            Debug.WriteLine(record.Text);
        }
    }
}
