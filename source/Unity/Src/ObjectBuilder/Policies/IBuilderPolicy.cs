// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Represents a builder policy interface. Since there are no fixed requirements
    /// for policies, it acts as a marker interface from which to derive all other
    /// policy interfaces.
    /// </summary>
    // FxCop suppression: No
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IBuilderPolicy { }
}
