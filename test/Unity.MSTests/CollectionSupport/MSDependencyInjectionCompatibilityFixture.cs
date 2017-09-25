using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Unity.MSTests.TestObjects;
using Unity;
using Unity.Tests.Generics;
using Unity.Tests.TestObjects;

namespace Unity.Tests.CollectionSupport
{
    [TestClass]
    public class MSDependencyInjectionCompatibilityFixture
    {
        [TestMethod]
        public void Enumerable_ServicesRegisteredWithImplementationTypeCanBeResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();
                
                // Act
                var service = provider.Resolve<IService>();

                // Assert
                Assert.IsNotNull(service);
                Assert.IsInstanceOfType(service, typeof(EmailService));
            }
        }

        [TestMethod]
        public void Enumerable_ServicesRegisteredWithImplementationType_ReturnDifferentInstancesPerResolution_ForTransientServices()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();

                // Act
                var service1 = provider.Resolve<IService>();
                var service2 = provider.Resolve<IService>();

                // Assert
                Assert.IsInstanceOfType(service1, typeof(EmailService));
                Assert.IsInstanceOfType(service1, typeof(EmailService));

                Assert.AreNotSame(service1, service2);
            }

        }

        [TestMethod]
        public void Enumerable_ServicesRegisteredWithImplementationType_ReturnSameInstancesPerResolution_ForSingletons()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new ContainerControlledLifetimeManager());

                // Act
                var service1 = provider.Resolve<IService>();
                var service2 = provider.Resolve<IService>();

                // Assert
                Assert.IsInstanceOfType(service1, typeof(EmailService));
                Assert.IsInstanceOfType(service1, typeof(EmailService));

                Assert.AreSame(service1, service2);
            }
        }

        [TestMethod]
        public void Enumerable_ServiceInstanceCanBeResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                var instance = new EmailService();
                provider.RegisterInstance<IService>(instance);

                // Act
                var service = provider.Resolve<IService>();

                // Assert
                Assert.AreSame(instance, service);
            }
        }

        [TestMethod]
        public void Enumerable_TransientServiceCanBeResolvedFromProvider()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();

                // Act
                var service1 = provider.Resolve<IService>();
                var service2 = provider.Resolve<IService>();

                // Assert
                Assert.IsNotNull(service1);
                Assert.AreNotSame(service1, service2);
            }
        }

        [TestMethod]
        public void Enumerable_TransientServiceCanBeResolvedFromScope()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();

                // Act
                var service1 = provider.Resolve<IService>();
                using (var scope = provider.CreateChildContainer())
                {
                    var scopedService1 = scope.Resolve<IService>();
                    var scopedService2 = scope.Resolve<IService>();

                    // Assert
                    Assert.AreNotSame(service1, scopedService1);
                    Assert.AreNotSame(service1, scopedService2);
                    Assert.AreNotSame(scopedService1, scopedService2);
                }
            }
        }

        [TestMethod]
        public void Enumerable_SingleServiceCanBeIEnumerableResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();
    
                // Act
                var services = provider.Resolve<IEnumerable<IService>>();
                var service = services.Single();

                // Assert
                Assert.IsNotNull(service);
                Assert.IsInstanceOfType(service, typeof(EmailService));
            }
        }

        [TestMethod]
        public void Enumerable_MixedServicesCanBeIEnumerableResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(typeof(EmailService).FullName);
                provider.RegisterType<IService, EmailService>();
                provider.RegisterType<IService, OtherEmailService>(typeof(OtherEmailService).FullName);

                // Act
                var registered = provider.Registrations;
                var services = provider.Resolve<IEnumerable<IService>>();

                // Assert
                var enumerator = services.GetEnumerator();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(EmailService));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(EmailService));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(OtherEmailService));
            }
        }

        [TestMethod]
        public void Enumerable_MultipleServiceCanBeIEnumerableResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(typeof(EmailService).FullName);
                provider.RegisterType<IService, OtherEmailService>(typeof(OtherEmailService).FullName);

                // Act
                var services = provider.Resolve<IEnumerable<IService>>();

                // Assert
                var enumerator = services.GetEnumerator();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(EmailService));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(OtherEmailService));
            }
        }

        [TestMethod]
        public void Enumerable_RegistrationOrderIsPreservedWhenServicesAreIEnumerableResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, OtherEmailService>(typeof(OtherEmailService).FullName);
                provider.RegisterType<IService, EmailService>(typeof(EmailService).FullName);

                // Act
                var services = provider.Resolve<IEnumerable<IService>>();

                // Assert
                var enumerator = services.GetEnumerator();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(OtherEmailService));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsInstanceOfType(enumerator.Current, typeof(EmailService));
            }
        }

        [TestMethod]
        public void Enumerable_OuterServiceCanHaveOtherServicesInjected()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                var fakeService = new EmailService();
                provider.RegisterInstance<IService>(fakeService);
                provider.RegisterType<IBase, Base>();

                // Act
                var service = provider.Resolve<IBase>();

                // Assert
                Assert.AreSame(fakeService, service.Service);
            }
        }

        [TestMethod]
        public void Enumerable_FactoryServicesCanBeCreatedByGetService()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                var fakeService = new EmailService();
                provider.RegisterInstance<IService>(fakeService);
                provider.RegisterType<IBase>(new InjectionFactory((u) => new Base { Service = u.Resolve<IService>() }));

                // Act
                var service = provider.Resolve<IBase>();
                
                // Assert
                Assert.IsNotNull(service);
                Assert.AreSame(fakeService, service.Service);
            }

        }

        [TestMethod]
        public void Enumerable_FactoryServicesAreCreatedAsPartOfCreatingObjectGraph()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                var fakeService = new EmailService();
                provider.RegisterInstance<IService>(fakeService);
                provider.RegisterType<IBase>(new InjectionFactory((u) => new Base { Service = u.Resolve<IService>() }));

                var lazy = provider.Resolve<Lazy<IBase>>();
                var service = lazy.Value;

                // Assert
                Assert.IsNotNull(service);
                Assert.AreSame(fakeService, service.Service);
            }
        }

        [TestMethod]
        public void Enumerable_LastServiceReplacesPreviousServices()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();
                provider.RegisterType<IService, OtherEmailService>();

                // Act
                var service = provider.Resolve<IService>();

                // Assert
                Assert.IsNotNull(service);
                Assert.IsInstanceOfType(service, typeof(OtherEmailService));
            }
        }

        [TestMethod]
        public void Enumerable_SingletonServiceCanBeResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new ContainerControlledLifetimeManager());

                // Act
                var service1 = provider.Resolve<IService>();
                var service2 = provider.Resolve<IService>();

                // Assert
                Assert.IsNotNull(service1);
                Assert.IsInstanceOfType(service1, typeof(EmailService));
                Assert.AreSame(service1, service2);
            }
        }

        [TestMethod]
        public void Enumerable_ScopedServiceCanBeResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new HierarchicalLifetimeManager());

                // Act
                using (var scope = provider.CreateChildContainer())
                {
                    var providerScopedService = provider.Resolve<IService>();
                    var scopedService1 = scope.Resolve<IService>();
                    var scopedService2 = scope.Resolve<IService>();

                    // Assert
                    Assert.AreNotSame(providerScopedService, scopedService1);
                    Assert.AreSame(scopedService1, scopedService2);
                }
            }
        }

        [TestMethod]
        public void Enumerable_NestedScopedServiceCanBeResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new HierarchicalLifetimeManager());

                // Act
                using (var outerScope = provider.CreateChildContainer())
                using (var innerScope = outerScope.CreateChildContainer())
                {
                    var outerScopedService = outerScope.Resolve<IService>();
                    var innerScopedService = innerScope.Resolve<IService>();

                    // Assert
                    Assert.IsNotNull(outerScopedService);
                    Assert.IsNotNull(innerScopedService);
                    Assert.AreNotSame(outerScopedService, innerScopedService);
                }
            }
        }

        [TestMethod]
        public void Enumerable_DisposingScopeDisposesService()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>("Singleton", new ContainerControlledLifetimeManager());
                provider.RegisterType<IService, EmailService>("Scoped", new HierarchicalLifetimeManager());
                provider.RegisterType<IService, OtherEmailService>("Transient", new HierarchicalLifetimeManager());

                EmailService disposableService;
                OtherEmailService transient1;
                OtherEmailService transient2;
                EmailService singleton;

                // Act and Assert
                OtherEmailService transient3 = (OtherEmailService)provider.Resolve<IService>("Transient");
                using (var scope = provider.CreateChildContainer())
                {
                    disposableService = (EmailService)scope.Resolve<IService>("Scoped");
                    transient1 = (OtherEmailService)scope.Resolve<IService> ("Transient");
                    transient2 = (OtherEmailService)scope.Resolve<IService>("Transient");
                    singleton = (EmailService)scope.Resolve<IService>("Singleton");

                    Assert.IsFalse(disposableService.Disposed);
                    Assert.IsFalse(transient1.Disposed);
                    Assert.IsFalse(transient2.Disposed);
                    Assert.IsFalse(singleton.Disposed);
                }

                Assert.IsTrue(disposableService.Disposed);
                Assert.IsTrue(transient1.Disposed);
                Assert.IsTrue(transient2.Disposed);
                Assert.IsFalse(singleton.Disposed);

                var disposableProvider = provider as IDisposable;
                if (disposableProvider != null)
                {
                    disposableProvider.Dispose();
                    Assert.IsTrue(singleton.Disposed);
                    Assert.IsTrue(transient3.Disposed);
                }
            }
        }

        [TestMethod]
        public void Enumerable_SingletonServicesComeFromRootProvider()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new ContainerControlledLifetimeManager());

                EmailService disposableService1;
                EmailService disposableService2;

                // Act and Assert
                using (var scope = provider.CreateChildContainer())
                {
                    var service = scope.Resolve<IService>();
                    disposableService1 = (EmailService)service;
                    Assert.IsFalse(disposableService1.Disposed);
                }

                Assert.IsFalse(disposableService1.Disposed);

                using (var scope = provider.CreateChildContainer())
                {
                    var service = scope.Resolve<IService>();
                    disposableService2 = (EmailService)service;
                    Assert.IsFalse(disposableService2.Disposed);
                }

                Assert.IsFalse(disposableService2.Disposed);
                Assert.AreSame(disposableService1, disposableService2);
            }
        }

        [TestMethod]
        public void Enumerable_NestedScopedServiceCanBeResolvedWithNoFallbackProvider()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new HierarchicalLifetimeManager());
                // Act
                using (var outerScope = provider.CreateChildContainer())
                using (var innerScope = outerScope.CreateChildContainer())
                {
                    var outerScopedService = outerScope.Resolve<IService>();
                    var innerScopedService = innerScope.Resolve<IService>();

                    // Assert
                    Assert.AreNotSame(outerScopedService, innerScopedService);
                }
            }
        }

        [TestMethod]
        public void Enumerable_OpenGenericServicesCanBeResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>(new ContainerControlledLifetimeManager());
                provider.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            
                // Act
                var genericService = provider.Resolve<IFoo<IService>>();
                var singletonService = provider.Resolve<IService>();

                // Assert
                Assert.AreSame(singletonService, genericService.Value);
            }
        }

        [TestMethod]
        public void Enumerable_ClosedServicesPreferredOverOpenGenericServices()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();
                provider.RegisterType(typeof(IFoo<>), typeof(Foo<>));
                provider.RegisterType(typeof(IFoo<IService>), typeof(Foo<IService>));

                // Act
                var service = provider.Resolve<IFoo<IService>>();

                // Assert
                Assert.IsInstanceOfType(service.Value, typeof(EmailService));
            }
        }

        [TestMethod]
        public void Enumerable_NonexistentServiceCanBeIEnumerableResolved()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                var services = provider.Resolve<IEnumerable<IService>>();

                // Assert
                Assert.AreEqual(0, services.Count());
            }
        }

        [TestMethod]
        public void Enumerable_DisposesInReverseOrderOfCreation()
        {
            FakeDisposeCallback fake_callback = new FakeDisposeCallback();
            IFakeDisposableCallbackService[] list;
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterInstance(fake_callback);

                provider.RegisterType<IFakeDisposableCallbackService, FakeDisposableCallbackService>("1", new ContainerControlledLifetimeManager());
                provider.RegisterType<IFakeDisposableCallbackService, FakeDisposableCallbackService>("2", new HierarchicalLifetimeManager());
                provider.RegisterType<IFakeDisposableCallbackService, FakeDisposableCallbackService>("3", new HierarchicalTransientLifetimeManager());
                provider.RegisterType<IFakeDisposableCallbackService, FakeDisposableCallbackService>("4", new ContainerControlledLifetimeManager());

                list = provider.ResolveAll<IFakeDisposableCallbackService>().ToArray();
                for (var i = 0; i < list.Length; i++)
                    Assert.AreEqual(i, list[i].ID);

            } // Act

            // Assert
            for (var i = 0; i < list.Length; i++)
                Assert.AreEqual(list[list.Length - 1 - i].ID, ((IFakeDisposableCallbackService)fake_callback.Disposed[i]).ID);
        }

        [TestMethod]
        public void Enumerable_ResolvesMixedOpenClosedGenericsAsEnumerable()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                var instance = new Foo<IService>(new OtherEmailService());

                provider.RegisterType<IService>();
                provider.RegisterType<IService, EmailService>();
                provider.RegisterType(typeof(IFoo<IService>), typeof(Foo<IService>), "asdf", new ContainerControlledLifetimeManager());
                provider.RegisterType(typeof(IFoo<>), typeof(Foo<>), "fa", new ContainerControlledLifetimeManager());
                provider.RegisterInstance<IFoo<IService>>(instance);

                // Act
                var enumerable = provider.Resolve<IEnumerable<IFoo<IService>>>().ToArray();

                // Assert
                Assert.AreEqual(3, enumerable.Length);
                Assert.IsNotNull(enumerable[0]);
                Assert.IsNotNull(enumerable[1]);
                Assert.IsNotNull(enumerable[2]);

            }
        }

        [TestMethod]
        public void Enumerable_ResolvesDifferentInstancesWhenResolvingEnumerable()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>("1", new HierarchicalLifetimeManager());
                provider.RegisterType<IService, EmailService>("2", new HierarchicalLifetimeManager());
                provider.RegisterType<IService, EmailService>("3", new HierarchicalLifetimeManager());

                using (var scope = provider.CreateChildContainer())
                {
                    var enumerable = scope.Resolve<IEnumerable<IService>>().ToArray();
                    var service = scope.Resolve<IService>("3");

                    // Assert
                    Assert.AreEqual(3, enumerable.Length);
                    Assert.IsNotNull(enumerable[0]);
                    Assert.IsNotNull(enumerable[1]);
                    Assert.IsNotNull(enumerable[2]);

                    Assert.AreNotSame(enumerable[0], enumerable[1]);
                    Assert.AreNotSame(enumerable[1], enumerable[2]);
                    Assert.AreSame(service, enumerable[2]);
                }
            }
        }

        [TestMethod]
        public void Enumerable_ResolvesDifferentInstancesForOpenGenericsWhenResolvingEnumerable()
        {
            using (IUnityContainer provider = new UnityContainer())
            {
                // Arrange
                provider.RegisterType<IService, EmailService>();
                provider.RegisterType(typeof(IFoo<>), typeof(Foo<>), "1", new HierarchicalLifetimeManager());
                provider.RegisterType(typeof(IFoo<>), typeof(Foo<>), "2", new HierarchicalLifetimeManager());
                provider.RegisterType(typeof(IFoo<>), typeof(Foo<>), "3", new HierarchicalLifetimeManager());

                using (var scope = provider.CreateChildContainer())
                {
                    var enumerable = scope.Resolve<IEnumerable<IFoo<IService>>>().ToArray();
                    var service = scope.Resolve<IFoo<IService>>("3");

                    // Assert
                    Assert.AreEqual(3, enumerable.Length);
                    Assert.IsNotNull(enumerable[0]);
                    Assert.IsNotNull(enumerable[1]);
                    Assert.IsNotNull(enumerable[2]);

                    Assert.AreNotSame(enumerable[0], enumerable[1]);
                    Assert.AreNotSame(enumerable[1], enumerable[2]);
                    Assert.AreSame(service, enumerable[2]);
                }
            }
        }
    }
}
