// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Base class for Unity configuration section extensions.
    /// Derived classes are used to add custom elments and aliases
    /// into the configuration section being loaded.
    /// </summary>
    public abstract class SectionExtension
    {
        /// <summary>
        /// Add the extensions to the section via the context.
        /// </summary>
        /// <param name="context">Context object that can be used to add elements and aliases.</param>
        public abstract void AddExtensions(SectionExtensionContext context);
    }
}
