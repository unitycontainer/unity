// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for ContainerRegistrationsFixture
    /// </summary>
     
    public class ContainerRegistrationsFixture
    {
        private IUnityContainer container;

        public ContainerRegistrationsFixture()
        {
            container = new UnityContainer();
        }

        [Fact]
        public void ContainerListsItselfAsRegistered()
        {
            Assert.True(container.IsRegistered(typeof(IUnityContainer)));
        }

        [Fact]
        public void ContainerDoesNotListItselfUnderNonDefaultName()
        {
            Assert.False(container.IsRegistered(typeof(IUnityContainer), "other"));
        }

        [Fact]
        public void ContainerListsItselfAsRegisteredUsingGenericOverload()
        {
            Assert.True(container.IsRegistered<IUnityContainer>());
        }

        [Fact]
        public void ContainerDoesNotListItselfUnderNonDefaultNameUsingGenericOverload()
        {
            Assert.False(container.IsRegistered<IUnityContainer>("other"));
        }

        [Fact]
        public void IsRegisteredWorksForRegisteredType()
        {
            container.RegisterType<ILogger, MockLogger>();

            Assert.True(container.IsRegistered<ILogger>());
        }

        [Fact]
        public void ContainerIncludesItselfUnderRegistrations()
        {
            Assert.NotNull(container.Registrations.Where(r => r.RegisteredType == typeof(IUnityContainer)).FirstOrDefault());
        }

        [Fact]
        public void NewRegistrationsShowUpInRegistrationsSequence()
        {
            container.RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, MockLogger>("second");

            var registrations = (from r in container.Registrations
                                 where r.RegisteredType == typeof(ILogger)
                                 select r).ToList();

            Assert.Equal(2, registrations.Count);

            Assert.True(registrations.Any(r => r.Name == null));
            Assert.True(registrations.Any(r => r.Name == "second"));
        }

        [Fact]
        public void TypeMappingShowsUpInRegistrationsCorrectly()
        {
            container.RegisterType<ILogger, MockLogger>();

            var registration =
                (from r in container.Registrations where r.RegisteredType == typeof(ILogger) select r).First();
            Assert.Same(typeof(MockLogger), registration.MappedToType);
        }

        [Fact]
        public void NonMappingRegistrationShowsUpInRegistrationsSequence()
        {
            container.RegisterType<MockLogger>();
            var registration = (from r in container.Registrations
                                where r.RegisteredType == typeof(MockLogger)
                                select r).First();

            Assert.Same(registration.RegisteredType, registration.MappedToType);
            Assert.Null(registration.Name);
        }

        [Fact]
        public void RegistrationOfOpenGenericTypeShowsUpInRegistrationsSequence()
        {
            container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>), "test");
            var registration = container.Registrations.First(r => r.RegisteredType == typeof(IDictionary<,>));

            Assert.Same(typeof(Dictionary<,>), registration.MappedToType);
            Assert.Equal("test", registration.Name);
        }

        [Fact]
        public void RegistrationsInParentContainerAppearInChild()
        {
            container.RegisterType<ILogger, MockLogger>();
            var child = container.CreateChildContainer();

            var registration =
                (from r in child.Registrations where r.RegisteredType == typeof(ILogger) select r).First();

            Assert.Same(typeof(MockLogger), registration.MappedToType);
        }

        [Fact]
        public void RegistrationsInChildContainerDoNotAppearInParent()
        {
            var child = container.CreateChildContainer()
                .RegisterType<ILogger, MockLogger>("named");

            var childRegistration = child.Registrations.Where(r => r.RegisteredType == typeof(ILogger)).First();
            var parentRegistration =
                container.Registrations.Where(r => r.RegisteredType == typeof(ILogger)).FirstOrDefault();

            Assert.Null(parentRegistration);
            Assert.NotNull(childRegistration);
        }

        [Fact]
        public void DuplicateRegistrationsInParentAndChildOnlyShowUpOnceInChild()
        {
            container.RegisterType<ILogger, MockLogger>("one");

            var child = container.CreateChildContainer()
                .RegisterType<ILogger, SpecialLogger>("one");

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(ILogger)
                                select r;

            Assert.Equal(1, registrations.Count());

            var childRegistration = registrations.First();
            Assert.Same(typeof(SpecialLogger), childRegistration.MappedToType);
            Assert.Equal("one", childRegistration.Name);
        }
    }
}
