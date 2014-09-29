using System;

namespace MefBuild.Hosting
{
    internal class CommandLineArgument
    {
        private readonly string name;
        private readonly string value;
        private readonly string original;

        public CommandLineArgument(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            this.original = argument;

            argument = argument.TrimStart('/');
            int indexOfSeparator = argument.IndexOf('=');
            if (indexOfSeparator >= 0)
            {
                this.name = argument.Substring(0, indexOfSeparator);
                this.value = argument.Substring(indexOfSeparator + 1);
            }
            else
            {
                this.name = argument;
                this.value = string.Empty;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Original 
        {
            get { return this.original; }
        }

        public string Value 
        {
            get { return this.value; }
        }
    }
}
