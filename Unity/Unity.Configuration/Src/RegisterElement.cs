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
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element representing a single container type registration.
    /// </summary>
    public class RegisterElement : ContainerConfiguringElement
    {
        private const string InjectionMembersPropertyName = "";
        private const string LifetimePropertyName = "lifetime";
        private const string MapToPropertyName = "mapTo";
        private const string NamePropertyName = "name";
        private const string TypePropertyName = "type";

        /// <summary>
        /// The type that is registered.
        /// </summary>
        [ConfigurationProperty(TypePropertyName, IsRequired = true, IsKey = true)]
        public string TypeName
        {
            get { return (string) base[TypePropertyName]; }
            set { base[TypePropertyName] = value; }
        }

        /// <summary>
        /// Name registered under.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, DefaultValue = "", IsRequired = false)]
        public string Name
        {
            get { return (string) base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Type that is mapped to.
        /// </summary>
        [ConfigurationProperty(MapToPropertyName, DefaultValue = "", IsRequired = false)]
        public string MapToName
        {
            get { return (string) base[MapToPropertyName]; }
            set { base[MapToPropertyName] = value; }
        }

        /// <summary>
        /// Lifetime manager to register for this registration.
        /// </summary>
        [ConfigurationProperty(LifetimePropertyName, IsRequired = false)]
        public LifetimeElement Lifetime
        {
            get { return (LifetimeElement) base[LifetimePropertyName]; }
            set { base[LifetimePropertyName] = value; }
        }

        /// <summary>
        /// Any injection members (constructor, properties, etc.) that are specified for
        /// this registration.
        /// </summary>
        [ConfigurationProperty(InjectionMembersPropertyName, IsDefaultCollection = true)]
        public InjectionMemberElementCollection InjectionMembers
        {
            get { return (InjectionMemberElementCollection) base[InjectionMembersPropertyName]; }
        }

        /// <summary>
        /// Apply the registrations from this element to the given container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            Type registeringType = GetRegisteringType();
            Type mappedType = GetMappedType();
            LifetimeManager lifetime = Lifetime.CreateLifetimeManager();
            IEnumerable<InjectionMember> injectionMembers =
                InjectionMembers.SelectMany(m => m.GetInjectionMembers(container, registeringType, mappedType, Name));
            container.RegisterType(registeringType, mappedType, Name, lifetime, injectionMembers.ToArray());
        }

        private Type GetRegisteringType()
        {
            if (!string.IsNullOrEmpty(MapToName))
            {
                return TypeResolver.ResolveType(TypeName);
            }
            return null;
        }

        private Type GetMappedType()
        {
            if (string.IsNullOrEmpty(MapToName))
            {
                return TypeResolver.ResolveType(TypeName);
            }
            return TypeResolver.ResolveType(MapToName);
        }
    }
}
