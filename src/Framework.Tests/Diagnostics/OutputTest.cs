using System;
using System.Composition;
using System.Composition.Hosting;
using MefBuild.Hosting;
using Xunit;

namespace MefBuild.Diagnostics
{
    public class OutputTest
    {
        [Fact]
        public void VerbosityIsImportedFromCompositionContextToAllowSpecifyingItViaCommandLine()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/verbosity=Diagnostic" }))
                .CreateContainer();

            var output = new TestOutput();
            context.SatisfyImports(output);

            Assert.Equal(Verbosity.Diagnostic, output.Verbosity);
        }

        private class TestOutput : Output
        {
            public override void Write(Record record)
            {
                throw new NotImplementedException();
            }
        }
    }
}
