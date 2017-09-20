// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


using ObjectBuilder2;

namespace Unity
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child created as a singleton per container.
    /// </summary>
    public class HierarchicalLifetimeManager : ContainerControlledLifetimeManager, IScopeLifetimePolicy
    {
        public IBuilderPolicy CreateScope()
        {
            return new HierarchicalLifetimeManager();
        }
    }
}
