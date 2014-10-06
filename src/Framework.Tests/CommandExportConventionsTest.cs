using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MefBuild
{
    public static class CommandExportConventionsTest
    {
        [Fact]
        public static void ClassIsInternalAndUsedOnlyByEngine()
        {
            Assert.False(typeof(CommandExportConventions).IsPublic);
        }

        [Fact]
        public static void ClassInheritsFromAttributedModelProviderForCompatibilityWithMefInfrastructure()
        {
            Assert.True(typeof(AttributedModelProvider).IsAssignableFrom(typeof(CommandExportConventions)));
        }

        public static class GetCustomAttributesOfParameterInfo
        {
            [Fact]
            public static void ThrowsArgumentNullExceptionWhenTypeIsNullToPreventUsageErrors()
            {
                var provider = new CommandExportConventions();
                var e = Assert.Throws<ArgumentNullException>(() => provider.GetCustomAttributes(null, default(ParameterInfo)));
                Assert.Equal("reflectedType", e.ParamName);
            }

            [Fact]
            public static void ThrowsArgumentNullExceptionWhenParameterIsNullToPreventUsageErrors()
            {
                var provider = new CommandExportConventions();
                var e = Assert.Throws<ArgumentNullException>(() => provider.GetCustomAttributes(typeof(object), (ParameterInfo)null));
                Assert.Equal("parameter", e.ParamName);
            }

            [Fact]
            public static void ReturnsCustomAttributesAppliedToParameterInReflectedType()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(TestClass);
                ConstructorInfo constructor = reflectedType.GetConstructor(new[] { typeof(IEnumerable<object>) });
                ParameterInfo parameter = constructor.GetParameters().First();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, parameter);

                Assert.Equal(new[] { typeof(ImportManyAttribute) }, attributes.Select(a => a.GetType()));
            }

            public class TestClass
            {
                public TestClass([ImportMany]IEnumerable<object> testParameter)
                {
                }
            }
        }

        public static class GetCustomAttributesOfMemberInfo
        {
            [Fact]
            public static void ThrowsArgumentNullExceptionWhenTypeIsNullToPreventUsageErrors()
            {
                var provider = new CommandExportConventions();
                var e = Assert.Throws<ArgumentNullException>(() => provider.GetCustomAttributes(null, typeof(object)));
                Assert.Equal("reflectedType", e.ParamName);
            }

            [Fact]
            public static void ThrowsArgumentNullExceptionWhenMemberIsNullToPreventUsageErrors()
            {
                var provider = new CommandExportConventions();
                var e = Assert.Throws<ArgumentNullException>(() => provider.GetCustomAttributes(typeof(object), (MemberInfo)null));
                Assert.Equal("member", e.ParamName);
            }

            [Fact]
            public static void ReturnsAttributeOfSpecifiedMember()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(TestClass);
                PropertyInfo member = reflectedType.GetProperty("TestProperty");
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(new[] { typeof(ImportAttribute) }, attributes.Select(a => a.GetType()));
            }

            [Fact]
            public static void ReturnsAttributesOfSpecifiedType()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(TestClass);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(new[] { typeof(ExportAttribute) }, attributes.Select(a => a.GetType()));
            }

            [Fact]
            public static void DoesNotReturnAttributesOfMemberDeclaredInBaseClass() // why does DirectAttributeContext work this way?
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(DerivedTestClass);
                PropertyInfo member = reflectedType.GetProperty("TestProperty");
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Empty(attributes);
            }

            [Fact]
            public static void AddsExportAttributeWithCommandContractTypeRequiredByEngineToTypesWithCommandAttribute()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(ExportedCommand);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<ExportAttribute>().Count(a => a.ContractType == typeof(Command)));
            }

            [Fact]
            public static void DoesNotAddSecondExportAttributeWithCommandContractTypeToTypesThatAlreadyHaveIt()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(CommandWithAttributes);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<ExportAttribute>().Count(a => a.ContractType == typeof(Command)));
            }

            [Fact]
            public static void AddsSharedAttributeToTypesWithCommandAttributeToPreventMultipleExecutionOfSingleCommandType()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(ExportedCommand);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<SharedAttribute>().Count());
            }

            [Fact]
            public static void DoesNotAddSecondSharedAttributeToTypesThatAlreadyHaveIt()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(CommandWithAttributes);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<SharedAttribute>().Count());
            }

            [Fact]
            public static void AddsExportAttributeWithoutContractTypeToTypesWithCommandAttributeForUseInTests()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(ExportedCommand);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<ExportAttribute>().Count(a => a.ContractType == null));
            }

            [Fact]
            public static void DoesNotAddSecondExportAttributeToTypesThatAlreadyHaveIt()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(CommandWithAttributes);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<ExportAttribute>().Count(a => a.ContractType == null));
            }

            [Fact]
            public static void AddsCommandTypeMetadataRequiredByEngineToTypesWithCommandAttribute()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(ExportedCommand);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(1, attributes.OfType<ExportMetadataAttribute>().Count(a => a.Name == "CommandType" && (Type)a.Value == typeof(ExportedCommand)));
            }

            [Fact]
            public static void DoesNotAddCommandTypeMetadataToTypesThatDontHaveCommandAttribute()
            {
                var provider = new CommandExportConventions();

                Type reflectedType = typeof(UnexportedCommand);
                TypeInfo member = reflectedType.GetTypeInfo();
                IEnumerable<Attribute> attributes = provider.GetCustomAttributes(reflectedType, member);

                Assert.Equal(0, attributes.OfType<ExportMetadataAttribute>().Count(a => a.Name == "CommandType"));
            }

            [Export]
            public class TestClass
            {
                [Import]
                public virtual IEnumerable<object> TestProperty { get; set; }
            }

            public class DerivedTestClass : TestClass
            {
            }

            [Command]
            public class ExportedCommand : Command
            { 
            }

            public class UnexportedCommand : Command
            {
            }

            [Command, Shared, Export(typeof(Command)), Export]
            public class CommandWithAttributes : Command
            {
            }
        }
    }
}
