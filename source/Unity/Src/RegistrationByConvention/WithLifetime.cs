// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// Provides helper methods to specify the lifetime for a type with registration by convention.
    /// </summary>
    public static partial class WithLifetime
    {
        /// <summary>
        /// Returns a <see langword="null"/> <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A lifetime manager</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager None(Type type)
        {
            return null;
        }

        /// <summary>
        /// Returns a <see cref="ContainerControlledLifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A container controlled lifetime manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The purpose of the method is to return a new instance")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager ContainerControlled(Type type)
        {
            return new ContainerControlledLifetimeManager();
        }

        /// <summary>
        /// Returns a <see cref="ExternallyControlledLifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An externally controlled lifetime manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager ExternallyControlled(Type type)
        {
            return new ExternallyControlledLifetimeManager();
        }

        /// <summary>
        /// Returns a <see cref="HierarchicalLifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A hierarchical lifetime manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The purpose of the method is to return a new instance")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager Hierarchical(Type type)
        {
            return new HierarchicalLifetimeManager();
        }

        /// <summary>
        /// Returns a <see cref="PerResolveLifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A per resolve lifetime manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager PerResolve(Type type)
        {
            return new PerResolveLifetimeManager();
        }

        /// <summary>
        /// Returns a <see cref="TransientLifetimeManager"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A transient lifetime manager.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager Transient(Type type)
        {
            return new TransientLifetimeManager();
        }

        /// <summary>
        /// Returns a <see cref="LifetimeManager" />.
        /// </summary>
        /// <typeparam name="T">The custom <see cref="LifetimeManager"/> type.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A lifetime manager.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "Need to match signature Func<Type, string>")]
        public static LifetimeManager Custom<T>(Type type)
            where T : LifetimeManager, new()
        {
            return new T();
        }
    }
}
