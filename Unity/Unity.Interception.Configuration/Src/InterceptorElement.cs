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

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element for configuring interceptors.
    /// </summary>
    public class InterceptorElement : InterceptionMemberElement
    {
        /// <summary>
        /// Returns the InjectionMember object represented by this configuration
        /// element.
        /// </summary>
        /// <returns>The <see cref="AdditionalInterface"/> object.</returns>
        public override InjectionMember CreateInjectionMember()
        {
            return new Interceptor(this.GetResolvedType<IInterceptor>(TypeName));
        }

        /// <summary>
        /// Returns the string name of the type of the represented object.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }
}
