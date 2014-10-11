using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using MefBuild.Diagnostics;

namespace MefBuild
{
    [Command]
    internal class ExecuteCommands : Command
    {
        private readonly IEnumerable<Type> commandTypes;
        private readonly IEnumerable<Assembly> assemblies;

        [ImportingConstructor]
        public ExecuteCommands(
            [Import(ContractNames.Command)] IEnumerable<Type> commandTypes,
            [Import] IEnumerable<Assembly> assemblies)
        {
            this.commandTypes = commandTypes;
            this.assemblies = assemblies;
        }

        public override void Execute()
        {
            var configuration = new ContainerConfiguration()
                .WithAssembly(typeof(Engine).Assembly)
                .WithPart<ConsoleOutput>()
                .WithAssemblies(this.assemblies);
            
            var engine = new Engine(configuration);
            foreach (Type commandType in this.commandTypes)
            {
                engine.Execute(commandType);
            }
        }
    }
}
