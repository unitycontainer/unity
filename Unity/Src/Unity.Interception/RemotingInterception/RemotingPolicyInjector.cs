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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This class holds onto a <see cref="PolicySet" /> and can
    /// inject that policy via a remoting proxy into either a newly created object or
    /// an existing object.
    /// </summary>
    public class RemotingPolicyInjector : PolicyInjector
    {
        /// <summary>
        /// Creates a new <see cref="RemotingPolicyInjector" /> with an 
        /// empty <see cref="PolicySet" />.
        /// </summary>
        public RemotingPolicyInjector()
        {
        }

        /// <summary>
        /// Checks to see if the given type can be intercepted.
        /// </summary>
        /// <remarks>In this implementation, only interfaces and types derived from MarshalByRefObject
        /// can have policies applied.</remarks>
        /// <param name="t">Type to check.</param>
        /// <returns>True if this type can be intercepted, false if it cannot.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public override bool TypeSupportsInterception(Type t)
        {
            Guard.ArgumentNotNull(t, "t");

            return (typeof(MarshalByRefObject).IsAssignableFrom(t) || t.IsInterface);
        }

        /// <summary>
        /// Wraps the given instance in a proxy with interception hooked up if it
        /// is required by policy. If not required, returns the unwrapped instance.
        /// </summary>
        /// <param name="instance">object to wrap.</param>
        /// <param name="typeToReturn">Type of the reference to return.</param>
        /// <param name="policiesForThisType">Policy set specific to typeToReturn.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>The object with policy added.</returns>
        protected override object DoWrap(
            object instance,
            Type typeToReturn,
            PolicySet policiesForThisType,
            IUnityContainer container)
        {
            if (PolicyRequiresInterception(policiesForThisType))
            {
                object unwrappedInstance = UnwrapTarget(instance);
                InterceptingRealProxy proxy =
                    new InterceptingRealProxy(unwrappedInstance, typeToReturn, policiesForThisType, container);
                return proxy.GetTransparentProxy();
            }
            return instance;
        }

        private static object UnwrapTarget(object target)
        {
            if (RemotingServices.IsTransparentProxy(target))
            {
                InterceptingRealProxy realProxy =
                    RemotingServices.GetRealProxy(target) as InterceptingRealProxy;
                if (realProxy != null)
                {
                    return realProxy.Target;
                }
            }
            return target;
        }
    }
}
