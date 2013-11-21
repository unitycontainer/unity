// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Attribute used to indicate that no interception should be applied to
    /// the attribute target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class ApplyNoPoliciesAttribute : Attribute
    {
    }
}
