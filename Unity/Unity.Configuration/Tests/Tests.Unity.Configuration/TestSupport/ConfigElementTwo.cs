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
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.TestSupport;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class ConfigElementTwo : UnityContainerExtensionConfigurationElement
    {
        [ConfigurationProperty("message")]
        public string Message
        {
            get { return (string)this["message"]; }
            set { this["message"] = value; }
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
            container.Configure<IConfigTwo>().SetMessage(Message);
        }
    }
}
