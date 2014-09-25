using System;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace MefBuild.Hosting
{
    public class DebugLoggerTest
    {
        [Fact]
        public void ClassIsPublicBecauseItMayBeInstantiatedByUserCodeHostingMefBuild()
        {
            Assert.True(typeof(DebugLogger).IsPublic);
        }

        [Fact]
        public void ClassInheritsFromLoggerForCompatibilityWithLoggingInfrastructure()
        {
            Assert.True(typeof(Logger).IsAssignableFrom(typeof(DebugLogger)));
        }

        [Fact]
        public void WriteWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new DebugLogger();
                log.Write("Test", default(EventType), default(EventImportance));
                Assert.Equal("Test" + Environment.NewLine, output.ToString());
            }
        }

        private class DebugTraceListener : TraceListener
        {
            private readonly StringBuilder output;

            public DebugTraceListener(StringBuilder output)
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
}
