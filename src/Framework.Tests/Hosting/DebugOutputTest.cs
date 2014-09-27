using System;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace MefBuild.Hosting
{
    public class DebugOutputTest
    {
        [Fact]
        public void ClassIsPublicBecauseItMayBeInstantiatedByUserCodeHostingMefBuild()
        {
            Assert.True(typeof(DebugOutput).IsPublic);
        }

        [Fact]
        public void ClassInheritsFromOutputForCompatibilityWithLoggingInfrastructure()
        {
            Assert.True(typeof(Output).IsAssignableFrom(typeof(DebugOutput)));
        }

        [Fact]
        public void ClassIsExportedForComposition()
        {
            CompositionContext context = new ContainerConfiguration().WithPart<DebugOutput>().CreateContainer();
            var output = context.GetExport<Output>();
            Assert.IsType<DebugOutput>(output);
        }

        [Fact]
        public void WriteWritesLineToDebugOutput()
        {
            var output = new StringBuilder();
            using (new DebugTraceListener(output))
            {
                var log = new DebugOutput();
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
