// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Unity.Configuration.Properties;

namespace Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A helper class that implements the actual logic for resolving a shorthand
    /// type name (alias or raw type name) into an actual type object.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Impl is common suffix for implementation class")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Impl", Justification = "Impl is common suffix for implementation class")]
    public class TypeResolverImpl
    {
        private readonly Dictionary<string, string> aliases;
        private readonly List<string> namespaces;
        private readonly List<string> assemblies;

        private static readonly Dictionary<string, Type> DefaultAliases = new Dictionary<string, Type>
            {
                { "sbyte", typeof(sbyte) },
                { "short", typeof(short) },
                { "int", typeof(int) },
                { "integer", typeof(int) },
                { "long", typeof(long) },
                { "byte", typeof(byte) },
                { "ushort", typeof(ushort) },
                { "uint", typeof(uint) },
                { "ulong", typeof(ulong) },
                { "float", typeof(float) },
                { "single", typeof(float) },
                { "double", typeof(double) },
                { "decimal", typeof(decimal) },
                { "char", typeof(char) },
                { "bool", typeof(bool) },
                { "object", typeof(object) },
                { "string", typeof(string) },
                { "datetime", typeof(DateTime) },
                { "DateTime", typeof(DateTime) },
                { "date", typeof(DateTime) },
                { "singleton", typeof(ContainerControlledLifetimeManager) },
                { "ContainerControlledLifetimeManager", typeof(ContainerControlledLifetimeManager) },
                { "transient", typeof(TransientLifetimeManager) },
                { "TransientLifetimeManager", typeof(TransientLifetimeManager) },
                { "perthread", typeof(PerThreadLifetimeManager) },
                { "PerThreadLifetimeManager", typeof(PerThreadLifetimeManager) },
                { "external", typeof(ExternallyControlledLifetimeManager) },
                { "ExternallyControlledLifetimeManager", typeof(ExternallyControlledLifetimeManager) },
                { "hierarchical", typeof(HierarchicalLifetimeManager) },
                { "HierarchicalLifetimeManager", typeof(HierarchicalLifetimeManager) },
                { "resolve", typeof(PerResolveLifetimeManager) },
                { "perresolve", typeof(PerResolveLifetimeManager) },
                { "PerResolveLifetimeManager", typeof(PerResolveLifetimeManager) },
            };

        /// <summary>
        /// Construct a new <see cref="TypeResolverImpl"/> that uses the given
        /// sequence of alias, type name pairs to resolve types.
        /// </summary>
        /// <param name="aliasesSequence">Type aliases from the configuration file.</param>
        /// <param name="assemblies">Assembly names to search.</param>
        /// <param name="namespaces">Namespaces to search.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Use of nested generic types is appropriate here")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        public TypeResolverImpl(IEnumerable<KeyValuePair<string, string>> aliasesSequence,
            IEnumerable<string> namespaces, IEnumerable<string> assemblies)
        {
            Unity.Utility.Guard.ArgumentNotNull(aliasesSequence, "aliasesSequence");

            aliases = new Dictionary<string, string>();
            foreach (var pair in aliasesSequence)
            {
                aliases.Add(pair.Key, pair.Value);
            }

            this.namespaces = new List<string>(namespaces);
            this.assemblies = new List<string>(assemblies);
            this.assemblies.Add(typeof(object).Assembly.FullName);
            this.assemblies.Add(typeof(Uri).Assembly.FullName);
        }

        /// <summary>
        /// Resolves a type alias or type FullName to a concrete type.
        /// </summary>
        /// <param name="typeNameOrAlias">Alias or name to resolve.</param>
        /// <param name="throwIfResolveFails">if true and the alias does not
        /// resolve, throw an <see cref="InvalidOperationException"/>, otherwise 
        /// return null on failure.</param>
        /// <returns>The type object or null if resolve fails and 
        /// <paramref name="throwIfResolveFails"/> is false.</returns>
        public Type ResolveType(string typeNameOrAlias, bool throwIfResolveFails)
        {
            Type resolvedType = ResolveTypeInternal(typeNameOrAlias) ??
                ResolveGenericTypeShorthand(typeNameOrAlias);

            if (resolvedType == null && throwIfResolveFails)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.CouldNotResolveType, typeNameOrAlias));
            }
            return resolvedType;
        }

        private Type ResolveTypeInternal(string typeNameOrAlias)
        {
            return ResolveTypeDirectly(typeNameOrAlias) ??
                ResolveAlias(typeNameOrAlias) ??
                ResolveDefaultAlias(typeNameOrAlias) ??
                ResolveTypeThroughSearch(typeNameOrAlias);
        }

        /// <summary>
        /// Resolve a type alias or type full name to a concrete type.
        /// If <paramref name="typeNameOrAlias"/> is null or empty, return the
        /// given <paramref name="defaultValue"/> instead.
        /// </summary>
        /// <param name="typeNameOrAlias">Type alias or full name to resolve.</param>
        /// <param name="defaultValue">Value to return if typeName is null or empty.</param>
        /// <param name="throwIfResolveFails">if true and the alias does not
        /// resolve, throw an <see cref="InvalidOperationException"/>, otherwise 
        /// return null on failure.</param>
        /// <returns>
        /// <para>If <paramref name="typeNameOrAlias"/> is null or an empty string,
        /// then return <paramref name="defaultValue"/>.</para>
        /// <para>Otherwise, return the resolved type object. If the resolution fails
        /// and <paramref name="throwIfResolveFails"/> is false, then return null.</para>
        /// </returns>
        public Type ResolveTypeWithDefault(string typeNameOrAlias, Type defaultValue, bool throwIfResolveFails)
        {
            if (String.IsNullOrEmpty(typeNameOrAlias))
            {
                return defaultValue;
            }
            return ResolveType(typeNameOrAlias, throwIfResolveFails);
        }

        private static Type ResolveTypeDirectly(string typeNameOrAlias)
        {
            return Type.GetType(typeNameOrAlias);
        }

        private Type ResolveAlias(string typeNameOrAlias)
        {
            string mappedTypeName = aliases.GetOrNull(typeNameOrAlias) ??
                aliases.GetOrNull(RemoveGenericWart(typeNameOrAlias));
            if (mappedTypeName != null)
            {
                return Type.GetType(mappedTypeName);
            }
            return null;
        }

        private static Type ResolveDefaultAlias(string typeNameOrAlias)
        {
            Type mappedType;
            if (DefaultAliases.TryGetValue(typeNameOrAlias, out mappedType) ||
                DefaultAliases.TryGetValue(RemoveGenericWart(typeNameOrAlias), out mappedType))
            {
                return mappedType;
            }
            return null;
        }

        private static string RemoveGenericWart(string typeNameOrAlias)
        {
            string result = typeNameOrAlias;
            int backtickIndex = typeNameOrAlias.IndexOf('`');
            if (backtickIndex != -1)
            {
                result = typeNameOrAlias.Substring(0, backtickIndex);
            }
            return result;
        }

        private Type ResolveTypeThroughSearch(string typeNameOrAlias)
        {
            if (namespaces.Count == 0)
            {
                return SearchAssemblies(typeNameOrAlias);
            }
            return SearchAssembliesAndNamespaces(typeNameOrAlias);
        }

        private Type ResolveGenericTypeShorthand(string typeNameOrAlias)
        {
            Type result = null;
            TypeNameInfo parseResult = TypeNameParser.Parse(typeNameOrAlias);
            if (parseResult != null && parseResult.IsGenericType)
            {
                result = ResolveTypeInternal(parseResult.FullName);
                if (result == null)
                {
                    return null;
                }

                var genericParams = new List<Type>(parseResult.NumGenericParameters);
                bool isOpenGeneric = (parseResult.GenericParameters[0] == null);

                if (!isOpenGeneric)
                {
                    foreach (var genericParamInfo in parseResult.GenericParameters)
                    {
                        Type genericParam = ResolveType(genericParamInfo.FullNameWithNestedGenerics, false);
                        if (genericParam == null)
                        {
                            return null;
                        }
                        genericParams.Add(genericParam);
                    }

                    if (genericParams.Count > 0)
                    {
                        result = result.MakeGenericType(genericParams.ToArray());
                    }
                }
            }
            return result;
        }

        private Type SearchAssembliesAndNamespaces(string typeNameOrAlias)
        {
            foreach (var asm in assemblies)
            {
                foreach (var ns in namespaces)
                {
                    try
                    {
                        Type result = Type.GetType(MakeTypeName(ns, typeNameOrAlias, asm));
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    catch (FileLoadException)
                    {
                    }
                }
            }
            return null;
        }

        private Type SearchAssemblies(string typeNameOrAlias)
        {
            foreach (var asm in assemblies)
            {
                try
                {
                    Type result = Type.GetType(MakeAssemblyQualifiedName(typeNameOrAlias, asm));
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (FileLoadException)
                {
                }
            }
            return null;
        }

        private static string MakeTypeName(string ns, string typename, string assembly)
        {
            return MakeAssemblyQualifiedName(ns + "." + typename, assembly);
        }

        private static string MakeAssemblyQualifiedName(string typename, string assembly)
        {
            return typename + ", " + assembly;
        }
    }
}
