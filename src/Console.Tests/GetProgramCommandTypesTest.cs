using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class GetProgramCommandTypesTest
    {
        [Fact]
        public void ExecutePopulatesCommandTypesCollectionWithCommandTypesTypesDefinedInProgramAssembly()
        {
            var command = new GetProgramCommandTypes();

            command.Execute();

            var expectedTypes = typeof(Program).Assembly.DefinedTypes.Where(t => typeof(Command).IsAssignableFrom(t));
            Assert.Equal(expectedTypes, command.CommandTypes);
        }

        [Fact]
        public void CommandTypesCollectionIsExportedForConsumptionByOtherCommands()
        {
            var context = new ContainerConfiguration().WithPart<GetProgramCommandTypes>().CreateContainer();
            var command = context.GetExport<GetProgramCommandTypes>();
            command.Execute();

            var exportedTypes = context.GetExport<IEnumerable<Type>>();

            Assert.Same(command.CommandTypes, exportedTypes);
        }
    }
}
