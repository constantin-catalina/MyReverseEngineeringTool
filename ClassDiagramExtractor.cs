using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace assignment_3
{
    public class ClassDiagramExtractor
    {
        private readonly Assembly assembly;
        private readonly Dictionary<Type, List<Type>> inheritanceRelationships;
        private readonly Dictionary<Type, List<Type>> implementationRelationships;
        private readonly Dictionary<Type, List<Type>> associationRelationships;
        private readonly Dictionary<Type, List<Type>> dependencyRelationships;
        private DiagramOptions options;

        public ClassDiagramExtractor(Assembly assembly, DiagramOptions options = null)
        {
            this.assembly = assembly;
            this.options = options ?? new DiagramOptions();
            inheritanceRelationships = new Dictionary<Type, List<Type>>();
            implementationRelationships = new Dictionary<Type, List<Type>>();
            associationRelationships = new Dictionary<Type, List<Type>>();
            dependencyRelationships = new Dictionary<Type, List<Type>>();
        }

        public string GenerateClassDiagram(IDiagramFormatter formatter)
        {
            DiagramModel model = BuildDiagramModel();
            return formatter.FormatClassDiagram(model);
        }

        private DiagramModel BuildDiagramModel()
        {
            DiagramModel model = new DiagramModel
            {
                AssemblyName = assembly.GetName().Name
            };

            Type[] types = assembly.GetTypes();

            // Filter types based on ignored classes, interfaces and namespaces
            IEnumerable<Type> filteredTypes = types;
            if (options.IgnoredClasses != null && options.IgnoredClasses.Count > 0)
            {
                List<Type> tempFilteredTypes = new List<Type>();

                foreach (Type t in types)
                {
                    bool ignore = false;

                    foreach (string ignoreClass in options.IgnoredClasses)
                    {
                        if (string.Equals(t.Name, ignoreClass, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(t.FullName, ignoreClass, StringComparison.OrdinalIgnoreCase) ||
                            (t.Namespace != null && t.Namespace.StartsWith(ignoreClass, StringComparison.OrdinalIgnoreCase)))
                        {
                            ignore = true;
                            break;
                        }
                    }

                    if (!ignore)
                    {
                        tempFilteredTypes.Add(t);
                    }
                }

                filteredTypes = tempFilteredTypes;
            }


            foreach (Type type in filteredTypes)
            {
                AnalyzeTypeRelationships(type);
                TypeModel typeModel = CreateTypeModel(type);
                model.Types.Add(typeModel);
            }

            AddRelationships(model);

            return model;
        }

        private void AnalyzeTypeRelationships(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                if (!inheritanceRelationships.ContainsKey(type))
                {
                    inheritanceRelationships[type] = new List<Type>();
                }
                inheritanceRelationships[type].Add(type.BaseType);
            }

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

            AnalyzeAssociations(type);

            AnalyzeDependencies(type);
        }

        private void AnalyzeAssociations(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
            {
                Type fieldType = field.FieldType;

                if (fieldType.IsPrimitive || fieldType == typeof(string))
                    continue;

                if (IsCollectionType(fieldType, out Type elementType))
                {
                    fieldType = elementType;
                }

                if (!associationRelationships.ContainsKey(type))
                {
                    associationRelationships[type] = new List<Type>();
                }

                if (!associationRelationships[type].Contains(fieldType) &&
                    fieldType != type &&
                    fieldType.Assembly == assembly)
                {
                    associationRelationships[type].Add(fieldType);
                }
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo property in properties)
            {
                Type propertyType = property.PropertyType;

                if (propertyType.IsPrimitive || propertyType == typeof(string))
                    continue;

                if (IsCollectionType(propertyType, out Type elementType))
                {
                    propertyType = elementType;
                }

                if (!associationRelationships.ContainsKey(type))
                {
                    associationRelationships[type] = new List<Type>();
                }

                if (!associationRelationships[type].Contains(propertyType) &&
                    propertyType != type &&
                    propertyType.Assembly == assembly)
                {
                    associationRelationships[type].Add(propertyType);
                }
            }
        }

        private void AnalyzeDependencies(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
            {
                Type returnType = method.ReturnType;
                if (!returnType.IsPrimitive && returnType != typeof(void) &&
                    returnType != typeof(string) && returnType != type &&
                    returnType.Assembly == assembly)
                {
                    if (IsCollectionType(returnType, out Type elementType))
                    {
                        returnType = elementType;
                    }

                    AddDependency(type, returnType);
                }

                foreach (ParameterInfo param in method.GetParameters())
                {
                    Type paramType = param.ParameterType;
                    if (!paramType.IsPrimitive && paramType != typeof(string) &&
                        paramType != type && paramType.Assembly == assembly)
                    {
                        if (IsCollectionType(paramType, out Type elementType))
                        {
                            paramType = elementType;
                        }

                        AddDependency(type, paramType);
                    }
                }
            }
        }

        private void AddDependency(Type source, Type target)
        {
            bool hasStrongerRelationship = false;

            if (inheritanceRelationships.ContainsKey(source) && inheritanceRelationships[source].Contains(target))
            {
                hasStrongerRelationship = true;
            }
            else if (implementationRelationships.ContainsKey(source) && implementationRelationships[source].Contains(target))
            {
                hasStrongerRelationship = true;
            }
            else if (associationRelationships.ContainsKey(source) && associationRelationships[source].Contains(target))
            {
                hasStrongerRelationship = true;
            }

            if (hasStrongerRelationship)
            {
                return;
            }

            if (!dependencyRelationships.ContainsKey(source))
            {
                dependencyRelationships[source] = new List<Type>();
            }

            if (!dependencyRelationships[source].Contains(target))
            {
                dependencyRelationships[source].Add(target);
            }
        }

        private bool IsCollectionType(Type type, out Type elementType)
        {
            elementType = null;

            if (type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(List<>) ||
                        genericTypeDefinition == typeof(IList<>) ||
                        genericTypeDefinition == typeof(ICollection<>) ||
                        genericTypeDefinition == typeof(IEnumerable<>) ||
                        genericTypeDefinition == typeof(HashSet<>) ||
                        genericTypeDefinition == typeof(Dictionary<,>))
                    {
                        elementType = genericArgs[0];
                        return true;
                    }
                }
            }
            else if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            return false;
        }

        private void AddRelationships(DiagramModel model)
        {
            foreach (var pair in inheritanceRelationships)
            {
                foreach (var baseType in pair.Value)
                {
                    model.Relationships.Add(new RelationshipModel
                    {
                        SourceTypeName = GetTypeName(pair.Key),
                        TargetTypeName = GetTypeName(baseType),
                        Type = RelationshipModel.RelationshipType.Inheritance
                    });
                }
            }

            foreach (var pair in implementationRelationships)
            {
                foreach (var interfaceType in pair.Value)
                {
                    model.Relationships.Add(new RelationshipModel
                    {
                        SourceTypeName = GetTypeName(pair.Key),
                        TargetTypeName = GetTypeName(interfaceType),
                        Type = RelationshipModel.RelationshipType.Implementation
                    });
                }
            }

            foreach (var pair in associationRelationships)
            {
                foreach (var targetType in pair.Value)
                {
                    model.Relationships.Add(new RelationshipModel
                    {
                        SourceTypeName = GetTypeName(pair.Key),
                        TargetTypeName = GetTypeName(targetType),
                        Type = RelationshipModel.RelationshipType.Association
                    });
                }
            }

            foreach (var pair in dependencyRelationships)
            {
                foreach (var targetType in pair.Value)
                {
                    model.Relationships.Add(new RelationshipModel
                    {
                        SourceTypeName = GetTypeName(pair.Key),
                        TargetTypeName = GetTypeName(targetType),
                        Type = RelationshipModel.RelationshipType.Dependency
                    });
                }
            }
        }

        private string GetTypeName(Type type)
        {
            return options.UseFullyQualifiedNames ? type.FullName : type.Name;
        }

        private TypeModel CreateTypeModel(Type type)
        {
            TypeModel typeModel = new TypeModel
            {
                Name = type.Name,
                FullName = type.FullName,
                Namespace = type.Namespace,
                IsInterface = type.IsInterface
            };

            if (options.ShowAttributes)
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo property in properties)
                {
                    PropertyModel propModel = new PropertyModel
                    {
                        Name = property.Name,
                        TypeName = GetFriendlyTypeName(property.PropertyType),
                        AccessModifier = GetAccessModifier(property),
                        IsStatic = property.GetMethod?.IsStatic ?? false,
                        IsAbstract = property.GetMethod?.IsAbstract ?? false,
                        IsVirtual = (property.GetMethod?.IsVirtual ?? false) && !(property.GetMethod?.IsAbstract ?? false),
                        HasGetter = property.GetMethod != null,
                        HasSetter = property.SetMethod != null
                    };
                    typeModel.Properties.Add(propModel);
                }

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (FieldInfo field in fields)
                {
                    if (field.Name.Contains("k__BackingField")) continue;

                    FieldModel fieldModel = new FieldModel
                    {
                        Name = field.Name,
                        TypeName = GetFriendlyTypeName(field.FieldType),
                        AccessModifier = GetAccessModifier(field),
                        IsStatic = field.IsStatic,
                        IsReadOnly = field.IsInitOnly
                    };
                    typeModel.Fields.Add(fieldModel);
                }
            }

            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (ConstructorInfo constructor in constructors)
            {
                ConstructorModel ctorModel = new ConstructorModel
                {
                    AccessModifier = GetAccessModifier(constructor),
                    IsStatic = constructor.IsStatic
                };

                ParameterInfo[] parameters = constructor.GetParameters();
                foreach (ParameterInfo param in parameters)
                {
                    ctorModel.Parameters.Add(new ParameterModel
                    {
                        Name = param.Name,
                        TypeName = GetFriendlyTypeName(param.ParameterType)
                    });
                }

                typeModel.Constructors.Add(ctorModel);
            }

            if (options.ShowMethods)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (MethodInfo method in methods)
                {
                    if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
                        continue;

                    MethodModel methodModel = new MethodModel
                    {
                        Name = method.Name,
                        ReturnTypeName = GetFriendlyTypeName(method.ReturnType),
                        AccessModifier = GetAccessModifier(method),
                        IsStatic = method.IsStatic,
                        IsAbstract = method.IsAbstract,
                        IsVirtual = method.IsVirtual && !method.IsAbstract,
                        IsOverride = method.GetBaseDefinition().DeclaringType != method.DeclaringType
                    };

                    ParameterInfo[] parameters = method.GetParameters();
                    foreach (ParameterInfo param in parameters)
                    {
                        methodModel.Parameters.Add(new ParameterModel
                        {
                            Name = param.Name,
                            TypeName = GetFriendlyTypeName(param.ParameterType)
                        });
                    }

                    typeModel.Methods.Add(methodModel);
                }
            }

            return typeModel;
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
                if (method.IsPublic) return "public";
                if (method.IsPrivate) return "private";
                if (method.IsFamily) return "protected";
                if (method.IsAssembly) return "internal";
                if (method.IsFamilyOrAssembly) return "protected internal";
            }
            else if (member is FieldInfo field)
            {
                if (field.IsPublic) return "public";
                if (field.IsPrivate) return "private";
                if (field.IsFamily) return "protected";
                if (field.IsAssembly) return "internal";
                if (field.IsFamilyOrAssembly) return "protected internal";
            }
            else if (member is ConstructorInfo constructor)
            {
                if (constructor.IsPublic) return "public";
                if (constructor.IsPrivate) return "private";
                if (constructor.IsFamily) return "protected";
                if (constructor.IsAssembly) return "internal";
                if (constructor.IsFamilyOrAssembly) return "protected internal";
            }
            else if (member is PropertyInfo property)
            {
                MethodInfo accessor = property.GetMethod ?? property.SetMethod;
                if (accessor != null)
                {
                    if (accessor.IsPublic) return "public";
                    if (accessor.IsPrivate) return "private";
                    if (accessor.IsFamily) return "protected";
                    if (accessor.IsAssembly) return "internal";
                    if (accessor.IsFamilyOrAssembly) return "protected internal";
                }
            }

            return "private";
        }
    }
}