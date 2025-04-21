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

            if (options.ShowAttributes)
            {
                AppendProperties(classDiagram, type);
                AppendFields(classDiagram, type);
            }

            if (options.ShowMethods)
            {
                AppendConstructors(classDiagram, type);
                AppendMethods(classDiagram, type);
            }
        }

        private void AppendProperties(StringBuilder classDiagram, TypeModel type)
        {
            if (type.Properties.Count > 0)
            {
                classDiagram.AppendLine("Properties:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (PropertyModel property in type.Properties)
                {
                    string modifiers = GetModifiers(property.AccessModifier, property.IsStatic, property.IsAbstract, property.IsVirtual);
                    string getterSetter = GetGetterSetter(property.HasGetter, property.HasSetter);

                    classDiagram.AppendLine($"{modifiers} {property.TypeName} {property.Name} {{ {getterSetter}}}");
                    classDiagram.AppendLine();
                }
            }
        }

        private void AppendFields(StringBuilder classDiagram, TypeModel type)
        {
            if (type.Fields.Count > 0)
            {
                classDiagram.AppendLine("Fields:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (FieldModel field in type.Fields)
                {
                    string modifiers = GetModifiers(field.AccessModifier, field.IsStatic, false, false, field.IsReadOnly);
                    classDiagram.AppendLine($"{modifiers} {field.TypeName} {field.Name}");
                    classDiagram.AppendLine();
                }
            }
        }

        private void AppendConstructors(StringBuilder classDiagram, TypeModel type)
        {
            if (type.Constructors.Count > 0)
            {
                classDiagram.AppendLine("Constructors:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (ConstructorModel constructor in type.Constructors)
                {
                    string modifiers = GetModifiers(constructor.AccessModifier, constructor.IsStatic);
                    string parameterList = GetParameterList(constructor.Parameters);

                    classDiagram.AppendLine($"{modifiers} {type.Name}({parameterList})");
                    classDiagram.AppendLine();
                }
            }
        }

        private void AppendMethods(StringBuilder classDiagram, TypeModel type)
        {
            if (type.Methods.Count > 0)
            {
                classDiagram.AppendLine("Methods:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (MethodModel method in type.Methods)
                {
                    string modifiers = GetModifiers(method.AccessModifier, method.IsStatic, method.IsAbstract, method.IsVirtual, method.IsOverride);
                    string parameterList = GetParameterList(method.Parameters);

                    classDiagram.AppendLine($"{modifiers} {method.ReturnTypeName} {method.Name}({parameterList})");
                    classDiagram.AppendLine();
                }
            }
        }

        private string GetModifiers(string accessModifier, bool isStatic, bool isAbstract = false, bool isVirtual = false, bool isOverride = false)
        {
            StringBuilder modifiers = new StringBuilder();

            if (accessModifier != null)
            {
                modifiers.Append(accessModifier);
            }

            if (isStatic)
            {
                modifiers.Append(" static");
            }

            if (isAbstract)
            {
                modifiers.Append(" abstract");
            }

            if (isVirtual)
            {
                modifiers.Append(" virtual");
            }

            if (isOverride)
            {
                modifiers.Append(" override");
            }

            return modifiers.ToString().Trim();
        }

        private string GetGetterSetter(bool hasGetter, bool hasSetter)
        {
            string getterSetter = "";

            if (hasGetter)
            {
                getterSetter += "get; ";
            }

            if (hasSetter)
            {
                getterSetter += "set; ";
            }

            return getterSetter.Trim();
        }

        private string GetParameterList(List<ParameterModel> parameters)
        {
            List<string> paramStrings = new List<string>();

            foreach (ParameterModel param in parameters)
            {
                paramStrings.Add($"{param.TypeName} {param.Name}");
            }

            return string.Join(", ", paramStrings);
        }

        private void AppendRelationships(StringBuilder classDiagram, List<RelationshipModel> relationships)
        {
            List<RelationshipModel> inheritanceRels = new List<RelationshipModel>();
            List<RelationshipModel> implementationRels = new List<RelationshipModel>();
            List<RelationshipModel> associationRels = new List<RelationshipModel>();
            List<RelationshipModel> dependencyRels = new List<RelationshipModel>();

            foreach (var rel in relationships)
            {
                if (rel.Type == RelationshipModel.RelationshipType.Inheritance)
                {
                    inheritanceRels.Add(rel);
                }
                else if (rel.Type == RelationshipModel.RelationshipType.Implementation)
                {
                    implementationRels.Add(rel);
                }
                else if (rel.Type == RelationshipModel.RelationshipType.Association)
                {
                    associationRels.Add(rel);
                }
                else if (rel.Type == RelationshipModel.RelationshipType.Dependency)
                {
                    dependencyRels.Add(rel);
                }
            }

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