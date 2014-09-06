using System;
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
        public void ClassInheritsFromExecuteAttributeToReuseValidationLogic()
        {
            Assert.True(typeof(ExecuteAttribute).IsAssignableFrom(typeof(ExecuteBeforeAttribute)));
        }

        [Fact]
        public void AttributeCanBeAppliedToClassesMultipleTimes()
        {
            var attributeUsage = typeof(ExecuteBeforeAttribute).GetCustomAttributes(false).OfType<AttributeUsageAttribute>().Single();
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
