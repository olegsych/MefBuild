using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using MefBuild.Diagnostics;

namespace MefBuild
{
    [Export, Shared, Command(typeof(ExecuteCommands))]
    internal class ExecuteCommands : Command
    {
        [Import(ContractNames.Command)]
        public IEnumerable<Type> CommandTypes { get; set; }

        [Import]
        public IEnumerable<Assembly> Assemblies { get; set; }

        public override void Execute()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithAssembly(typeof(Engine).Assembly)
                .WithPart<ConsoleOutput>()
                .WithAssemblies(this.Assemblies)
                .CreateContainer();
            
            var engine = new Engine(context);
            foreach (Type commandType in this.CommandTypes)
            {
                engine.Execute(commandType);
            }
        }
    }
}
