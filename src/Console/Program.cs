using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using MefBuild.Hosting;

namespace MefBuild
{
    /// <summary>
    /// Implements MefBuild.exe logic.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of MefBuild.exe.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Justification = "Temporary")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args", Justification = "Temporary")]
        public static void Main(params string[] args)
        {
            CompositionContext context = new ContainerConfiguration()
                .WithAssembly(typeof(Program).Assembly)
                .WithAssembly(typeof(Engine).Assembly)
                .WithProvider(new CommandLineExportDescriptorProvider(args))
                .CreateContainer();

            var engine = new Engine(context);
            engine.Execute<Execute>();
        }
    }
}
