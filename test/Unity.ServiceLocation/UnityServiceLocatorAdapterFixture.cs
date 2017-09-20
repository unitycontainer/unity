// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ServiceLocation;
using ServiceLocation.Tests.Components;
using Xunit;
using Unity;
using System.Collections;

namespace ServiceLocation.Tests
{
    /// <summary>
    /// Summary description for UnityServiceLocatorAdapterFixture
    /// </summary>
     
    public class UnityServiceLocatorAdapterFixture : ServiceLocatorFixture
    {
        public UnityServiceLocatorAdapterFixture()
        {
            this.locator = this.CreateServiceLocator();
        }

        protected override IServiceLocator CreateServiceLocator()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, AdvancedLogger>()
                .RegisterType<ILogger, SimpleLogger>(typeof(SimpleLogger).FullName)
                .RegisterType<ILogger, AdvancedLogger>(typeof(AdvancedLogger).FullName);

            return new UnityServiceLocator(container);
        }

        [Fact]
        public new void GetInstance()
        {
            base.GetInstance();
        }

        [Fact]
        public new void AskingForInvalidComponentShouldRaiseActivationException()
        {
            Assert.Throws<ActivationException>(() => locator.GetInstance<IDictionary>());
        }

        [Fact]
        public new void GetNamedInstance()
        {
            base.GetNamedInstance();
        }

        [Fact]
        public new void GetNamedInstance2()
        {
            base.GetNamedInstance2();
        }

        [Fact]
        public new void GetUnknownInstance2()
        {
            Assert.Throws<ActivationException>(() => locator.GetInstance<ILogger>("test"));
        }

        [Fact]
        public new void GetAllInstances()
        {
            base.GetAllInstances();
        }

        [Fact]
        public new void GetAllInstance_ForUnknownType_ReturnEmptyEnumerable()
        {
            base.GetAllInstance_ForUnknownType_ReturnEmptyEnumerable();
        }

        [Fact]
        public new void GenericOverload_GetInstance()
        {
            base.GenericOverload_GetInstance();
        }

        [Fact]
        public new void GenericOverload_GetInstance_WithName()
        {
            base.GenericOverload_GetInstance_WithName();
        }

        [Fact]
        public new void Overload_GetInstance_NoName_And_NullName()
        {
            base.Overload_GetInstance_NoName_And_NullName();
        }

        [Fact]
        public new void GenericOverload_GetAllInstances()
        {
            base.GenericOverload_GetAllInstances();
        }

        [Fact]
        public void Get_WithZeroLenName_ReturnsDefaultInstance()
        {
            Assert.Same(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance<ILogger>(String.Empty).GetType());
        }
    }
}
