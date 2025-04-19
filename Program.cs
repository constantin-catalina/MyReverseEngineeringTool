using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reflection;
using assignment_3;

namespace MyReverseEngineeringTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("[ERROR] Please provide a file path.");
                return;
            }
            
            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[ERROR] File {filePath} not found.");
                return;
            }

            string fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension != ".jar" && fileExtension != ".dll" && fileExtension != ".exe")
            {
                Console.WriteLine("[ERROR] Unsupported file type.");
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string folderPath = Path.GetDirectoryName(filePath);
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";
                string assemblyPath = Path.Combine(folderPath, assemblyName);

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }

                return null;
            };

            try
            {
                Assembly assembly = Assembly.LoadFile(filePath);
                ClassDiagramExtractor extractor = new ClassDiagramExtractor(assembly);
                string classDiagram = extractor.GenerateClassDiagram();
                Console.WriteLine(classDiagram);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An error occurred while processing the file: {ex.Message}");
                return;
            }
        }
    }
}