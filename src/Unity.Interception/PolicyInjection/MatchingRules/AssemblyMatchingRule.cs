// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// An <see cref="IMatchingRule"/> that matches the assembly name of the
    /// given member.
    /// </summary>
    public class AssemblyMatchingRule : IMatchingRule
    {
        private readonly string assemblyName;

        /// <summary>
        /// Constructs a new <see cref="AssemblyMatchingRule"/> with the given
        /// assembly name (or partial name).
        /// </summary>
        /// <param name="assemblyName">Assembly name to match.</param>
        public AssemblyMatchingRule(string assemblyName)
        {
            this.assemblyName = assemblyName;
        }

        /// <summary>
        /// Constructs a new <see cref="AssemblyMatchingRule"/> that matches
        /// against the given assembly.
        /// </summary>
        /// <param name="assembly">Assembly to match.</param>
        public AssemblyMatchingRule(Assembly assembly)
            : this((assembly != null) ? assembly.FullName : null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
        }

        /// <summary>
        /// Determines if the supplied <paramref name="member"/> matches the rule.
        /// </summary>
        /// <remarks>
        /// This rule matches if the assembly containing the given <paramref name="member"/>
        /// matches the name given. The rule used for matches lets you include the parts
        /// of the assembly name in order. You can specify assembly name only, assembly and version,
        /// assembly, version and culture, or the fully qualified assembly name.
        /// </remarks>
        /// <param name="member">Member to check.</param>
        /// <returns>true if <paramref name="member"/> is in a matching assembly, false if not.</returns>
        public bool Matches(MethodBase member)
        {
            Guard.ArgumentNotNull(member, "member");

            if (member.DeclaringType == null)
            {
                return false;
            }

            AssemblyName assembly = member.DeclaringType.Assembly.GetName();

            return DoesAssemblyNameMatchString(assemblyName, assembly);
        }

        private static bool DoesAssemblyNameMatchString(string assemblyNameString, AssemblyName assemblyName)
        {
            AssemblyName assemblyNameToMatch = null;
            try
            {
                assemblyNameToMatch = new AssemblyName(assemblyNameString);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (string.Compare(assemblyName.Name, assemblyNameToMatch.Name, StringComparison.Ordinal) == 0)
            {
                if (assemblyNameToMatch.Version != null &&
                    assemblyNameToMatch.Version.CompareTo(assemblyName.Version) != 0)
                {
                    return false;
                }
                byte[] requestedAsmPublicKeyToken = assemblyNameToMatch.GetPublicKeyToken();
                if (requestedAsmPublicKeyToken != null)
                {
                    byte[] cachedAssemblyPublicKeyToken = assemblyName.GetPublicKeyToken();

                    if (Convert.ToBase64String(requestedAsmPublicKeyToken) != Convert.ToBase64String(cachedAssemblyPublicKeyToken))
                    {
                        return false;
                    }
                }

                CultureInfo requestedAssemblyCulture = assemblyNameToMatch.CultureInfo;

                if (requestedAssemblyCulture != null && !requestedAssemblyCulture.IsInvariantCulture())
                {
                    if (!assemblyName.CultureInfo.IsSameAs(requestedAssemblyCulture))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
