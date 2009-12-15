using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class ContainerConfigElementOne : ContainerConfiguringElement
    {
        public static bool ConfigureWasCalled = false;
        /// <summary>
        /// Apply this element's configuration to the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Container to configure.</param>
        protected override void ConfigureContainer(IUnityContainer container)
        {
            ConfigureWasCalled = true;
        }
    }
}
