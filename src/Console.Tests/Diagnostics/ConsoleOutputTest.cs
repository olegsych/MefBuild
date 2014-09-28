using System;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Text;
using Xunit;

namespace MefBuild.Diagnostics
{
    public sealed class ConsoleOutputTest
    {
        [Fact]
        public void ClassInheritsFromOutputForCompatibilityWithLoggingInfrastructure()
        {
            Assert.True(typeof(Output).IsAssignableFrom(typeof(ConsoleOutput)));
        }

        [Fact]
        public void ClassIsExportedForComposition()
        {
            CompositionContext context = new ContainerConfiguration().WithPart<ConsoleOutput>().CreateContainer();
            var output = context.GetExport<Output>();
            Assert.IsType<ConsoleOutput>(output);
        }

        [Fact]
        public void WriteThrowsArgumentNullExpceptionToPreventUsageErrors()
        {
            var output = new ConsoleOutput();
            var e = Assert.Throws<ArgumentNullException>(() => output.Write(null));
            Assert.Equal("record", e.ParamName);
        }

        [Fact]
        public void WritesTextToConsole()
        {
            var text = new StringBuilder();
            using (new ConsoleOutputInterceptor(text))
            {
                var output = new ConsoleOutput();
                output.Write(new Record("Test Message", RecordType.Error, Importance.High));

                Assert.Equal("Test Message" + Environment.NewLine, text.ToString());
            }
        }

        [Theory,
        InlineData(RecordType.Error,   Importance.High,   ConsoleColor.Red),
        InlineData(RecordType.Error,   Importance.Normal, ConsoleColor.Red),
        InlineData(RecordType.Error,   Importance.Low,    ConsoleColor.DarkRed),
        InlineData(RecordType.Warning, Importance.High,   ConsoleColor.Yellow),
        InlineData(RecordType.Warning, Importance.Normal, ConsoleColor.Yellow),
        InlineData(RecordType.Warning, Importance.Low,    ConsoleColor.DarkYellow),
        InlineData(RecordType.Message, Importance.High,   ConsoleColor.White),
        InlineData(RecordType.Message, Importance.Normal, ConsoleColor.Gray),
        InlineData(RecordType.Message, Importance.Low,    ConsoleColor.DarkGray),
        InlineData(RecordType.Start,   Importance.High,   ConsoleColor.Cyan),
        InlineData(RecordType.Start,   Importance.Normal, ConsoleColor.Cyan),
        InlineData(RecordType.Start,   Importance.Low,    ConsoleColor.DarkCyan),
        InlineData(RecordType.Stop,    Importance.High,   ConsoleColor.Cyan),
        InlineData(RecordType.Stop,    Importance.Normal, ConsoleColor.Cyan),
        InlineData(RecordType.Stop,    Importance.Low,    ConsoleColor.DarkCyan)]
        public void WritesTextToConsoleWithExpectedForegroundColor(RecordType eventType, Importance importance, ConsoleColor expectedColor)
        {
            var actualColor = default(ConsoleColor);
            WithStubConsoleOut(
                new StubTextWriter(() => actualColor = Console.ForegroundColor),
                () => 
                {
                    var output = new ConsoleOutput();
                    output.Write(new Record("Test Message", eventType, importance));
                    Assert.Equal(expectedColor, actualColor);
                });
        }

        [Fact]
        public void RestoresPreviousForegroundColorAfterWriting()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            var output = new ConsoleOutput();
            output.Write(new Record("Test Message", RecordType.Error, Importance.High));
            Assert.Equal(ConsoleColor.Black, Console.ForegroundColor);
        }

        private static void WithStubConsoleOut(TextWriter stubOut, Action test)
        {
            TextWriter oldOut = Console.Out;
            try
            {
                Console.SetOut(stubOut);
                test();
            }
            finally
            {
                Console.SetOut(oldOut);
            }
        }

        private class StubTextWriter : TextWriter
        {
            private readonly Action onWrite;

            public StubTextWriter(Action onWrite)
            {
                this.onWrite = onWrite;
            }

            public override Encoding Encoding
            {
                get { return Encoding.Unicode; }
            }

            public override void Write(char value)
            {
                this.onWrite();
            }
        }
    }
}
