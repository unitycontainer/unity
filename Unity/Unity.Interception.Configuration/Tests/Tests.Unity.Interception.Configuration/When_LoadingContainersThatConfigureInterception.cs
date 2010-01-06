//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_LoadingContainersThatConfigureInterception
    /// </summary>
    [TestClass]
    public class When_LoadingContainersThatConfigureInterception : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingContainersThatConfigureInterception() : base("InterceptionInjectionMembers")
        {
        }

        private IEnumerable<RegisterElement> GetRegistration(string containerName, Func<RegisterElement, bool> predicate)
        {
            var containerElement = Section.Containers[containerName];
            return containerElement.Registrations.Where(predicate);
        }

        private static bool IsInterceptableType(RegisterElement element)
        {
            return element.TypeName == "Interceptable";
        }

        [TestMethod]
        public void Then_InterceptionElementIsPresentInRegistrationInjectionMembers()
        {
            var registration = GetRegistration("configuringInterceptorThroughConfigurationFile",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<InterceptorElement>()
                .Select(element => element.TypeName)
                .AssertContainsExactly("VirtualMethodInterceptor");
        }

        [TestMethod]
        public void Then_ExtraInterfacesCanBeAddedInConfig()
        {
            var registration = GetRegistration(
                "configuringAdditionalInterfaceThroughConfigurationFile",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<AddInterfaceElement>()
                .Select(element => element.TypeName)
                .AssertContainsExactly("IServiceProvider", "IComponent");
        }

        [TestMethod]
        public void Then_BehaviorsCanBeAddedInConfig()
        {
            var registration = GetRegistration(
                "configuringInterceptionBehaviorWithTypeThroughConfigurationFile",
                IsInterceptableType).First();

            registration.InjectionMembers.OfType<InterceptionBehaviorElement>()
                .Select(element => element.TypeName)
                .AssertContainsExactly("GlobalCountInterceptionBehavior");
        }
    }
}
