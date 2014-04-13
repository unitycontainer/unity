// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.TransparentProxyInterception
{
    [TestClass]
    public class IntegrationFixture
    {
        [TestMethod]
        public void CanInterceptGenericMethodWithHandlerAttributeThroughInterface()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IInterfaceWithGenericMethod, ClassWithGenericMethod>(
                new Interceptor(new TransparentProxyInterceptor()));

            var instance = container.Resolve<IInterfaceWithGenericMethod>();

            var result = instance.DoSomething<int>();

            Assert.AreEqual(0, result);
        }
    }
}
