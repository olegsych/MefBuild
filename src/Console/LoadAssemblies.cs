using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MefBuild
{
    [Shared, Command]
    internal class LoadAssemblies : Command
    {
        private readonly ICollection<Assembly> assemblies = new List<Assembly>();

        [ImportMany(ContractNames.Assembly)]
        public IEnumerable<string> AssemblyFileNames { get; set; }

        [Export]
        public IEnumerable<Assembly> Assemblies 
        {
            get { return this.assemblies; }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "LoadFrom allows loading command assemblies from outside of MefBuild's own probing path.")]
        public override void Execute()
        {
            foreach (string assemblyFileName in this.AssemblyFileNames)
            {
                Assembly assembly = Assembly.LoadFrom(assemblyFileName);
                this.assemblies.Add(assembly);
            }
        }
    }
}
