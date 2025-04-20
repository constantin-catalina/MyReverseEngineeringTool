using System;
using System.Collections.Generic;

namespace assignment_3
{
    public class DiagramModel
    {
        public string AssemblyName { get; set; }
        public List<TypeModel> Types { get; set; } = new List<TypeModel>();
        public List<RelationshipModel> Relationships { get; set; } = new List<RelationshipModel>();
    }

    public class TypeModel
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Namespace { get; set; }
        public bool IsInterface { get; set; }
        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
        public List<FieldModel> Fields { get; set; } = new List<FieldModel>();
        public List<MethodModel> Methods { get; set; } = new List<MethodModel>();
        public List<ConstructorModel> Constructors { get; set; } = new List<ConstructorModel>();
    }

    public class PropertyModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string AccessModifier { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }
    }

    public class FieldModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string AccessModifier { get; set; }
        public bool IsStatic { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class MethodModel
    {
        public string Name { get; set; }
        public string ReturnTypeName { get; set; }
        public string AccessModifier { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public List<ParameterModel> Parameters { get; set; } = new List<ParameterModel>();
    }

    public class ConstructorModel
    {
        public string AccessModifier { get; set; }
        public bool IsStatic { get; set; }
        public List<ParameterModel> Parameters { get; set; } = new List<ParameterModel>();
    }

    public class ParameterModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
    }

    public class RelationshipModel
    {
        public enum RelationshipType
        {
            Inheritance,
            Implementation,
            Association,
            Dependency
        }

        public string SourceTypeName { get; set; }
        public string TargetTypeName { get; set; }
        public RelationshipType Type { get; set; }
    }
}