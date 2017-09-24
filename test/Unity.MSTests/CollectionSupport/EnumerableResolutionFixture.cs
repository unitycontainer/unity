using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;

namespace Unity.Tests.CollectionSupport
{
    [TestClass]
    public class EnumerableResolutionFixture
    {
        private IUnityContainer container;


        [TestInitialize()]
        public void Initialize()
        {
            container = new UnityContainer();
            container.RegisterType<ITestInterface, TestClass>();
            container.RegisterType<ITestInterface, TestClass>("a");
            container.RegisterType<ITestInterface, TestClass>("b");
            container.RegisterType<ITestInterface, TestClass>("c");
            container.RegisterType<ITestInterface, TestClass>("d");
        }


        [TestMethod]
        public void Enumerable_SanityCheck()
        {
            var noname = container.Resolve<ITestInterface>();
            var named = container.ResolveAll<ITestInterface>();

            Assert.IsNotNull(noname);
            Assert.IsNotNull(named);
        }


        [TestMethod]
        public void Enumerable_ResolveAsEnumerable()
        {
//            var all = container.Resolve<IEnumerable<ITestInterface>>();

//            Assert.IsNotNull(all);
        }


        [TestCleanup()]
        public void Cleanup()
        {
            container.Dispose();
            container = null;
        }

    }
}
