// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.TransparentProxyInterception
{
    [TestClass]
    public class TransparentProxyInterceptorFixture
    {
        [TestMethod]
        public void InterceptorReturnsSingleMethod()
        {
            List<MethodImplementationInfo> methods = GetMethods<SingleInterceptableMethod>();

            CollectionAssertExtensions.AreEquivalent(GetOnlyImplementations(typeof(SingleInterceptableMethod), "MyMethod"),
                methods);
        }

        [TestMethod]
        public void InterceptorReturnsAllMethodsIncludingInheritedOnes()
        {
            List<MethodImplementationInfo> methods = GetMethods<InheritsSingleMethodAndAdds>();

            CollectionAssertExtensions.AreEquivalent(
                GetOnlyImplementations(typeof(InheritsSingleMethodAndAdds), "MyMethod", "AnotherMethod", "StillMoreMethod"),
                methods);
        }

        [TestMethod]
        public void NonMBROReturnsInterfaceMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<Operations>();

            Assert.AreEqual(2, methods.Count);

            List<MethodImplementationInfo> expected = GetExpectedMethodImplementations(typeof(IMyOperations), typeof(Operations));

            CollectionAssertExtensions.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void ReturnsAllInterfaceMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<Incoherent>();

            CollectionAssertExtensions.AreEquivalent(
                GetExpectedMethodImplementations(typeof(IMyOperations), typeof(Incoherent))
                    .Concat(GetExpectedMethodImplementations(typeof(ImTiredOfInterfaces), typeof(Incoherent))).ToList(),
                methods);
        }

        [TestMethod]
        public void EmptyInterfacesContributeNoMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<Marked>();

            CollectionAssertExtensions.AreEquivalent(
                GetExpectedMethodImplementations(typeof(IMyOperations), typeof(Marked)),
                methods);
        }

        [TestMethod]
        public void PropertiesAreReturnedAsGetAndSetMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<HasProperties>();

            CollectionAssertExtensions.AreEquivalent(
                GetOnlyImplementations(typeof(HasProperties), "get_SettableProp", "set_SettableProp", "get_GetOnly"),
                methods);
        }

        [TestMethod]
        public void InterfacePropertiesAreReturned()
        {
            List<MethodImplementationInfo> methods = GetMethods<PropThroughInterface>();

            CollectionAssertExtensions.AreEquivalent(
                GetExpectedMethodImplementations(typeof(IHasProperties), typeof(PropThroughInterface)),
                methods);
        }

        [TestMethod]
        public void MBROReturnBothInterfaceAndClassProperties()
        {
            List<MethodImplementationInfo> methods = GetMethods<MBROWithPropThroughInterface>();

            var expected = GetExpectedMethodImplementations(typeof(IHasProperties), typeof(MBROWithPropThroughInterface))
                .Concat(GetOnlyImplementations(typeof(MBROWithPropThroughInterface), "get_Gettable"));

            CollectionAssertExtensions.AreEquivalent(expected.ToList(), methods);
        }

        [TestMethod]
        public void ExplicitImplementationsAreFound()
        {
            List<MethodImplementationInfo> methods = GetMethods<ExplicitImplementation>();

            var expected = GetExpectedMethodImplementations(typeof(IMyOperations), typeof(ExplicitImplementation))
                .Concat(GetOnlyImplementations(typeof(ExplicitImplementation), "AClassMethod"));
            CollectionAssertExtensions.AreEquivalent(expected.ToList(), methods);
        }

        [TestMethod]
        public void AssortedParameterKindsAreProperlyHandled()
        {
            var interceptor = new TransparentProxyInterceptor();
            var target = new AssortedParameterKindsAreProperlyHandledHelper.TypeWithAssertedParameterKinds();

            IInterceptingProxy proxy =
                interceptor.CreateProxy(
                    typeof(AssortedParameterKindsAreProperlyHandledHelper.TypeWithAssertedParameterKinds),
                    target);

            AssortedParameterKindsAreProperlyHandledHelper.PerformTest(proxy);
        }

        private static List<MethodImplementationInfo> GetMethods<T>()
        {
            return new List<MethodImplementationInfo>(new TransparentProxyInterceptor().GetInterceptableMethods(typeof(T), typeof(T)));
        }

        private static List<MethodImplementationInfo> GetExpectedMethodImplementations(Type interfaceType, Type implementationType)
        {
            InterfaceMapping mapping = implementationType.GetInterfaceMap(interfaceType);
            List<MethodImplementationInfo> results = new List<MethodImplementationInfo>(mapping.InterfaceMethods.Length);

            for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
            {
                results.Add(new MethodImplementationInfo(mapping.InterfaceMethods[i], mapping.TargetMethods[i]));
            }
            return results;
        }

        private static List<MethodImplementationInfo> GetOnlyImplementations(Type implementationType, params string[] methodNames)
        {
            return
                methodNames.Select(
                    methodName => new MethodImplementationInfo(null, implementationType.GetMethod(methodName))).ToList();
        }

        [TestMethod]
        public void CanCreateProxyWithAdditionalInterfaces()
        {
            IInstanceInterceptor interceptor = new TransparentProxyInterceptor();
            SingleInterceptableMethod target = new SingleInterceptableMethod();

            object proxy = interceptor.CreateProxy(typeof(SingleInterceptableMethod), target, typeof(IMyOperations));

            Assert.IsTrue(proxy is IMyOperations);
        }
    }

    internal class SingleInterceptableMethod : MarshalByRefObject
    {
        public void MyMethod() { }
    }

    internal class InheritsSingleMethodAndAdds : SingleInterceptableMethod
    {
        public void AnotherMethod()
        {
        }

        public void StillMoreMethod()
        {
        }
    }

    internal interface IMyOperations
    {
        void Add();
        void Multiply();
    }

    internal interface ImTiredOfInterfaces
    {
        void YaddaYadda();
    }

    internal interface IMarkerInterface
    {
    }

    internal class Operations : IMyOperations
    {
        public void Add()
        {
        }

        public void Multiply()
        {
        }

        public void NotAnInterfaceMethod()
        {
        }
    }

    internal class Incoherent : IMyOperations, ImTiredOfInterfaces
    {
        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void Multiply()
        {
            throw new System.NotImplementedException();
        }

        public void YaddaYadda()
        {
            throw new System.NotImplementedException();
        }
    }

    internal class Marked : IMyOperations, IMarkerInterface
    {
        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void Multiply()
        {
            throw new System.NotImplementedException();
        }
    }

    internal class HasProperties : MarshalByRefObject
    {
        public string SettableProp
        {
            get { return null; }
            set { }
        }

        public int GetOnly
        {
            get { return 42; }
        }
    }

    internal interface IHasProperties
    {
        string StringProp { get; set; }
    }

    internal class PropThroughInterface : IHasProperties
    {
        public string StringProp
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }

    internal class MBROWithPropThroughInterface : MarshalByRefObject, IHasProperties
    {
        public string StringProp
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public int Gettable
        {
            get { return 37; }
        }
    }

    internal class ExplicitImplementation : MarshalByRefObject, IMyOperations
    {
        void IMyOperations.Add()
        {
            throw new System.NotImplementedException();
        }

        void IMyOperations.Multiply()
        {
            throw new System.NotImplementedException();
        }

        public void AClassMethod()
        {
        }
    }
}
