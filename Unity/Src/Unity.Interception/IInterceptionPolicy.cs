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
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A policy that determines how to add interception to an object.
    /// </summary>
    public interface IInterceptionPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="PolicyInjector"/> that must be used to intercept an object.
        /// </summary>
        PolicyInjector Injector
        {
            get;
        }
    }
}
