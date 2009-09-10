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
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    [TestClass]
    public class ExistingInterceptionBehaviorDescriptorFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreatingANewInstanceWithANullBehaviorThrows()
        {
            new ExistingInterceptionBehaviorDescriptor(null);
        }

        [TestMethod]
        public void DescriptorReturnsSuppliedBehavior()
        {
            IInterceptionBehavior behavior = new DelegateInterceptionBehavior((mi, gn) => null);
            IInterceptionBehaviorDescriptor descriptor = new ExistingInterceptionBehaviorDescriptor(behavior);

            IInterceptionBehavior returnedBehavior = descriptor.GetInterceptionBehavior(null, null, null, null);

            Assert.AreSame(behavior, returnedBehavior);
        }
    }
}
