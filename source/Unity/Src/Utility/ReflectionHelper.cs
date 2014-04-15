// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.Practices.Unity.Utility
{
    /// <summary>
    /// A small helper class to encapsulate details of the
    /// reflection API, particularly around generics.
    /// </summary>
    public class ReflectionHelper
    {
        private readonly Type t;

        /// <summary>
        /// Create a new <see cref="ReflectionHelper"/> instance that
        /// lets you look at information about the given type.
        /// </summary>
        /// <param name="typeToReflect">Type to do reflection on.</param>
        public ReflectionHelper(Type typeToReflect)
        {
            t = typeToReflect;
        }

        /// <summary>
        /// The <see cref="Type"/> object we're reflecting over.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "This is the type part of the key.")]
        public Type Type
        {
            get { return t; }
        }

        /// <summary>
        /// Is this type generic?
        /// </summary>
        public bool IsGenericType
        {
            get { return t.GetTypeInfo().IsGenericType; }
        }

        /// <summary>
        /// Is this type an open generic (no type parameter specified)
        /// </summary>
        public bool IsOpenGeneric
        {
            get { return t.GetTypeInfo().IsGenericType && t.GetTypeInfo().ContainsGenericParameters; }
        }

        /// <summary>
        /// Is this type an array type?
        /// </summary>
        public bool IsArray
        {
            get { return t.IsArray; }
        }

        /// <summary>
        /// Is this type an array of generic elements?
        /// </summary>
        public bool IsGenericArray
        {
            get { return IsArray && ArrayElementType.GetTypeInfo().IsGenericParameter; }
        }

        /// <summary>
        /// The type of the elements in this type (if it's an array).
        /// </summary>
        public Type ArrayElementType
        {
            get { return t.GetElementType(); }
        }

        /// <summary>
        /// Test the given <see cref="MethodBase"/> object, looking at
        /// the parameters. Determine if any of the parameters are
        /// open generic types that need type attributes filled in.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <returns>True if any of the parameters are open generics. False if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class")]
        public static bool MethodHasOpenGenericParameters(MethodBase method)
        {
            Guard.ArgumentNotNull(method, "method");
            foreach (ParameterInfo param in method.GetParameters())
            {
                var r = new ReflectionHelper(param.ParameterType);
                if (r.IsOpenGeneric)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If this type is an open generic, use the
        /// given <paramref name="genericArguments"/> array to
        /// determine what the required closed type is and return that.
        /// </summary>
        /// <remarks>If the parameter is not an open type, just
        /// return this parameter's type.</remarks>
        /// <param name="genericArguments">Type arguments to substitute in for
        /// the open type parameters.</param>
        /// <returns>Corresponding closed type of this parameter.</returns>
        public Type GetClosedParameterType(Type[] genericArguments)
        {
            Guard.ArgumentNotNull(genericArguments, "genericArguments");

            // Prior version of the framework returned both generic type arguments and parameters
            // through one mechanism, now they are broken out.  May want to consider different reflection
            // helpers instead of this case statement.

            if (IsOpenGeneric)
            {
                var typeInfo = Type.GetTypeInfo();
                Type[] typeArgs = typeInfo.IsGenericTypeDefinition ? typeInfo.GenericTypeParameters : typeInfo.GenericTypeArguments;
                for (int i = 0; i < typeArgs.Length; ++i)
                {
                    typeArgs[i] = genericArguments[typeArgs[i].GenericParameterPosition];
                }
                return Type.GetGenericTypeDefinition().MakeGenericType(typeArgs);
            }
            if (Type.GetTypeInfo().IsGenericParameter)
            {
                return genericArguments[Type.GenericParameterPosition];
            }
            if (IsArray && ArrayElementType.GetTypeInfo().IsGenericParameter)
            {
                int rank;
                if ((rank = Type.GetArrayRank()) == 1)
                {
                    // work around to the fact that Type.MakeArrayType() != Type.MakeArrayType(1)
                    return genericArguments[Type.GetElementType().GenericParameterPosition]
                        .MakeArrayType();
                }

                return genericArguments[Type.GetElementType().GenericParameterPosition]
                    .MakeArrayType(rank);
            }
            return Type;
        }

        /// <summary>
        /// Given a generic argument name, return the corresponding type for this
        /// closed type. For example, if the current type is SomeType&lt;User&gt;, and the
        /// corresponding definition was SomeType&lt;TSomething&gt;, calling this method
        /// and passing "TSomething" will return typeof(User).
        /// </summary>
        /// <param name="parameterName">Name of the generic parameter.</param>
        /// <returns>Type of the corresponding generic parameter, or null if there
        /// is no matching name.</returns>
        public Type GetNamedGenericParameter(string parameterName)
        {
            TypeInfo openType = Type.GetGenericTypeDefinition().GetTypeInfo();
            Type result = null;
            int index = -1;

            foreach (Type genericArgumentType in openType.GenericTypeParameters)
            {
                if (genericArgumentType.GetTypeInfo().Name == parameterName)
                {
                    index = genericArgumentType.GenericParameterPosition;
                    break;
                }
            }
            if (index != -1)
            {
                result = Type.GenericTypeArguments[index];
            }
            return result;
        }

        /// <summary>
        /// Returns all the public constructors defined for the current reflected <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// An enumeration of <see cref="ConstructorInfo"/> ConstructorInfo objects representing all the public instance constructors defined for the 
        /// current reflected <see cref="Type"/>, but not including the type initializer (static constructor).
        /// </value>
        public IEnumerable<ConstructorInfo> InstanceConstructors
        {
            get
            {
                return Type.GetTypeInfo().DeclaredConstructors.Where(c => c.IsStatic == false && c.IsPublic);
            }
        }
    }
}
