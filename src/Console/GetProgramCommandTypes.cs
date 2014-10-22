using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace MefBuild
{
    [Shared, Export]
    internal class GetProgramCommandTypes : Command
    {
        private IEnumerable<Type> commandTypes;

        [Export]
        public IEnumerable<Type> CommandTypes 
        {
            get { return this.commandTypes; }
        }

        public override void Execute()
        {
            this.commandTypes = typeof(Program).Assembly.DefinedTypes.Where(t => typeof(Command).IsAssignableFrom(t));
        }
    }
}
