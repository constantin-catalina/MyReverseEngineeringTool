using System;
using System.Collections.Generic;
using System.Text;

namespace assignment_3
{
    public class TextFormatter : IDiagramFormatter
    {
        private readonly DiagramOptions options;

        public TextFormatter(DiagramOptions options)
        {
            this.options = options;
        }

        public string FileExtension => "txt";

        public string FormatClassDiagram(DiagramModel model)
        {
            StringBuilder classDiagram = new StringBuilder();

            classDiagram.AppendLine(new string('=', 80));
            classDiagram.AppendLine($"Class Diagram for: {model.AssemblyName}");
            classDiagram.AppendLine(new string('=', 80));
            classDiagram.AppendLine();

            foreach (TypeModel type in model.Types)
            {
                AppendTypeDetails(classDiagram, type);
                classDiagram.AppendLine();
            }

            classDiagram.AppendLine("Relationships:");
            classDiagram.AppendLine(new string('=', 80));
            classDiagram.AppendLine();
            AppendRelationships(classDiagram, model.Relationships);

            return classDiagram.ToString();
        }

        private void AppendTypeDetails(StringBuilder classDiagram, TypeModel type)
        {
            string typeName = options.UseFullyQualifiedNames ? type.FullName : type.Name;

            if (type.IsInterface)
            {
                classDiagram.AppendLine($"Interface: {typeName}");
                classDiagram.AppendLine(new string('=', 80));
            }
            else
            {
                classDiagram.AppendLine($"Class: {typeName}");
                classDiagram.AppendLine(new string('=', 80));
            }
            classDiagram.AppendLine($"Namespace: {type.Namespace}");
            classDiagram.AppendLine();

            if(options.ShowAttributes)
            {
                // Properties
                if (type.Properties.Count > 0)
                {
                    classDiagram.AppendLine("Properties:");
                    classDiagram.AppendLine(new string('-', 80));
                    foreach (PropertyModel property in type.Properties)
                    {
                        string modifiers = $"{property.AccessModifier} {(property.IsStatic ? "static " : "")}{(property.IsAbstract ? "abstract " : "")}{(property.IsVirtual ? "virtual " : "")}";
                        classDiagram.AppendLine($"{modifiers}{property.TypeName} {property.Name} {{ {(property.HasGetter ? "get; " : "")}{(property.HasSetter ? "set; " : "")}}}");
                        classDiagram.AppendLine();
                    }
                }

                // Fields
                if (type.Fields.Count > 0)
                {
                    classDiagram.AppendLine("Fields:");
                    classDiagram.AppendLine(new string('-', 80));
                    foreach (FieldModel field in type.Fields)
                    {
                        string modifiers = $"{field.AccessModifier} {(field.IsStatic ? "static " : "")}{(field.IsReadOnly ? "readonly " : "")}";
                        classDiagram.AppendLine($"{modifiers}{field.TypeName} {field.Name}");
                        classDiagram.AppendLine();
                    }
                }

            }
            if (options.ShowMethods)
            {
                // Constructors
                if (type.Constructors.Count > 0)
                {
                    classDiagram.AppendLine("Constructors:");
                    classDiagram.AppendLine(new string('-', 80));
                    foreach (ConstructorModel constructor in type.Constructors)
                    {
                        string modifiers = $"{constructor.AccessModifier} {(constructor.IsStatic ? "static " : "")}";

                        List<string> paramStrings = new List<string>();
                        foreach (ParameterModel param in constructor.Parameters)
                        {
                            paramStrings.Add($"{param.TypeName} {param.Name}");
                        }
                        string parameterList = string.Join(", ", paramStrings);

                        classDiagram.AppendLine($"{modifiers}{type.Name}({parameterList})");
                        classDiagram.AppendLine();
                    }
                }

                // Methods
                if (type.Methods.Count > 0)
                {
                    classDiagram.AppendLine("Methods:");
                    classDiagram.AppendLine(new string('-', 80));
                    foreach (MethodModel method in type.Methods)
                    {
                        string modifiers = $"{method.AccessModifier} {(method.IsStatic ? "static " : "")}{(method.IsAbstract ? "abstract " : "")}{(method.IsVirtual ? "virtual " : "")}{(method.IsOverride ? "override " : "")}";

                        List<string> paramStrings = new List<string>();
                        foreach (ParameterModel param in method.Parameters)
                        {
                            paramStrings.Add($"{param.TypeName} {param.Name}");
                        }
                        string parameterList = string.Join(", ", paramStrings);

                        classDiagram.AppendLine($"{modifiers}{method.ReturnTypeName} {method.Name}({parameterList})");
                        classDiagram.AppendLine();
                    }
                }
            }
        }

        private void AppendRelationships(StringBuilder classDiagram, List<RelationshipModel> relationships)
        {
            // Group relationships by type
            var inheritanceRels = relationships.FindAll(r => r.Type == RelationshipModel.RelationshipType.Inheritance);
            var implementationRels = relationships.FindAll(r => r.Type == RelationshipModel.RelationshipType.Implementation);
            var associationRels = relationships.FindAll(r => r.Type == RelationshipModel.RelationshipType.Association);
            var dependencyRels = relationships.FindAll(r => r.Type == RelationshipModel.RelationshipType.Dependency);

            // Inheritance
            if (inheritanceRels.Count > 0)
            {
                classDiagram.AppendLine("Inheritance:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (var rel in inheritanceRels)
                {
                    classDiagram.AppendLine($"{rel.SourceTypeName} extends {rel.TargetTypeName}");
                    classDiagram.AppendLine();
                }
            }

            // Implementation
            if (implementationRels.Count > 0)
            {
                classDiagram.AppendLine("Implementation:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (var rel in implementationRels)
                {
                    classDiagram.AppendLine($"{rel.SourceTypeName} implements {rel.TargetTypeName}");
                    classDiagram.AppendLine();
                }
            }

            // Association
            if (associationRels.Count > 0)
            {
                classDiagram.AppendLine("Association:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (var rel in associationRels)
                {
                    classDiagram.AppendLine($"{rel.SourceTypeName} has association with {rel.TargetTypeName}");
                    classDiagram.AppendLine();
                }
            }

            // Dependency
            if (dependencyRels.Count > 0)
            {
                classDiagram.AppendLine("Dependency:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (var rel in dependencyRels)
                {
                    classDiagram.AppendLine($"{rel.SourceTypeName} depends on {rel.TargetTypeName}");
                    classDiagram.AppendLine();
                }
            }
        }
    }
}