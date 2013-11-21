// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Web.Http.Controllers;
using Microsoft.Practices.Unity.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.WebIntegation.Tests
{
    [TestClass]
    public class WebApiUnityDependencyResolverFixture
    {
        [TestMethod]
        public void when_resolving_then_returns_registered_instance()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
                using (var resolver = new UnityDependencyResolver(container))
                {
                    var actual = (IFoo)resolver.GetService(typeof(IFoo));
                    Assert.AreEqual("value", actual.TestProperty);
                }
            }
        }

        [TestMethod]
        public void when_resolving_multiple_then_returns_all_registered_instances()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>("instance1", new Foo { TestProperty = "value1" });
                container.RegisterInstance<IFoo>("instance2", new Foo { TestProperty = "value2" });
                using (var resolver = new UnityDependencyResolver(container))
                {
                    var actual = resolver.GetServices(typeof(IFoo)).Cast<IFoo>().ToList();
                    Assert.IsTrue(actual.Any(x => x.TestProperty == "value1"));
                    Assert.IsTrue(actual.Any(x => x.TestProperty == "value2"));
                }
            }
        }

        [TestMethod]
        public void when_resolving_unregistered_type_then_returns_null()
        {
            using (var container = new UnityContainer())
            {
                using (var resolver = new UnityDependencyResolver(container))
                {
                    Assert.IsNull(resolver.GetService(typeof(IFoo)));
                }
            }
        }

        [TestMethod]
        public void when_resolving_concrete_controller_inside_scope_then_returns_injected_instance()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
                using (var resolver = new UnityDependencyResolver(container))
                {
                    using (var scope = resolver.BeginScope())
                    {
                        var actual = (TestController)scope.GetService(typeof(TestController));
                        Assert.AreEqual("value", actual.Foo.TestProperty);
                    }
                }
            }
        }

        [TestMethod]
        public void when_resolving_controller_inside_scope_with_unregistered_dependencies_then_throws()
        {
            using (var container = new UnityContainer())
            {
                using (var resolver = new UnityDependencyResolver(container))
                {
                    using (var scope = resolver.BeginScope())
                    {
                        AssertThrows<ResolutionFailedException>(() => scope.GetService(typeof(TestController)));
                    }
                }
            }
        }

        [TestMethod]
        public void when_resolving_unregistered_type_inside_scope_then_throws()
        {
            using (var container = new UnityContainer())
            {
                using (var resolver = new UnityDependencyResolver(container))
                {
                    using (var scope = resolver.BeginScope())
                    {
                        AssertThrows<ResolutionFailedException>(() => scope.GetService(typeof(IFoo)));
                    }
                }
            }
        }

        [TestMethod]
        public void when_disposing_resolver_then_disposes_container()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
                var resolver = new UnityDependencyResolver(container);
                resolver.Dispose();

                // ObjectDisposedException?
                AssertThrows<ResolutionFailedException>(() => container.Resolve(typeof(IFoo)));
            }
        }

        [TestMethod]
        public void when_disposing_scope_then_does_not_dispose_container()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
                using (var resolver = new UnityDependencyResolver(container))
                {
                    resolver.BeginScope().Dispose();

                    var actual = (IFoo)resolver.GetService(typeof(IFoo));

                    Assert.AreEqual("value", actual.TestProperty);
                }
            }
        }

        [TestMethod]
        public void when_resolving_type_with_container_controlled_lifetime_then_returns_same_instance_every_time()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IFoo, Foo>(new ContainerControlledLifetimeManager());
                using (var resolver = new UnityDependencyResolver(container))
                {
                    IFoo parentResolve = (IFoo)resolver.GetService(typeof(IFoo));
                    IFoo scope1Resolve;
                    IFoo scope2Resolve;

                    using (var scope = resolver.BeginScope())
                    {
                        scope1Resolve = (IFoo)scope.GetService(typeof(IFoo));
                    }

                    using (var scope = resolver.BeginScope())
                    {
                        scope2Resolve = (IFoo)scope.GetService(typeof(IFoo));
                    }

                    Assert.IsNotNull(parentResolve);
                    Assert.AreSame(parentResolve, scope1Resolve);
                    Assert.AreSame(parentResolve, scope2Resolve);
                }
            }
        }

        [TestMethod]
        public void when_resolving_type_with_hierarchical_lifetime_then_returns_same_instance_every_time()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IFoo, Foo>(new HierarchicalLifetimeManager());
                using (var resolver = new UnityDependencyResolver(container))
                {
                    IFoo parentResolve = (IFoo)resolver.GetService(typeof(IFoo));
                    IFoo scope1Resolve;
                    IFoo scope2Resolve;

                    using (var scope = resolver.BeginScope())
                    {
                        scope1Resolve = (IFoo)scope.GetService(typeof(IFoo));
                    }

                    using (var scope = resolver.BeginScope())
                    {
                        scope2Resolve = (IFoo)scope.GetService(typeof(IFoo));
                    }

                    Assert.IsNotNull(parentResolve);
                    Assert.AreSame(parentResolve, scope1Resolve);
                    Assert.AreSame(parentResolve, scope2Resolve);
                }
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

        public class TestController : IHttpController
        {
            public TestController(IFoo foo)
            {
                this.Foo = foo;
            }

            public IFoo Foo { get; set; }

            System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> IHttpController.ExecuteAsync(HttpControllerContext controllerContext, System.Threading.CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
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
