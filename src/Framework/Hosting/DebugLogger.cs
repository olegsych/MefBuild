#define DEBUG

using System.Composition;
using System.Diagnostics;

namespace MefBuild.Hosting
{
    /// <summary>
    /// Similar to the default trace listener, this <see cref="Logger"/> writes text to the <see cref="Debug"/> output.
    /// </summary>
    [Export(typeof(Logger))]
    public class DebugLogger : Logger
    {
        /// <summary>
        /// Writes the specified <paramref name="text"/> to the <see cref="Debug"/> output.
        /// </summary>
        public override void Write(string text, EventType eventType, EventImportance importance)
        {
            Debug.WriteLine(text);
        }
    }
}
