using System;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Text;
using MefBuild.Hosting;
using Xunit;

namespace MefBuild
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
        public void WritesTextToConsoleOutput()
        {
            var text = new StringBuilder();
            WithStubConsoleOut(
                new StringWriter(text), 
                () =>
                {
                    var output = new ConsoleOutput();
                    output.Write("Test Message", EventType.Error, EventImportance.High);

                    Assert.Equal("Test Message" + Environment.NewLine, text.ToString());
                });
        }

        [Theory,
        InlineData(EventType.Error,   EventImportance.High,   ConsoleColor.Red),
        InlineData(EventType.Error,   EventImportance.Normal, ConsoleColor.Red),
        InlineData(EventType.Error,   EventImportance.Low,    ConsoleColor.DarkRed),
        InlineData(EventType.Warning, EventImportance.High,   ConsoleColor.Yellow),
        InlineData(EventType.Warning, EventImportance.Normal, ConsoleColor.Yellow),
        InlineData(EventType.Warning, EventImportance.Low,    ConsoleColor.DarkYellow),
        InlineData(EventType.Message, EventImportance.High,   ConsoleColor.White),
        InlineData(EventType.Message, EventImportance.Normal, ConsoleColor.Gray),
        InlineData(EventType.Message, EventImportance.Low,    ConsoleColor.DarkGray),
        InlineData(EventType.Start,   EventImportance.High,   ConsoleColor.Cyan),
        InlineData(EventType.Start,   EventImportance.Normal, ConsoleColor.Cyan),
        InlineData(EventType.Start,   EventImportance.Low,    ConsoleColor.DarkCyan),
        InlineData(EventType.Stop,    EventImportance.High,   ConsoleColor.Cyan),
        InlineData(EventType.Stop,    EventImportance.Normal, ConsoleColor.Cyan),
        InlineData(EventType.Stop,    EventImportance.Low,    ConsoleColor.DarkCyan)]
        public void WritesEventsWithExpectedColors(EventType eventType, EventImportance importance, ConsoleColor expectedColor)
        {
            var actualColor = default(ConsoleColor);
            WithStubConsoleOut(
                new StubTextWriter(() => actualColor = Console.ForegroundColor),
                () => 
                {
                    var output = new ConsoleOutput();
                    output.Write("Test Message", eventType, importance);
                    Assert.Equal(expectedColor, actualColor);
                });
        }

        [Fact]
        public void RestoresPreviousConsoleColorAfterWriting()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            var output = new ConsoleOutput();
            output.Write("Test Message", EventType.Error, EventImportance.High);
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
