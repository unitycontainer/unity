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
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A <see cref="IInterceptionPolicy"/> that holds a reference to a <see cref="PolicyInjector"/>
    /// </summary>
    public class InterceptionPolicy : IInterceptionPolicy
    {
        private readonly PolicyInjector policyInjector;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptionPolicy"/> class with an injector.
        /// </summary>
        /// <param name="policyInjector">The <see cref="PolicyInjector"/>.</param>
        public InterceptionPolicy(PolicyInjector policyInjector)
        {
            Guard.ArgumentNotNull(policyInjector, "policyInjector");

            this.policyInjector = policyInjector;
        }

        /// <summary>
        /// Gets the <see cref="PolicyInjector"/> that must be used to intercept an object.
        /// </summary>
        public PolicyInjector Injector
        {
            get { return this.policyInjector; }
        }
    }
}
