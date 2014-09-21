using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;

namespace MefBuild
{
    [Export, Shared]
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
