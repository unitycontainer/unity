using System;
using System.Collections.Generic;

namespace System.Reflection
{
    internal class TypeInfo 
    {
        private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        private Type _type;


        internal TypeInfo(Type type)
        {
            _type = type;
        }


        public Assembly Assembly => _type.Assembly;

        public bool IsGenericTypeDefinition => _type.IsGenericTypeDefinition;

        public Type[] GenericTypeArguments => _type.GetGenericArguments();

        public Type[] GenericTypeParameters => _type.IsGenericTypeDefinition ? _type.GetGenericArguments()
                                                                             : Type.EmptyTypes;
        public string Name => _type.Name;

        public Type BaseType => _type.BaseType;

        public bool IsGenericType => _type.IsGenericType;

        public Type AsType() => _type;

        public bool IsAssignableFrom(TypeInfo typeInfo) => _type.IsAssignableFrom(typeInfo.AsType());

        public bool IsGenericParameter => _type.IsGenericParameter;

        public bool IsInterface => _type.IsInterface;

        public bool IsAbstract => _type.IsAbstract;

        public bool IsSubclassOf(Type type) => _type.IsSubclassOf(type);

        public bool IsValueType => _type.IsValueType;

        public bool ContainsGenericParameters => _type.ContainsGenericParameters;



        #region moved over from Type

        //// Fields

        public virtual EventInfo GetDeclaredEvent(String name)
        {
            return _type.GetEvent(name, DeclaredOnlyLookup);
        }
        public virtual FieldInfo GetDeclaredField(String name)
        {
            return _type.GetField(name, DeclaredOnlyLookup);
        }
        public virtual MethodInfo GetDeclaredMethod(String name)
        {
            return _type.GetMethod(name, DeclaredOnlyLookup);
        }

        public virtual IEnumerable<MethodInfo> GetDeclaredMethods(String name)
        {
            foreach (MethodInfo method in _type.GetMethods(DeclaredOnlyLookup))
            {
                if (method.Name == name)
                    yield return method;
            }
        }

        public virtual System.Reflection.TypeInfo GetDeclaredNestedType(String name)
        {
            var nt = _type.GetNestedType(name, DeclaredOnlyLookup);
            if (nt == null)
            {
                return null; //the extension method GetTypeInfo throws for null
            }
            else
            {
                return nt.GetTypeInfo();
            }
        }

        public virtual PropertyInfo GetDeclaredProperty(String name)
        {
            return _type.GetProperty(name, DeclaredOnlyLookup);
        }


        //// Properties

        public virtual IEnumerable<ConstructorInfo> DeclaredConstructors
        {
            get
            {
                return _type.GetConstructors(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<EventInfo> DeclaredEvents
        {
            get
            {
                return _type.GetEvents(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<FieldInfo> DeclaredFields
        {
            get
            {
                return _type.GetFields(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<MemberInfo> DeclaredMembers
        {
            get
            {
                return _type.GetMembers(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<MethodInfo> DeclaredMethods
        {
            get
            {
                return _type.GetMethods(DeclaredOnlyLookup);
            }
        }
        public virtual IEnumerable<System.Reflection.TypeInfo> DeclaredNestedTypes
        {
            get
            {
                foreach (var t in _type.GetNestedTypes(DeclaredOnlyLookup))
                {
                    yield return t.GetTypeInfo();
                }
            }
        }

        public virtual IEnumerable<PropertyInfo> DeclaredProperties
        {
            get
            {
                return _type.GetProperties(DeclaredOnlyLookup);
            }
        }


        public virtual IEnumerable<Type> ImplementedInterfaces
        {
            get
            {
                return _type.GetInterfaces();
            }
        }


        #endregion

    }


    internal static class IntrospectionExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return new TypeInfo(type);
        }
    }
}