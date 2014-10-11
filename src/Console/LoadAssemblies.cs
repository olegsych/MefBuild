using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MefBuild
{
    [Shared, Command]
    internal class LoadAssemblies : Command
    {
        private readonly IEnumerable<string> assemblyFileNames;
        private readonly ICollection<Assembly> assemblies;

        [ImportingConstructor]
        public LoadAssemblies([ImportMany(ContractNames.Assembly)] IEnumerable<string> assemblyFileNames)
        {
            this.assemblyFileNames = assemblyFileNames;
            this.assemblies = new List<Assembly>();
        }

        [Export]
        public IEnumerable<Assembly> Assemblies 
        {
            get { return this.assemblies; }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "LoadFrom allows loading command assemblies from outside of MefBuild's own probing path.")]
        public override void Execute()
        {
            foreach (string assemblyFileName in this.assemblyFileNames)
            {
                Assembly assembly = Assembly.LoadFrom(assemblyFileName);
                this.assemblies.Add(assembly);
            }
        }
    }
}
