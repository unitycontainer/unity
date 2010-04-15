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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity.TestSupport;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_SerializingInterceptionInjectionMembers
    /// </summary>
    [TestClass]
    public class When_SerializingInterceptionInjectionMembers : SerializationFixture
    {
        [TestMethod]
        public void Then_InterceptorGetsSerialized()
        {
            var loadedSection = SerializeAndLoadConfig("SerializingInjectionMembers.config", c =>
            {
                var reg = new RegisterElement()
                {
                    TypeName = "SomeType"
                };

                reg.InjectionMembers.Add(new InterceptorElement()
                {
                    Name = "interceptor",
                    TypeName = "NoSuchInterceptor"
                });

                var reg2 = new RegisterElement()
                {
                    TypeName = "SomeType",
                    Name = "Default"
                };

                reg2.InjectionMembers.Add(new InterceptorElement()
                {
                    IsDefaultForType = true,
                    Name = "otherInterceptor",
                    TypeName = "AnotherInterceptor"
                });

                c.Registrations.Add(reg);
                c.Registrations.Add(reg2);
            });

            var firstInterceptor =
                (InterceptorElement) loadedSection.Containers.Default.Registrations[0].InjectionMembers[0];
            Assert.IsFalse(firstInterceptor.IsDefaultForType);
            Assert.AreEqual("interceptor", firstInterceptor.Name);
            Assert.AreEqual("NoSuchInterceptor", firstInterceptor.TypeName);

            var secondInterceptor =
                (InterceptorElement)loadedSection.Containers.Default.Registrations[1].InjectionMembers[0];
            Assert.IsTrue(secondInterceptor.IsDefaultForType);
            Assert.AreEqual("otherInterceptor", secondInterceptor.Name);
            Assert.AreEqual("AnotherInterceptor", secondInterceptor.TypeName);
        }

        [TestMethod]
        public void Then_AdditionalInterfacesGetSerialized()
        {
            var loadedSection = SerializeAndLoadConfig("SerializingInjectionMembers.config", c =>
            {
                var reg = new RegisterElement
                {
                    TypeName = "SomeType"
                };

                reg.InjectionMembers.Add(new AddInterfaceElement()
                {
                    TypeName = "InterfaceOne"
                });

                c.Registrations.Add(reg);
            });

            var additionalInterface = (AddInterfaceElement)
                loadedSection.Containers.Default.Registrations[0].InjectionMembers[0];
            Assert.AreEqual("InterfaceOne", additionalInterface.TypeName);
        }

        [TestMethod]
        public void Then_BehaviorElementsGetSerialized()
        {
            var loadedSection = SerializeAndLoadConfig("SerializingInjectionMembers.config", c =>
            {
                var reg = new RegisterElement
                {
                    TypeName = "SomeType"
                };

                reg.InjectionMembers.Add(new InterceptionBehaviorElement()
                {
                    TypeName = "SomeBehavior"
                });

                reg.InjectionMembers.Add(new InterceptionBehaviorElement()
                {
                    Name = "NamedBehavior",
                    TypeName = "SomeOtherBehavior"
                });
                c.Registrations.Add(reg);
            });

            var injectionMembers =
                loadedSection.Containers.Default.Registrations[0].InjectionMembers
                    .Cast<InterceptionBehaviorElement>();

            injectionMembers.Select(m => m.TypeName)
                .AssertContainsExactly("SomeBehavior", "SomeOtherBehavior");
            injectionMembers.Select(m => m.Name)
                .AssertContainsExactly("", "NamedBehavior");
        }
    }
}
