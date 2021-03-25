using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;

namespace DynamicCompile
{
    class Program
    {
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

        private static void CompileCode()
        {
            var asm = Assembly.GetExecutingAssembly();
            var sourceFiles = (from type in asm.GetTypes()
                               select Attribute.GetCustomAttributes(type, typeof(SourceMarkerAttribute))).ToList();
                                                
            sourceFiles.ToList();

            foreach (var entry in SourceMarkerAttribute.GetAllSources)
            {
                Console.WriteLine($"group {entry.Key}");
                foreach (var file in entry.Value)
                {
                    Console.WriteLine($"    {file}");
                }
            }
            var sourceFiles1 = SourceMarkerAttribute.GetGroupSources("groupA");
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
    }
}
