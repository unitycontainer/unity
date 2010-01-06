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

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// The &lt;key&gt; element that occurs inside an &lt;interceptor&gt; element
    /// </summary>
    public class KeyElement : InterceptorRegistrationElement
    {
        private const string NamePropertyName = "name";

        /// <summary>
        /// Name registration should be under. To register under the default, leave blank.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = false)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        internal override string Key
        {
            get { return "key:" + TypeName + ":" + Name; }
        }

        /// <summary>
        /// Actually register the interceptor against this type.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        /// <param name="interceptor">interceptor to register.</param>
        internal override void RegisterInterceptor(IUnityContainer container, IInterceptor interceptor)
        {
            var typeInterceptor = interceptor as ITypeInterceptor;
            if (typeInterceptor != null)
            {
                container.Configure<Interception>().SetInterceptorFor(
                    ResolvedType, Name,
                    typeInterceptor);
            }
            else
            {
                container.Configure<Interception>().SetInterceptorFor(
                    ResolvedType, Name,
                    (IInstanceInterceptor)interceptor);

            }
        }
    }
}
