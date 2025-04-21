using System;
using System.Collections.Generic;
using System.Text;

namespace assignment_3
{
    public class YumlFormatter : IDiagramFormatter
    {
        private readonly DiagramOptions options;

        public YumlFormatter(DiagramOptions options)
        {
            this.options = options;
        }

        public string FileExtension => "yuml";

        public string FormatClassDiagram(DiagramModel model)
        {
            StringBuilder yumlCode = new StringBuilder();

            // Define classes and interfaces first
            foreach (TypeModel type in model.Types)
            {
                AppendTypeDefinition(yumlCode, type);
            }

            // Then add relationships
            foreach (RelationshipModel relationship in model.Relationships)
            {
                AppendRelationship(yumlCode, relationship);
            }

            return yumlCode.ToString();
        }

        private void AppendTypeDefinition(StringBuilder yumlCode, TypeModel type)
        {
            string typeName = type.Name;

            // Format: [ClassName|+attribute1:type;-attribute2:type|+method1():returnType;#method2():returnType]
            StringBuilder typeDefinition = new StringBuilder();
            typeDefinition.Append('[');

            // Add stereotype for interfaces
            if (type.IsInterface)
            {
                typeDefinition.Append("<<interface>>");
            }

            typeDefinition.Append(typeName);

            // Add attributes if enabled
            if (options.ShowAttributes && (type.Properties.Count > 0 || type.Fields.Count > 0))
            {
                typeDefinition.Append('|');

                // Add fields
                foreach (FieldModel field in type.Fields)
                {
                    string visibility = GetVisibilitySymbol(field.AccessModifier);
                    typeDefinition.Append($"{visibility}{field.Name}:{field.TypeName};");
                }

                // Add properties
                foreach (PropertyModel property in type.Properties)
                {
                    string visibility = GetVisibilitySymbol(property.AccessModifier);
                    typeDefinition.Append($"{visibility}{property.Name}:{property.TypeName};");
                }

                // Remove the last semicolon
                if (typeDefinition[typeDefinition.Length - 1] == ';')
                {
                    typeDefinition.Length--;
                }
            }

            // Add methods if enabled
            if (options.ShowMethods && (type.Methods.Count > 0 || type.Constructors.Count > 0))
            {
                typeDefinition.Append('|');

                // Add constructors
                foreach (ConstructorModel ctor in type.Constructors)
                {
                    string visibility = GetVisibilitySymbol(ctor.AccessModifier);
                    typeDefinition.Append($"{visibility}{type.Name}(");

                    // Add parameters
                    List<string> paramStrings = new List<string>();
                    foreach (ParameterModel param in ctor.Parameters)
                    {
                        paramStrings.Add($"{param.Name}:{param.TypeName}");
                    }
                    typeDefinition.Append(string.Join(", ", paramStrings));

                    typeDefinition.Append(");");
                }

                // Add methods
                foreach (MethodModel method in type.Methods)
                {
                    string visibility = GetVisibilitySymbol(method.AccessModifier);
                    typeDefinition.Append($"{visibility}{method.Name}(");

                    // Add parameters
                    List<string> paramStrings = new List<string>();
                    foreach (ParameterModel param in method.Parameters)
                    {
                        paramStrings.Add($"{param.Name}:{param.TypeName}");
                    }
                    typeDefinition.Append(string.Join(", ", paramStrings));

                    typeDefinition.Append($"):{method.ReturnTypeName};");
                }

                // Remove the last semicolon
                if (typeDefinition[typeDefinition.Length - 1] == ';')
                {
                    typeDefinition.Length--;
                }
            }

            typeDefinition.Append(']');
            yumlCode.AppendLine(typeDefinition.ToString());
        }

        private void AppendRelationship(StringBuilder yumlCode, RelationshipModel relationship)
        {
            string relationshipSymbol;

            switch (relationship.Type)
            {
                case RelationshipModel.RelationshipType.Inheritance:
                    relationshipSymbol = "^-";
                    break;
                case RelationshipModel.RelationshipType.Implementation:
                    relationshipSymbol = "^-.-";
                    break;
                case RelationshipModel.RelationshipType.Association:
                    relationshipSymbol = "->";
                    break;
                case RelationshipModel.RelationshipType.Dependency:
                    relationshipSymbol = "-.->";
                    break;
                default:
                    relationshipSymbol = "-";
                    break;
            }

            yumlCode.AppendLine($"[{relationship.SourceTypeName}]{relationshipSymbol}[{relationship.TargetTypeName}]");
        }

        private string GetVisibilitySymbol(string accessModifier)
        {
            switch (accessModifier)
            {
                case "public":
                    return "+";
                case "private":
                    return "-";
                case "protected":
                    return "#";
                case "internal":
                    return "~";
                case "protected internal":
                    return "#";
                default:
                    return "-";
            }
        }
    }
}