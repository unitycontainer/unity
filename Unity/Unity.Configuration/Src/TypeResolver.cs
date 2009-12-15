using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Type that manage access to a set of type aliases and implements
    /// the logic for converting aliases to their actual types.
    /// </summary>
    public static class TypeResolver
    {
        [ThreadStatic] private static AliasElementCollection aliases;

        private static readonly Dictionary<string, Type> defaultAliases = new Dictionary<string, Type>
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
        /// Set the set of aliases to use for resolution.
        /// </summary>
        /// <param name="aliases">An <see cref="AliasElementCollection"/> contain a set
        /// of type alias definitions.</param>
        public static void SetAliases(AliasElementCollection aliases)
        {
            TypeResolver.aliases = aliases;
        }

        /// <summary>
        /// Resolves a type alias or type fullname to a concrete type.
        /// </summary>
        /// <param name="typeNameOrAlias">Type alias or type fullname</param>
        /// <returns>Type object.</returns>
        public static Type ResolveType(string typeNameOrAlias)
        {
            return ResolveTypeDirectly(typeNameOrAlias) ??
                ResolveAlias(typeNameOrAlias) ??
                    ResolveDefaultAlias(typeNameOrAlias);
        }

        private static Type ResolveTypeDirectly(string typeNameOrAlias)
        {
            return Type.GetType(typeNameOrAlias);
        }

        private static Type ResolveAlias(string typeNameOrAlias)
        {
            var mappedName = aliases[typeNameOrAlias];
            if(mappedName != null)
            {
                return Type.GetType(mappedName);
            }
            return null;
        }

        private static Type ResolveDefaultAlias(string typeNameOrAlias)
        {
            Type mappedType;
            if(defaultAliases.TryGetValue(typeNameOrAlias, out mappedType))
            {
                return mappedType;
            }
            return null;
        }

        /// <summary>
        /// Resolve a type alias or type full name to a concrete type.
        /// If <paramref name="typeNameOrAlias"/> is null or empty, return the
        /// given <paramref name="defaultValue"/> instead.
        /// </summary>
        /// <param name="typeNameOrAlias">Type alias or full name to resolve.</param>
        /// <param name="defaultValue">Value to return if typeName is null or empty.</param>
        /// <returns>The concrete <see cref="Type"/>.</returns>
        public static Type ResolveTypeWithDefault(string typeNameOrAlias, Type defaultValue)
        {
            if (string.IsNullOrEmpty(typeNameOrAlias))
            {
                return defaultValue;
            }
            return ResolveType(typeNameOrAlias);
        }
    }
}
