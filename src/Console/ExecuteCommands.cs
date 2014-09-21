using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace MefBuild
{
    [Export, Shared]
    internal class ExecuteCommands : Command
    {
        [Import(ContractNames.Command)]
        public IEnumerable<Type> CommandTypes { get; set; }

        [Import]
        public IEnumerable<Assembly> Assemblies { get; set; }

        public override void Execute()
        {
            CompositionContext context = new ContainerConfiguration()
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
