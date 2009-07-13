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
    public interface IBuilderPolicy {}
}
