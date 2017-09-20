// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Configuration;
using Unity.TestSupport;
using Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Interception.Tests;

namespace Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingContainersThatConfigureInterception
    /// </summary>
    [TestClass]
    public class When_LoadingContainersThatConfigureInterception : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingContainersThatConfigureInterception()
            : base("InterceptionInjectionMembers")
        {
        }

        private IEnumerable<RegisterElement> GetRegistration(string containerName, Func<RegisterElement, bool> predicate)
        {
            var containerElement = section.Containers[containerName];
            return containerElement.Registrations.Where(predicate);
        }

        private static bool IsInterceptableType(RegisterElement element)
        {
            return element.TypeName == "Interceptable";
        }

        [TestMethod]
        public void Then_InterceptionElementIsPresentInRegistrationInjectionMembers()
        {
            var registration = this.GetRegistration("configuringInterceptorThroughConfigurationFile",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<InterceptorElement>()
                .Select(element => element.TypeName)
                .AssertContainsExactly("VirtualMethodInterceptor");
        }

        [TestMethod]
        public void Then_ExtraInterfacesCanBeAddedInConfig()
        {
            var registration = this.GetRegistration(
                "configuringAdditionalInterfaceThroughConfigurationFile",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<AddInterfaceElement>()
                .Select(element => element.TypeName)
                .AssertContainsExactly("IServiceProvider", "IComponent");
        }

        [TestMethod]
        public void Then_BehaviorsCanBeAddedInConfig()
        {
            var registration = this.GetRegistration(
                "configuringInterceptionBehaviorWithTypeThroughConfigurationFile",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<InterceptionBehaviorElement>()
                .Select(element => element.TypeName)
                .AssertContainsExactly("GlobalCountInterceptionBehavior");
        }

        [TestMethod]
        public void Then_MultipleBehaviorsCanBeAddedInConfig()
        {
            var registration = this.GetRegistration("multipleBehaviorsOnOneRegistration",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<InterceptionBehaviorElement>()
                .Select(element => element.Name)
                .AssertContainsExactly("addInterface", "fixed");
        }
    }
}
