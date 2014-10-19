using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class ExecuteBeforeAttributeTest
    {
        [Fact]
        public void ClassIsPublicForAndMeantForUsers()
        {
            Assert.True(typeof(ExecuteBeforeAttribute).IsPublic);
        }

        [Fact]
        public void ClassInheritsFromExportAttributeToMakeClassesItAppliesToDiscoverableByMef()
        {
            Assert.True(typeof(ExportAttribute).IsAssignableFrom(typeof(ExecuteBeforeAttribute)));
        }

        [Fact]
        public void ConstructorInitializesAttributePropertiesWithValuesExpectedByEngine()
        {
            var targetCommandType = typeof(StubCommand);
            var attribute = new ExecuteBeforeAttribute(typeof(StubCommand));
            Assert.Equal("ExecuteBefore", attribute.ContractName);
            Assert.Equal(typeof(Command), attribute.ContractType);
            Assert.Same(targetCommandType, attribute.TargetCommandType);
        }

        [Fact]
        public void AttributeCanBeAppliedToClassesMultipleTimesBecauseCommandMayNeedToBeInjectedInMultipleBuildStages()
        {
            var attributeUsage = typeof(ExecuteBeforeAttribute).GetCustomAttributes(false).OfType<AttributeUsageAttribute>().Single();
            Assert.Equal(AttributeTargets.Class, attributeUsage.ValidOn);
            Assert.True(attributeUsage.AllowMultiple);
        }

        [Fact]
        public void AttributeMakesClassesDiscoverableThroughCompositionContext()
        {
            var configuration = new ContainerConfiguration()
                .WithPart<TestCommand1>()
                .WithPart<TestCommand2>();
            CompositionHost container = configuration.CreateContainer();

            Type contractType = typeof(Command[]);
            string contractName = "ExecuteBefore";
            var constraints = new Dictionary<string, object>
            {
                { "IsImportMany", true },
                { "TargetCommandType", typeof(StubCommand) },
            };
            var contract = new CompositionContract(contractType, contractName, constraints);

            object export;
            Assert.True(container.TryGetExport(contract, out export));

            var exports = (IEnumerable<Command>)export;

            Assert.Equal(2, exports.Count());
            Assert.Equal(typeof(TestCommand1), exports.First().GetType());
            Assert.Equal(typeof(TestCommand2), exports.Last().GetType());
        }

        [ExecuteBefore(typeof(StubCommand))]
        private class TestCommand1 : Command
        {
        }

        [ExecuteBefore(typeof(StubCommand))]
        private class TestCommand2 : Command
        {
        }
    }
}
