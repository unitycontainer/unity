// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Web.Mvc;
using Unity.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.WebIntegation.Tests
{
    [TestClass]
    public class MvcUnityDependencyResolverFixture
    {
        [TestMethod]
        public void When_resolving_then_returns_registered_instance()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
                var resolver = new UnityDependencyResolver(container);

                var actual = (IFoo)resolver.GetService(typeof(IFoo));
                Assert.AreEqual("value", actual.TestProperty);
            }
        }

        [TestMethod]
        public void When_resolving_multiple_then_returns_all_registered_instances()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>("instance1", new Foo { TestProperty = "value1" });
                container.RegisterInstance<IFoo>("instance2", new Foo { TestProperty = "value2" });
                var resolver = new UnityDependencyResolver(container);

                var actual = resolver.GetServices(typeof(IFoo)).Cast<IFoo>().ToList();
                Assert.IsTrue(actual.Any(x => x.TestProperty == "value1"));
                Assert.IsTrue(actual.Any(x => x.TestProperty == "value2"));
            }
        }

        [TestMethod]
        public void When_resolving_unregistered_type_then_returns_null()
        {
            using (var container = new UnityContainer())
            {
                var resolver = new UnityDependencyResolver(container);

                Assert.IsNull(resolver.GetService(typeof(IFoo)));
            }
        }

        [TestMethod]
        public void When_resolving_concrete_controller_then_returns_injected_instance()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
                var resolver = new UnityDependencyResolver(container);

                var actual = (TestController)resolver.GetService(typeof(TestController));
                Assert.AreEqual("value", actual.Foo.TestProperty);
            }
        }

        [TestMethod]
        public void When_resolving_controller_with_unregistered_dependencies_then_throws()
        {
            using (var container = new UnityContainer())
            {
                var resolver = new UnityDependencyResolver(container);

                AssertThrows<ResolutionFailedException>(() => resolver.GetService(typeof(TestController)));
            }
        }

        [TestMethod]
        public void When_resolving_type_with_container_controlled_lifetime_then_returns_same_instance_every_time()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IFoo, Foo>(new ContainerControlledLifetimeManager());
                var resolver = new UnityDependencyResolver(container);
                IFoo resolve1 = (IFoo)resolver.GetService(typeof(IFoo));
                IFoo resolve2 = (IFoo)resolver.GetService(typeof(IFoo));

                Assert.IsNotNull(resolve1);
                Assert.AreSame(resolve1, resolve2);
            }
        }

        public interface IFoo
        {
            string TestProperty { get; set; }
        }

        public class Foo : IFoo
        {
            public string TestProperty { get; set; }
        }

        public class TestController : IController
        {
            public TestController(IFoo foo)
            {
                this.Foo = foo;
            }

            public IFoo Foo { get; set; }

            void IController.Execute(System.Web.Routing.RequestContext requestContext)
            {
                throw new System.NotImplementedException();
            }
        }

        private static void AssertThrows<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception {0}, but instead exception {1} was thrown",
                    typeof(TException).Name,
                    ex.GetType().Name);
            }
            Assert.Fail("Expected exception {0}, no exception thrown", typeof(TException).Name);
        }
    }
}
