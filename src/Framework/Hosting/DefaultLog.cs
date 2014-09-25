#define DEBUG

using System;
using System.Diagnostics;

namespace MefBuild.Hosting
{
    /// <summary>
    /// Similar to the default trace listener, this <see cref="Log"/> implementation writes messages to the debugger output.
    /// </summary>
    internal class DefaultLog : Log
    {
        protected override void Write(EventType eventType, string text)
        {
            Debug.WriteLine(text);
        }
    }
}
