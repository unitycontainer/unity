// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    public class HierarchicalLifetimeManager : ContainerControlledLifetimeManager
    {
    }
}
