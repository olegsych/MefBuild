using System;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace MefBuild.Hosting
{
    public class DefaultLogTest
    {
        [Fact]
        public void ClassIsInternalBecauseItIsAutomaticallyCreatedByCommand()
        {
            Assert.False(typeof(DefaultLog).IsPublic);
        }

        [Fact]
        public void ClassImplementsILogInterfaceForCompatibilityWithCommandImports()
        {
            Assert.True(typeof(Log).IsAssignableFrom(typeof(DefaultLog)));
        }

        [Fact]
        public void WriteWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new TestableDefaultLog();
                log.Write(default(EventType), "Test");
                Assert.Equal("Test" + Environment.NewLine, output.ToString());
            }
        }

        private class TestableDefaultLog : DefaultLog
        {
            public new void Write(EventType messageType, string message)
            {
                base.Write(messageType, message);
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
