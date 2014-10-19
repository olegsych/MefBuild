using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
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
        public static void Main(params string[] args)
        {
            IEnumerable<Type> commandTypes = typeof(Program).Assembly.DefinedTypes
                .Where(t => typeof(Command).IsAssignableFrom(t) 
                    && t.GetCustomAttributes<ExportAttribute>().Any());

            var configuration = new ContainerConfiguration()
                .WithParts(commandTypes)
                .WithAssembly(typeof(Engine).Assembly)
                .WithProvider(new CommandLineExportDescriptorProvider(args));

            var engine = new Engine(configuration);

            if (args != null && args.Length > 0)
            {
                engine.Execute<Execute>();
            }
            else
            {
                engine.Execute<Usage>();
            }
        }
    }
}
