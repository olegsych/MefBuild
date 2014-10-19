using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MefBuild
{
    public class ParameterAttributeTest
    {
        [Fact]
        public void ClassIsMetadataAttributeThatCanBeAppliedToPropertiesAndParameters()
        {
            Assert.True(typeof(ParameterAttribute).IsPublic);
            Assert.True(typeof(Attribute).IsAssignableFrom(typeof(ParameterAttribute)));
            Assert.NotNull(typeof(ParameterAttribute).GetCustomAttribute<MetadataAttributeAttribute>());
            Assert.Equal(
                AttributeTargets.Parameter | AttributeTargets.Property,
                typeof(ParameterAttribute).GetCustomAttribute<AttributeUsageAttribute>().ValidOn);
        }

        [Fact]
        public void NameProvidesNameMetadata()
        {
            IEnumerable<KeyValuePair<string, object>> metadata = GetImportContractMetadata<TestCommand>();
            Assert.Contains(new KeyValuePair<string, object>("Name", "Test Parameter"), metadata);
        }

        private static IEnumerable<KeyValuePair<string, object>> GetImportContractMetadata<T>() where T : new()
        {
            CompositionContract importContract = null;
            var provider = new StubExportDescriptorProvider();
            provider.OnGetExportDescriptors = (contract, accessor) =>
            {
                importContract = contract;
                return Enumerable.Empty<ExportDescriptorPromise>();
            };

            var container = new ContainerConfiguration()
                .WithProvider(provider)
                .CreateContainer();

            container.SatisfyImports(new T());

            return importContract.MetadataConstraints;
        }

        public class TestCommand : Command
        {
            [Import(AllowDefault = true), 
            Parameter(Name = "Test Parameter")]
            public int Property { get; set; }
        }

        public class StubExportDescriptorProvider : ExportDescriptorProvider
        {
            public Func<CompositionContract, DependencyAccessor, IEnumerable<ExportDescriptorPromise>> OnGetExportDescriptors { get; set; }

            public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
            {
                return this.OnGetExportDescriptors(contract, descriptorAccessor);
            }
        }
    }
}
