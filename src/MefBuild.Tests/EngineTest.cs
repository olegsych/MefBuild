using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using Xunit;

namespace MefBuild
{
    public class EngineTest
    {
        [Fact]
        public void ClassIsPublicAndCanBeUsedDirectly()
        {
            Assert.True(typeof(Engine).IsPublic);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            CompositionContext context = null;
            var e = Assert.Throws<ArgumentNullException>(() => new Engine(context));
            Assert.Equal("context", e.ParamName);
        }

        [Fact]
        public void ExecuteThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var engine = new Engine(new StubCompositionContext());
            Type commandType = null;
            var e = Assert.Throws<ArgumentNullException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
        }

        [Fact]
        public void ExecuteThrowsArgumentExceptionWhenGivenTypeIsNotCommandToPreventUsageErrors()
        {
            var engine = new Engine(new StubCompositionContext());
            Type commandType = typeof(object);
            var e = Assert.Throws<ArgumentException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
            Assert.Contains("Command", e.Message);
        }

        [Fact]
        public void ExecuteGetsCommandOfGivenTypeFromCompositionContext()
        {
            CompositionContract contractRequested = null;

            var compositionContext = new StubCompositionContext();
            compositionContext.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                contractRequested = contract;
                export = new StubCommand();
                return true;
            };

            var engine = new Engine(compositionContext);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(typeof(StubCommand), contractRequested.ContractType);
        }

        [Fact]
        public void ExecuteExecutesAndItsDependenciesOnceRegardlessOfHowManyTimesDependencyAppearsInTree()
        {
            var executed = new List<Command>();

            var adam = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var eve = new StubCommand(adam) { OnExecute = @this => executed.Add(@this) };
            var cain = new StubCommand(adam, eve) { OnExecute = @this => executed.Add(@this) };
            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                export = cain;
                return true;
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(new[] { adam, eve, cain }, executed);
        }
    }
}
