using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace assignment_3
{
    public class ClassDiagramExtractor
    {
        private readonly Assembly assembly;
        private readonly Dictionary<Type, List<Type>> inheritanceRelationships;
        private readonly Dictionary<Type, List<Type>> implementationRelationships;
        public ClassDiagramExtractor(Assembly assembly)
        {
            this.assembly = assembly;
            inheritanceRelationships = new Dictionary<Type, List<Type>>();
            implementationRelationships = new Dictionary<Type, List<Type>>();
        }
        public string GenerateClassDiagram()
        {
            StringBuilder classDiagram = new StringBuilder();

            classDiagram.AppendLine(new string('=', 80));
            classDiagram.AppendLine($"Class Diagram for: {assembly.GetName().Name}");
            classDiagram.AppendLine(new string('=', 80));
            classDiagram.AppendLine();

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                AnalyzeTypeRelationships(type);
                AppendTypeDetails(classDiagram, type);
                classDiagram.AppendLine();
            }

            classDiagram.AppendLine("Relationships:");
            classDiagram.AppendLine(new string('=', 80));
            classDiagram.AppendLine();
            AppendRelationships(classDiagram);

            return classDiagram.ToString();
        }
        private void AnalyzeTypeRelationships(Type type)
        {
            // Inheritance

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                if (!inheritanceRelationships.ContainsKey(type))
                {
                    inheritanceRelationships[type] = new List<Type>();
                }
                inheritanceRelationships[type].Add(type.BaseType);
            }

            // Implementation

            if (type.IsClass)
            {
                Type[] interfaces = type.GetInterfaces();
                foreach (Type interfaceType in interfaces)
                {
                    if (!implementationRelationships.ContainsKey(type))
                    {
                        implementationRelationships[type] = new List<Type>();
                    }
                    implementationRelationships[type].Add(interfaceType);
                }
            }
        }
        private void AppendRelationships(StringBuilder classDiagram)
        {
            // Inheritance

            if (inheritanceRelationships.Count > 0)
            {
                classDiagram.AppendLine("Inheritance:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (var pair in inheritanceRelationships)
                {
                    foreach (var baseType in pair.Value)
                    {
                        classDiagram.AppendLine($"{pair.Key.Name} extends {baseType.Name}");
                        classDiagram.AppendLine();
                    }
                }
            }

            // Implementation

            if (implementationRelationships.Count > 0)
            {
                classDiagram.AppendLine("Implementation:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (var pair in implementationRelationships)
                {
                    foreach (var interfaceType in pair.Value)
                    {
                        classDiagram.AppendLine($"{pair.Key.Name} implements {interfaceType.Name}");
                        classDiagram.AppendLine();
                    }
                }
            }
        }
        private void AppendTypeDetails(StringBuilder classDiagram, Type type)
        {
            if (type.IsInterface)
            {
                classDiagram.AppendLine($"Interface: {type.Name}");
                classDiagram.AppendLine(new string('=',80));
            }
            else
            {
                classDiagram.AppendLine($"Class: {type.Name}");
                classDiagram.AppendLine(new string('=', 80));
            }
            classDiagram.AppendLine($"Namespace: {type.Namespace}");
            classDiagram.AppendLine();

            // Properties

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            if(properties.Length > 0)
            {
                classDiagram.AppendLine("Properties:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (PropertyInfo property in properties)
                {
                    string accessModifier = GetAccessModifier(property);
                    string staticModifier = "";
                    if (property.GetMethod.IsStatic)
                    {
                        staticModifier = "static ";
                    }
                    string abstractModifier = "";
                    if (property.GetMethod.IsAbstract)
                    {
                        abstractModifier = "abstract ";
                    }
                    string virtualModifier = "";
                    if (property.GetMethod.IsVirtual && !property.GetMethod.IsAbstract)
                    {
                        virtualModifier = "virtual ";
                    }
                    string typeName = GetFriendlyTypeName(property.PropertyType);
                    classDiagram.AppendLine($"{accessModifier}{staticModifier}{abstractModifier}{virtualModifier}{typeName} {property.Name} {{ get; {(property.SetMethod != null ? "set; " : "")}}}");
                    classDiagram.AppendLine();
                }
            }

            // Fields

            bool validFields = false;
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            
            if (fields.Length > 0)
            {   
                foreach (FieldInfo field in fields)
                {
                    if (field.Name.Contains("k__BackingField")) continue;
                    if(validFields == false)
                    {
                        validFields = true;
                        classDiagram.AppendLine("Fields:");
                        classDiagram.AppendLine(new string('-', 80));
                    }
                    string accessModifier = GetAccessModifier(field);
                    string staticModifier = "";
                    if (field.IsStatic)
                    {
                        staticModifier = "static ";
                    }
                    string readonlyModifier = "";
                    if (field.IsInitOnly)
                    {
                        readonlyModifier = "readonly ";
                    }
                    string typeName = GetFriendlyTypeName(field.FieldType);
                    classDiagram.AppendLine($"{accessModifier}{staticModifier}{readonlyModifier}{typeName} {field.Name}");
                    classDiagram.AppendLine();
                }
            }

            // Constructors

            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

            if (constructors.Length > 0)
            {
                classDiagram.AppendLine("Constructors:");
                classDiagram.AppendLine(new string('-', 80));
                foreach (ConstructorInfo constructor in constructors)
                {
                    string accessModifier = GetAccessModifier(constructor);
                    string staticModifier = "";
                    if (constructor.IsStatic)
                    {
                        staticModifier = "static ";
                    }

                    // Parameters

                    ParameterInfo[] parameters = constructor.GetParameters();
                    List<string> paramStrings = new List<string>();
                    foreach (ParameterInfo p in parameters)
                    {
                        string paramType = GetFriendlyTypeName(p.ParameterType);
                        string paramName = p.Name;
                        paramStrings.Add($"{paramType} {paramName}");
                    }
                    string parameterList = string.Join(", ", paramStrings);
                    classDiagram.AppendLine($"{accessModifier}{staticModifier}{type.Name}({parameterList})");
                    classDiagram.AppendLine();
                }
            }

            // Methods

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

            if (methods.Length > 0)
            {
                classDiagram.AppendLine("Methods:");
                classDiagram.AppendLine(new string('-', 80));

                foreach (MethodInfo method in methods)
                {
                    string accessModifier = GetAccessModifier(method);
                    string staticModifier = "";
                    if (method.IsStatic)
                    {
                        staticModifier = "static ";
                    }
                    string abstractModifier = "";
                    if (method.IsAbstract)
                    {
                        abstractModifier = "abstract ";
                    }
                    string virtualModifier = "";
                    if (method.IsVirtual)
                    {
                        virtualModifier = "virtual ";
                    }
                    string overrideModifier = "";
                    if (method.GetBaseDefinition().DeclaringType != method.DeclaringType)
                    {
                        overrideModifier = "override ";
                    }

                    // Parameters

                    ParameterInfo[] parameters = method.GetParameters();
                    List<string> paramStrings = new List<string>();

                    foreach (ParameterInfo p in parameters)
                    {
                        string paramType = GetFriendlyTypeName(p.ParameterType);
                        string paramName = p.Name;
                        paramStrings.Add($"{paramType} {paramName}");
                    }

                    string parameterList = string.Join(", ", paramStrings);

                    classDiagram.AppendLine($"{accessModifier}{staticModifier}{abstractModifier}{virtualModifier}{overrideModifier}{GetFriendlyTypeName(method.ReturnType)} {method.Name}({parameterList})");
                    classDiagram.AppendLine();
                }
            }
        }
        private string GetFriendlyTypeName(Type type)
        {
            if (type == typeof(Single))
            {
                return "Float";
            }

            if (!type.IsGenericType)
                return type.Name;

            string typeName = type.Name;
            int index = typeName.IndexOf('`');
            if (index > 0)
                typeName = typeName.Substring(0, index);

            Type[] genericArguments = type.GetGenericArguments();
            string[] genericArgs = new string[genericArguments.Length];

            for (int i = 0; i < genericArguments.Length; i++)
            {
                genericArgs[i] = GetFriendlyTypeName(genericArguments[i]);
            }

            return $"{typeName}<{string.Join(", ", genericArgs)}>";
        }
        private string GetAccessModifier(MemberInfo member)
        {
            if (member is MethodInfo method)
            {
                if (method.IsPublic) return "public ";
                if (method.IsPrivate) return "private ";
                if (method.IsFamily) return "protected ";
                if (method.IsAssembly) return "internal ";
                if (method.IsFamilyOrAssembly) return "protected internal ";
            }
            else if (member is FieldInfo field)
            {
                if (field.IsPublic) return "public ";
                if (field.IsPrivate) return "private ";
                if (field.IsFamily) return "protected ";
                if (field.IsAssembly) return "internal ";
                if (field.IsFamilyOrAssembly) return "protected internal ";
            }
            else if (member is ConstructorInfo constructor)
            {
                if (constructor.IsPublic) return "public ";
                if (constructor.IsPrivate) return "private ";
                if (constructor.IsFamily) return "protected ";
                if (constructor.IsAssembly) return "internal ";
                if (constructor.IsFamilyOrAssembly) return "protected internal ";
            }
            else if (member is PropertyInfo property)
            {
                MethodInfo accesor = property.GetMethod ?? property.SetMethod;
                if(accesor != null)
                {
                    if (accesor.IsPublic) return "public ";
                    if (accesor.IsPrivate) return "private ";
                    if (accesor.IsFamily) return "protected ";
                    if (accesor.IsAssembly) return "internal ";
                    if (accesor.IsFamilyOrAssembly) return "protected internal ";
                }
            }

                return "";
        }
    }
}