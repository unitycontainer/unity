// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Microsoft.Practices.Unity
{
    partial class AllClasses
    {
        /// <summary>
        /// Returns all visible, non-abstract classes from all assemblies that are loaded in the current application domain.
        /// </summary>
        /// <param name="includeSystemAssemblies"><see langword="false" /> to include system assemblies; otherwise, <see langword="false" />. Defaults to <see langword="false" />.</param>
        /// <param name="includeUnityAssemblies"><see langword="false" /> to include the Unity assemblies; otherwise, <see langword="false" />. Defaults to <see langword="false" />.</param>
        /// <param name="includeDynamicAssemblies"><see langword="false" /> to include dynamic assemblies; otherwise, <see langword="false" />. Defaults to <see langword="false" />.</param>
        /// <param name="skipOnError"><see langword="true"/> to skip errors; otherwise, <see langword="true"/>.</param>
        /// <returns>
        /// All visible, non-abstract classes in the loaded assemblies.
        /// </returns>
        /// <remarks>
        /// If <paramref name="skipOnError" /> is <see langword="true" />, all exceptions thrown while getting types from the assemblies are ignored, and the types
        /// that can be retrieved are returned; otherwise, the original exception is thrown.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<Type> FromLoadedAssemblies(bool includeSystemAssemblies = false, bool includeUnityAssemblies = false, bool includeDynamicAssemblies = false, bool skipOnError = true)
        {
            return FromCheckedAssemblies(GetLoadedAssemblies(includeSystemAssemblies, includeUnityAssemblies, includeDynamicAssemblies), skipOnError);
        }

        /// <summary>
        /// Returns all visible, non-abstract classes from all assemblies that are located in the base folder of the current application domain.
        /// </summary>
        /// <param name="includeSystemAssemblies"><see langword="false" /> to include system assemblies; otherwise, <see langword="false" />. Defaults to <see langword="false" />.</param>
        /// <param name="includeUnityAssemblies"><see langword="false" /> to include the Unity assemblies; otherwise, <see langword="false" />. Defaults to <see langword="false" />.</param>
        /// <param name="skipOnError"><see langword="true"/> to skip errors; otherwise, <see langword="true"/>.</param>
        /// <returns>
        /// All visible, non-abstract classes.
        /// </returns>
        /// <remarks>
        /// If <paramref name="skipOnError" /> is <see langword="true" />, all exceptions thrown while loading assemblies or getting types from the assemblies are ignored, and the types
        /// that can be retrieved are returned; otherwise, the original exception is thrown.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<Type> FromAssembliesInBasePath(bool includeSystemAssemblies = false, bool includeUnityAssemblies = false, bool skipOnError = true)
        {
            return FromCheckedAssemblies(GetAssembliesInBasePath(includeSystemAssemblies, includeUnityAssemblies, skipOnError), skipOnError);
        }

        private static IEnumerable<Assembly> GetAssembliesInBasePath(bool includeSystemAssemblies, bool includeUnityAssemblies, bool skipOnError)
        {
            string basePath;

            try
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch (SecurityException)
            {
                if (!skipOnError)
                {
                    throw;
                }

                return new Assembly[0];
            }

            return GetAssemblyNames(basePath, skipOnError)
                    .Select(an => LoadAssembly(Path.GetFileNameWithoutExtension(an), skipOnError))
                    .Where(a => a != null && (includeSystemAssemblies || !IsSystemAssembly(a)) && (includeUnityAssemblies || !IsUnityAssembly(a)));
        }

        private static IEnumerable<string> GetAssemblyNames(string path, bool skipOnError)
        {
            try
            {
                return Directory.EnumerateFiles(path, "*.dll").Concat(Directory.EnumerateFiles(path, "*.exe"));
            }
            catch (Exception e)
            {
                if (!(skipOnError && (e is DirectoryNotFoundException || e is IOException || e is SecurityException || e is UnauthorizedAccessException)))
                {
                    throw;
                }

                return new string[0];
            }
        }

        private static Assembly LoadAssembly(string assemblyName, bool skipOnError)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
                if (!(skipOnError && (e is FileNotFoundException || e is FileLoadException || e is BadImageFormatException)))
                {
                    throw;
                }

                return null;
            }
        }

        private static IEnumerable<Assembly> GetLoadedAssemblies(bool includeSystemAssemblies, bool includeUnityAssemblies, bool includeDynamicAssemblies)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (includeSystemAssemblies && includeDynamicAssemblies)
            {
                return assemblies;
            }

            return assemblies.Where(a => (includeDynamicAssemblies || !a.IsDynamic) && (includeSystemAssemblies || !IsSystemAssembly(a)) && (includeUnityAssemblies || !IsUnityAssembly(a)));
        }
    }
}
