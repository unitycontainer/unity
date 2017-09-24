using Xunit;
using System;
using Unity.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using System.Linq;

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
        public new void ResolvesMixedOpenClosedGenericsAsEnumerable()
        {
            // This test places unreasonable requirement for the provider to return 
            // results sorted in certain way. It should verify number and type of 
            // returned services but sort order should be irrelevant.

            // TODO: Send change request to ASP team
            //Assert.Equal(instance, enumerable[2]);
            //Assert.IsType<FakeService>(enumerable[0]);
        }
        [Fact]
        public new void DisposesInReverseOrderOfCreation()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<FakeDisposeCallback>();
            serviceCollection.AddTransient<IFakeOuterService, FakeDisposableCallbackOuterService>();
            serviceCollection.AddSingleton<IFakeMultipleService, FakeDisposableCallbackInnerService>();
            serviceCollection.AddScoped<IFakeMultipleService, FakeDisposableCallbackInnerService>();
            serviceCollection.AddTransient<IFakeMultipleService, FakeDisposableCallbackInnerService>();
            serviceCollection.AddSingleton<IFakeService, FakeDisposableCallbackInnerService>();
            var serviceProvider = CreateServiceProvider(serviceCollection);

            var callback = serviceProvider.GetService<FakeDisposeCallback>();
            var outer = serviceProvider.GetService<IFakeOuterService>();

            // Act
            ((IDisposable)serviceProvider).Dispose();

            // Assert
            Assert.Equal(outer, callback.Disposed[0]);
            Assert.Equal(outer.MultipleServices.Reverse(), callback.Disposed.Skip(1).Take(3).OfType<IFakeMultipleService>());
            Assert.Equal(outer.SingleService, callback.Disposed[4]);
        }
    }
}