// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.TestSupport
{
    public class RegistrationsToAssertOn
    {
        public readonly IEnumerable<ContainerRegistration> Registrations;

        public RegistrationsToAssertOn(IEnumerable<ContainerRegistration> registrations)
        {
            this.Registrations = registrations;
        }

        public void HasLifetime<TLifetime>() where TLifetime : LifetimeManager
        {
            Assert.IsTrue(Registrations.All(r => r.LifetimeManagerType == typeof(TLifetime)));
        }
    }
}
