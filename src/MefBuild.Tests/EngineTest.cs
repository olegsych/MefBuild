using System;
using System.Collections.Generic;
using System.Composition;
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
        public void ExecuteExecutesAndItsDependenciesOnceRegardlessOfHowManyTimesDependencyAppearsInTree()
        {
            var executed = new List<Command>();

            var adam = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var eve = new StubCommand(adam) { OnExecute = @this => executed.Add(@this) };
            var cain = new StubCommand(adam, eve) { OnExecute = @this => executed.Add(@this) };
            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = cain;
                    return true;
                }
                else
                {
                    export = null;
                    return false;
                }
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(new[] { adam, eve, cain }, executed);
        }

        [Fact]
        public void ExecuteGetsCommandsThatExecuteBeforeGivenTypeFromCompositionContext()
        {
            var requestedContracts = new List<CompositionContract>();

            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = new StubCommand();
                    return true;
                }
                else
                {
                    requestedContracts.Add(contract);
                    export = null;
                    return false;
                }    
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            CompositionContract requestedContract = requestedContracts[0];
            Assert.Equal(typeof(Command[]), requestedContract.ContractType);
            Assert.Equal("ExecuteBefore", requestedContract.ContractName);
            Assert.Contains(new KeyValuePair<string, object>("IsImportMany", true), requestedContract.MetadataConstraints);
            Assert.Contains(new KeyValuePair<string, object>("TargetCommandType", typeof(StubCommand)), requestedContract.MetadataConstraints);
        }

        [Fact]
        public void ExecuteExecutesBeforeCommandsBeforeCommandItself()
        {
            var executed = new List<Command>();

            var beforeCommand = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var targetCommand = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = targetCommand;
                    return true;
                }
                else if (contract.ContractName == "ExecuteBefore")
                {
                    export = new Command[] { beforeCommand };
                    return true;
                }
                else
                {
                    export = null;
                    return false;
                }
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(new[] { beforeCommand, targetCommand }, executed);
        }

        [Fact]
        public void ExecuteDoesNotExecuteBeforeCommandIfItHasAlreadyBeenExecuted()
        {
            var executed = new List<Command>();

            var beforeCommand = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var targetCommand = new StubCommand(beforeCommand) { OnExecute = @this => executed.Add(@this) };
            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = targetCommand;
                    return true;
                }
                else if (contract.ContractName == "ExecuteBefore")
                {
                    export = new Command[] { beforeCommand };
                    return true;
                }
                else
                {
                    export = null;
                    return false;
                }
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(new[] { beforeCommand, targetCommand }, executed);
        }

        [Fact]
        public void ExecuteGetsCommandsThatExecuteAfterGivenTypeFromCompositionContext()
        {
            var requestedContracts = new List<CompositionContract>();

            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = new StubCommand();
                    return true;
                }
                else
                {
                    requestedContracts.Add(contract);
                    export = null;
                    return false;
                }
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            CompositionContract requestedContract = requestedContracts[1];
            Assert.Equal(typeof(Command[]), requestedContract.ContractType);
            Assert.Equal("ExecuteAfter", requestedContract.ContractName);
            Assert.Contains(new KeyValuePair<string, object>("IsImportMany", true), requestedContract.MetadataConstraints);
            Assert.Contains(new KeyValuePair<string, object>("TargetCommandType", typeof(StubCommand)), requestedContract.MetadataConstraints);
        }

        [Fact]
        public void ExecuteExecutesAfterCommandsAfterCommandItself()
        {
            var executed = new List<Command>();

            var afterCommand = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var targetCommand = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = targetCommand;
                    return true;
                }
                else if (contract.ContractName == "ExecuteAfter")
                {
                    export = new Command[] { afterCommand };
                    return true;
                }
                else
                {
                    export = null;
                    return false;
                }
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(new[] { targetCommand, afterCommand }, executed);
        }

        [Fact]
        public void ExecuteDoesNotExecuteAfterCommandIfItHasAlreadyBeenExecuted()
        {
            var executed = new List<Command>();

            var afterCommand = new StubCommand { OnExecute = @this => executed.Add(@this) };
            var targetCommand = new StubCommand(afterCommand) { OnExecute = @this => executed.Add(@this) };
            var context = new StubCompositionContext();
            context.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                if (contract.ContractType == typeof(StubCommand))
                {
                    export = targetCommand;
                    return true;
                }
                else if (contract.ContractName == "ExecuteAfter")
                {
                    export = new Command[] { afterCommand };
                    return true;
                }
                else
                {
                    export = null;
                    return false;
                }
            };

            var engine = new Engine(context);
            engine.Execute(typeof(StubCommand));

            Assert.Equal(new[] { afterCommand, targetCommand }, executed);
        }
    }
}
