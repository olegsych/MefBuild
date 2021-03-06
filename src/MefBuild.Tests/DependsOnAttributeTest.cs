﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class DependsOnAttributeTest
    {
        [Fact]
        public void ClassIsPublicAndMeantForUsers()
        {
            Assert.True(typeof(DependsOnAttribute).IsPublic);
        }

        [Fact]
        public void ClassInheritsFromAttributeToExtendMefAttributeModel()
        {
            Assert.True(typeof(Attribute).IsAssignableFrom(typeof(DependsOnAttribute)));
        }

        [Fact]
        public void ClassIsMarkedWithMetadataAttributeToGetRecognizedByMef()
        {
            Assert.NotEmpty(typeof(DependsOnAttribute).GetCustomAttributes(false).OfType<MetadataAttributeAttribute>());
        }

        [Fact]
        public void ClassAllowsAttributeToBeAppliedToClassesOnlyOnce()
        {
            var attributeUsage = typeof(DependsOnAttribute).GetCustomAttributes(false).OfType<AttributeUsageAttribute>().Single();
            Assert.Equal(AttributeTargets.Class, attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            Type[] commandTypes = null;
            var e = Assert.Throws<ArgumentNullException>(() => new DependsOnAttribute(commandTypes));
            Assert.Equal("dependencyCommandTypes", e.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenGivenArrayIsEmptyToPreventUsageErrors()
        {
            var commandTypes = new Type[0];
            var e = Assert.Throws<ArgumentException>(() => new DependsOnAttribute(commandTypes));
            Assert.Equal("dependencyCommandTypes", e.ParamName);
            Assert.False(e.Message.StartsWith(Environment.NewLine));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenGivenArrayWithNullElementsToPreventUsageErrors()
        {
            var commandTypes = new Type[] { null };
            var e = Assert.Throws<ArgumentException>(() => new DependsOnAttribute(commandTypes));
            Assert.Equal("dependencyCommandTypes", e.ParamName);
            Assert.False(e.Message.StartsWith(Environment.NewLine));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenGivenArrayWithUnexpectedTypesToPreventUsageErrors()
        {
            var commandTypes = new Type[] { typeof(object) };
            var e = Assert.Throws<ArgumentException>(() => new DependsOnAttribute(commandTypes));
            Assert.Equal("dependencyCommandTypes", e.ParamName);
            Assert.False(e.Message.StartsWith(Environment.NewLine));
        }

        [Fact]
        public void CommandTypesPropertyIsInitializedByConstructor()
        {
            var attribute = new DependsOnAttribute(typeof(StubCommand));
            Assert.Equal(new[] { typeof(StubCommand) }, attribute.DependencyCommandTypes);
        }

        [Fact]
        public void CommandTypesPropertyIsReadOnlyBecauseAttributeInstanceIsImmutable()
        {
            Assert.Null(typeof(DependsOnAttribute).GetProperty("DependencyCommandTypes").SetMethod);
        }

        [Fact]
        public void AttributeAllowsDependencyCommandTypesToBeDiscoerableThroughMefMetadata()
        {
            CompositionContext container = new ContainerConfiguration()
                .WithPart<TestCommand>()
                .CreateContainer();

            var export = container.GetExport<Lazy<TestCommand, DependsOnMetadata>>();

            Assert.Equal(new[] { typeof(StubCommand) }, export.Metadata.DependencyCommandTypes);
        }

        [Export, DependsOn(typeof(StubCommand))]
        public class TestCommand : Command
        {
        }

        private class DependsOnMetadata
        {
            public IEnumerable<Type> DependencyCommandTypes { get; set; }
        }
    }
}
