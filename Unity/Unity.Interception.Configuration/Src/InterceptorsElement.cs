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
using System.Text;
using System.Xml;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element that provides a top-level element for
    /// configuration interceptors for types in a container.
    /// </summary>
    public class InterceptorsElement : ContainerConfiguringElement
    {
        private const string InterceptorsPropertyName = "";

        /// <summary>
        /// The various child elements that are contained in this element.
        /// </summary>
        [ConfigurationProperty(InterceptorsPropertyName, IsDefaultCollection = true)]
        public InterceptorsInterceptorElementCollection Interceptors
        {
            get { return (InterceptorsInterceptorElementCollection) base[InterceptorsPropertyName]; }
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        public override void SerializeContent(XmlWriter writer)
        {
            foreach(var interceptorElement in Interceptors)
            {
                writer.WriteElement("interceptor", interceptorElement.SerializeContent);
            }
        }

        /// <summary>
        /// Apply this element's configuration to the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            foreach(var interceptorElement in Interceptors)
            {
                interceptorElement.ConfigureContainer(container);
            }
        }
    }
}
