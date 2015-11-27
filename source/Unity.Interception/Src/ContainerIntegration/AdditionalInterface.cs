// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using ObjectBuilder2;
using Unity.InterceptionExtension.Properties;
using Unity.Utility;

namespace Unity.InterceptionExtension
{
    /// <summary>
    /// Stores information about a single <see cref="Type"/> to be an additional interface for an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    public class AdditionalInterface : InterceptionMember
    {
        private readonly Type additionalInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalInterface"/> with a 
        /// <see cref="Type"/>.
        /// </summary>
        /// <param name="additionalInterface">A descriptor representing the interception behavior to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="additionalInterface"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="additionalInterface"/> is not an interface.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public AdditionalInterface(Type additionalInterface)
        {
            Guard.ArgumentNotNull(additionalInterface, "additionalInterface");
            if (!additionalInterface.IsInterface)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ExceptionTypeIsNotInterface,
                        additionalInterface.Name),
                    "additionalInterface");
            }

            this.additionalInterface = additionalInterface;
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the container to use the represented 
        /// <see cref="Type"/> as an additional interface for the supplied parameters.
        /// </summary>
        /// <param name="serviceType">Interface being registered.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            AdditionalInterfacesPolicy policy =
                AdditionalInterfacesPolicy.GetOrCreate(policies, implementationType, name);
            policy.AddAdditionalInterface(this.additionalInterface);
        }
    }

    /// <summary>
    /// Stores information about a single <see cref="Type"/> to be an additional interface for an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    /// <typeparam name="T">The interface.</typeparam>
    public class AdditionalInterface<T> : AdditionalInterface
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalInterface{T}"/>.
        /// </summary>
        public AdditionalInterface()
            : base(typeof(T))
        {
        }
    }
}
