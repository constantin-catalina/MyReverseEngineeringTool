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

            foreach (TypeModel type in model.Types)
            {
                AppendTypeDefinition(yumlCode, type);
            }

            foreach (RelationshipModel relationship in model.Relationships)
            {
                AppendRelationship(yumlCode, relationship);
            }

            return yumlCode.ToString();
        }

        private void AppendTypeDefinition(StringBuilder yumlCode, TypeModel type)
        {
            string typeName = type.Name;

            StringBuilder typeDefinition = new StringBuilder();
            typeDefinition.Append('[');

            if (type.IsInterface)
            {
                typeDefinition.Append("<<interface>>");
            }

            typeDefinition.Append(typeName);

            // Add attributes and methods if enabled
            if (options.ShowAttributes || options.ShowMethods)
            {
                typeDefinition.Append('|');

                if (options.ShowAttributes)
                {
                    AppendFieldsAndProperties(typeDefinition, type);
                }

                if (options.ShowMethods)
                {
                    AppendMethodsAndConstructors(typeDefinition, type);
                }
            }

            typeDefinition.Append(']');
            yumlCode.AppendLine(typeDefinition.ToString());
        }

        private void AppendFieldsAndProperties(StringBuilder typeDefinition, TypeModel type)
        {
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

        private void AppendMethodsAndConstructors(StringBuilder typeDefinition, TypeModel type)
        {
            // Add constructors
            foreach (ConstructorModel ctor in type.Constructors)
            {
                string visibility = GetVisibilitySymbol(ctor.AccessModifier);
                typeDefinition.Append($"{visibility}{type.Name}(");

                // Add parameters
                string parameterList = GetParameterList(ctor.Parameters);
                typeDefinition.Append(parameterList);

                typeDefinition.Append(");");
            }

            // Add methods
            foreach (MethodModel method in type.Methods)
            {
                string visibility = GetVisibilitySymbol(method.AccessModifier);
                typeDefinition.Append($"{visibility}{method.Name}(");

                // Add parameters
                string parameterList = GetParameterList(method.Parameters);
                typeDefinition.Append(parameterList);

                typeDefinition.Append($"):{method.ReturnTypeName};");
            }

            // Remove the last semicolon
            if (typeDefinition[typeDefinition.Length - 1] == ';')
            {
                typeDefinition.Length--;
            }
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

        private string GetParameterList(List<ParameterModel> parameters)
        {
            StringBuilder paramList = new StringBuilder();
            foreach (ParameterModel param in parameters)
            {
                if (paramList.Length > 0)
                    paramList.Append(", ");
                paramList.Append($"{param.Name}:{param.TypeName}");
            }
            return paramList.ToString();
        }
    }
}