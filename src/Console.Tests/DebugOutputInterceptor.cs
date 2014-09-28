using System;
using System.Diagnostics;
using System.Text;

namespace MefBuild
{
    internal class DebugOutputInterceptor : TraceListener
    {
        private readonly StringBuilder output;

        public DebugOutputInterceptor(StringBuilder output)
        {
            Debug.Listeners.Add(this);
            this.output = output;
        }

        public override void Write(string message)
        {
            this.output.Append(message);
        }

        public override void WriteLine(string message)
        {
            this.Write(message + Environment.NewLine);
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Listeners.Remove(this);
            base.Dispose(disposing);
        }
    }
}
