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

using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Base class for elements representing parts of a <see cref="RuleDrivenPolicy"/> in a
    /// &lt;policy&gt; node in the configuration file.
    /// </summary>
    public class PolicyElementConfigurationElement : TypeResolvingConfigurationElement
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
        /// Indicates whether the configuration element has information.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the configuration element has information, 
        /// otherwise <see langword="false"/>.
        /// </value>
        public bool HasData
        {
            get { return this.InjectionMembers.Count > 0; }
        }

        internal InjectionMember[] GetInjectionMembers()
        {
            List<InjectionMember> injections = new List<InjectionMember>();
            foreach (InjectionMemberElement element in InjectionMembers)
            {
                injections.Add(element.CreateInjectionMember());
            }

            return injections.ToArray();
        }
    }
}
