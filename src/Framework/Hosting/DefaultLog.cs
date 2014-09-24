#define DEBUG

using System;
using System.Diagnostics;

namespace MefBuild.Hosting
{
    /// <summary>
    /// Similar to the default trace listener, this <see cref="ILog"/> implementation writes messages to the debugger output.
    /// </summary>
    internal class DefaultLog : ILog
    {
        public void WriteCritical(string message)
        {
            Debug.WriteLine(message);
        }

        public void WriteError(string message)
        {
            Debug.WriteLine(message);
        }

        public void WriteInformation(string message)
        {
            Debug.WriteLine(message);
        }

        public void WriteVerbose(string message)
        {
            Debug.WriteLine(message);
        }

        public void WriteWarning(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
