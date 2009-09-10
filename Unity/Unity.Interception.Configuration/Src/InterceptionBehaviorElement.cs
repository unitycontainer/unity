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
using System.Xml;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Properties;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element for configuring interception behaviors.
    /// </summary>
    public class InterceptionBehaviorElement : InterceptionMemberElement
    {
        /// <summary>
        /// Returns the InjectionMember object represented by this configuration
        /// element.
        /// </summary>
        /// <remarks>
        /// The nature of the returned <see cref="InterceptionBehavior"/> depends on how  the 
        /// element is configured: an interception behavior type, an interception behavior descriptor type
        /// or a key to resolve a behavior from a unity container.
        /// </remarks>
        /// <returns>The <see cref="InterceptionBehavior"/> object.</returns>
        public override InjectionMember CreateInjectionMember()
        {
            if (!string.IsNullOrEmpty(this.BehaviorTypeName))
            {
                return new InterceptionBehavior(this.CreateInstance<IInterceptionBehavior>(BehaviorTypeName));
            }
            if (!string.IsNullOrEmpty(this.BehaviorDescriptorTypeName))
            {
                return new InterceptionBehavior(
                    this.CreateInstance<IInterceptionBehaviorDescriptor>(BehaviorDescriptorTypeName));
            }

            return new InterceptionBehavior(new ContainerResolvedInterceptionBehaviorDescriptor(this.BehaviorName));
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> that reads from the configuration file.</param>
        /// <param name="serializeCollectionKey"><see langword="true"/> to serialize only the collectionkey properties; 
        /// otherwise, <see langword="false"/>.</param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            int dataItems = 0;

            if (reader.MoveToAttribute("behaviorType")) dataItems++;
            if (reader.MoveToAttribute("behaviorDescriptorType")) dataItems++;
            if (reader.MoveToAttribute("behavior")) dataItems++;
            reader.MoveToElement();

            if (dataItems != 1)
            {
                throw new ConfigurationErrorsException(Resources.MustHaveAtLeastOneBehaviorAttribute, reader);
            }

            base.DeserializeElement(reader, serializeCollectionKey);
        }

        /// <summary>
        /// Returns the string name of the type of the interception behavior represented by this element.
        /// </summary>
        [ConfigurationProperty("behaviorType")]
        public string BehaviorTypeName
        {
            get { return (string)this["behaviorType"]; }
            set { this["behaviorType"] = value; }
        }

        /// <summary>
        /// Returns the string name of the type of the interception behavior descriptor represented by this element.
        /// </summary>
        [ConfigurationProperty("behaviorDescriptorType")]
        public string BehaviorDescriptorTypeName
        {
            get { return (string)this["behaviorDescriptorType"]; }
            set { this["behaviorDescriptorType"] = value; }
        }

        /// <summary>
        /// Returns the name to use to resolve the interception behavior represented by this element.
        /// </summary>
        [ConfigurationProperty("behavior")]
        public string BehaviorName
        {
            get { return (string)this["behavior"]; }
            set { this["behavior"] = value; }
        }
    }
}
