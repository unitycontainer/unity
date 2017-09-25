using Xunit;
using System;
using Unity.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Tests.Unity.Microsoft.DependencyInjection
{
    public class DependencyInjectionComplianceTests : DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            return serviceCollection.CreateServiceProvider();
        }

        [Theory, MemberData("ServiceContainerPicksConstructorWithLongestMatchesData", new object[] { })]
        public new void ServiceContainerPicksConstructorWithLongestMatches(IServiceCollection serviceCollection, TypeWithSupersetConstructors expected)
        {
            // This strategy is not supported by Unity Container
            // TODO: Implement Container Picks Constructor With Longest Matches strategy 
        }


        [Fact]
        public new void DisposesInReverseOrderOfCreation()
        {
            //// Arrange
            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddSingleton<FakeDisposeCallback>();
            //serviceCollection.AddTransient<IFakeOuterService, FakeDisposableCallbackOuterService>();
            //serviceCollection.AddSingleton<IFakeMultipleService, FakeDisposableCallbackInnerService>();
            //serviceCollection.AddScoped<IFakeMultipleService, FakeDisposableCallbackInnerService>();
            //serviceCollection.AddTransient<IFakeMultipleService, FakeDisposableCallbackInnerService>();
            //serviceCollection.AddSingleton<IFakeService, FakeDisposableCallbackInnerService>();
            //var serviceProvider = CreateServiceProvider(serviceCollection);

            //var callback = serviceProvider.GetService<FakeDisposeCallback>();
            //var outer = serviceProvider.GetService<IFakeOuterService>();

            //// Act
            //((IDisposable)serviceProvider).Dispose();

            //// Assert
            //Assert.Equal(outer, callback.Disposed[0]);
            //Assert.Equal(outer.MultipleServices.Reverse(), callback.Disposed.Skip(1).Take(3).OfType<IFakeMultipleService>());
            //Assert.Equal(outer.SingleService, callback.Disposed[4]);
        }

        [Fact]
        public new void LastServiceReplacesPreviousServices()
        {
            //// Arrange
            //var collection = new ServiceCollection();
            //collection.AddTransient<IFakeMultipleService, FakeOneMultipleService>();
            //collection.AddTransient<IFakeMultipleService, FakeTwoMultipleService>();
            //var provider = CreateServiceProvider(collection);

            //// Act
            //var service = provider.GetService<IFakeMultipleService>();

            //// Assert
            //Assert.IsType<FakeTwoMultipleService>(service);
        }

        [Fact]
        public new void ResolvesMixedOpenClosedGenericsAsEnumerable()
        {
            //// Arrange
            //var serviceCollection = new ServiceCollection();
            //var instance = new FakeOpenGenericService<PocoClass>(null);

            //serviceCollection.AddTransient<PocoClass, PocoClass>();
            //serviceCollection.AddSingleton(typeof(IFakeOpenGenericService<PocoClass>), typeof(FakeService));
            //serviceCollection.AddSingleton(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>));
            //serviceCollection.AddSingleton<IFakeOpenGenericService<PocoClass>>(instance);

            //var serviceProvider = CreateServiceProvider(serviceCollection);

            //var enumerable = serviceProvider.GetService<IEnumerable<IFakeOpenGenericService<PocoClass>>>().ToArray();

            //// Assert
            //Assert.Equal(3, enumerable.Length);
            //Assert.NotNull(enumerable[0]);
            //Assert.NotNull(enumerable[1]);
            //Assert.NotNull(enumerable[2]);

            //Assert.Equal(instance, enumerable[2]);
            //Assert.IsType<FakeService>(enumerable[0]);
        }


        [Fact]
        public new void TransientServiceCanBeResolvedFromScope()
        {
            //// Arrange
            //var collection = new ServiceCollection();
            //collection.AddTransient(typeof(IFakeService), typeof(FakeService));
            //var provider = CreateServiceProvider(collection);

            //// Act
            //var service1 = provider.GetService<IFakeService>();

            //using (var scope = provider.CreateScope())
            //{
            //    var scopedService1 = scope.ServiceProvider.GetService<IFakeService>();
            //    var scopedService2 = scope.ServiceProvider.GetService<IFakeService>();

            //    // Assert
            //    Assert.NotSame(service1, scopedService1);
            //    Assert.NotSame(service1, scopedService2);
            //    Assert.NotSame(scopedService1, scopedService2);
            //}
        }


        [Theory]
        [InlineData(typeof(IFakeService), typeof(FakeService), typeof(IFakeService), ServiceLifetime.Scoped)]
        [InlineData(typeof(IFakeService), typeof(FakeService), typeof(IFakeService), ServiceLifetime.Singleton)]
        [InlineData(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>), typeof(IFakeOpenGenericService<IServiceProvider>), ServiceLifetime.Scoped)]
        [InlineData(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>), typeof(IFakeOpenGenericService<IServiceProvider>), ServiceLifetime.Singleton)]
        public new void ResolvesDifferentInstancesForServiceWhenResolvingEnumerable(Type serviceType, Type implementation, Type resolve, ServiceLifetime lifetime)
        {
            // Arrange
            //var serviceCollection = new ServiceCollection
            //{
            //    ServiceDescriptor.Describe(serviceType, implementation, lifetime),
            //    ServiceDescriptor.Describe(serviceType, implementation, lifetime),
            //    ServiceDescriptor.Describe(serviceType, implementation, lifetime)
            //};

            //var serviceProvider = CreateServiceProvider(serviceCollection);
            //using (var scope = serviceProvider.CreateScope())
            //{
            //    var enumerable = (scope.ServiceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(resolve)) as IEnumerable)
            //        .OfType<object>().ToArray();
            //    var service = scope.ServiceProvider.GetService(resolve);

            //    // Assert
            //    Assert.Equal(3, enumerable.Length);
            //    Assert.NotNull(enumerable[0]);
            //    Assert.NotNull(enumerable[1]);
            //    Assert.NotNull(enumerable[2]);

            //    Assert.NotEqual(enumerable[0], enumerable[1]);
            //    Assert.NotEqual(enumerable[1], enumerable[2]);
            //    Assert.Equal(service, enumerable[2]);
            //}
        }
    }
}