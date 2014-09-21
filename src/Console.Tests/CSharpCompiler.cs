using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Xunit;

namespace MefBuild
{
    internal static class CSharpCompiler
    {
        public static Assembly CompileInMemory(string code, params Assembly[] references)
        {
            string[] referenceFileNames = references.Select(a => a.Location).ToArray();

            var compilerParameters = new CompilerParameters(referenceFileNames);
            compilerParameters.GenerateInMemory = true;

            var codeProvider = new CSharpCodeProvider();
            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, code);

            Assert.Empty(compilerResults.Errors);

            return compilerResults.CompiledAssembly;
        }
    }
}
