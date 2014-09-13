using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using MefBuild;

class Program
{
    static void Main(string[] args)
    {
        string[] assemblyFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        IEnumerable<Assembly> assemblies = assemblyFiles.Select(file => Assembly.LoadFrom(file));

        CompositionContext context = new ContainerConfiguration()
            .WithAssemblies(assemblies)
            .CreateContainer();

        var engine = new Engine(context);
        engine.Execute<Build>();

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
