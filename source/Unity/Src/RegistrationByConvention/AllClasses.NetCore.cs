//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Microsoft.Practices.Unity
{
    partial class AllClasses
    {
        /// <summary>
        /// Returns all visible, non-abstract classes from all assemblies located where the application is installed.
        /// </summary>
        /// <param name="includeUnityAssemblies"><see langword="false" /> to include the Unity assemblies; otherwise, <see langword="false" />. Defaults to <see langword="false" />.</param>
        /// <param name="skipOnError"><see langword="true"/> to skip errors; otherwise, <see langword="true"/>.</param>
        /// <returns>
        /// All visible, non-abstract classes.
        /// </returns>
        /// <remarks>
        /// If <paramref name="skipOnError" /> is <see langword="true" />, all exceptions thrown while loading assemblies or getting types from the assemblies 
        /// are ignored, and the types that can be retrieved are returned; otherwise, the original exception is thrown. These exceptions might be wrapped in a
        /// <see cref="AggregateException"/>.
        /// </remarks>
        public static IEnumerable<Type> FromApplication(bool includeUnityAssemblies = false, bool skipOnError = true)
        {
            return FromCheckedAssemblies(GetAssembliesApplicationAsync(includeUnityAssemblies, skipOnError).Result, skipOnError);
        }

        private static async Task<IEnumerable<Assembly>> GetAssembliesApplicationAsync(bool includeUnityAssemblies, bool skipOnError)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assemblies = new List<Assembly>();

            foreach (var file in await folder.GetFilesAsync().AsTask().ConfigureAwait(false))
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    var name = new AssemblyName() { Name = Path.GetFileNameWithoutExtension(file.Name) };
                    Assembly assembly;

                    try
                    {
                        assembly = Assembly.Load(name);
                    }
                    catch (Exception e)
                    {
                        if (!(skipOnError && (e is FileNotFoundException || e is BadImageFormatException)))
                        {
                            throw;
                        }

                        continue;
                    }

                    if (includeUnityAssemblies || !IsUnityAssembly(assembly))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }

            return assemblies;
        }
    }
}