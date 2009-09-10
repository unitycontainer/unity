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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> class used to manage the contents
    /// of a &lt;type&gt; node in the configuration file.
    /// </summary>
    /// <remarks>
    /// Using the Type element in configuration can result in both type mappings
    /// and singletons being registered. If a name is specified, it is used
    /// for both the type mapping and singleton setting.</remarks>
    public class UnityTypeElement : TypeResolvingConfigurationElement, IContainerConfigurationCommand
    {
        /// <summary>
        /// Name to use when registering this type. 
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = null, IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Source type to configure in the container.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// The actual <see cref="System.Type"/> object for the 
        /// type this element is registering.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Back compat")]
        public Type Type
        {
            get { return this.TypeResolver.ResolveType(this.TypeName); }
        }

        /// <summary>
        /// Destination type for type mapping.
        /// </summary>
        [ConfigurationProperty("mapTo", IsRequired = false)]
        public string MapToName
        {
            get { return (string)this["mapTo"]; }
            set { this["mapTo"] = value; }
        }

        /// <summary>
        /// The actual <see cref='Type'/> object for the mapTo element in
        /// the configuration file.
        /// </summary>
        public Type MapTo
        {
            get
            {
                if(string.IsNullOrEmpty(MapToName)) return null;
                return this.TypeResolver.ResolveType(this.MapToName);
            }
        }

        /// <summary>
        /// Sets the lifetime for the given type and name. Transient means 
        /// to create a new instance every type and is the default.
        /// Singleton means to return the same instance on every request.
        /// </summary>
        /// <remarks>
        /// When configuring a singleton, if both the type and mapTo attributes
        /// are specified, the SetSingleton call will be done on the type
        /// specified in the mapTo attribute. Otherwise it'll be done on the
        /// type specified in the Type attribute.
        /// </remarks>
        [ConfigurationProperty("lifetime", IsRequired = false)]
        public UnityLifetimeElement Lifetime
        {
            get
            {
                UnityLifetimeElement lifetimeElement = (UnityLifetimeElement)this["lifetime"];
                lifetimeElement.TypeResolver = TypeResolver;
                return lifetimeElement;
            }

            set { this["lifetime"] = value; }
        }

        /// <summary>
        /// The collection of type configuration elements that are used to add
        /// arbitrary new types to the configuration file.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(UnityContainerTypeConfigurationElementCollection), AddItemName = "typeConfig")]
        public UnityContainerTypeConfigurationElementCollection TypeConfig
        {
            get 
            { 
                UnityContainerTypeConfigurationElementCollection typeConfigCollection = (UnityContainerTypeConfigurationElementCollection)this[""];
                typeConfigCollection.TypeResolver = TypeResolver;
                return typeConfigCollection;
            }
        }

        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.</remarks>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        // FxCop suppression: Validation done via Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        // FxCop suppression: Lifetime manager is passed to container to dispose.
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public void Configure(IUnityContainer container)
        {
            Guard.ArgumentNotNull(container, "container");
            LifetimeManager lifetimeManager = null;
            if(Lifetime != null)
            {
                lifetimeManager = Lifetime.CreateLifetimeManager();
            }

            var serviceType = this.Type;
            var implementationType = this.MapTo;

            if(MapTo == null)
            {
                implementationType = this.Type;
                serviceType = null;
            }


            container.RegisterType(serviceType, implementationType, Name, lifetimeManager);

            foreach (UnityContainerTypeConfigurationElement typeConfigElement in this.TypeConfig)
            {
                typeConfigElement.ParentElement = this;
                typeConfigElement.Configure(container);
            }
        }
    }
}
