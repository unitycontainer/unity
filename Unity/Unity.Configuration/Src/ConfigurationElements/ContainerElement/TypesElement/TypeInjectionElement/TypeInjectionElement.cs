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

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// Parent element used to configure member injection for a type.
    /// </summary>
    public class TypeInjectionElement : UnityContainerTypeConfigurationElement, IContainerConfigurationCommand
    {
        /// <summary>
        /// This element is just a collection of <see cref="InjectionMemberElement"/>s. This
        /// property controls that collection.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(InjectionMemberElementCollection), AddItemName = "injector")]
        public InjectionMemberElementCollection InjectionMembers
        {
            get
            {
                InjectionMemberElementCollection members =
                    (InjectionMemberElementCollection)this[""];
                members.TypeResolver = TypeResolver;
                return members;
            }
        }


        /// <summary>
        /// Execute this command against the given container.
        /// </summary>
        /// <remarks>
        /// Interface implementors will implement this method to
        /// apply configuration changes to the container.</remarks>
        /// <param name="container">The <see cref="IUnityContainer"/> to configure.</param>
        public override void Configure(IUnityContainer container)
        {
            List<InjectionMember> injections = new List<InjectionMember>();
            foreach(InjectionMemberElement element in InjectionMembers)
            {
                injections.Add(element.CreateInjectionMember());
            }

            var serviceType = ParentElement.Type;
            var implementationType = ParentElement.MapTo;
            if(implementationType == null)
            {
                implementationType = serviceType;
                serviceType = null;
            }
            
            container.RegisterType(serviceType, implementationType, ParentElement.Name, injections.ToArray());
        }
    }
}
