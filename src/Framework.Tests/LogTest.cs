using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using MefBuild.Hosting;
using Xunit;
using Record = MefBuild.Hosting.Record;

namespace MefBuild
{
    public class LogTest
    {
        [Fact]
        public void ClassIsPublicForLoggingFromUserCommands()
        {
            Assert.True(typeof(Log).IsPublic);
        }

        [Fact]
        public void ClassIsSealedBecauseItDelegatesWritingToOutputs()
        {
            Assert.True(typeof(Log).IsSealed);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenArrayIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new Log(null));
            Assert.Equal("outputs", e.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenOutputIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new Log(new[] { new StubOutput(), null }));
            Assert.Equal("outputs[1]", e.ParamName);
        }

        [Fact]
        public void ConstructorImportsOutputsFromCompositionContext()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithParts(typeof(Log), typeof(StubOutput))
                .CreateContainer();

            var output = context.GetExport<StubOutput>();
            bool recordWrittenToOutput = false;
            output.OnWrite = record => recordWrittenToOutput = true;

            var log = context.GetExport<Log>();
            log.Write("Test Message", EventType.Error, EventImportance.High);

            Assert.True(recordWrittenToOutput);
        }

        [Fact]
        public void EmptyReturnsLogInstanceUsedByEngineAndCommandsWhenCompositionContextHasNoExport()
        {
            Log instance = Log.Empty;
            Assert.NotNull(instance);
        }

        [Fact]
        public void EmptyPropertyIsReadOnlyBecauseItIsSingleton()
        {
            Assert.Null(typeof(Log).GetProperty("Empty").SetMethod);
        }

        [Fact]
        public void WritePassesGivenMessageEventTypeAndImportanceToOutputWriteMethods()
        {
            Record outputRecord = null;
            var output = new StubOutput();
            output.OnWrite = record => outputRecord = record;

            var log = new Log(output);
            log.Write("Test Message", EventType.Error, EventImportance.High);

            Assert.Equal("Test Message", outputRecord.Text);
            Assert.Equal(EventType.Error, outputRecord.RecordType);
            Assert.Equal(EventImportance.High, outputRecord.Importance);
        }

        [Fact]
        public void WritesExpectedEventsToOutputsWithQuietVerbosity()
        {
            VerifyExpectedEventsForVerbosityLevel(new[] { "ErrorHigh" }, Verbosity.Quiet);
        }

        [Fact]
        public void WritesExpectedEventsToOutputsWithMinimalVerbosity()
        {
            var expectedEvents = new[]
            {
                "ErrorHigh", "ErrorNormal",
                "WarningHigh", 
                "MessageHigh"
            };

            VerifyExpectedEventsForVerbosityLevel(expectedEvents, Verbosity.Minimal);
        }

        [Fact]
        public void WritesExpectedEventsToOutputsWithNormalVerbosity()
        {
            var expectedEvents = new[]
            {
                "ErrorHigh",   "ErrorNormal",
                "WarningHigh", "WarningNormal",
                "MessageHigh", 
                "StartHigh"
            };

            VerifyExpectedEventsForVerbosityLevel(expectedEvents, Verbosity.Normal);
        }

        [Fact]
        public void WritesExpectedEventsToOutputsWithDetailedVerbosity()
        {
            var expectedEvents = new[]
            {
                "ErrorHigh",   "ErrorNormal",   
                "WarningHigh", "WarningNormal", 
                "MessageHigh", "MessageNormal", 
                "StartHigh",   "StartNormal",   
                "StopHigh",    "StopNormal"
            };

            VerifyExpectedEventsForVerbosityLevel(expectedEvents, Verbosity.Detailed);
        }

        [Fact]
        public void WritesExpectedEventsToOutputsWithDiagnosticVerbosity()
        {
            var expectedEvents = new[] 
            {
                "ErrorHigh",   "ErrorNormal",   "ErrorLow",
                "WarningHigh", "WarningNormal", "WarningLow",
                "MessageHigh", "MessageNormal", "MessageLow",
                "StartHigh",   "StartNormal",   "StartLow",
                "StopHigh",    "StopNormal",    "StopLow"
            };

            VerifyExpectedEventsForVerbosityLevel(expectedEvents, Verbosity.Diagnostic);
        }

        private static void VerifyExpectedEventsForVerbosityLevel(string[] expectedEvents, Verbosity verbosity)
        {
            var events = new List<string>();
            var output = new StubOutput();
            output.OnWrite = record => events.Add(record.Text);
            output.Verbosity = verbosity;
            var log = new Log(output);

            WriteAllEventTypeAndImportanceCombinationsTo(log);

            Assert.Equal(expectedEvents, events);
        }

        private static void WriteAllEventTypeAndImportanceCombinationsTo(Log log)
        {
            foreach (int eventType in Enum.GetValues(typeof(EventType)).Cast<int>().OrderByDescending(value => value))
            {
                foreach (int importance in Enum.GetValues(typeof(EventImportance)).Cast<int>().OrderByDescending(value => value))
                {
                    log.Write(
                        Enum.GetName(typeof(EventType), eventType) + Enum.GetName(typeof(EventImportance), importance),
                        (EventType)eventType, 
                        (EventImportance)importance);
                }
            }
        }
    }
}
