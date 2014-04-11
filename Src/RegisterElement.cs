// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
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
            get { return (string)base[TypePropertyName]; }
            set { base[TypePropertyName] = value; }
        }

        /// <summary>
        /// Name registered under.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, DefaultValue = "", IsRequired = false, IsKey = true)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Type that is mapped to.
        /// </summary>
        [ConfigurationProperty(MapToPropertyName, DefaultValue = "", IsRequired = false)]
        public string MapToName
        {
            get { return (string)base[MapToPropertyName]; }
            set { base[MapToPropertyName] = value; }
        }

        /// <summary>
        /// Lifetime manager to register for this registration.
        /// </summary>
        [ConfigurationProperty(LifetimePropertyName, IsRequired = false)]
        public LifetimeElement Lifetime
        {
            get { return (LifetimeElement)base[LifetimePropertyName]; }
            set { base[LifetimePropertyName] = value; }
        }

        /// <summary>
        /// Any injection members (constructor, properties, etc.) that are specified for
        /// this registration.
        /// </summary>
        [ConfigurationProperty(InjectionMembersPropertyName, IsDefaultCollection = true)]
        public InjectionMemberElementCollection InjectionMembers
        {
            get { return (InjectionMemberElementCollection)base[InjectionMembersPropertyName]; }
        }

        /// <summary>
        /// Apply the registrations from this element to the given container.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            Type registeringType = this.GetRegisteringType();
            Type mappedType = this.GetMappedType();
            LifetimeManager lifetime = this.Lifetime.CreateLifetimeManager();
            IEnumerable<InjectionMember> injectionMembers =
                this.InjectionMembers.SelectMany(m => m.GetInjectionMembers(container, registeringType, mappedType, this.Name));
            container.RegisterType(registeringType, mappedType, this.Name, lifetime, injectionMembers.ToArray());
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void SerializeContent(XmlWriter writer)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(writer, "writer");

            writer.WriteAttributeString(TypePropertyName, this.TypeName);
            writer.WriteAttributeIfNotEmpty(MapToPropertyName, this.MapToName)
                .WriteAttributeIfNotEmpty(NamePropertyName, this.Name);

            if (!string.IsNullOrEmpty(this.Lifetime.TypeName))
            {
                writer.WriteElement("lifetime", this.Lifetime.SerializeContent);
            }

            this.SerializeInjectionMembers(writer);
        }

        private Type GetRegisteringType()
        {
            if (!string.IsNullOrEmpty(this.MapToName))
            {
                return TypeResolver.ResolveType(this.TypeName);
            }
            return null;
        }

        private Type GetMappedType()
        {
            if (string.IsNullOrEmpty(this.MapToName))
            {
                return TypeResolver.ResolveType(this.TypeName);
            }
            return TypeResolver.ResolveType(this.MapToName);
        }

        private void SerializeInjectionMembers(XmlWriter writer)
        {
            foreach (var member in this.InjectionMembers)
            {
                writer.WriteElement(member.ElementName, member.SerializeContent);
            }
        }
    }
}
