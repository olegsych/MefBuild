using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using MefBuild.Hosting;
using Xunit;

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
        public void ClassIsSealedBecauseItDelegatesWritingToLoggers()
        {
            Assert.True(typeof(Log).IsSealed);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenArrayIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new Log(null));
            Assert.Equal("loggers", e.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenLoggerIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new Log(new[] { new StubLogger(), null }));
            Assert.Equal("loggers[1]", e.ParamName);
        }

        [Fact]
        public void ConstructorImportsLoggersFromCompositionContext()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithParts(typeof(Log), typeof(StubLogger))
                .CreateContainer();

            var logger = context.GetExport<StubLogger>();
            bool eventWrittenToLogger = false;
            logger.OnWrite = (e, t, i) => eventWrittenToLogger = true;

            var log = context.GetExport<Log>();
            log.Write("Test Message", EventType.Error, EventImportance.High);

            Assert.True(eventWrittenToLogger);
        }

        [Fact]
        public void WritePassesGivenMessageEventTypeAndImportanceToLoggerWriteMethods()
        {
            var loggerMessage = string.Empty;
            var loggerEventType = EventType.Message;
            var loggerImportance = EventImportance.Low;
            var logger = new StubLogger();
            logger.OnWrite = (message, eventType, importance) =>
            {
                loggerMessage = message;
                loggerEventType = eventType;
                loggerImportance = importance;
            };

            var log = new Log(logger);
            log.Write("Test Message", EventType.Error, EventImportance.High);

            Assert.Equal("Test Message", loggerMessage);
            Assert.Equal(EventType.Error, loggerEventType);
            Assert.Equal(EventImportance.High, loggerImportance);
        }

        [Fact]
        public void WritesExpectedEventsToLoggersWithQuietVerbosity()
        {
            VerifyExpectedEventsForVerbosityLevel(new[] { "ErrorHigh" }, Verbosity.Quiet);
        }

        [Fact]
        public void WritesExpectedEventsToLoggersWithMinimalVerbosity()
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
        public void WritesExpectedEventsToLoggersWithNormalVerbosity()
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
        public void WritesExpectedEventsToLoggersWithDetailedVerbosity()
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
        public void WritesExpectedEventsToLoggersWithDiagnosticVerbosity()
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
            var logger = new StubLogger();
            logger.OnWrite = (message, type, importance) => events.Add(message);
            logger.Verbosity = verbosity;
            var log = new Log(logger);

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
