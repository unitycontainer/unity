//using System;
//using Xunit;
//using Microsoft.Extensions.DependencyInjection.Specification;

//namespace Unity.Mvc.Tests
//{

//    public class ResolverFixture
//    {
//        // TODO: [Fact]
//        //public void When_resolving_then_returns_registered_instance()
//        //{
//        //    using (var container = new UnityContainer())
//        //    {
//        //        container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
//        //        var serviceCollection = new ServiceCollection();
                
//        //        Configuration.Register(serviceCollection, container);
//        //        var resolver = container.Resolve<IServiceProvider>();

//        //        var actual = (IFoo)resolver.GetService(typeof(IFoo));
//        //        Assert.Equal("value", actual.TestProperty);
//        //    }
//        //}

//        // TODO:
//        //[Fact]
//        //public void When_resolving_multiple_then_returns_all_registered_instances()
//        //{
//        //    using (var container = new UnityContainer())
//        //    {
//        //        container.RegisterInstance<IFoo>("instance1", new Foo { TestProperty = "value1" });
//        //        container.RegisterInstance<IFoo>("instance2", new Foo { TestProperty = "value2" });
//        //        var serviceCollection = new ServiceCollection();
//        //        Configuration.Register(serviceCollection, container);
//        //        var resolver = container.Resolve<IServiceProvider>();

//        //        var actual = ServiceProviderExtensions.GetServices(resolver, typeof(IFoo)).Cast<IFoo>().ToList();
//        //        Assert.True(actual.Any(x => x.TestProperty == "value1"));
//        //        Assert.True(actual.Any(x => x.TestProperty == "value2"));
//        //    }
//        //}

//        //[Fact]
//        public void When_resolving_unregistered_type_then_returns_null()
//        {
//            using (var container = new UnityContainer())
//            {
//                container.RegisterInstance<IBar>("instance1", new Bar { TestProperty = "value1" });
//                var serviceCollection = new ServiceCollection();
//                Configuration.Register(serviceCollection, container);
//                var resolver = container.Resolve<IServiceProvider>();

//                Assert.Null(resolver.GetService(typeof(IFoo)));
//            }
//        }

//        // TODO: [Fact]
//        public void When_resolving_concrete_controller_then_returns_injected_instance()
//        {
//            using (var container = new UnityContainer())
//            {
//                var serviceCollection = new ServiceCollection();
//                Configuration.Register(serviceCollection, container);

//                container.RegisterInstance<IFoo>(new Foo { TestProperty = "value" });
//                var resolver = container.Resolve<IServiceProvider>();

//                var actual = (TestController)resolver.GetService(typeof(TestController));
//                Assert.Equal("value", actual.Foo.TestProperty);
//            }
//        }

//        // TODO: [Fact]
//        public void When_resolving_controller_with_unregistered_dependencies_then_null()
//        {
//            using (var container = new UnityContainer())
//            {
//                var serviceCollection = new ServiceCollection();
//                Configuration.Register(serviceCollection, container);
//                var resolver = container.Resolve<IServiceProvider>();

//                Assert.Null(resolver.GetService(typeof(TestController)));
//            }
//        }

//        public class TestController 
//        {
//            public TestController(IFoo foo)
//            {
//                this.Foo = foo;
//            }

//            public IFoo Foo { get; set; }
//        }

//        // TODO: [Fact]
//        public void When_resolving_type_with_container_controlled_lifetime_then_returns_same_instance_every_time()
//        {
//            using (var container = new UnityContainer())
//            {
//                var serviceCollection = new ServiceCollection();
//                Configuration.Register(serviceCollection, container);
//                container.RegisterType<IFoo, Foo>(new ContainerControlledLifetimeManager());
//                var resolver = container.Resolve<IServiceProvider>();
//                IFoo resolve1 = (IFoo)resolver.GetService(typeof(IFoo));
//                IFoo resolve2 = (IFoo)resolver.GetService(typeof(IFoo));

//                Assert.NotNull(resolve1);
//                Assert.Same(resolve1, resolve2);
//            }
//        }

//        public interface IFoo
//        {
//            string TestProperty { get; set; }
//        }

//        public class Foo : IFoo
//        {
//            public string TestProperty { get; set; }
//        }

//        public interface IBar
//        {
//            string TestProperty { get; set; }
//        }

//        public class Bar : IBar
//        {
//            public string TestProperty { get; set; }
//        }

//        private static void AssertThrows<TException>(Action action)
//            where TException : Exception
//        {
//            try
//            {
//                action();
//            }
//            catch (TException)
//            {
//                return;
//            }
//            catch (Exception ex)
//            {
//                Assert.True(false,
//                    string.Format("Expected exception {0}, but instead exception {1} was thrown",
//                        typeof (TException).Name, ex.GetType().Name));
//            }
//            Assert.True(false, string.Format("Expected exception {0}, no exception thrown", typeof(TException).Name));
//        }
//    }
//}
