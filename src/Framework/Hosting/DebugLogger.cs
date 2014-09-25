#define DEBUG

using System.Diagnostics;

namespace MefBuild.Hosting
{
    /// <summary>
    /// Similar to the default trace listener, this <see cref="Logger"/> writes text to the <see cref="Debug"/> output.
    /// </summary>
    public class DebugLogger : Logger
    {
        /// <summary>
        /// Writes the specified <paramref name="message"/> to the <see cref="Debug"/> output.
        /// </summary>
        public override void Write(string message, EventType eventType, EventImportance importance)
        {
            Debug.WriteLine(message);
        }
    }
}
