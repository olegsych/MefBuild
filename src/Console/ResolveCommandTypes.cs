using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Reflection;

namespace MefBuild
{
    [Shared, Export(typeof(Command))]
    internal class ResolveCommandTypes : Command
    {
        private readonly IEnumerable<Assembly> assemblies;
        private readonly IEnumerable<string> commandTypeNames;
        private readonly ICollection<Type> commandTypes;

        [ImportingConstructor]
        public ResolveCommandTypes(
            [ImportMany(ContractNames.Command)] IEnumerable<string> commandTypeNames,
            [Import] IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
            this.commandTypeNames = commandTypeNames;
            this.commandTypes = new List<Type>();
        }

        [Export(ContractNames.Command)]
        public IEnumerable<Type> CommandTypes 
        {
            get { return this.commandTypes; }
        }

        public override void Execute()
        {
            foreach (string commandTypeName in this.commandTypeNames)
            {
                bool commandTypeFound = false;
                foreach (Assembly assembly in this.assemblies)
                {
                    Type commandType = assembly.GetType(commandTypeName);
                    if (commandType != null)
                    {
                        this.commandTypes.Add(commandType);
                        commandTypeFound = true;
                    }
                }

                if (!commandTypeFound)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Command type '{0}' could not be found", commandTypeName));
                }
            }
        }
    }
}
