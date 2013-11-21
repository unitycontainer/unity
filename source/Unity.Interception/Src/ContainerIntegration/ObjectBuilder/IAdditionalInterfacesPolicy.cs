// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An <see cref="IBuilderPolicy"/> that returns a sequence of <see cref="Type"/> 
    /// instances representing the additional interfaces for an intercepted object.
    /// </summary>
    public interface IAdditionalInterfacesPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="Type"/> instances accumulated by this policy.
        /// </summary>
        IEnumerable<Type> AdditionalInterfaces { get; }
    }
}
