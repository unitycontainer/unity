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

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> that holds the information
    /// about instances to be placed in the container.
    /// </summary>
    public class UnityInstanceElement : InstanceValueElement, IContainerConfigurationCommand
    {
        /// <summary>
        /// Name to use when registering this instance. Optional.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false, DefaultValue = null)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.</remarks>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        // FxCop suppression: Lifetime manager is given to container to dispose
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        // FxCop suppression: Validation done via Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public void Configure(IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            string nameToRegister = Name;

            // Config system returns empty string if not specified. Make sure it's null.
            if (string.IsNullOrEmpty(nameToRegister))
            {
                nameToRegister = null;
            }

            object valueToRegister = CreateInstance();
            container.RegisterInstance(TypeToCreate, nameToRegister, 
                valueToRegister, new ContainerControlledLifetimeManager());
        }
    }
}
