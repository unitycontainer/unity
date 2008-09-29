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
using System.Globalization;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="PolicyInjector"/> is a class that is responsible for
    /// determining if a particular type is interceptable based on the
    /// specific interception mechanism it implements, and will create
    /// the interceptor.
    /// </summary>
    public abstract class PolicyInjector
    {
        /// <summary>
        /// Creates a new <see cref="TransparentProxyPolicyInjector" /> with an 
        /// empty <see cref="PolicySet" />.
        /// </summary>
        protected PolicyInjector()
        {
        }

        /// <summary>
        /// Checks to see if the given type can be intercepted.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if this type can be intercepted, false if it cannot.</returns>
        public abstract bool TypeSupportsInterception(Type t);

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
        protected abstract object DoWrap(
            object instance,
            Type typeToReturn,
            PolicySet policiesForThisType,
            IUnityContainer container);

        /// <summary>
        /// Takes an existing object and returns a new reference that includes
        /// the policies specified in the current <see cref="PolicySet"/>.
        /// </summary>
        /// <param name="instance">The object to wrap.</param>
        /// <param name="typeToReturn">Type to return. This can be either an
        /// interface implemented by the object, or its concrete class.</param>
        /// <param name="allPolicies">The <see cref="PolicySet"/> containing all the available policies.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>A new reference to the object that includes the policies.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public object Wrap(
            object instance,
            Type typeToReturn,
            PolicySet allPolicies,
            IUnityContainer container)
        {
            Guard.ArgumentNotNull(allPolicies, "allPolicies");

            PolicySet policiesForThisType = allPolicies.GetPoliciesFor(instance.GetType());
            EnsureTypeIsInterceptable(typeToReturn, policiesForThisType);
            return DoWrap(instance, typeToReturn, policiesForThisType, container);
        }

        /// <summary>
        /// Takes an existing object and returns a new reference that includes
        /// the policies specified in the current <see cref="PolicySet"/>.
        /// </summary>
        /// <typeparam name="TInterface">The type of wrapper to return. Can be either
        /// an interface implemented by the target instance or its entire concrete type.</typeparam>
        /// <param name="instance">The object to wrap.</param>
        /// <param name="allPolicies">The <see cref="PolicySet"/> containing all the available policies.</param>
        /// <param name="container">The <see cref="IUnityContainer"/> to use when creating handlers,
        /// if necessary.</param>
        /// <returns>A new reference to the object that includes the policies.</returns>
        public TInterface Wrap<TInterface>(
            object instance,
            PolicySet allPolicies,
            IUnityContainer container)
        {
            return (TInterface)Wrap(instance, typeof(TInterface), allPolicies, container);
        }

        /// <summary>
        /// Checks to see if the given type has any policies that apply to it.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <param name="allPolicies">The <see cref="PolicySet"/> containing all the available policies.</param>
        /// <returns>true if the current set of policies will require interception to be added,
        /// false if no policies apply to type t</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        [SuppressMessage("Microsoft.Performance", "CA1822")]
        public bool TypeRequiresInterception(Type t, PolicySet allPolicies)
        {
            Guard.ArgumentNotNull(allPolicies, "allPolicies");
            return PolicyRequiresInterception(allPolicies.GetPoliciesFor(t));
        }

        /// <summary>
        /// Checks to see if the given policy set requires interception on targets that it is applied to.
        /// </summary>
        /// <param name="policies">Policy set to check.</param>
        /// <returns>True if policy set contains anything, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by callers.")]
        protected static bool PolicyRequiresInterception(PolicySet policies)
        {
            return policies.Count > 0;
        }

        /// <summary>
        /// Checks to see if the given type requires interception and if so if it
        /// is actually interceptable or not. If not, throws <see cref="System.ArgumentException"/>.
        /// </summary>
        /// <param name="typeToReturn">Type to check.</param>
        /// <param name="policiesForThisType">Policy set specific to typeToReturn.</param>
        private void EnsureTypeIsInterceptable(Type typeToReturn, PolicySet policiesForThisType)
        {
            if (PolicyRequiresInterception(policiesForThisType))
            {
                if (!TypeSupportsInterception(typeToReturn))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.InterceptionNotSupported,
                            typeToReturn.Name),
                        "typeToReturn");
                }
            }
        }
    }
}