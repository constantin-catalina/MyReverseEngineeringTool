using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.IO.Compression;
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
            if (fileExtension != ".dll" && fileExtension != ".exe")
            {
                Console.WriteLine("[ERROR] Unsupported file type.");
                return;
            }

            DiagramOptions options = new DiagramOptions();
            string outputFormat = "text";
            string outputPath = null;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i].ToLower();

                if (arg == "--format" || arg == "-f")
                {
                    if (i + 1 < args.Length)
                    {
                        outputFormat = args[++i].ToLower();
                    }
                }
                else if (arg == "--output" || arg == "-o")
                {
                    if (i + 1 < args.Length)
                    {
                        outputPath = args[++i];
                    }
                }
                else if (arg == "--fully-qualified" || arg == "-fq")
                {
                    if (outputFormat == "plantuml" || outputFormat == "yuml")
                    {
                        Console.WriteLine("[WARNING] Fully qualified names are not supported for this format.");
                    }
                    else
                    {
                        options.UseFullyQualifiedNames = true;
                    }
                }
                else if (arg == "--no-methods" || arg == "-nm")
                {
                    options.ShowMethods = false;
                }
                else if (arg == "--no-attributes" || arg == "-na")
                {
                    options.ShowAttributes = false;
                }
                else if (arg == "--ignore" || arg == "-i")
                {
                    if (i + 1 < args.Length)
                    {
                        options.IgnoredClasses.Add(args[++i]);
                    }
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string folderPath = Path.GetDirectoryName(filePath);

                string ddlName = new AssemblyName(args.Name).Name + ".dll";
                string ddlPath = Path.Combine(folderPath, ddlName);

                if (File.Exists(ddlPath))
                {
                    return Assembly.LoadFrom(ddlPath);
                }

                string exeName = new AssemblyName(args.Name).Name + ".exe";
                string exePath = Path.Combine(folderPath, exeName);

                if (File.Exists(exePath))
                {
                    return Assembly.LoadFrom(exePath);
                }

                return null;
            };

            try
            {
                Assembly assembly = Assembly.LoadFile(filePath);
                ClassDiagramExtractor extractor = new ClassDiagramExtractor(assembly, options);

                // Create formatter based on output format
                IDiagramFormatter formatter = Factory.CreateFormatter(outputFormat, options);
                string classDiagram = extractor.GenerateClassDiagram(formatter);

                // Output the class diagram
                if (outputPath != null)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    string formatterExtension = formatter.FileExtension.ToLower();
                    string outputFileName;

                    if (formatterExtension == "txt")
                    {
                        outputFileName = $"{fileNameWithoutExtension}.txt";
                    }
                    else if (formatterExtension == "yuml")
                    {
                        outputFileName = $"{fileNameWithoutExtension}-yuml.txt";
                    }
                    else
                    {
                        outputFileName = $"{fileNameWithoutExtension}-plantuml.txt";
                    }
                        string outputFilePath = Path.Combine(outputPath, outputFileName);

                    File.WriteAllText(outputFilePath, classDiagram);
                }
                else
                {
                    Console.WriteLine(classDiagram);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An error occurred while processing the file: {ex.Message}");
                return;
            }
        }
    }
}