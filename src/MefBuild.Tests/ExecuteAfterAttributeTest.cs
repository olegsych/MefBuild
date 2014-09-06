using System;
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
        public void ClassInheritsFromExecuteAttributeToReuseValidationLogic()
        {
            Assert.True(typeof(ExecuteAttribute).IsAssignableFrom(typeof(ExecuteAfterAttribute)));
        }

        [Fact]
        public void AttributeCanBeAppliedToClassesMultipleTimes()
        {
            var attributeUsage = typeof(ExecuteAfterAttribute).GetCustomAttributes(false).OfType<AttributeUsageAttribute>().Single();
            Assert.Equal(AttributeTargets.Class, attributeUsage.ValidOn);
            Assert.True(attributeUsage.AllowMultiple);
        }

        [Fact]
        public void ConstructorSetsContractNameToBeforeFollowedByFullNameOfGivenType()
        {
            var attribute = new ExecuteBeforeAttribute(typeof(Command));
            Assert.Equal("Before." + typeof(Command).FullName, attribute.ContractName);
        }
    }
}
