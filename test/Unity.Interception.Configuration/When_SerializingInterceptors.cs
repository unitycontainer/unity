// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class When_SerializingInterceptors : SerializationFixture
    {
        private InterceptorsElement DoSerialization(Action<InterceptorsElement> interceptorInitializer)
        {
            var section = SerializeAndLoadConfig("SerializingInterceptors.config", c =>
            {
                var interceptors = new InterceptorsElement();
                interceptorInitializer(interceptors);
                c.ConfiguringElements.Add(interceptors);
            });

            return (InterceptorsElement)section.Containers.Default.ConfiguringElements[0];
        }

        [TestMethod]
        public void Then_SingleInterceptorWithKeyIsSerialized()
        {
            var result = this.DoSerialization(itc =>
            {
                var interceptorElement = new InterceptorsInterceptorElement()
                {
                    TypeName = "InterfaceInterceptor"
                };
                interceptorElement.Registrations.Add(new KeyElement() { TypeName = "MyType" });
                itc.Interceptors.Add(interceptorElement);
            });

            Assert.AreEqual(1, result.Interceptors.Count);
            var resultElement = result.Interceptors[0];
            Assert.AreEqual("InterfaceInterceptor", resultElement.TypeName);

            Assert.AreEqual(1, result.Interceptors[0].Registrations.Count);

            var key = (KeyElement)resultElement.Registrations[0];
            Assert.AreEqual("MyType", key.TypeName);
            Assert.AreEqual(String.Empty, key.Name);
        }

        [TestMethod]
        public void Then_MultipleInterceptorsWithKeysAreSerialized()
        {
            var result = this.DoSerialization(itc =>
            {
                var interceptorElement1 = new InterceptorsInterceptorElement()
                {
                    TypeName = "InterfaceInterceptor"
                };
                interceptorElement1.Registrations.Add(new KeyElement() { TypeName = "MyType" });
                itc.Interceptors.Add(interceptorElement1);

                var interceptorElement2 = new InterceptorsInterceptorElement
                {
                    TypeName = "TransparentProxyInterceptor"
                };
                interceptorElement2.Registrations.Add(new DefaultElement { TypeName = "MyOtherType" });
                itc.Interceptors.Add(interceptorElement2);
            });

            Assert.AreEqual(2, result.Interceptors.Count);

            result.Interceptors.Select(i => i.Registrations.Count)
                .AssertContainsExactly(1, 1);
            result.Interceptors.SelectMany(i => i.Registrations)
                .Select(r => r.TypeName)
                .AssertContainsExactly("MyType", "MyOtherType");
        }

        [TestMethod]
        public void Then_InterceptorWithMultipleRegistrationsIsSerialized()
        {
            var result = this.DoSerialization(itc =>
            {
                var i1 = new InterceptorsInterceptorElement
                {
                    TypeName = "VirtualMethodInterceptor"
                };

                i1.Registrations.Add(new DefaultElement { TypeName = "Type1" });
                i1.Registrations.Add(new KeyElement { TypeName = "Type2", Name = "name1" });
                i1.Registrations.Add(new KeyElement { TypeName = "Type2", Name = "name2" });
                i1.Registrations.Add(new KeyElement { TypeName = "Type3" });

                itc.Interceptors.Add(i1);
            });

            result.Interceptors.SelectMany(i => i.Registrations)
                .Select(r => r.TypeName)
                .AssertContainsExactly("Type1", "Type2", "Type2", "Type3");

            result.Interceptors.SelectMany(i => i.Registrations)
                .OfType<KeyElement>()
                .Select(k => k.Name)
                .AssertContainsExactly("name1", "name2", String.Empty);
        }
    }
}
