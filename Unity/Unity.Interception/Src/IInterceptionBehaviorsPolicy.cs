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

using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An <see cref="IBuilderPolicy"/> that returns a sequence of <see cref="IInterceptionBehaviorDescriptor"/> 
    /// instances representing the behaviors for an intercepted object.
    /// </summary>
    public interface IInterceptionBehaviorsPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="IInterceptionBehaviorDescriptor"/> instances accumulated by this policy.
        /// </summary>
        IEnumerable<IInterceptionBehaviorDescriptor> InterceptionBehaviorDescriptors { get; }
    }
}
