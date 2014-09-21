using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Reflection;

namespace MefBuild
{
    [Shared, Export]
    internal class ResolveCommandTypes : Command
    {
        private readonly ICollection<Type> commandTypes;

        public ResolveCommandTypes()
        {
            this.commandTypes = new List<Type>();
        }

        [ImportMany(ContractNames.Command)]
        public IEnumerable<string> CommandTypeNames { get; set; }

        [Import]
        public IEnumerable<Assembly> Assemblies { get; set; }

        [Export(ContractNames.Command)]
        public IEnumerable<Type> CommandTypes 
        {
            get { return this.commandTypes; }
        }

        public override void Execute()
        {
            base.Execute();

            foreach (string commandTypeName in this.CommandTypeNames)
            {
                bool commandTypeFound = false;
                foreach (Assembly assembly in this.Assemblies)
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
