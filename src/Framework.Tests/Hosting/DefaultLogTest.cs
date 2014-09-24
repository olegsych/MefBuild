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
            Assert.True(typeof(ILog).IsAssignableFrom(typeof(DefaultLog)));
        }

        [Fact]
        public void CriticalWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            { 
                var log = new DefaultLog();
                log.WriteCritical("Test");
                Assert.Equal("Test" + Environment.NewLine, output.ToString());
            }
        }

        [Fact]
        public void ErrorWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new DefaultLog();
                log.WriteError("Test");
                Assert.Equal("Test" + Environment.NewLine, output.ToString());
            }
        }

        [Fact]
        public void WarningWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new DefaultLog();
                log.WriteWarning("Test");
                Assert.Equal("Test" + Environment.NewLine, output.ToString());
            }
        }

        [Fact]
        public void InformationWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new DefaultLog();
                log.WriteInformation("Test");
                Assert.Equal("Test" + Environment.NewLine, output.ToString());
            }
        }

        [Fact]
        public void VerboseWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new DefaultLog();
                log.WriteVerbose("Test");
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
