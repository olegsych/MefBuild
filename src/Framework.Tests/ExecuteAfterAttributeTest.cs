using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class ExecuteAfterAttributeTest
    {
        [Fact]
        public void ClassIsPublicForAndMeantForUsers()
        {
            Assert.True(typeof(ExecuteAfterAttribute).IsPublic);
        }

        [Fact]
        public void ClassInheritsFromExportAttributeToMakeClassesItAppliesToDiscoverableByMef()
        {
            Assert.True(typeof(ExportAttribute).IsAssignableFrom(typeof(ExecuteAfterAttribute)));
        }

        [Fact]
        public void ConstructorInitializesAttributePropertiesWithValuesExpectedByEngine()
        {
            var targetCommandType = typeof(StubCommand);
            var attribute = new ExecuteAfterAttribute(targetCommandType);
            Assert.Equal("ExecuteAfter", attribute.ContractName);
            Assert.Equal(typeof(Command), attribute.ContractType);
            Assert.Same(targetCommandType, attribute.TargetCommandType);
        }

        [Fact]
        public void AttributeCanBeAppliedToClassesMultipleTimesBecauseCommandMayNeedToBeInjectedInMultiplePlaces()
        {
            var attributeUsage = typeof(ExecuteAfterAttribute).GetCustomAttributes(false).OfType<AttributeUsageAttribute>().Single();
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
            string contractName = "ExecuteAfter";
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

        [ExecuteAfter(typeof(StubCommand))]
        private class TestCommand1 : Command
        {
        }

        [ExecuteAfter(typeof(StubCommand))]
        private class TestCommand2 : Command
        {
        }
    }
}
