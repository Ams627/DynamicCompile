using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.CodeDom.Compiler;

namespace DynamicCompile
{
    class Program
    {
        private static void CompileCode()
        {
            var asm = Assembly.GetExecutingAssembly();
            var sourceFiles = (from type in asm.GetTypes()
                               select Attribute.GetCustomAttributes(type, typeof(SourceMarkerAttribute))).ToList();

            sourceFiles.ToList();

            foreach (var entry in SourceMarkerAttribute.AllSourcesByGroup)
            {
                Console.WriteLine($"group {entry.Key}");
                foreach (var file in entry.Value)
                {
                    Console.WriteLine($"    {file}");
                }
            }

            foreach (var entry in SourceMarkerAttribute.AllSources)
            {
                Console.WriteLine($"entry {entry}");
            }

            var sourceFiles1 = SourceMarkerAttribute.GroupSources("groupA");
            Console.WriteLine();

            var dll = "test001.dll";
            var cparams = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = dll
            };

            cparams.ReferencedAssemblies.Add(asm.Location);
            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromFile(cparams, sourceFiles1);
            Console.WriteLine($"Error count = {r.Errors.Count}");
            foreach (var error in r.Errors)
            {
                Console.WriteLine($"{error}");
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                CompileCode();
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }

        }
    }
}
