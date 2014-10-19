using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using Xunit;

namespace MefBuild
{
    public class DependsOnAttributeTest
    {
        [Fact]
        public void ClassIsPublicForUsersToSpecifyDependenciesOfTheirCommands()
        {
            Assert.True(typeof(DependsOnAttribute).IsPublic);
        }

        [Fact]
        public void ConstructorInitializesDependsOnProperty()
        {
            var types = new[] { typeof(object), typeof(string) };
            var attribute = new DependsOnAttribute(types);
            Assert.Equal(types, attribute.Dependencies);
        }

        [Fact]
        public void DependenciesPropertyIsReadonlyBecauseConstructorInitializesIt()
        {
            Assert.Null(typeof(DependsOnAttribute).GetProperty("Dependencies").SetMethod);
        }

        [Fact]
        public void AttributeCanBeAppliedToClassesToSpecifyDependsOnMetadata()
        {
            var container = new ContainerConfiguration().WithPart<ClassWithDependencies>().CreateContainer();
            var export = container.GetExport<Lazy<ClassWithDependencies, Metadata>>();
            Assert.NotNull(export.Metadata.Dependencies);
        }

        public class Metadata
        {
            public IEnumerable<Type> Dependencies { get; set; }
        }

        [Export, DependsOn(typeof(string))]
        public class ClassWithDependencies
        {
        }
    }
}
