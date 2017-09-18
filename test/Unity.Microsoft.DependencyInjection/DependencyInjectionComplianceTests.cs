using Xunit;
using System;
using Unity.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;

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
        public new void DisposingScopeDisposesService()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddSingleton<IFakeSingletonService, FakeService>();
            collection.AddScoped<IFakeScopedService, FakeService>();
            collection.AddTransient<IFakeService, FakeService>();

            var provider = CreateServiceProvider(collection);
            FakeService disposableService;
            FakeService transient1;
            FakeService transient2;
            FakeService singleton;

            // Act and Assert
            var transient3 = Assert.IsType<FakeService>(provider.GetService<IFakeService>());
            using (var scope = provider.CreateScope())
            {
                disposableService = (FakeService)scope.ServiceProvider.GetService<IFakeScopedService>();
                transient1 = (FakeService)scope.ServiceProvider.GetService<IFakeService>();
                transient2 = (FakeService)scope.ServiceProvider.GetService<IFakeService>();
                singleton = (FakeService)scope.ServiceProvider.GetService<IFakeSingletonService>();

                Assert.False(disposableService.Disposed);
                Assert.False(transient1.Disposed);
                Assert.False(transient2.Disposed);
                Assert.False(singleton.Disposed);
            }

            Assert.True(disposableService.Disposed);
            Assert.True(transient1.Disposed);
            Assert.True(transient2.Disposed);
            Assert.False(singleton.Disposed);

            var disposableProvider = provider as IDisposable;
            if (disposableProvider != null)
            {
                disposableProvider.Dispose();
                Assert.True(singleton.Disposed);
                Assert.True(transient3.Disposed);
            }
        }
    }
}