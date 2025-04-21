using System;
using System.Collections.Generic;
using System.Text;

namespace assignment_3
{
    public class PlantumlFormatter : IDiagramFormatter
    {
        private readonly DiagramOptions options;

        public PlantumlFormatter(DiagramOptions options)
        {
            this.options = options;
        }

        public string FileExtension => "plantuml";

        public string FormatClassDiagram(DiagramModel model)
        {
            StringBuilder plantUml = new StringBuilder();

            // Start the UML diagram
            plantUml.AppendLine("@startuml");
            plantUml.AppendLine($"title Class Diagram for: {model.AssemblyName}");
            plantUml.AppendLine();

            // Define classes and interfaces
            foreach (TypeModel type in model.Types)
            {
                AppendTypeDefinition(plantUml, type);
                plantUml.AppendLine();
            }

            // Define relationships
            foreach (RelationshipModel relationship in model.Relationships)
            {
                AppendRelationship(plantUml, relationship);
            }

            // End the UML diagram
            plantUml.AppendLine("@enduml");

            return plantUml.ToString();
        }

        private void AppendTypeDefinition(StringBuilder plantUml, TypeModel type)
        {
            if (type.IsInterface)
            {
                plantUml.AppendLine($"interface {type.Name} {{");
            }
            else
            {
                plantUml.AppendLine($"class {type.Name} {{");
            }

            // Add fields if enabled
            if (options.ShowAttributes)
            {
                AppendFields(plantUml, type);
                AppendProperties(plantUml, type);
            }

            // Add methods if enabled
            if (options.ShowMethods)
            {
                AppendConstructors(plantUml, type);
                AppendMethods(plantUml, type);
            }

            plantUml.AppendLine("}");
        }

        private void AppendFields(StringBuilder plantUml, TypeModel type)
        {
            foreach (FieldModel field in type.Fields)
            {
                string visibility = GetVisibilitySymbol(field.AccessModifier);
                string staticModifier = field.IsStatic ? "{static} " : "";
                plantUml.AppendLine($"  {visibility} {staticModifier}{field.TypeName} {field.Name}");
            }
        }

        private void AppendProperties(StringBuilder plantUml, TypeModel type)
        {
            foreach (PropertyModel property in type.Properties)
            {
                string visibility = GetVisibilitySymbol(property.AccessModifier);
                string staticModifier = property.IsStatic ? "{static} " : "";
                string accessors = GetPropertyAccessors(property);
                plantUml.AppendLine($"  {visibility} {staticModifier} {property.TypeName} {property.Name} {{{accessors}}}");
            }
        }

        private void AppendConstructors(StringBuilder plantUml, TypeModel type)
        {
            foreach (ConstructorModel constructor in type.Constructors)
            {
                string visibility = GetVisibilitySymbol(constructor.AccessModifier);
                string staticModifier = constructor.IsStatic ? "{static} " : "";
                string parameterList = GetParameterList(constructor.Parameters);

                plantUml.AppendLine($"  {visibility} {staticModifier}{type.Name}({parameterList})");
            }
        }

        private void AppendMethods(StringBuilder plantUml, TypeModel type)
        {
            foreach (MethodModel method in type.Methods)
            {
                string visibility = GetVisibilitySymbol(method.AccessModifier);
                string staticModifier = method.IsStatic ? "{static} " : "";
                string abstractModifier = method.IsAbstract ? "{abstract} " : "";
                string parameterList = GetParameterList(method.Parameters);

                plantUml.AppendLine($"  {visibility} {staticModifier} {abstractModifier} {method.ReturnTypeName} {method.Name}({parameterList})");
            }
        }

        private void AppendRelationship(StringBuilder plantUml, RelationshipModel relationship)
        {
            string relationshipSymbol;

            switch (relationship.Type)
            {
                case RelationshipModel.RelationshipType.Inheritance:
                    relationshipSymbol = "<|--";
                    break;
                case RelationshipModel.RelationshipType.Implementation:
                    relationshipSymbol = "<|..";
                    break;
                case RelationshipModel.RelationshipType.Association:
                    relationshipSymbol = "-->";
                    break;
                case RelationshipModel.RelationshipType.Dependency:
                    relationshipSymbol = "..>";
                    break;
                default:
                    relationshipSymbol = "--";
                    break;
            }

            plantUml.AppendLine($"{relationship.SourceTypeName} {relationshipSymbol} {relationship.TargetTypeName}");
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
                    return "#~";
                default:
                    return "-";
            }
        }

        private string GetPropertyAccessors(PropertyModel property)
        {
            if (property.HasGetter && property.HasSetter)
            {
                return "get set";
            }
            else if (property.HasGetter)
            {
                return "get";
            }
            else
            {
                return "set";
            }
        }

        private string GetParameterList(List<ParameterModel> parameters)
        {
            StringBuilder paramList = new StringBuilder();
            foreach (ParameterModel param in parameters)
            {
                if (paramList.Length > 0)
                    paramList.Append(", ");
                paramList.Append($"{param.TypeName} {param.Name}");
            }
            return paramList.ToString();
        }
    }
}