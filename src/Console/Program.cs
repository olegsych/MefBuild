using System;
using System.Diagnostics.CodeAnalysis;

namespace MefBuild
{
    /// <summary>
    /// Implements MefBuild.exe logic.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of MefBuild.exe.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Justification = "Temporary")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args", Justification = "Temporary")]
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
