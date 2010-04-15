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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// Summary description for CodeplexIssuesFixture
    /// </summary>
    [TestClass]
    public class CodeplexIssuesFixture
    {
        public interface IRepository { }
        public class TestRepository : IRepository { }
        public class TestService
        {
            public TestService(IRepository repository)
            {
            }
        }

        [TestMethod]
        public void DependenciesAndInterceptionMixProperly()
        {
            var container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<IRepository, TestRepository>()
                .RegisterType<TestService>(
                    new Interceptor<VirtualMethodInterceptor>())
                    ;

            var svc1 = container.Resolve<TestService>();
            var svc2 = container.Resolve<TestService>();

            Assert.AreNotSame(svc1, svc2);
            Assert.IsNotNull(svc1 as IInterceptingProxy);
            Assert.IsNotNull(svc2 as IInterceptingProxy);
        }
    }
}
