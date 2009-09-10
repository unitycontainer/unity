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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class DelegateInterceptionBehaviorDescriptor : IInterceptionBehaviorDescriptor
    {
        private Func<IInterceptor, Type, Type, IUnityContainer, IInterceptionBehavior> getInterceptionBehavior;

        public DelegateInterceptionBehaviorDescriptor(
            Func<IInterceptor, Type, Type, IUnityContainer, IInterceptionBehavior> getInterceptionBehavior)
        {
            this.getInterceptionBehavior = getInterceptionBehavior;
        }

        public IInterceptionBehavior GetInterceptionBehavior(
            IInterceptor interceptor,
            Type interceptedType,
            Type implementationType,
            IUnityContainer container)
        {
            return this.getInterceptionBehavior(interceptor, interceptedType, implementationType, container);
        }
    }
}
